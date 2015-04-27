using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace FreeSun.FS_SMISCloud.Server.DataCalc
{
    class XmlHelper
    {
        protected XmlDocument objXmlDoc;
        protected string strXmlFile;

        public XmlHelper()
        {
            objXmlDoc = new XmlDocument();
        }

        public XmlHelper(string XmlFile)
        {
            objXmlDoc = new XmlDocument();

            try
            {
                objXmlDoc.Load(XmlFile);

            }
            catch (System.Exception ex)
            {
                objXmlDoc = null;
                throw ex;
            }
            strXmlFile = XmlFile;
        }

        ~XmlHelper()
        {
            objXmlDoc = null;
        }

        public void NewXmlFile(string FileName, string strXml)
        {
            string fPath = Path.GetDirectoryName(FileName);

            if (!System.IO.Directory.Exists(fPath))
            {
                //目录不存在，首先需要创建此目录
                System.IO.Directory.CreateDirectory(fPath);
            }

            objXmlDoc = new XmlDocument();
            objXmlDoc.LoadXml(strXml);

            if (File.Exists(FileName))
            {
                File.SetAttributes(FileName, System.IO.FileAttributes.Normal);
            }

            objXmlDoc.Save(FileName);

            strXmlFile = FileName;

        }

        //根据指定的路径读取一个值
        public string GetNodeValue(string nodPath)
        {
            XmlNode gNode = objXmlDoc.SelectSingleNode(nodPath);

            if (gNode == null)
                return "";
            else
                return gNode.InnerText.ToString();
        }

        //根据指定的路径读取一个值
        public string GetNodeValue(string nodPath, string recNodPath)
        {
            XmlNode gNode = objXmlDoc.SelectSingleNode(nodPath);
            string s = "";
            if (gNode == null)
                return "";
            else
                s = gNode.LastChild.InnerText.Trim();
            //s= (gNode as XmlElement).Attributes[recNodPath].Value;
            return s;
        }

        public string GetNodeChildValue(string nodPath, string childNodPath)
        {
            XmlNode gNode = objXmlDoc.SelectSingleNode(nodPath);
            if (gNode == null)
                return String.Empty;
            foreach (XmlNode nod in gNode.ChildNodes)
            {
                if (StringComparer.OrdinalIgnoreCase.Compare(childNodPath, nod.Name) == 0)
                {
                    return nod.InnerText.Trim();
                }
            }
            return string.Empty;
        }

        public string GetNodeChildAttribute(string nodPath, string childNodPath, string attrName)
        {
            XmlNode gNode = objXmlDoc.SelectSingleNode(nodPath);
            if (gNode == null)
                return String.Empty;
            try
            {
                foreach (XmlNode nod in gNode.ChildNodes)
                {
                    if (StringComparer.OrdinalIgnoreCase.Compare(childNodPath, nod.Name) == 0)
                    {
                        return nod.Attributes[attrName].Value;
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        //根据节点返回数据,类型为dataview
        public DataView GetData(string XmlPathNode)
        {
            DataSet ds = new DataSet();
            StringReader read = new StringReader(objXmlDoc.SelectSingleNode(XmlPathNode).OuterXml);
            ds.ReadXml(read);
            if (ds.Tables.Count == 0)
            {
                return null;
            }
            else
            {
                return ds.Tables[0].DefaultView;
            }
        }

        //根据指定节点的编号,来返回节点内容,尽管返回的是一条记录,
        //但还是作为DataView进行返回,这样做的目的是为了更好的访问性
        public DataView GetData(string NodeCollection, string Node, string content)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name == Node && nodes[i].ChildNodes[j].InnerText == content)
                    {
                        StringReader read = new StringReader(nodes[i].OuterXml);
                        DataSet ds = new DataSet();
                        ds.ReadXml(read);
                        if (ds.Tables.Count == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return ds.Tables[0].DefaultView;
                        }

                    }
                }

            }
            return null;

        }

        //删除节点
        //根据指定的节点删除此节点以及此节点一下的内容
        public void DeleteNode(string Node)
        {
            XmlNodeList nodes = objXmlDoc.GetElementsByTagName(Node);
            XmlNode delNode = nodes[0];
            delNode.ParentNode.RemoveChild(delNode);
        }

        //根据指定的节点，删除子节点符合content内容的子节点
        //此方法比较特殊，是针对soukey采摘中特有的xml来进行操作，并非支持所有的xml文件
        //在soukey采摘中的xml文件中，通常都会有1对多的关系，这种关系通过一个可
        //重复的节点来表示，而在删除的时候，并非指定这个节点，而是指定这个节点的父节点
        //因为要循环集合中的内容，根据集合中的一个节点，中下的内容来进行删除
        //举例<tasks><task><id>1</id><name>soukey</name></task><task><id>2</id><name>采摘</name></task></tasks>
        //删除子节点是指删除task节点，但根据的条件是指定的id或者name符合content的内容，
        //所以调用方法是DeleteChildNodes("tasks","name","soukey")
        //调用后，将删除task中name＝soukey的task节点,传入的MainNode必须是一个集合，如果传入的是一个结合子节点，
        //将导致错误
        public void DeleteChildNodes(string NodeCollection, string Node, string content)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name == Node && nodes[i].ChildNodes[j].InnerText == content)
                    {
                        fathernode[0].RemoveChild(nodes[i]);
                        return;
                    }
                }

            }

        }

        //插入一个节点和此节点的一子节点
        public void InsertNode(string MainNode, string ChildNode, string[] Element, string[] Content)
        {
            XmlNode objRootNode = objXmlDoc.SelectSingleNode(MainNode);
            XmlElement objChildNode = objXmlDoc.CreateElement(ChildNode);
            objRootNode.AppendChild(objChildNode);
            if (Element.Length == Content.Length && Element.Length > 1)
            {
                for (int i = 0; i < Element.Length; i++)
                {
                    XmlElement objElement = objXmlDoc.CreateElement(Element[i]);
                    objElement.InnerText = Content[i];
                    objChildNode.AppendChild(objElement);
                }
            }
        }
        //修改一个节点包含的信息信息
        public void EditNode(string Element, string Old_Content, string Content)
        {

            XmlNodeList nodes = objXmlDoc.GetElementsByTagName(Element);

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (nodes[i].ChildNodes[0].InnerText == Old_Content)
                {
                    nodes[i].ChildNodes[0].InnerText = Content;
                }

            }
        }

        //修改一个节点本身的值
        public void EditNodeName(string nodPath, string OldName, string NewName)
        {
            XmlNode Nod = objXmlDoc.SelectSingleNode(nodPath);
            string xml = Nod.InnerXml;

            DeleteNode(OldName);

            nodPath = nodPath.Substring(0, nodPath.LastIndexOf("/"));

            InsertElement(nodPath, NewName, xml);

        }

        //根据指定的节点修改器值
        public void EditNodeValue(string nodPath, string NewValue)
        {
            XmlNode Nod = objXmlDoc.SelectSingleNode(nodPath);
            Nod.LastChild.InnerText = NewValue;
        }

        //插入一个节点，带一个属性
        public void InsertElement(string MainNode, string Element, string Attrib, string AttribContent, string Content)
        {
            XmlNode objNode = objXmlDoc.SelectSingleNode(MainNode);
            XmlElement objElement = objXmlDoc.CreateElement(Element);
            objElement.SetAttribute(Attrib, AttribContent);
            objElement.InnerText = Content;
            objNode.AppendChild(objElement);
        }

        //插入一个节点
        public void InsertElement(string MainNode, string Element, string Content)
        {
            XmlNode objNode = objXmlDoc.SelectSingleNode(MainNode);
            XmlElement objElement = objXmlDoc.CreateElement(Element);
            objElement.InnerXml = Content;
            objNode.AppendChild(objElement);
        }

        private readonly Object m_fileLock = new Object();
        //保存xml文件
        public void Save()
        {
            try
            {
                if (File.Exists(strXmlFile))
                {
                    File.SetAttributes(strXmlFile, System.IO.FileAttributes.Normal);
                }
                objXmlDoc.Save(strXmlFile);

            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        public void EditNodeValue(string NodeCollection, string Node, string condition, string ValueName, string value)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name == Node && nodes[i].ChildNodes[j].InnerText == condition)
                    {
                        XmlNode nod = nodes[i].SelectSingleNode(ValueName);
                        nod.InnerText = value;
                        return;
                    }
                }

            }

        }

        /// <summary>
        /// 修改时间记录，如果不存在自动创建
        /// </summary>
        public void EditTimeRecordNodeValueAndInsertIfNotExist(int structId, DateTime time)
        {
            string nodPath = "//Structs/Struct[Id=" + structId + "]";
            XmlNode gNode = objXmlDoc.SelectSingleNode(nodPath);
            if (gNode == null)
            {
                string mainNod = "//Structs";
                string childNod = "Struct";
                string[] elements = new string[] { "Id", "Desc", "Time" };
                string[] contents = new string[] { structId.ToString(CultureInfo.InvariantCulture), "DEF", time.ToString(CultureInfo.InvariantCulture) };
                InsertNode(mainNod, childNod, elements, contents);
            }
            else
            {
                try
                {
                    foreach (XmlNode nod in gNode.ChildNodes)
                    {
                        if (StringComparer.OrdinalIgnoreCase.Compare("Time", nod.Name) == 0)
                        {
                            nod.InnerText = time.ToString(CultureInfo.InvariantCulture);
                            return;
                        }
                    }
                }
                catch (Exception)
                {
                }
                var ele = objXmlDoc.CreateElement("Time");
                ele.InnerText = time.ToString(CultureInfo.InvariantCulture);
                gNode.AppendChild(ele);
            }
        }

        /// <summary>
        /// 选择所有结构物ID
        /// </summary>
        /// <returns></returns>
        public List<int> SelectAllStructs()
        {
            List<int> list = new List<int>();
            XmlNodeList nodes = objXmlDoc.SelectNodes("//Structs/Struct");
            foreach (XmlNode node in nodes)
            {
                foreach (XmlNode idnode in node.ChildNodes)
                {
                    if (idnode.Name == "Id")
                    {
                        list.Add(Convert.ToInt32(idnode.InnerText));
                        break;
                    }
                }
            }
            return list;
        }

        public Dictionary<int, string> GetStructWorkPaths()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            XmlNodeList nodes = objXmlDoc.SelectNodes("//Structs/Struct");
            foreach (XmlNode node in nodes)
            {
                string path = "";
                int structid = -1;
                foreach (XmlNode idnode in node.ChildNodes)
                {
                    if (idnode.Name == "Id")
                    {
                        structid = Convert.ToInt32(idnode.InnerText);
                    }
                    if (idnode.Name == "Path")
                    {
                        path = idnode.InnerText;
                    }
                }
                if (structid > 0 && !string.IsNullOrEmpty(path))
                {
                    res.Add(structid, path);
                }
            }
            return res;
        }
    }
}
