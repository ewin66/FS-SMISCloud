// // --------------------------------------------------------------------------------------------
// // <copyright file="ConsumerConfig.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150318
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace Agg.Process
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Linq;

    using log4net;

    public abstract class ConsumerConfig
    {
        private static ILog Log = LogManager.GetLogger("ConsumerConfig");
        public static Dictionary<string, Type> FromXML(string fileName)
        {
            Dictionary<string, Type> ret = new Dictionary<string, Type>();
            if (!System.IO.File.Exists(fileName))
            {
                Log.Warn("ResultConsumers.xml not found");
                return ret;
            }

            try
            {
                var doc = XDocument.Load(fileName);
                var nodes = doc.Root.Element("Consumers").Elements();
                foreach (var node in nodes)
                {
                    var name = node.Attribute("name").Value;
                    var assembly = node.Attribute("assembly").Value;
                    var consumerType = node.Attribute("module").Value;
                    var type = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\" + assembly).GetType(consumerType);
                    ret.Add(name, type);
                }

            }
            catch (Exception e)
            {

                Log.Warn("ResultConsumers.xml parse error", e);
            }
            return ret;
        }
    }
}