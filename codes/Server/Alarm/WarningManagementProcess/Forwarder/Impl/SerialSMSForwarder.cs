using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using FS.SMIS_Cloud.Alarm.Forwarder.Config;
using FS.SMIS_Cloud.Alarm.Forwarder.Dal;
using FS.SMIS_Cloud.Alarm.Forwarder.Model;
using FS.SMIS_Cloud.Alarm.SmsService;
using log4net;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Impl
{
    public class SMSData
    {
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Sms { get; set; }
        public int Id { get; set; }
    }

    public class SerialSMSForwarder : IForwarder
    {
        private readonly ConcurrentQueue<SMSData> _smsQueue = new ConcurrentQueue<SMSData>();

        private readonly Dictionary<int, Dictionary<int, List<ContactsInfo>>> _stuctToRoleContacts =
            new Dictionary<int, Dictionary<int, List<ContactsInfo>>>();

        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        private SerialSMS _config;
        private List<string> _organizationids = new List<string>();
        private List<RemoteDtu> _remoteDtus = new List<RemoteDtu>();
        private List<Sensor> _sensors = new List<Sensor>();
        private Thread _smsSendThread;
        private List<string> _structureId = new List<string>();
        private List<StructureInfo> _structureInfos = new List<StructureInfo>();
        private List<WarningType> _warningTypes = new List<WarningType>();
        private Thread _workThread;

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="config"></param>
        public void Init(ConfigurationElement config)
        {
            if (config is SerialSMS)
            {
                _config = config as SerialSMS;
                _organizationids.Clear();
                _organizationids = _config.Organizationids.Replace(" ", "").Trim(',').Split(',').ToList();
                InitSMSCat();
            }
            else
            {
                throw new ConfigErrorException("Invalid serial configuration in SerialSMSForWarder.");
            }
        }

        /// <summary>
        ///     开启线程
        /// </summary>
        public void Start()
        {
            _workThread = new Thread(Run);
            _workThread.Start();
            _smsSendThread = new Thread(DispatchSMS);
            _smsSendThread.Start();
        }

        /// <summary>
        ///     初始化短信猫
        /// </summary>
        private void InitSMSCat()
        {
            int com = _config.Com;
            try
            {
                // 启动服务,打开串口，初始化Modem, 0为失败，非0为成功  
                if (SMS.SMSStartService(com, _config.Baudrate, 2, 8, 0, 0, "card") != 0)
                {
                    logger.Debug("Success to start sms module.");
                }
                else
                {
                    logger.Warn("Failed to start sms module.");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to open sms port.", ex);
            }
        }

        public void Run()
        {
            ReloadWarningType();
            while (true)
            {
                ReloadStructureIds();
                ReloadStructures();
                ReloadSensors();
                ReloadRemoteDtu();
                ReloadContacts();
                EnqueueAlarms();
                Thread.Sleep(_config.Interval);
            }
        }

        /// <summary>
        ///     加载结构物信息
        /// </summary>
        private void ReloadStructures()
        {
            _structureInfos = DataAccess.GetStructureInfo();
        }

        /// <summary>
        ///     加载传感器信息
        /// </summary>
        private void ReloadSensors()
        {
            _sensors = DataAccess.GetSensorInfo();
        }

        /// <summary>
        ///     加载dtu信息
        /// </summary>
        private void ReloadRemoteDtu()
        {
            _remoteDtus = DataAccess.GetRemoteDtuInfo();
        }

        /// <summary>
        ///     加载结构物信息
        /// </summary>
        private void ReloadStructureIds()
        {
            if (_organizationids.Count > 0 && _organizationids[0] != string.Empty && _organizationids[0] != " ")
            {
                _structureId = DataAccess.GetStructureIdsByOrganizationId(_organizationids);
            }
            else
            {
                _structureId.Clear();
            }
        }

        /// <summary>
        ///     加载告警类型信息
        /// </summary>
        private void ReloadWarningType()
        {
            _warningTypes = DataAccess.GetWarningTypeInfo();
        }

        /// <summary>
        ///     加载联系人以及用户与结构物信息
        /// </summary>
        private void ReloadContacts()
        {
            List<ContactsInfo> contacts = DataAccess.GetReceiverInfo();
            List<UserStructure> userStructures = DataAccess.GetUserStructureInfo();
            MapContacts(contacts, userStructures);
        }

        /// <summary>
        ///     结构物-用户角色-联系人映射
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="userStructures"></param>
        private void MapContacts(List<ContactsInfo> contacts, List<UserStructure> userStructures)
        {
            _stuctToRoleContacts.Clear();
            if (contacts.Count == 0 || userStructures.Count == 0)
            {
                return;
            }
            Dictionary<int, List<int>> structToUser =
                userStructures.GroupBy(x => x.Structure_Id)
                              .ToDictionary(x => x.Key, x => x.ToList().Select(p => p.User_No).ToList());
            Dictionary<int, List<ContactsInfo>> userToContacts = contacts.GroupBy(c => c.UserNo)
                                                                         .ToDictionary(c => c.Key, c => c.ToList());
            foreach (var item in structToUser)
            {
                if (!_stuctToRoleContacts.ContainsKey(item.Key))
                {
                    _stuctToRoleContacts[item.Key] = new Dictionary<int, List<ContactsInfo>>();
                    _stuctToRoleContacts[item.Key][(int) ContactType.Support] = new List<ContactsInfo>();
                    _stuctToRoleContacts[item.Key][(int) ContactType.Client] = new List<ContactsInfo>();
                }
                else
                {
                    _stuctToRoleContacts[item.Key][(int) ContactType.Support].Clear();
                    _stuctToRoleContacts[item.Key][(int) ContactType.Client].Clear();
                }
                List<int> userList = item.Value;
                foreach (int userNo in userList)
                {
                    List<ContactsInfo> contactsOfUser = userToContacts.ContainsKey(userNo)
                                                            ? userToContacts[userNo]
                                                            : null;
                    if (contactsOfUser == null || contactsOfUser.Count == 0) continue;
                    foreach (ContactsInfo contactsInfo in contactsOfUser)
                    {
                        switch (contactsInfo.RoleId)
                        {
                            case (int) ContactType.Support:
                                _stuctToRoleContacts[item.Key][(int) ContactType.Support].Add(contactsInfo);
                                break;
                            case (int) ContactType.Client:
                                _stuctToRoleContacts[item.Key][(int) ContactType.Client].Add(contactsInfo);
                                break;
                            default:
                                logger.Debug(string.Format("Invalid contact info: {0}, {1}",
                                                           contactsInfo.UserNo,
                                                           contactsInfo.ReceiverName))
                                    ;
                                break;
                        }
                    }
                }
            }
        }

        public void EnqueueAlarms()
        {
            //
            EnqueueToUser(ContactType.Support, WarningStatus.NoSupportReceiverOrEnqueue, WarningStatus.SupportReceived);
            //
            EnqueueToUser(ContactType.Client, WarningStatus.NoClientReceiverOrEnqueue, WarningStatus.ClientReceived);
        }

        private void EnqueueToUser(ContactType userType, WarningStatus statusOfFailedOrEnqueue,
                                   WarningStatus statusOfSendSuccess)
        {
            List<WarningInfo> alarms;
            //
            if (userType == ContactType.Support)
            {
                alarms = DataAccess.GetWarningInfo(_config.Top, (int) WarningStatus.WaitToSendToSupport,
                                                   (int) DealFlag.UnConfiremBySupport, _structureId);
            }
            else
            {
                alarms = DataAccess.GetWarningInfo(_config.Top, (int) WarningStatus.WaitToSendToClient,
                                                   (int) DealFlag.UnConfiremByClient, _structureId);
            }
            //
            if (alarms.Count == 0)
            {
                //logger.Info(string.Format("No alarm to send to {0}.", userType));
                return;
            }
            EnqueueAlarmsToUser(alarms, userType, statusOfFailedOrEnqueue, statusOfSendSuccess);
        }

        /// <summary>
        ///     入队列
        /// </summary>
        /// <param name="alarms"></param>
        /// <param name="userType"></param>
        /// <param name="statusOfFailedOrEnqueue"></param>
        /// <param name="statusOfSendSuccess"></param>
        private void EnqueueAlarmsToUser(List<WarningInfo> alarms, ContactType userType,
                                         WarningStatus statusOfFailedOrEnqueue,
                                         WarningStatus statusOfSendSuccess)
        {
            var enqueuedAlarms = new List<int>();
            foreach (WarningInfo alarm in alarms)
            {
                if (alarm.WarningLevel != string.Empty)
                {
                    try
                    {
                        if (_stuctToRoleContacts.ContainsKey(Convert.ToInt32(alarm.StructId)) &&
                            _stuctToRoleContacts[Convert.ToInt32(alarm.StructId)].ContainsKey((int) userType))
                        {
                            List<ContactsInfo> receivers =
                                _stuctToRoleContacts[Convert.ToInt32(alarm.StructId)][(int) userType].Where(
                                    m => m.FilterLevel >= Convert.ToInt32(alarm.WarningLevel)).ToList();
                            if (receivers.Count == 0)
                            {
                                enqueuedAlarms.Add(alarm.Id);
                                continue;
                            }
                            string sms = CreateSMS(alarm, userType);
                            foreach (ContactsInfo contact in receivers)
                            {
                                var smsData = new SMSData();
                                smsData.Phone = contact.ReceiverPhone;
                                smsData.Name = contact.ReceiverName;
                                smsData.Sms = sms;
                                smsData.Id = alarm.Id;
                                _smsQueue.Enqueue(smsData);
                                if (_smsQueue.Count >= _config.MaxCapacity)
                                {
                                    logger.Error("Queue reach it's capacity.");
                                    return;
                                }
                                enqueuedAlarms.Add(alarm.Id);
                                logger.Debug(string.Format("Enqueue sms to {0}-{1}-{2} ; msg:{3}", userType,
                                                           contact.ReceiverName, contact.ReceiverPhone, sms));
                            }
                        }
                        else
                        {
                            enqueuedAlarms.Add(alarm.Id);
                        }
                    }
                    catch (CreateSMSException e)
                    {
                        logger.Error(e);
                        enqueuedAlarms.Add(alarm.Id);
                    }
                }
                else
                {
                    enqueuedAlarms.Add(alarm.Id);
                }
            }
            //
            if (enqueuedAlarms.Count != 0)
            {
                DataAccess.UpdateStatusById(enqueuedAlarms, (int) statusOfFailedOrEnqueue);
            }
        }

        /// <summary>
        ///     短消息构建
        /// </summary>
        /// <param name="alarm"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        private string CreateSMS(WarningInfo alarm, ContactType userType)
        {
            string result = string.Empty;
            // 结构物名称
            StructureInfo structs = _structureInfos.FirstOrDefault(m => m.Id == Convert.ToInt32(alarm.StructId));
            // 告警原因
            WarningType warningtype = _warningTypes.FirstOrDefault(m => m.TypeId == alarm.WarningTypeId);
            string warningReason = string.Empty;
            // 设备信息
            int deviceTypeId = Convert.ToInt32(alarm.DeviceTypeId);
            if (userType == ContactType.Support)
            {
                if (warningtype != null) warningReason = "告警原因：" + warningtype.Reason;
            }
            switch (deviceTypeId)
            {
                case (int) DeviceType.Dtu:
                    List<RemoteDtu> dtus =
                        _remoteDtus.Where(m => Convert.ToInt32(m.RemoteDtuNumber) == Convert.ToInt32(alarm.DeviceId))
                                   .ToList();
                    if (dtus.Count == 0)
                    {
                        throw new CreateSMSException(string.Format("Unknown DTU({0}) of alarm({1}).", alarm.DeviceId,
                                                                   alarm.Id));
                    }
                    result = string.Format("{0},{1}: {2}-{3}-{4} 产生 {5} 级告警, 告警描述: {6}. {7} {8}",
                                           Convert.ToDateTime(alarm.Time).ToString("yyyy-MM-dd HH:mm:ss"), "DTU设备",
                                           dtus.First().Description, dtus.First().RemoteDtuNumber,
                                           structs != null ? structs.StructureNameCn : "", alarm.WarningLevel,
                                           alarm.Description, alarm.Content, warningReason);
                    break;
                case (int) DeviceType.Sensor:
                    string sensorInfo = string.Empty; // 传感器归属dtu、模块号、通道号
                    List<Sensor> sensor = _sensors.Where(m => m.Sensor_Id == alarm.DeviceId).ToList();
                    if (sensor.Count == 0)
                    {
                        throw new CreateSMSException(string.Format("Unknown Sensor({0}) of alarm({1}).", alarm.DeviceId,
                                                                   alarm.Id));
                    }
                    if (userType == ContactType.Support)
                    {
                        List<RemoteDtu> rdtus =
                            _remoteDtus.Where(m => Convert.ToString(m.Id) == sensor.First().Dtu_Id).ToList();
                        if (rdtus.Count > 0)
                        {
                            sensorInfo = string.Format("传感器归属DTU: {0} {1}, 模块号: {2}, 通道号: {3}",
                                                       rdtus.First().Description,
                                                       rdtus.First().RemoteDtuNumber,
                                                       Convert.ToString(sensor.First().Module_No),
                                                       Convert.ToString(sensor.First().Dai_Channel_Number));
                        }
                    }
                    result = string.Format("{0},{1} {2} {3} 产生 {4} 级告警, 告警描述:{5}. {6} {7} {8} ",
                                           Convert.ToDateTime(alarm.Time).ToString("yyyy-MM-dd HH:mm:ss"),
                                           structs != null ? structs.StructureNameCn : "",
                                           sensor.First().SensorLocationDs,
                                           "传感器设备", alarm.WarningLevel, alarm.Description, alarm.Content,
                                           warningReason, sensorInfo);
                    break;
            }
            return result;
        }

        /// <summary>
        ///     消息分发
        /// </summary>
        private void DispatchSMS()
        {
            var sendedAlarms = new List<int>();
            int countTime = 0;
            while (true)
            {
                SMSData smsData;
                countTime++;
                if (_smsQueue.TryDequeue(out smsData))
                {
                    bool result = SendSMS(smsData.Phone, smsData.Sms, smsData.Name);
                    //logger.Debug(string.Format("Send sms to {0}-{1} ; msg:{2}", smsData.Name, smsData.Phone, smsData.Sms));
                    sendedAlarms.Add(smsData.Id);
                }
                if (sendedAlarms.Count >= _config.CommitCount || countTime*_config.DispatchInterval >= 1000*60)
                {
                    logger.Debug("Update alarm status to sended.");
                    if (sendedAlarms.Count > 0)
                    {
                        DataAccess.UpdateStatusToSendedById(sendedAlarms);
                    }

                    sendedAlarms.Clear();
                    countTime = 0;
                }
                Thread.Sleep(_config.DispatchInterval);
            }
        }

        /// <summary>
        ///     消息发送
        /// </summary>
        /// <param name="receiverPhone"></param>
        /// <param name="sms"></param>
        /// <param name="receiverName"></param>
        /// <returns></returns>
        private bool SendSMS(string receiverPhone, string sms, string receiverName)
        {
            if (receiverPhone.Length != 11)
            {
                logger.Error(string.Format("Invalied phone number: {0} {1} {2} ", receiverPhone, receiverName, sms));
                return false;
            }
            string SMSProfile = ConfigurationManager.AppSettings["SMSProfile"];
            if (!(SMSProfile == null || SMSProfile.Equals("PRODUCT")))
            {
                return true;
            }
            SMS.SMSEnableLongSms(1);
            // 发送短消息,返回短消息编号:index，从0开始递增
            uint messageIndex = SMS.SMSSendMessage(sms, receiverPhone);
            //@Todo  Sleep?
            Thread.Sleep(_config.SendInterval);
            // 查询指定序号的短信是否发送成功
            int statusSendMessage = SMS.SMSQuery(messageIndex);
            switch (statusSendMessage)
            {
                case 1:
                    logger.Info(string.Format("Success to Send SMS: {0} {1} {2} ", receiverPhone, receiverName, sms));
                    return true;
                case 0:
                    logger.Error(string.Format("Failed to Send SMS: {0} {1} {2} ", receiverPhone, receiverName, sms));
                    return false;
                case -1:
                    logger.Warn(string.Format("Unknown status of Send SMS: {0} {1} {2} ", receiverPhone, receiverName,
                                              sms));
                    return false;
                default:
                    return false;
            }
        }
    }
}