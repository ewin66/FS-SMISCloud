using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry;

namespace FreeSun.FS_SMISCloud.Server.DataCalc
{
    /// <summary>
    /// 记录计算时间
    /// </summary>
    static class ConfigHelper
    {
        public static DateTime GetRecordTime(int structId)
        {
            string path = "TimeRecord.xml";
            if (!AssertFileExist(path)) throw new Exception("时间记录文件不存在");
            XmlHelper xmlConfig = new XmlHelper(path);
            string node = "//Structs/Struct[Id=" + structId + "]";
            string strtime = xmlConfig.GetNodeChildValue(node, "Time");
            DateTime time;
            if (string.IsNullOrEmpty(strtime) || !DateTime.TryParse(strtime, out time)) // 没有有效记录，从当前时间开始记录(不进行历史数据提取)
            {
                return DateTime.Now;
            }
            else
            {
                return time;
            }
        }

        public static AccFilter GetAccFilter(int structId)
        {
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "TimeRecord.xml";
            if (!AssertFileExist(path)) throw new Exception("结构物配置文件不存在");
            XmlHelper xmlConfig = new XmlHelper(path);
            string node = "//Structs/Struct[Id=" + structId + "]";
            string strR = xmlConfig.GetNodeChildAttribute(node, "AccFilter", "R");
            string strP = xmlConfig.GetNodeChildAttribute(node, "AccFilter", "P");
            if (string.IsNullOrEmpty(strR) || string.IsNullOrEmpty(strP)) return null;
            float fR = 0.0f, fP = 0.0f;
            if (float.TryParse(strR, out fR) && float.TryParse(strP, out fP))
            {
                return new AccFilter(fR, fP);
            }
            return null;
        }

        public static void SetRecordTime(int structId, DateTime time)
        {
            string path = "TimeRecord.xml";
            if (!AssertFileExist(path)) throw new Exception("时间记录文件不存在");
            XmlHelper xmlConfig = new XmlHelper(path);
            xmlConfig.EditTimeRecordNodeValueAndInsertIfNotExist(structId, time);
            xmlConfig.Save();
        }

        private static bool AssertFileExist(string path)
        {
            if (!File.Exists(path))
            {
                try
                {
                    var fs = File.Create(path);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                    return true;
                }
                catch { /* failed create*/ }
                return false;
            }
            return true;
        }

        public static int[] GetStructs()
        {
            string path = "TimeRecord.xml";
            if (!AssertFileExist(path)) throw new Exception("时间记录文件不存在");
            XmlHelper xmlConfig = new XmlHelper(path);
            return xmlConfig.SelectAllStructs().ToArray();
        }

        public static Dictionary<int, string> GetStructWorkPaths()
        {
            string path = "TimeRecord.xml";
            if (!AssertFileExist(path)) throw new Exception("时间记录文件不存在");
            XmlHelper xmlConfig = new XmlHelper(path);
            return xmlConfig.GetStructWorkPaths();
        }
    }
}
