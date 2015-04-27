using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sms.Controllers
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    using Newtonsoft.Json;

    public class SmsUserController : ApiController
    {
        /// <summary>
        /// 获取用户下的接收人列表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [LogInfo("获取用户下的短信接收人", false)]
        public object GetList(int userId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var query = from s in db.T_WARNING_SMS_RECIEVER
                            where s.UserNo == userId
                            select
                                new
                                    {
                                        receiverId = s.ReceiverId,
                                        receiverName = s.RecieverName,
                                        receiverPhone = s.RecieverPhone,
                                        receiverMail = s.RecieverMail,
                                        roleId = s.RoleId,
                                        filterLevel = s.FilterLevel,
                                        userId = s.UserNo,
                                        receiveMode = s.ReceiveMode
                                    };
                return query.ToList();
            }
        }

        [AcceptVerbs("Post")]
        [LogInfo("添加短信接收人", true)]
        public HttpResponseMessage Add([FromBody]SmsUser smsUser)
        {
            using (var db = new SecureCloud_Entities())
            {
                var receiver = new T_WARNING_SMS_RECIEVER
                                   {
                                       RecieverName = smsUser.ReceiverName,
                                       RecieverPhone = smsUser.ReceiverPhone,
                                       RecieverMail = smsUser.ReceiverMail,
                                       RoleId = smsUser.RoleId,
                                       FilterLevel = smsUser.FilterLevel,
                                       UserNo = smsUser.UserId,
                                       ReceiveMode = smsUser.ReceiveMode,
                                   };
                var entry = db.Entry(receiver);
                entry.State = System.Data.EntityState.Added;

                #region 日志信息

                this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(smsUser);
                this.Request.Properties["ActionParameterShow"] =
                    string.Format(
                        "接收人：{0},电话：{1},邮箱：{2},接收人角色：{3},接收等级：{4},接收模式：{5}",
                        smsUser.ReceiverName,
                        smsUser.ReceiverPhone,
                        smsUser.ReceiverMail,
                        smsUser.RoleId == 0 ? "用户" : "技术支持",
                        smsUser.FilterLevel,
                        smsUser.ReceiveMode ? "短信" : "邮箱");
                #endregion
                try
                {
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("添加成功"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("添加失败"));
                }
            }
        }

        [AcceptVerbs("Post")]
        [LogInfo("删除短信接收人", true)]
        public HttpResponseMessage Remove(int receiverId)
        {
            using (var db = new SecureCloud_Entities())
            {
                var smsUser = db.T_WARNING_SMS_RECIEVER.FirstOrDefault(r => r.ReceiverId == receiverId);

                #region 日志信息

                string sb = string.Empty;
                if (smsUser != null)
                {
                    sb = string.Format(
                        "接收人：{0},电话：{1},邮箱：{2},接收人角色：{3},接收等级：{4},接收模式：{5}",
                        smsUser.RecieverName,
                        smsUser.RecieverPhone,
                        smsUser.RecieverMail,
                        smsUser.RoleId == 0 ? "用户" : "技术支持",
                        smsUser.FilterLevel,
                        smsUser.ReceiveMode == null ? string.Empty : ((bool)smsUser.ReceiveMode ? "短信" : "邮箱"));
                }
                
                this.Request.Properties["ActionParameterShow"] = sb;
                    
                #endregion

                var entry = db.Entry(smsUser);
                entry.State = System.Data.EntityState.Deleted;
                try
                {
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("添加成功"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("添加失败"));
                }
            }
        }
    }

    public class SmsUser
    {       
        public string ReceiverName { get; set; }

        public string ReceiverPhone { get; set; }

        public int RoleId { get; set; }

        public int FilterLevel { get; set; }

        public int UserId { get; set; }

        public string ReceiverMail { get; set; }

        public bool ReceiveMode { get; set; }
    }
}
