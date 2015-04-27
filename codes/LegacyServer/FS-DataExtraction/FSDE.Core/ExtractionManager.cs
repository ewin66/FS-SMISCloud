#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ExtractionManager.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140603 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using FSDE.Model.Fixed;

namespace FSDE.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ExtractionProtocol;
    using FSDE.BLL.Select;
    using FSDE.Commn;
    using FSDE.Dictionaries;
    using FSDE.Dictionaries.config;
    using FSDE.Model;
    using FSDE.Model.Config;
    using FSDE.Model.Events;
    using log4net;

    public class ExtractionManager
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ExtractionManager));

        private readonly ConcurrentDictionary<int, string[]> structModuleNodic = new ConcurrentDictionary<int, string[]>();

        private int byteint = 200;

        //存放处理完后的数据
        private ConcurrentQueue<Data> datasTobesent = new ConcurrentQueue<Data>();
        
        //存放准备发送的数据包
        private ConcurrentQueue<byte[]> packetsToSend = new ConcurrentQueue<byte[]>();

        /// <summary>
        /// 串口通信用
        /// </summary>
        private readonly PortManager portManager = PortManager.GetPortManager();

        //定时提取用定时器
        private Timer extractionInterval;

        private ExtractionManager()
        {
            //需要对packetsToSend进行初始化，将之前未发送的添加到队列
           IList<byte[]> list = PacketsToSendDic.GetPacketsToSendDic().GetAllPackets2Send();
            foreach (byte[] bytes in list)
            {
                packetsToSend.Enqueue(bytes);
            }
            PacketsToSendDic.GetPacketsToSendDic().DeleteAll();
            string path = Environment.CurrentDirectory + @"\StructuresMoudles.ini";
            //初始化结构物模块号关系
            List<string> sections = IniFileHelper.ReadSections(path);
            foreach (string section in sections)
            {
                string modulestr = IniFileHelper.GetString(section, "moduleNo", string.Empty, path);
                string structurestr = IniFileHelper.GetString(section, "structureId", string.Empty, path);
                int structureId;
                if (int.TryParse(structurestr.Trim(), out structureId) && !string.IsNullOrEmpty(modulestr))
                {
                    string[] modules = modulestr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    structModuleNodic.TryAdd(structureId, modules);
                }
            }
        }

        private static ExtractionManager extractionManager;

        private static readonly object Obj = new object();

        public static ExtractionManager GetExtractionManager()
        {
            if (null == extractionManager)
            {
                lock (Obj)
                {
                    if (null == extractionManager)
                    {
                        extractionManager = new ExtractionManager();
                    }
                }
            }
            return extractionManager;
        }

        private Task task;

        private bool isready =false;

        /// <summary>
        /// 开始提取
        /// </summary>
        public void StartExtractDb()
        {
            ConfigInfoTable.InitializationConfigtableInfo();
            Task.Factory.StartNew(() => this.startService());
        }

        private int heartbeatInterval;

        private void startService()
        {
            if (this.messagesShowEventHandler != null)
            {
                this.messagesShowEventHandler(this, new MessagesShowEventArgs { MessageType = MsgType.Info, MessagesShow = "开始提取" });
            }
            portManager.StartService();

            var file = new ExeConfigurationFileMap { ExeConfigFilename = "config/Params.config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            string strInterval = config.AppSettings.Settings["heartbeatInterval"].Value;
            if (!int.TryParse(strInterval, out heartbeatInterval))
            {
                heartbeatInterval = 3;
            }
            heartbeatInterval = heartbeatInterval * 60000;

            if (extractionInterval == null)
            {
                this.extractionInterval = new Timer(new TimerCallback(ExtractDb), null, Timeout.Infinite, Timeout.Infinite);
            }

            this.extractionInterval.Change(100, ProjectInfoDic.GetInstance().GetProjectInfo().IntervalTime*60000);

            //创建发送数据线程
            isready = true;
            task = new Task(() => this.Send());

            task.Start();
        }


        /// <summary>
        /// 发送数据包
        /// </summary>
        private void Send()
        {
            log.Info("进入发送函数Send");
            try
            {
                var file = new ExeConfigurationFileMap { ExeConfigFilename = @".\config\Params.config" };
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(
                    file,
                    ConfigurationUserLevel.None);
                if (!int.TryParse(config.AppSettings.Settings["byteint"].Value, out byteint))
                {
                    byteint = 200;
                }
                this.ShowLog("Interval of package is {0}", byteint);
                // Stopwatch timer = new Stopwatch();
                Thread.Sleep(300);
                //  timer.Start();
                // portManager.SendSatateBytes();

                while (isready)
                {
                        try
                        {
                            //如果没有可发送数据等20s
                            if (this.packetsToSend.Count == 0)
                            {
                               // log.Info("packetsToSend.Count == 0");
                                Thread.Sleep(20000);
                            }
                            else
                            {
                                //log.Info("packetsToSend.Count ==" + packetsToSend.Count);
                                byte[] packetBytes;
                                this.packetsToSend.TryDequeue(out packetBytes);
                                if (packetBytes != null)
                                {
                                    //log.Info("packetBytes != null");
                                    bool issend = portManager.SendData(packetBytes);
                                    //log.Info("离开发送函数SendData");
                                    StringBuilder msg = new StringBuilder();
                                    //msg.Append(ValueHelper.Byte2HexStr(packetBytes, 0, packetBytes.Length));
                                    if (issend)
                                    {
                                        // this.packetsToSend.TryDequeue(out packetBytes);
                                        msg.Insert(0, string.Format("{0},发送成功，数据长度：",DateTime.Now));
                                        msg.Append(packetBytes.Length);
                                        msg.Append(",模块号：").Append(BitConverter.ToInt16(packetBytes, 7));
                                        msg.Append("通道号：").Append(packetBytes[9]);
                                        msg.Append("总包数：")
                                            .Append(packetBytes[10])
                                            .Append("本包号:")
                                            .Append(packetBytes[11]);
                                        log.Info(msg.ToString());
                                        ShowLog(msg.ToString());
                                        log.DebugFormat(ValueHelper.Byte2HexStr(packetBytes, 0, packetBytes.Length));
                                        // 发送成功等待 封包间隔*2 毫秒
                                        Thread.Sleep(byteint);
                                    }
                                    else
                                    {
                                        msg.Insert(0, "发送失败：");
                                        this.packetsToSend.Enqueue(packetBytes);
                                       // this.packetsToSend.Enqueue(null);
                                        log.Info(msg.ToString());
                                        ShowLog(msg.ToString());
                                        Thread.Sleep(10);
                                    }
                                }
                                else
                                {
                                    log.Info("packetsToSend.TryPeek失败");
                                   // this.packetsToSend.TryDequeue(out packetBytes);
                                    // 获取失败，10毫秒后重新读取
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                            ShowLog(ex.Message);
                        }
                    }
               // }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }


        private void ShowLog(string msg, params object[] args )
        {
            string fmtMsg = string.Format(msg, args);
            if (this.messagesShowEventHandler != null)
            {
                this.messagesShowEventHandler(this, new MessagesShowEventArgs { MessageType = MsgType.TransInfo, MessagesShow = fmtMsg });
            }
        }

        /// <summary>
        /// 停止提取
        /// </summary>
        public void StopExtract()
        {
            if (extractionInterval != null)
            {
                extractionInterval.Change(Timeout.Infinite, Timeout.Infinite);
            }
            isready = false;
        }

        /// <summary>
        /// 提取、处理并发送数据
        /// </summary>
        /// <param name="obj"></param>
        private void ExtractDb(object obj)
        {
            //if (!string.IsNullOrEmpty(DataBaseNameDic.GetDataBaseNameDic().GetFSUSBaseName().DataBaseCode))
            //{
            //    this.ExtractFSDB();
            //}
            
            //开始提取
            //如有多个目标数据库
            //最好使用多线程实现提取
            //数据的计算在提取每个数据库后计算
            
            var taskfactory = new TaskFactory();
            Task[] childTask = {
                                   taskfactory.StartNew(() => this.ExtractFSDB()),
                                   taskfactory.StartNew(() => this.ExtractOtherDb()),
                                   taskfactory.StartNew(() => this.ExtractTextDb())
                               };

            // 等待所有任务完成数据处理完成后开始生成命令
            try
            {
                Task.WaitAll(childTask);
            }
            catch (AggregateException ae)
            {
                log.Fatal(ae.Message, ae.Flatten());
                if (this.messagesShowEventHandler != null)
                {
                    this.messagesShowEventHandler(this, new MessagesShowEventArgs { MessageType = MsgType.Error, MessagesShow = ae.Message });
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
                if (this.messagesShowEventHandler != null)
                {
                    this.messagesShowEventHandler(this, new MessagesShowEventArgs { MessageType = MsgType.Error, MessagesShow = ex.Message });
                }
            }
            try
            {
                this.CreatePacketesToSend();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            
        
        }
        
        /// <summary>
        /// 提取统一采集软件数据
        /// </summary>
        private void ExtractFSDB()
        {
            try
            {
                if (!string.IsNullOrEmpty(DataBaseNameDic.GetDataBaseNameDic().GetFSUSBaseName().DataBaseCode))
                {
                   var bll = new SelectFSUSDBTablesBll();
                    
                    DataBaseName dataBase = DataBaseNameDic.GetDataBaseNameDic().GetFSUSBaseName();
                    DataSet ds = bll.Select(dataBase);
                    foreach (DataTable dt in ds.Tables)
                    {
                        try
                        {
                            if (this.messagesShowEventHandler != null)
                            {
                                var msg = new StringBuilder();
                                msg.Append("提取").Append(dt.TableName).Append(dt.Rows.Count).Append("条数据");
                                log.Info(msg.ToString());
                                this.messagesShowEventHandler(
                                    this,
                                    new MessagesShowEventArgs
                                    {
                                        MessageType = MsgType.Info,
                                        MessagesShow = msg.ToString()
                                    });
                            }
                            List<Data> list = new List<Data>();
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                
                                log.Info("进入提取数据表");
                                foreach (DataRow row in dt.Rows)
                                {
                                    DateTime collectTime;
                                    ValueHelper.String2Time(row[5].ToString().Trim(), out collectTime);
                                    Data data = new Data
                                    {
                                        ProjectCode = Convert.ToInt16(row[0].ToString().Trim()),
                                        DataBaseId = Convert.ToInt32(row[1].ToString().Trim()),
                                        SensorId = Convert.ToInt32(row[2].ToString().Trim()),
                                        MoudleNo = row[3].ToString().Trim(),
                                        ChannelId = Convert.ToInt32(row[4].ToString().Trim()),
                                        CollectTime = collectTime,
                                        SafeTypeId = Convert.ToInt32(row[6].ToString().Trim())
                                    };
                                    data.DataSet = new List<double>();
                                    for (int i = 7; i < row.ItemArray.Length; i++)
                                    {
                                        if (row[i] != DBNull.Value && !string.IsNullOrEmpty(row[i].ToString().Trim()))
                                        {
                                            double value;
                                            double.TryParse(row[i].ToString().Trim(), out value);
                                            data.DataSet.Add(value);
                                        }
                                        else
                                        {
                                            data.DataSet.Add(0);
                                        }
                                    }
                                    list.Add(data);
                                }
                                string timestr = dt.Compute("Max(ACQUISITION_DATETIME)", Boolean.TrueString).ToString();
                                ExtractionConfigDic.GetExtractionConfigDic()
                                    .UpdateExtractionConfig(
                                        new ExtractionConfig
                                        {
                                            DataBaseId = (int) dataBase.ID,
                                            TableName = dt.TableName,
                                            Acqtime = timestr
                                        });
                                log.Info("提取数据表结束");
                            }
                            //添加到队列
                            foreach (Data data in list)
                            {
                                datasTobesent.Enqueue(data);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        /// <summary>
        /// 提取其他数据库
        /// </summary>
        private void ExtractOtherDb()
        {
            try
            {
                List<DataBaseName> dataBaselist = DataBaseNameDic.GetDataBaseNameDic().GetOtherDataBaseName();
                if (dataBaselist.Count > 0)
                {
                    foreach (DataBaseName baseName in dataBaselist)
                    {
                        try
                        {
                            this.ExtractOtherOneDb(baseName);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            
        }

        private void ExtractTextDb()
        {
            try
            {
                List<DataBaseName> dataBaselist = DataBaseNameDic.GetDataBaseNameDic().GetTextBaseNames();
                if (dataBaselist.Count > 0)
                {
                    foreach (DataBaseName baseName in dataBaselist)
                    {
                        this.ExtractOneTextDb(baseName);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        /// <summary>
        /// 提取文本类数据
        /// </summary>
        /// <param name="baseName"></param>
        private void ExtractOneTextDb(DataBaseName baseName)
        {
            ITextOrBinarySelectBll bll;
            List<Data> list = new List<Data>();
            list.Clear();
            switch (baseName.DataBaseType)
            {
                case (int)DataBaseType.Shake:
                    bll=new ArtVibrationDataSelectBll();
                    list = bll.TextOrBinarySelect(baseName).ToList();
                    if (this.messagesShowEventHandler != null)
                    {
                        var msg = new StringBuilder();
                        if (list.Count > 0)
                        {
                            msg.Append("成功提取").Append("振动文本").Append("数据！");
                        }
                        else
                        {
                            msg.Append("振动文本").Append("无新增数据！");
                        }
                        log.Info(msg.ToString());
                        this.messagesShowEventHandler(
                            this,
                            new MessagesShowEventArgs { MessageType = MsgType.Info, MessagesShow = msg.ToString() });
                    }
                    break;
                case (int)DataBaseType.Fiber:
                    bll=new MoiFiberGratingDataSelectBll();
                    list = bll.TextOrBinarySelect(baseName).ToList();
                    Console.WriteLine("list.count = {0}",list.Count);
                    if (this.messagesShowEventHandler != null)
                    {
                        var msg = new StringBuilder();
                        if (list.Count > 0)
                        {
                            msg.Append("成功提取").Append("光纤文本").Append("数据！");
                        }
                        else
                        {
                            msg.Append("光纤文本").Append("无新增数据！");
                        }
                        log.Info(msg.ToString());
                        this.messagesShowEventHandler(
                            this,
                            new MessagesShowEventArgs { MessageType = MsgType.Info, MessagesShow = msg.ToString() });
                    }
                    break;
                case (int)DataBaseType.Vibration:
                    bll = new OurVibrationBll();
                    list = bll.TextOrBinarySelect(baseName).ToList();
                    if (this.messagesShowEventHandler != null)
                    {
                        var msg = new StringBuilder();
                        if (list.Count > 0)
                        {
                            msg.Append("成功提取").Append("振动").Append("数据！");
                        }
                        else
                        {
                            msg.Append("振动").Append("无新增数据！");
                        }
                        log.Info(msg.ToString());
                        this.messagesShowEventHandler(
                            this,
                            new MessagesShowEventArgs { MessageType = MsgType.Info, MessagesShow = msg.ToString() });
                    }
                    break;
                default:
                    throw new Exception("不支持的数据类型");
            }

            if (list.Count>0)
            {
                foreach (Data data in list)
                {
                    datasTobesent.Enqueue(data);
                }
            }
        }

        /// <summary>
        /// 生成可发送命令(旧)
        /// </summary>
        private void CreatePacketesToSendNoOrder()
        {
            List<byte> packet =new List<byte>();
            IMakeDataTransportPacket createbigDataTransportPacket = ProtocolFactory.CreateDataTransportPacket(TransportDataType.BigData);
            IMakeDataTransportPacket createGeneralDataTransportPacket =
                ProtocolFactory.CreateDataTransportPacket(TransportDataType.General);
            while (datasTobesent.Count>0)
            {
                Data data;
                if (datasTobesent.TryDequeue(out data))
                {
                    if (data != null)
                    {
                        int structureId = GetStructureId(data);
                        if (data.SafeTypeId == (int)SensorCategory.Vibration)
                        {
                            byte[][] packeBytes = createbigDataTransportPacket.MakeDataTransportPacket(data, structureId);
                            foreach (byte[] packeByte in packeBytes)
                            {
                                this.packetsToSend.Enqueue(packeByte);
                              //  this.packetsToSend.Enqueue(null);
                            }
                        }
                        else
                        {
                            byte[][] packeBytes = createGeneralDataTransportPacket.MakeDataTransportPacket(data, structureId);
                            
                            if (IsPacketed(ref packet, packeBytes[0]))
                            {
                                datasTobesent.Enqueue(data);
                                this.packetsToSend.Enqueue(packet.ToArray());
                              //  this.packetsToSend.Enqueue(null);
                                packet.Clear();
                            }
                            else if (datasTobesent.Count<=0)
                            {
                                this.packetsToSend.Enqueue(packet.ToArray());
                               // this.packetsToSend.Enqueue(null);
                                packet.Clear();
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 生成可发送命令(按时间排序)
        /// </summary>
        private void CreatePacketesToSend()
        {
            var packet = new List<byte>();
            IMakeDataTransportPacket createbigDataTransportPacket = ProtocolFactory.CreateDataTransportPacket(TransportDataType.BigData);
            IMakeDataTransportPacket createGeneralDataTransportPacket =
                ProtocolFactory.CreateDataTransportPacket(TransportDataType.General);
            var datumList=new List<Data>();
            while (datasTobesent.Count > 0)
            {
                Data data;
                if (datasTobesent.TryDequeue(out data))
                {
                    if (data != null)
                    {
                        datumList.Add(data);
                    }
                }
            }

            var sortedList=datumList.OrderBy(i => i.CollectTime).ToList();

            var N = sortedList.Count;
            for (var i=0;i<N;i++)
            {
                var data = sortedList[i];
                int structureId = GetStructureId(data);
                if (data.SafeTypeId == (int)SensorCategory.Vibration)
                {
                    byte[][] packeBytes = createbigDataTransportPacket.MakeDataTransportPacket(data, structureId);
                    foreach (byte[] packeByte in packeBytes)
                    {
                        this.packetsToSend.Enqueue(packeByte);
                        //  this.packetsToSend.Enqueue(null);
                    }
                }
                else
                {
                    var packeBytes = createGeneralDataTransportPacket.MakeDataTransportPacket(data, structureId);
                    if (packet.Count + packeBytes[0].Length < 1024)
                    {
                        packet.AddRange(packeBytes[0]);
                        if (i == N - 1)
                        {
                            this.packetsToSend.Enqueue(packet.ToArray());
                            packet.Clear();
                        }
                    }
                    else
                    {
                        this.packetsToSend.Enqueue(packet.ToArray());
                        packet.Clear();
                        packet.AddRange(packeBytes[0]);
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否可以封包
        /// </summary>
        /// <param name="list"></param>
        /// <param name="packeBytes"></param>
        /// <returns></returns>
        private bool IsPacketed(ref List<byte> list, byte[] packeBytes)
        {
            if (list.Count + packeBytes.Length < 1024)
            {
               list.AddRange(packeBytes);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 提取其他数据库
        /// </summary>
        /// <param name="dbName"></param>
        private void ExtractOtherOneDb(DataBaseName dbName)
        {
            var bll = new SelectOtherTablesBll();
            DataSet ds=null;
            log.Debug("提取其他数据库");
            try
            {
                ds = bll.Select(dbName);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            
            if (ds != null)
            {
                log.Debug("提取成功");
                foreach (DataTable dt in ds.Tables)
                {
                    try
                    {
                        List<Data> list = new List<Data>();
                        if (this.messagesShowEventHandler != null)
                        {
                            var msg = new StringBuilder();
                            msg.Append("提取").Append(dt.TableName).Append(dt.Rows.Count).Append("条数据");
                            log.Info(msg.ToString());
                            this.messagesShowEventHandler(
                                this,
                                new MessagesShowEventArgs { MessageType = MsgType.Info, MessagesShow = msg.ToString() });
                        }

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int index = 0;
                            bool flag = false;
                            bool ultraSetFlag = false;//@Modify20150107by yww
                            int flagCount = 0;
                            ConcurrentDictionary<int, ConfigTableInfo> dic = new ConcurrentDictionary<int, ConfigTableInfo>();
                            ConfigTableInfo config = new ConfigTableInfo();
                            if (
                                !string.IsNullOrEmpty(
                                    TableFieldInfoDic.GetTableFieldInfoDic()
                                     .GeTableFieldInfo((int)dbName.ID, dt.TableName)
                                     .OtherFlag))
                            {
                                index = 1;
                            }


                            if (ConfigTableDic.GetConfigTableDic().SelectList().Count > 0)
                            {
                                ConfigTable configtable = ConfigTableDic.GetConfigTableDic().SelecConfigTable((int)dbName.ID);
                                //ConfigTable configtable = ConfigTableDic.GetConfigTableDic().SelecConfigTable((int)dbName.ID, dt.TableName);
                                if (configtable != null)
                                {
                                    flag = true;
                                    flagCount = 1;
                                }
                                // 特殊处理(在有Sensor配置的时候直接拿SensorID做为模块号)
                                if (dt.TableName == "MainStreeData")
                                {
                                    ultraSetFlag = true;
                                    flag = false;
                                    flagCount = 1;
                                }
                            }
                            else
                            {
                                flag = false;
                                flagCount = 2;
                            }

                            foreach (DataRow row in dt.Rows)
                            {

                                try
                                {
                                    var data = new Data
                                    {
                                        ProjectCode = Convert.ToInt16(row[0]),
                                        DataBaseId = Convert.ToInt32(row[1]),
                                        SafeTypeId = Convert.ToInt32(row[2]),
                                        ChannelId = 1
                                    };
                                    data.DataSet = new List<double>();

                                    if (flag)
                                    {
                                        int sensorid = 0;
                                        bool identifyBySerialNo = false;
                                        string serialNo = "";
                                        if (row[4] != DBNull.Value)
                                        {
                                            if (int.TryParse(row[4].ToString(), out sensorid))
                                            {
                                                data.SensorId = sensorid;
                                                identifyBySerialNo = false;
                                            }
                                            else
                                            {
                                                serialNo = row[4].ToString();
                                                identifyBySerialNo = true;
                                            }
                                            //data.SensorId = Convert.ToInt32(row[4]);
                                            if (ConfigInfoTable.ConfigtableInfoDictionary.ContainsKey(data.DataBaseId))
                                            {
                                                dic = ConfigInfoTable.ConfigtableInfoDictionary[data.DataBaseId];
                                            }
                                            else
                                            {
                                                log.Error("字典中数据库ID关键字不存在");
                                                continue;
                                            }
                                            if (!identifyBySerialNo)
                                            {
                                                if (dic.ContainsKey(data.SensorId))
                                                {
                                                    config = dic[data.SensorId];
                                                }
                                                else
                                                {
                                                    log.Error("字典中传感器ID关键字不存在");
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                config = (from s in dic.Values
                                                          where StringComparer.OrdinalIgnoreCase.Compare(s.Otherflag, serialNo) == 0
                                                          select s).FirstOrDefault();
                                                if (config == null)
                                                {
                                                    log.Error("字典中传感器唯一标示符不存在");
                                                    continue;
                                                }
                                            }

                                            data.ChannelId = config.ChannelId;
                                            data.MoudleNo = config.MoudleNo;
                                        }
                                        else // 没有传感器ID
                                        {
                                            data.MoudleNo = "1";
                                        }
                                    }
                                    else
                                    {
                                        data.ChannelId = 1;
                                        if (!ultraSetFlag)
                                        {
                                            if (row[5] != DBNull.Value)
                                                data.ChannelId = Convert.ToInt32(row[5]);
                                        }
                                        if (row[4] != DBNull.Value)
                                            data.MoudleNo = row[4].ToString();
                                    }
                                    if (index == 1)
                                    {
                                        data.OFlag = Convert.ToInt32(row[6]);
                                    }

                                    // 采集时间转换问题，提供几种常见格式的时间转换
                                    DateTime acqtime = Convert.ToDateTime(row[3].ToString().Trim());
                                    //string timestr = ;
                                    //string[] timeformats =
                                    //    {
                                    //        "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff",
                                    //        "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.fff", "yyyyMMddHHmmss",
                                    //        "yyyyMMddHHmmss.fff","yyyy-MM-dd h:mm:ss","yyyy-M-d h:mm:ss"
                                    //    };
                                    //timestr
                                    //bool isSuccess = DateTime.TryParseExact(
                                    //    timestr,
                                    //    timeformats,
                                    //    CultureInfo.CurrentCulture,
                                    //    DateTimeStyles.None,
                                    //    out acqtime); //AssumeLocal
                                    //if (!isSuccess)
                                    //{
                                    //    try
                                    //    {
                                    //        acqtime = Convert.ToDateTime(timestr);
                                    //    }
                                    //    catch
                                    //    {
                                    //        log.Error("时间格式转换失败；" + timestr);
                                    //    }
                                    //}
                                    data.CollectTime = acqtime;

                                    for (int i = 4 + flagCount + index; i < row.ItemArray.Length; i++)
                                    {
                                        double value;

                                        double.TryParse(row[i].ToString().Trim(), out value);
                                        data.DataSet.Add(value);
                                    }
                                    list.Add(data);
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex.Message);
                                }
                            }
                            string strtime = dt.Compute("Max(ACQUISITION_DATETIME)", Boolean.TrueString).ToString();
                            ExtractionConfigDic.GetExtractionConfigDic()
                                .UpdateExtractionConfig(
                                    new ExtractionConfig
                                    {
                                        DataBaseId = (int)dbName.ID,
                                        TableName = dt.TableName,
                                        Acqtime = strtime
                                    });
                        }

                        //添加到队列
                        foreach (Data data in list)
                        {
                            datasTobesent.Enqueue(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                    }
                }
            }
            else
            {
                log.Debug("DataSet==null");
            }
        }

        private int GetStructureId(Data data)
        {
            foreach (int id  in structModuleNodic.Keys)
            {
                if (structModuleNodic[id].Contains(data.MoudleNo))
                {
                    return id;
                }
            }

            return -1;
        }



        /// <summary>
        /// 内部构建数据表（暂时无用）
        /// </summary>
        /// <returns></returns>
        private Dictionary<string,DataTable> CreateTables()
        {
            var file = new ExeConfigurationFileMap { ExeConfigFilename = "config/Params.config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            string tablestr = config.AppSettings.Settings["Tables"].Value;
            string[] tableNames = tablestr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var tables = new Dictionary<string,DataTable>();
            foreach (string tableName in tableNames)
            {
                string[] columnNames = config.AppSettings.Settings[tableName].Value.Split(
                    new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);
                var table = new DataTable();
                foreach (string columnName in columnNames)
                {
                    var column = new DataColumn(columnName);
                    table.Columns.Add(column);
                }
                if (!tables.ContainsKey(tableName))
                {
                    tables.Add(tableName, table);
                }
            }

            return tables;
        }

        #region 界面显示内容事件

        private event EventHandler<MessagesShowEventArgs> messagesShowEventHandler;

        public event EventHandler<MessagesShowEventArgs> MessagesShowEventHandler
        {
            add
            {
                messagesShowEventHandler += value;
            }

            remove
            {
                messagesShowEventHandler -= value;
            }
        }


        #endregion 界面显示内容事件

    }
}