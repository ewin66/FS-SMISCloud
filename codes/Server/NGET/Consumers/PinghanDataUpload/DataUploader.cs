namespace FS.SMIS_Cloud.NGET.PinghanDataUpload
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGET.Consumer;
    using FS.SMIS_Cloud.NGET.Model;
    using FS.SMIS_Cloud.Services.ConsoleCtrlManager;

    using log4net;

    using Newtonsoft.Json;

    using DbType = FS.DbHelper.DbType;

    internal enum Switch
    {
        Off,
        On
    };

    public class UploadConfig
    {
        public string Url { get; set; }
        public int TimeOut { get; set; }
        public int EnableException { get; set; }
        public string ExceptionUrl { get; set; }
        public int EnableDebug { get; set; }
    }

    public class UploadItem
    {
        [JsonIgnore]
        public uint Id { get; set; }

        [JsonIgnore]
        public int Enable { get; set; }

        public string BDBH { get; set; } //	标段号	Varchar(20)	

        public string SDMC { get; set; } // 隧道名称	varchar(50)	

        public string SDZH { get; set; } //	里程桩号	varchar(20)	

        public string ZYF { get; set; } // 左右幅	Varchar(10)	左|右

        public string SBWZ { get; set; } // 设备位置	varchar(20)	

        public string SBSM { get; set; } // 设备说明	varchar(50)	

        public double? GDCJ_SL { get; set; } // 拱顶沉降/收敛(mm)	decimal(20,4)

        public string CJSJ { get; set; } // 采集时间  varchar(20)	格式为yyyy-mm-dd hh:mm:ss

        public string SJLX { get; set; } // 数据类别	varchar(2)	SL|CJ表示收敛|沉降

        public string FHDZ { get; set; } // 返回地址	Varchar(200)	不合格数据点击时的链接地址
    }

    public class DataUploader : IDacTaskResultConsumer
    {
        private readonly ILog _log = LogManager.GetLogger("DataUploader");
        public UploadConfig Config { private set; get; }

        public SensorType[] SensorTypeFilter { get; set; }

        public DataUploader()
        {
            this.Config = this.ReloadBasicConfig();
            ConsoleCtrlManager.Instance.RegCmd("pud", this.UploadHandler);
        }

        public string UploadHandler(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                return this.CmdHelp();
            }
            try
            {
                var id = Convert.ToUInt32(args[0]);
                var begin = DateTime.Parse(args[1]);
                var end = DateTime.Parse(args[2]);
                this.Upload(id, begin, end);
                return "Finished.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("\terror: {0}", ex.Message);
                return this.CmdHelp();
            }
        }

        private void Upload(uint id, DateTime begin, DateTime end)
        {
            var items = this.ReloadSensorConfig();
            if (!items.ContainsKey(id))
            {
                Console.WriteLine("Sensor {0} not exist in config file.", id);
            }
            else
            {
                var item = items[id];
                var cs = ConfigurationManager.AppSettings["SecureCloud"];
                var sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);

                var sql = @"";
                switch (item.SJLX)
                {
                    case "SL":
                        sql = string.Format(@"
SELECT SENSOR_ID,SAFETY_FACTOR_TYPE_ID,CRACK_VALUE AS VALUE,ACQUISITION_DATETIME
FROM T_THEMES_DEFORMATION_CRACK
WHERE SENSOR_ID = {0} and (ACQUISITION_DATETIME >= '{1:yyyy-MM-dd HH:mm:ss}' and ACQUISITION_DATETIME <='{2:yyyy-MM-dd HH:mm:ss}')",
                            item.Id, begin, end);
                        break;
                    case "CJ":
                        sql = string.Format(@"
SELECT SENSOR_ID,SAFETY_FACTOR_TYPE_ID,SETTLEMENT_VALUE AS VALUE,ACQUISITION_DATETIME
FROM T_THEMES_DEFORMATION_SETTLEMENT
WHERE SENSOR_ID = {0} and (ACQUISITION_DATETIME >= '{1:yyyy-MM-dd HH:mm:ss}' and ACQUISITION_DATETIME <='{2:yyyy-MM-dd HH:mm:ss}')",
                            item.Id, begin, end);
                        break;
                    default:
                        sql = @"";
                        break;
                }
                if (sql == "")
                {
                    Console.WriteLine("\titem type error: {0}", item.SJLX);
                    return;
                }
                try
                {
                    var dt = sqlHelper.Query(sql).Tables[0];
                    foreach (DataRow data in dt.Rows)
                    {
                        var request = this.CreateRequest(data, item);
                        Console.WriteLine("\tupload item: {0}", request);
                        var result = this.UploadRequest(request);
                        Console.WriteLine("\tresult: {0}", result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\terror: {0}", ex.Message);
                }
            }
        }

        private string CreateRequest(DataRow data, UploadItem item)
        {
            item.GDCJ_SL = Convert.ToDouble(data["VALUE"]);
            item.FHDZ = "";
            item.CJSJ = DateTime.Parse(data["ACQUISITION_DATETIME"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            return JsonConvert.SerializeObject(new[] {item});
        }

        private string UploadRequest(string requestUrl)
        {
            this._log.DebugFormat("upload item: {0}", requestUrl);
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(this.Config.Url);
                request.Method = "POST";
                request.Timeout = this.Config.TimeOut;
                request.ContentType = "application/x-www-form-urlencoded";
                var load = "params.jsondata=" + requestUrl;
                //DEBUG return;
                if (this.Config.EnableDebug == (int) Switch.On) return load;
                var requestWriter = new StreamWriter(request.GetRequestStream());
                requestWriter.Write(load);
                requestWriter.Close();
                var response = (HttpWebResponse) request.GetResponse();
                var myResponseStream = response.GetResponseStream();
                if (myResponseStream == null)
                {
                    this._log.Error("Failed to upload item.");
                    return "Failed.";
                }
                var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                var message = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return message;
            }
            catch (Exception e)
            {
                this._log.Error("Failed to upload item", e);
                return e.Message;
            }
        }

        private UploadConfig ReloadBasicConfig()
        {
            var config = new UploadConfig();
            if (File.Exists("upload.xml"))
            {
                try
                {
                    var doc = XDocument.Load("upload.xml");
                    var root = doc.Root;
                    //basic
                    config.Url = root.Attribute("url").Value;
                    config.TimeOut = Convert.ToInt32(root.Attribute("timeout").Value);
                    config.EnableException = Convert.ToInt16(root.Attribute("enableexception").Value);
                    config.ExceptionUrl = root.Attribute("exceptionurl").Value;
                    config.EnableDebug = Convert.ToInt16(root.Attribute("debug").Value);
                }
                catch (Exception ex)
                {
                    this._log.Error("Failed to load config file.", ex);
                }
            }
            else
            {
                this._log.Error("Config file not exist.");
            }
            return config;
        }

        public Dictionary<uint, UploadItem> ReloadSensorConfig()
        {
            var items = new Dictionary<uint, UploadItem>();
            if (File.Exists("upload.xml"))
            {
                try
                {
                    var doc = XDocument.Load("upload.xml");
                    var root = doc.Root;
                    //items
                    var sensors = root.Elements("sensor");
                    foreach (var sensor in sensors)
                    {
                        var item = new UploadItem
                        {
                            Enable = Convert.ToInt16(sensor.Attribute("enable").Value),
                            Id = Convert.ToUInt32(sensor.Attribute("id").Value),
                            BDBH = sensor.Attribute("BDBH").Value,
                            SDMC = sensor.Attribute("SDMC").Value,
                            SDZH = sensor.Attribute("SDZH").Value,
                            ZYF = sensor.Attribute("ZYF").Value,
                            SBWZ = sensor.Attribute("SBWZ").Value,
                            SBSM = sensor.Attribute("SBSM").Value,
                            SJLX = sensor.Attribute("SJLX").Value
                        };
                        items[item.Id] = item;
                    }
                }
                catch (Exception ex)
                {
                    this._log.Error("Failed to load config file.", ex);
                }
            }
            else
            {
                this._log.Error("Config file not exist.");
            }
            return items;
        }

        public string CmdHelp()
        {
            return
                string.Format(
                    "Usage:\r\n\tpud id timebegin timeend\r\n\texample: pud 1755 \"2014-12-02 17:13:00\" \"2014-12-03 17:13:00\"");
        }

        public void ProcessResult(List<SensorAcqResult> source)
        {
            var items = this.ReloadSensorConfig();
            foreach (var sensorResult in source)
            {
                if (sensorResult.IsOK && sensorResult.Data != null)
                {
                    if (items.ContainsKey(sensorResult.Sensor.SensorID)) //是需要上传的数据
                    {
                        try
                        {
                            var item = items[sensorResult.Sensor.SensorID];
                            //传感器已经关闭上报
                            if (item.Enable == (int) Switch.Off) return;
                            var request = this.CreateRequest(sensorResult, item, sensorResult.AcqTime);
                            //不上报异常
                            if (item.FHDZ != "" && this.Config.EnableException == (int) Switch.Off) return;
                            var result = this.UploadRequest(request);
                            this._log.DebugFormat("\tresult: {0}", result);
                        }
                        catch (Exception ex)
                        {
                            this._log.ErrorFormat("\tresult: {0}", ex.Message);
                        }
                    }
                }
            }
        }

        public string CreateExceptionUrl(UploadItem item)
        {
            var structId = QueryStructIdBySensor((int)item.Id) ?? 82;
            var themeType = 2; //主题
            var factorType = item.SJLX == "CJ" ? 40 : 41; //监测因素
            return string.Format(this.Config.ExceptionUrl, structId, themeType, factorType, item.Id);
        }

        private int? QueryStructIdBySensor(int sensorId)
        {
            string sql = string.Format(@"
SELECT [STRUCT_ID]
  FROM [T_DIM_SENSOR]
  WHERE [SENSOR_ID]={0}", sensorId);

            var cs = ConfigurationManager.AppSettings["SecureCloud"];
            var sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            var tb = sqlHelper.Query(sql, null).Tables[0];
            if (tb.Rows.Count > 0)
            {
                return Convert.ToInt32(tb.Rows[0][1]);
            }

            return null;
        }

        private string CreateRequest(SensorAcqResult sensorResult, UploadItem item, DateTime acqTime)
        {
            if (sensorResult.Data.ThemeValues[0] != null || sensorResult.IsOK)
            {
                item.GDCJ_SL = sensorResult.Data.ThemeValues[0];
                item.FHDZ = "";
            }
            else
            {
                //数据异常就记为0
                item.GDCJ_SL = 0;
                item.FHDZ = this.CreateExceptionUrl(item);
            }

            //item.CJSJ = sensorResult.Data.AcqTime.ToString("yyyy-MM-dd HH:mm:ss");
            item.CJSJ = acqTime.ToString("yyyy-MM-dd HH:mm:ss");
            if (item.SBWZ == string.Empty)
            {
                item.SBWZ = sensorResult.Sensor.Name;
            }
            return JsonConvert.SerializeObject(new[] {item});
        }
    }
}
