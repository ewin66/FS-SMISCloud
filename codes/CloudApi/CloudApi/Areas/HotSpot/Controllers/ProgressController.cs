
namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.HotSpot.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Authorization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Net;



    public class ProgressController : ApiController
    {
        /// <summary>
        /// 获取结构物下的进度信息struct/{structId}/constructInfo/list
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 施工进度列表 </returns>
        [AcceptVerbs("Get")]
        [Authorization(AuthorizationCode.S_Structure_Construct)]
        [Authorization(AuthorizationCode.U_Common)]
        public object GetScheduleConfig(int structId)
        {

            using (SecureCloud_Entities entities = new SecureCloud_Entities())
            {
                //关联表    
                var queryMaxProgress = from p in entities.T_DIM_STRUCTUER_PROGRESS
                                    group p by p.Line_Id into g
                                    select new
                                    {   
                                        g.Key,
                                        MaxDate = g.Max(p => p.Up_Datatime)
                                    };
                var queryProgress = from p in entities.T_DIM_STRUCTUER_PROGRESS
                                    from q in queryMaxProgress
                                    where p.Line_Id == q.Key && p.Up_Datatime == q.MaxDate
                                    select p;
    
                var query = from s in entities.T_DIM_STRUCTUER_LINE
                            join q in queryProgress
                            on s.Id equals q.Line_Id
                            into pro
                            from p in pro.DefaultIfEmpty()
                            where s.Structure_Id == structId
                            
                            select new
                            {

                                LineId = s.Id,//线路编号id
                                LineName = s.Line_Name,
                                LineLength = s.Line_Length,
                                ConstructLength = (decimal?)p.Construct_Length,//转换为空的
                                StartId = s.Start_Id,
                                EndId = s.End_Id,
                                DataTime = (DateTime?)p.Up_Datatime,//转换为空的
                                //需要变动
                                ProgressId = (int?)p.Id,
                                Unit1 = s.Unit,
                                Unit2 = p.Unit,
                                Color = s.Color

                            };

                var list = query.ToList();//排序

                return list;
                   
            }
        }

        /// <summary>
        /// 添加施工线路struct/{structId}/constructLine /add
        /// </summary>
        /// <param name="config">施工线路配置</param>
        /// <returns>添加结果</returns>
        [AcceptVerbs("Post")]
        [Authorization(AuthorizationCode.S_Structure_Construct_Route_Add)]
        public HttpResponseMessage AddScheduleConfig([FromUri] int structId, [FromBody] LineConfig config)
        {

            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    IQueryable<T_DIM_STRUCTUER_LINE> ScheduleConfig = from q in db.T_DIM_STRUCTUER_LINE
                                                                      where q.Id == config.LineId && q.Line_Name == config.LineName
                                                                      && q.Line_Length == config.LineLength && q.Start_Id == config.StartId
                                                                      && q.End_Id == config.EndId && q.Structure_Id == config.structureId
                                                                      select q;
                    if (ScheduleConfig != null && ScheduleConfig.Count() != 0)
                    { //判断是否有重复

                        //#region 日志信息
                        //this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                        //this.Request.Properties["ActionParameterShow"]
                        //    = string.Format("线路编号：{0}，线路名称：{1},线路长度：{2},开始位置Id：{3},结束位置Id：{4},结构物Id:{5}",
                        //    config.LineId,
                        //    config.LineName,
                        //    config.LineLength,
                        //    config.StartId,
                        //    config.EndId,
                        //    config.structureId);

                        //#endregion
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, StringHelper.GetMessageString("线路配置已存在"));

                    }



                    var us = new T_DIM_STRUCTUER_LINE
                    {
                        Id = config.LineId,
                        Line_Name = config.LineName,
                        Line_Length = config.LineLength,
                        Start_Id = config.StartId,
                        End_Id = config.EndId,
                        Structure_Id = config.structureId,
                        Color = config.Color ?? null,
                        Unit = config.Unit
                    };
                    db.T_DIM_STRUCTUER_LINE.Add(us);
                    db.SaveChanges();
                    
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("线路配置添加成功"));

                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路配置添加失败"));

                }
            }
        }

        /// <summary>
        /// 修改线路配置信息  constructLine /modify/{LineId}
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Post")]

        [Authorization(AuthorizationCode.S_Structure_Construct_Route_Modify)]
        public HttpResponseMessage ModifyScheduleConfig([FromUri] int LineId, [FromBody] LineConfig config)
        {
            using (var db = new SecureCloud_Entities())
            {
                var paraShow = new StringBuilder(100);
                try
                {
                    var configEntity = db.T_DIM_STRUCTUER_LINE.FirstOrDefault(u => u.Id == LineId);
                    if (configEntity == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路配置不存在"));
                    }
                    if (config.LineName != default(string) && config.LineName != configEntity.Line_Name)
                    {
                        configEntity.Line_Name = config.LineName;
                        paraShow.AppendFormat("线路名称改为：{0}，", config.LineName);
                    }
                    if (config.LineLength != default(decimal) && config.LineLength != configEntity.Line_Length)
                    {
                        configEntity.Line_Length = config.LineLength;
                        paraShow.AppendFormat("线路长度：{0}，", config.LineLength);
                    }


                    if (config.StartId != default(string) && config.StartId != configEntity.Start_Id)
                    {
                        configEntity.Start_Id = config.StartId;
                        paraShow.AppendFormat("开始位置Id改为：{0}，", config.StartId);
                    }

                    if (config.EndId != default(string) && config.EndId != configEntity.End_Id)
                    {
                        configEntity.End_Id = config.EndId;
                        paraShow.AppendFormat("结束位置Id改为：{0}，", config.EndId);
                    }

                    var entry = db.Entry(configEntity);
                    entry.State = System.Data.EntityState.Modified;

                    if (config.Unit != default(string) && config.Unit != configEntity.Unit)
                    {
                        configEntity.Unit = config.Unit;
                        paraShow.AppendFormat("长度单位改为：{0}，", config.Unit);
                    }

                    if (config.Color != default(string) && config.Color != configEntity.Color)
                    {
                        configEntity.Color = config.Color;
                        paraShow.AppendFormat("颜色改为：{0}，", config.Color);
                    }


                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("线路配置信息修改成功"));

                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路配置信息修改失败"));
                }
            }

        }



        /// <summary>
        /// 删除施工线路信息  constructLine /remove/{LineId}
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Post")]

        [Authorization(AuthorizationCode.S_Structure_Construct_Route_Modify)]
        public HttpResponseMessage RemoveScheduleConfig([FromUri] int LineId)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var config = db.T_DIM_STRUCTUER_LINE.FirstOrDefault(u => u.Id == LineId);

                    if (config == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路配置不存在"));
                    }

                    IQueryable<T_DIM_STRUCTUER_LINE> Line = from q in db.T_DIM_STRUCTUER_LINE
                                                            where q.Id == LineId
                                                            select q;
                    foreach (var LineConfig in Line)
                    {
                        db.T_DIM_STRUCTUER_LINE.Remove(LineConfig);
                    }

                    //表T_DIM_STRUCTUER_PROGRESS的同步变动
                    IQueryable<T_DIM_STRUCTUER_PROGRESS> Progress = from p in db.T_DIM_STRUCTUER_PROGRESS
                                                                    where p.Line_Id == LineId
                                                                    select p;
                    foreach (var ProgressConfig in Progress)
                    {
                        db.T_DIM_STRUCTUER_PROGRESS.Remove(ProgressConfig);
                    }


                    //#region 日志信息
                    //this.Request.Properties["ActionParameter"] = "Id:" + LineId;
                    //this.Request.Properties["ActionParameterShow"] =
                    //    string.Format("线路编号：{0}， 线路名称： {1},结构物编号:{2}",
                    //    config.Id,
                    //    config.Line_Name,
                    //    config.Structure_Id);
                    //#endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("线路配置删除成功"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路配置删除失败"));
                }
            }
        }


        /// <summary>
        /// 获取结构物下的进度信息列表struct/{lineId}/progressInfo/list
        /// </summary>
        /// <param name="lineId"> 线路编号 </param>
        /// <returns> 施工进度列表 </returns>
        [AcceptVerbs("Get")]

        public object GetProgressConfig(int lineId)
        {


            using (SecureCloud_Entities entities = new SecureCloud_Entities())
            {


                var query = from s in entities.T_DIM_STRUCTUER_LINE
                            from p in entities.T_DIM_STRUCTUER_PROGRESS

                            where p.Line_Id == lineId && s.Id == lineId
                            select new
                            {
                                ProgressId=p.Id,
                                LineId = p.Line_Id,//线路编号id
                                LineName = s.Line_Name,
                                LineLength = s.Line_Length,
                                ConstructLength = (decimal?)p.Construct_Length,//转换为空的                                
                                DataTime = (DateTime?)p.Up_Datatime,//转换为空的                                
                                Unit1 = p.Unit,
                                Unit2 = s.Unit,


                            };
                var list = query.ToList();//排序
                return list;

            }
        }


        /// <summary>
        /// 获取当前进度信息struct/{lineId}/{progressId}/progressInfo
        /// </summary>
        /// <param name="lineId"> 线路编号 </param>
        /// <returns> 施工进度信息 </returns>
        [AcceptVerbs("Get")]

        public object GetNowProgressConfig(int lineId, int progressId)
        {

            using (SecureCloud_Entities entities = new SecureCloud_Entities())
            {
                
                

                var query = from s in entities.T_DIM_STRUCTUER_LINE
                            
                            from q in entities.T_DIM_STRUCTUER_PROGRESS

                            where q.Id == progressId && s.Id == lineId

                            select new
                            {                               
                                LineName = s.Line_Name,
                                LineLength = s.Line_Length,
                                ConstructLength = (decimal?)q.Construct_Length,//转换为空的                                
                                DataTime = q.Up_Datatime,//转换为空的                                
                                Unit1 = s.Unit,                                
                            };

                var list = query.ToList();//排序

                return list;

            }
        }
        /// <summary>
        /// 新增线路进度 struct/{lineId }/progress/add
        /// </summary>
        /// <param name="config">施工线路进度配置</param>
        /// <returns>添加结果</returns>
        [AcceptVerbs("Post")]

        public HttpResponseMessage addProgressConfig([FromUri] int lineId, [FromBody] ProgressConfig config)
        {

            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    IQueryable<T_DIM_STRUCTUER_PROGRESS> ProgressConfig = from q in db.T_DIM_STRUCTUER_PROGRESS
                                                                          where q.Line_Id == config.LineId
                                                                          && q.Construct_Length == config.ConstructLength
                                                                          && q.Unit == config.Unit
                                                                          && q.Up_Datatime == config.dataTime

                                                                          select q;
                    if (ProgressConfig != null && ProgressConfig.Count() != 0)
                    { //判断是否有重复

                        //#region 日志信息
                        //this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                        //this.Request.Properties["ActionParameterShow"]
                        //    = string.Format("进度编号:{0},线路编号：{1}，施工长度:{2},长度单位:{3},配置时间:{4},进度颜色:{5}",
                        //    config.Id,
                        //    config.LineId,
                        //    config.ConstructLength,
                        //    config.Unit,
                        //    config.dataTime,
                        //    config.Color);

                        //#endregion
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, StringHelper.GetMessageString("线路配置已存在"));

                    }

                    var us = new T_DIM_STRUCTUER_PROGRESS
                    {
                        Id = config.Id,
                        Line_Id = config.LineId,
                        Construct_Length = config.ConstructLength,
                        Unit = config.Unit,
                        Up_Datatime = config.dataTime,


                    };
                    db.T_DIM_STRUCTUER_PROGRESS.Add(us);
                    db.SaveChanges();
                    //#region 日志信息
                    //this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(us);
                    //this.Request.Properties["ActionParameterShow"]
                    //    = string.Format("线路编号：{0}，施工长度:{1},长度单位:{2},配置时间:{3},进度颜色:{4}",
                    //            us.Line_Id,
                    //            us.Construct_Length,
                    //            us.Unit,
                    //            us.Up_Datatime,
                    //            us.Color);

                    //#endregion
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("线路进度添加成功"));

                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路进度添加失败"));

                }
            }
        }


        /// <summary>
        /// 修改进度信息  progress /modify/{ progressId }
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        // [LogInfo("修改进度信息", true)]

        public HttpResponseMessage ModifyProgress([FromUri] int progressId, [FromBody] ProgressConfig config)
        {
            using (var db = new SecureCloud_Entities())
            {
                var paraShow = new StringBuilder(100);
                try
                {
                    var configEntity = db.T_DIM_STRUCTUER_PROGRESS.FirstOrDefault(u => u.Id == progressId);
                    if (configEntity == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路进度配置不存在"));
                    }
                    if (config.LineId != default(int) && config.LineId != configEntity.Line_Id)
                    {
                        configEntity.Line_Id = config.LineId;
                        paraShow.AppendFormat("线路编号改为：{0}，", config.LineId);
                    }
                    if (config.ConstructLength != default(decimal) && config.ConstructLength != configEntity.Construct_Length)
                    {
                        configEntity.Construct_Length = config.ConstructLength;
                        paraShow.AppendFormat("施工长度改为：{0}，", config.ConstructLength);
                    }

                    if (config.Unit != default(string) && config.Unit != configEntity.Unit)
                    {
                        configEntity.Unit = config.Unit;
                        paraShow.AppendFormat("长度单位改为：{0}，", config.Unit);
                    }

                    //if (config.Color != default(string) && config.Color != configEntity.Color)
                    //{
                    //    configEntity.Color = config.Color;
                    //    paraShow.AppendFormat("颜色改为：{0}，", config.Color);
                    //}

                    if (config.dataTime != default(DateTime) && config.dataTime != configEntity.Up_Datatime)
                    {
                        configEntity.Up_Datatime = config.dataTime;
                        paraShow.AppendFormat("修改时间改为：{0}，", config.dataTime);
                    }
                    var entry = db.Entry(configEntity);
                    entry.State = System.Data.EntityState.Modified;

                    //#region 日志信息
                    //this.Request.Properties["ActionParameter"] = JsonConvert.SerializeObject(config);
                    //this.Request.Properties["ActionParameterShow"] = paraShow.ToString();

                    //#endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("线路配置信息修改成功"));

                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路配置信息修改失败"));
                }
            }

        }


        /// <summary>
        /// 删除进度信息  progress/remove/{ progressId }
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Post")]


        public HttpResponseMessage RemoveProgress([FromUri] int progressId)
        {
            using (var db = new SecureCloud_Entities())
            {
                try
                {
                    var config = db.T_DIM_STRUCTUER_PROGRESS.FirstOrDefault(u => u.Id == progressId);

                    if (config == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路配置不存在"));
                    }

                    IQueryable<T_DIM_STRUCTUER_PROGRESS> progress = from q in db.T_DIM_STRUCTUER_PROGRESS
                                                                    where q.Id == progressId
                                                                    select q;
                    foreach (var ProgressConfig in progress)
                    {
                        db.T_DIM_STRUCTUER_PROGRESS.Remove(ProgressConfig);
                    }



                    //#region 日志信息
                    //this.Request.Properties["ActionParameter"] = "Line_Id:" + progressId;
                    //this.Request.Properties["ActionParameterShow"] =
                    //    string.Format("线路编号：{0}， 施工长度： {1}",
                    //    config.Line_Id,
                    //    config.Construct_Length);
                    //#endregion

                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Accepted, StringHelper.GetMessageString("线路进度配置删除成功"));
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, StringHelper.GetMessageString("线路进度配置删除失败"));
                }
            }
        }


        public class LineConfig
        {

            /// <summary>
            /// 线路编号
            /// </summary>
            [JsonProperty("LineId")]
            public int LineId { get; set; }

            /// <summary>
            /// 线路名称
            /// </summary>
            [JsonProperty("LineName")]
            public string LineName { get; set; }

            /// <summary>
            /// 线路长度
            /// </summary>
            [JsonProperty("LineLength")]
            public decimal LineLength { get; set; }

            /// <summary>
            /// 当前位置id
            /// </summary>
            [JsonProperty("StartId")]
            public string StartId { get; set; }

            /// <summary>
            /// 结束位置id
            /// </summary>
            [JsonProperty("EndId")]
            public string EndId { get; set; }

            /// <summary>
            /// 结构物编号
            /// </summary>
            [JsonProperty("structureId")]
            public int structureId { get; set; }

            /// <summary>
            /// 单位
            /// </summary>
            [JsonProperty("Unit")]
            public string Unit { get; set; }

            /// <summary>
            /// 颜色
            /// </summary>
            [JsonProperty("Color  ")]
            public string Color { get; set; }

        }

        public class ProgressConfig
        {
            /// <summary>
            /// 进度编号
            /// </summary>
            [JsonProperty("Id")]
            public int Id { get; set; }

            /// <summary>
            /// 线路编号
            /// </summary>
            [JsonProperty("LineId")]
            public int LineId { get; set; }

            /// <summary>
            /// 施工长度
            /// </summary>
            [JsonProperty("ConstructLength")]
            public decimal ConstructLength { get; set; }

            /// <summary>
            /// 单位
            /// </summary>
            [JsonProperty("Unit")]
            public string Unit { get; set; }


            /// <summary>
            /// 配置时间
            /// </summary>
            [JsonProperty("dataTime")]
            public DateTime dataTime { get; set; }

        }

    }
}
