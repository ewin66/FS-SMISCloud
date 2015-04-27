namespace FreeSun.FS_SMISCloud.Server.DataCalc
{
    using System;
    class ProtocolFactory
    {
        XmlHelper xmlConfig = new XmlHelper(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ProtocolConfig.xml");

        private static ProtocolFactory protocolFactory = null;
        public static ProtocolFactory getInstance()
        {
            if (protocolFactory == null)
            {
                protocolFactory = new ProtocolFactory();
            }
            return protocolFactory;
        }

        /// <summary>
        /// 根据协议号获取传感器实例类型
        /// </summary>
        /// <param name="i">协议号</param>
        /// <returns>传感器类型名称</returns>
        public string GetSensorType(int i)
        {
            string node = "//Moudles/Moudle[Id=" + i + "]";
            return "FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry." + xmlConfig.GetNodeValue(node, "Sensor");
        }

    }
}
