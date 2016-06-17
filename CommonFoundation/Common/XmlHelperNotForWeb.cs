using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommonFoundation.Common
{
    /// <summary>
    /// 非web应用调用类
    /// </summary>
    public class XmlHelperNotForWeb
    {
        private static string rootPath = AppDomain.CurrentDomain.BaseDirectory;
        private static string FilePath;
        public static XmlDocument XmlDoc;
        /// <summary>
        /// 加载XMLDOC
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static XmlDocument LoadXmlDocument(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            
            try
            {                
                xmlDoc.Load(filePath);
            }
            catch {
                xmlDoc = null ;
            }
            
            return xmlDoc;
        }

        public static void Save()
        {
            XmlDoc.Save(FilePath);
        }
        /// <summary>
        /// 得到根节点
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static XmlNode GetRoot(string filePath, string xpath)
        {
            XmlNode node = null;
            FilePath = filePath;
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(xpath))
            {
                XmlDoc = LoadXmlDocument(filePath);
                if (XmlDoc != null)
                {
                    node = XmlDoc.SelectSingleNode(xpath);
                }
            }
            return node;
        }
        /// <summary>
        /// 得到NodeList
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static XmlNodeList GetNodeList(XmlNode root, string xpath)
        {
            System.Xml.XmlNodeList nodelist = null;
            if (root != null && !string.IsNullOrEmpty(xpath))
            {
                nodelist = root.SelectNodes(xpath);
            }
            return nodelist;

        }

        /// <summary>
        /// 获得xml节点
        /// </summary>
        /// <returns></returns>
        public static XmlNode GetXmlNode(XmlNode rootNode, string nodePath, XmlNamespaceManager xmlnsManager)
        {
            XmlNode node = null;

            if (rootNode != null && !string.IsNullOrEmpty(nodePath))
            {
                node = xmlnsManager == null ? rootNode.SelectSingleNode(nodePath) : rootNode.SelectSingleNode(nodePath, xmlnsManager);
            }

            return node;
        }

        /// <summary>
        /// 获得xml节点内容
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static string GetXmlNodeText(XmlNode rootNode, string nodePath, XmlNamespaceManager xmlnsManager)
        {
            string ret = string.Empty;

            if (rootNode != null && !string.IsNullOrEmpty(nodePath))
            {
                XmlNode node = GetXmlNode(rootNode, nodePath, xmlnsManager);
                if (node != null)
                {
                    ret = node.InnerText;
                }
            }

            return ret;
        }
        /// <summary>
        /// 获得xml节点内容
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static string GetXmlNodeText(XmlNode rootNode, string nodePath)
        {
           if(rootNode!=null&&!string.IsNullOrEmpty(nodePath))
           {
                return GetXmlNodeText(rootNode, nodePath,null);
           }
            return null;
        }
    }
}
