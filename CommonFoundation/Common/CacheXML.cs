using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Web;
using System.Collections;

namespace CommonFoundation.Common
{
    /// <summary>
    /// 缓存xml文件的静态类，缓存的对象为XmlDocument，文件路径+最新修改时间为key
    /// </summary>
    public static class XMLCacher
    {
        /// <summary>
        /// 存放xml文档
        /// </summary>
        private static Dictionary<string, XMLCacheInfo> XmlDocumentCache = new Dictionary<string, XMLCacheInfo>();

        /// <summary>
        /// 加载xml文档，以xml文件的物理路径为key缓存XmlDocument
        /// </summary>
        /// <param name="filePath">xml文件的物理路径</param>
        public static XmlDocument LoadXmlDocument(string filePath)
        {
            //判断参数合法性
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return null;


            XmlDocument doc = null;
            string key = filePath.Trim().ToUpper();
            long fileTime = File.GetLastWriteTime(filePath).ToFileTime();

            if (XmlDocumentCache.ContainsKey(key))
            {
                XMLCacheInfo info = XmlDocumentCache[key];
                doc = info.LastUpdateTime != fileTime ? Load(key, fileTime,filePath) : info.Doc;
            }
            else
            {
                doc = Load(key, fileTime,filePath);
            }

            return doc;

        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static XmlDocument Load(string key,long fileTime,string filePath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
                CacheXml(key, fileTime, doc);
            }
            catch
            {
                doc = null;
            }
            return doc;
        }

        /// <summary>
        /// 缓存xml
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fileTime"></param>
        /// <param name="doc"></param>
        private static void CacheXml(string key, long fileTime, XmlDocument doc)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                XmlDocumentCache.Remove(key);
                if (doc != null)
                {
                    XMLCacheInfo info = new XMLCacheInfo();
                    info.LastUpdateTime = fileTime;
                    info.Doc = doc;
                    XmlDocumentCache.Add(key, info);
                }
            }
        }

    }

    public class XMLCacheInfo
    {
        /// <summary>
        /// 最新更改时间
        /// </summary>
        public long LastUpdateTime { get; set; }
        /// <summary>
        /// 文档
        /// </summary>
        public XmlDocument Doc { get; set; }
    }
}
