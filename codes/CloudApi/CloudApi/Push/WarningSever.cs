using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using PushSharp.Apple;
using PushSharp;
using PushSharp.Core;
using System.Configuration;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Push
{
    using System.Data;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;

    using log4net;
    using System.Reflection;

    public class WarningServer
    {
        private DAL.Warning warningDal = new DAL.Warning();
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 构建告警信息
        /// </summary>
        /// <param name="warnId">告警编号</param>
        /// <returns>告警推送信息</returns>
        public static WarningPushInfo BuildWarningInfo(int warnId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from w in db.T_WARNING_SENSOR
                            from wt in db.T_DIM_WARNING_TYPE
                            from s in db.T_DIM_STRUCTURE
                            where w.WarningTypeId == wt.TypeId && w.StructId == s.ID && w.Id == warnId
                            select
                                new WarningPushInfo
                                {
                                    WarningId = w.Id,
                                    Level = (int)(wt.WarningLevel ?? 5),
                                    WarningTime = (DateTime)w.Time,
                                    StructId = (int)s.ID,
                                    StructName = s.STRUCTURE_NAME_CN
                                };

                return query.FirstOrDefault();
            }
        }

        /// <summary>
        /// 告警推送（Android）
        /// </summary>
        /// <param name="warnId">告警编号</param>
        public void PushWarn(WarningPushInfo warning)
        {
            if (warning == null) return;

            DataTable dt = this.warningDal.GetUserByWarnId(warning.WarningId);
            foreach (var dataRow in dt.AsEnumerable())
            {
                int structId = warning.StructId;
                string user = dataRow.Field<string>("USER_NAME");
                string structure = warning.StructName;
                string level = string.Empty;
                switch (warning.Level)
                {
                    case 1: level = "一级"; break;
                    case 2: level = "二级"; break;
                    case 3: level = "三级"; break;
                    case 4: level = "四级"; break;
                }
                DateTime warningTime = warning.WarningTime;
                string sendAndroidMessage = structId + "-" + "用户名：" + user + "\n" + "结构物：" + structure + "\n" + "告警等级：" + level + "\n"
                                     + "产生时间：" + warningTime;
                string sendAppleMessage = "用户名：" + user + "\n" + "结构物：" + structure + "\n" + "告警等级：" + level + "\n"
                                     + "产生时间：" + warningTime;
                this.PushWarnToAndroid(user, sendAndroidMessage);
                this.PushWarnToApple(user, sendAppleMessage);
            }
        }

        public void PushWarnToAndroid(string user, string sendMessage)
        {
            string apiurl = ConfigurationManager.AppSettings["PushInterface"];
            string channelId = "C53a7a1cf22da8";
            string apiKey = "37c316cd83ec6aa80f8bc1f2e95df7bf";

            int random = new Random().Next();
            string md5Source = channelId + apiurl + "/" + user + sendMessage + random + apiKey;
            byte[] tempEncrypt = Encoding.UTF8.GetBytes(md5Source);
            byte[] resultEncrypt = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(tempEncrypt);
            string hash = BitConverter.ToString(resultEncrypt).Replace("-", "").ToLower();
            string sendurl = "http://www.push-notification.org/handlers/apns_v1.php?ch=" + channelId + "&devId=" + apiurl + "/" + user
                          + "&msg=" + System.Web.HttpUtility.UrlEncode(sendMessage, System.Text.Encoding.UTF8)
                          + "&random=" + random + "&hash=" + hash;

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(sendurl).Result;
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
            }
            else {
                log.Error("Push to Android failed!");
            }
        }

        public void PushWarnToApple(string user, string sendMessage)
        {
            var push = new PushBroker();

            push.OnNotificationSent += NotificationSent;
            push.OnChannelException += ChannelException;
            push.OnServiceException += ServiceException;
            push.OnNotificationFailed += NotificationFailed;
            push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            push.OnChannelCreated += ChannelCreated;
            push.OnChannelDestroyed += ChannelDestroyed;

            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Push\\PushSharp.PushCert.Development.p12"));
            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, "123456")); //Extension method

            DataTable dt = this.warningDal.GetDeviceTokenByUser(user);
            foreach (var dataRow in dt.AsEnumerable())
            {
                string deviceToken = dataRow.Field<string>("DeviceToken");
                log.Error(deviceToken);
                push.QueueNotification(new AppleNotification()
                                       .ForDeviceToken(deviceToken)
                                       .WithAlert(sendMessage)
                                       .WithBadge(1)
                                       .WithSound("sound.caf")
                );
            }

            push.StopAllServices();

        }

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, INotification notification)
        {
            Console.WriteLine("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine("Channel Created for: " + sender);
        }
    }
}