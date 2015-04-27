// --------------------------------------------------------------------------------------------
// <copyright file="ServiceIocHelper.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20140902
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.Service
{
    using log4net;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;

    class ServiceIocHelper
    {
        private static ILog log = LogManager.GetLogger("SvrIocHelper");
        internal static IMessageBus FindMessageBus(string busPath)
        {
            string xmlFile;
            if (busPath == null)
            {
                busPath = "./";
            }
            xmlFile = busPath+@"ServiceBus.xml";
            string asmName, clsName;
            if (File.Exists(xmlFile))
            {
                log.InfoFormat("Loading ServiceBus from {0} ", xmlFile);
                TryGetFromXml(xmlFile, out asmName, out clsName);
            }
            else
            {
                log.InfoFormat("Loading ServiceBus using default: NetMQ, path={0}", busPath);
                asmName = "Service.NetMQ";
                clsName = "FS.Service.NetMQMessageBus";
            }
            return FindMessageBus(busPath, asmName, clsName);
        }


        static void TryGetFromXml(string file, out string asmName, out string clsName)
        {
            try
            {
                XDocument doc = XDocument.Load(file);
                XElement node = doc.Element("bus").Element("servicebus");
                asmName = node.Attribute("asm").Value; // Library Name(without ext);
                clsName = node.Attribute("impl").Value;   // Full Class Name(with namespace);                
            }
            catch (Exception e)
            {
                log.Error("Loading ServiceBus error: ", e);
                asmName = "Service.NetMQ";
                clsName = "FS.Service.NetMQMessageBus";
            }
        }
        internal static IMessageBus FindMessageBus(string dllPath, string asmName, string clsName)
        {
            IMessageBus bus = null;
            try
            {
                string fullAsmName = dllPath + asmName + ".dll";
                log.InfoFormat("Loading lib: {0}, class: {1} ", fullAsmName, clsName);
                Assembly asm = Assembly.LoadFile(fullAsmName);
                Type implClass = asm.GetType(clsName);
                Object obj = Activator.CreateInstance(implClass, null);
                bus = (IMessageBus)obj;
            }
            catch (Exception e)
            {
                log.Error("Loading ServiceBus error: ", e);
            }
            return bus;
        }
    }
}