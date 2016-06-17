using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace CommonFoundation.Common
{
    public class XmlHelper
    {

        #region 命名空间
        /// <summary>
        /// 创建xml命名空间
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static XmlNamespaceManager CreateXmlNameSpaceMgr(XmlDocument doc)
        {
            XmlNamespaceManager xmlnsManager = null;
            if (doc != null)
            {
                xmlnsManager = new XmlNamespaceManager(doc.NameTable);
                xmlnsManager.AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");
            }
            return xmlnsManager;
        }
        #endregion

        #region 创建文档
        /// <summary>
        /// 创建xml文档
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static XmlDocument CreateXmlDocument(string str)
        {
            XmlDocument doc = null;
            if (!string.IsNullOrEmpty(str))
            {
                doc = new XmlDocument();
                doc.LoadXml(str);
            }
            return doc;
        }
        #endregion

        #region 创建xsl
        /// <summary>
        /// 创建XslTransform
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XslTransform CreateXslTransform(string path)
        {
            XslTransform xslt = null;

            if (!string.IsNullOrEmpty(path))
            {
                xslt = new XslTransform();
                xslt.Load(HttpContext.Current.Server.MapPath(path));
            }

            return xslt;
        }
        #endregion

        #region 获取
        /// <summary>
        /// 获得xml节点
        /// </summary>
        /// <param name="docPath"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static XmlNode GetRootNode(string docPath, string nodePath)
        {
            XmlNode ret = null;
            if (!string.IsNullOrEmpty(docPath) && !string.IsNullOrEmpty(nodePath))
            {
                ret = TPI.CommonFunc.XmlAdapter.GetRoot(docPath, nodePath);
            }
            return ret;
        }
        /// <summary>
        /// 获得xml节点
        /// </summary>
        /// <param name="docPath"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static XmlNode GetXmlNode(string docPath, string nodePath)
        {
            XmlNode ret = null;
            if (!string.IsNullOrEmpty(docPath) && !string.IsNullOrEmpty(nodePath))
            {
                string xmlPath = HttpContext.Current.Server.MapPath(docPath);
                ret = TPI.CommonFunc.XmlAdapter.GetRoot(xmlPath, nodePath);
            }
            return ret;
        }
        /// <summary>
        /// 获得xml节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static XmlNode GetXmlNode(XmlDocument doc, string nodePath)
        {
            return GetXmlNode(doc, nodePath, null);
        }
        /// <summary>
        /// 获得xml节点
        /// </summary>
        public static XmlNode GetXmlNode(XmlDocument doc, string nodePath, XmlNamespaceManager xmlnsManager)
        {
            XmlNode node = null;

            if (doc != null && !string.IsNullOrEmpty(nodePath))
            {
                //添加命名空间

                node = xmlnsManager == null ? doc.SelectSingleNode(nodePath) : doc.SelectSingleNode(nodePath, xmlnsManager);
            }

            return node;
        }
        /// <summary>
        /// 获得xml节点
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static XmlNode GetXmlNode(XmlNode rootNode, string nodePath)
        {
            return GetXmlNode(rootNode, nodePath, null);
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
        /// 选择多个节点
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static XmlNodeList GetXmlNodeList(XmlNode rootNode, string nodePath)
        {
            return GetXmlNodeList(rootNode, nodePath, null);
        }
        /// <summary>
        /// 选择多个节点
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="nodePath"></param>
        /// <param name="xmlnsManager"></param>
        /// <returns></returns>
        public static XmlNodeList GetXmlNodeList(XmlNode rootNode, string nodePath, XmlNamespaceManager xmlnsManager)
        {
            XmlNodeList list = null;

            if (rootNode != null && !string.IsNullOrEmpty(nodePath))
            {
                list = xmlnsManager == null ? rootNode.SelectNodes(nodePath) : rootNode.SelectNodes(nodePath, xmlnsManager);
            }

            return list;
        }
        /// <summary>
        /// 选择多个节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static XmlNodeList GetXmlNodeList(XmlDocument doc, string nodePath)
        {
            return GetXmlNodeList(doc,nodePath,null);
        }
        /// <summary>
        /// 选择多个节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="nodePath"></param>
        /// <param name="xmlnsManager"></param>
        /// <returns></returns>
        public static XmlNodeList GetXmlNodeList(XmlDocument doc ,string nodePath,XmlNamespaceManager xmlnsManager)
        {
            XmlNodeList list = null;

            if (doc != null && !string.IsNullOrEmpty(nodePath))
            {
                list = xmlnsManager == null ? doc.SelectNodes(nodePath) : doc.SelectNodes(nodePath, xmlnsManager);
            }

            return list;
        }

        /// <summary>
        /// 获得xml节点内容
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static string GetXmlNodeText(XmlNode rootNode, string nodePath)
        {
            return GetXmlNodeText(rootNode, nodePath, null);
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
        public static string GetXmlNodeText(XmlDocument doc, string nodePath)
        {
            return GetXmlNodeText(doc, nodePath, null);
        }
        /// <summary>
        /// 获得xml节点内容
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public static string GetXmlNodeText(XmlDocument doc, string nodePath, XmlNamespaceManager xmlnsManager)
        {
            string ret = string.Empty;

            if (doc != null && !string.IsNullOrEmpty(nodePath))
            {
                //添加命名空间

                XmlNode node = GetXmlNode(doc, nodePath, xmlnsManager);
                if (node != null)
                {
                    ret = node.InnerText;
                }
            }

            return ret;
        }
        //public static string G
        #endregion

    }
}