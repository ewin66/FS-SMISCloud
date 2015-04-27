// --------------------------------------------------------------------------------------------
// <copyright file="ServiceConfig.cs" company="江苏飞尚安全监测咨询有限公司">
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
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class ServiceConfig
    {
        private static ILog log = LogManager.GetLogger("ServiceConfig");

        public QueueLinks PubSub { get; set; }
        public QueueLinks PushPull { get; set; }
        public QueueLinks ReqRep { get; set; }
        public Dictionary<string, string> Properties = null;

        public ServiceConfig()
        {
            this.Properties = new Dictionary<string, string>();
        }

        public static ServiceConfig FromXml(string file)
        {
            ServiceConfig sc = new ServiceConfig();
            try
            {
                log.InfoFormat("Parsing '{0}' ...", file);
                XDocument doc = XDocument.Load(file);
                var root = doc.Root;
                var psNode = root.Element("ps");
                if (psNode != null)
                {
                    var PubSub = new QueueLinks();
                    sc.PubSub = PubSub;
                    var pubNode = psNode.Element("publisher");
                    PubSub.ListenAddress = pubNode.Attribute("address").Value;
                    foreach (var node in psNode.Element("subscribe").Elements())
                    {
                        XAttribute addAttr = node.Attribute("address");
                        PubSub.Links.Add(new LinkAddress { Name = node.Attribute("name").Value, Address = addAttr != null ? addAttr.Value : null });
                    }
                }
                 var ppNode = root.Element("pp");
                 if (ppNode != null)
                 {
                     var pp = new QueueLinks();
                     sc.PushPull = pp;
                     var pullNode = ppNode.Element("pull");
                     pp.ListenAddress = pullNode.Attribute("address").Value;
                     foreach (var node in ppNode.Element("push").Elements())
                     {
                         XAttribute addAttr = node.Attribute("address");
                         pp.Links.Add(new LinkAddress { Name = node.Attribute("name").Value, Address = addAttr != null ? addAttr.Value : null });
                     }
                 }

                 var pairNode = root.Element("rr");
                 if (pairNode != null)
                 {
                     var rr = new QueueLinks();
                     sc.ReqRep = rr;
                     var repNode = pairNode.Element("response");
                     rr.ListenAddress= repNode.Attribute("address").Value;
                     foreach (var node in pairNode.Element("request").Elements())
                     {
                         XAttribute addAttr = node.Attribute("address");
                         rr.Links.Add(new LinkAddress { Name = node.Attribute("name").Value, Address = addAttr != null ? addAttr.Value : null });
                     }
                 }

                 var settingNode = root.Element("settings");
                 if (settingNode != null)
                 {
                     var settingsNodes = root.Element("settings").Elements();
                     foreach (var node in settingsNodes)
                     {
                         sc.Properties.Add(node.Attribute("key").Value, node.Attribute("value").Value);
                     }
                 }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return sc;
        }

        public string GetProperty(string propertyName)
        {
            if (!this.Properties.ContainsKey(propertyName))
            {
                return null;
            }
            return this.Properties[propertyName];
        }
    }

    public class QueueLinks
    {
        public string ListenAddress { get; set; }
        public IList<LinkAddress> Links { get; set; }
        public QueueLinks()
        {
            this.Links = new List<LinkAddress>();
        }

        public LinkAddress FindLinkAddress(string service)
        {
            foreach (LinkAddress ss in Links)
            {
                if (ss.Name == service)
                {
                    return ss;
                }
            }
            return null;
        }
    }
    
    public class LinkAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }

    }
 
}