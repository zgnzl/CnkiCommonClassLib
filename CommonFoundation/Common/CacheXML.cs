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
    /// ����xml�ļ��ľ�̬�࣬����Ķ���ΪXmlDocument���ļ�·��+�����޸�ʱ��Ϊkey
    /// </summary>
    public static class XMLCacher
    {
        /// <summary>
        /// ���xml�ĵ�
        /// </summary>
        private static Dictionary<string, XMLCacheInfo> XmlDocumentCache = new Dictionary<string, XMLCacheInfo>();

        /// <summary>
        /// ����xml�ĵ�����xml�ļ�������·��Ϊkey����XmlDocument
        /// </summary>
        /// <param name="filePath">xml�ļ�������·��</param>
        public static XmlDocument LoadXmlDocument(string filePath)
        {
            //�жϲ����Ϸ���
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
        /// ����xml
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
        /// ����xml
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
        /// ���¸���ʱ��
        /// </summary>
        public long LastUpdateTime { get; set; }
        /// <summary>
        /// �ĵ�
        /// </summary>
        public XmlDocument Doc { get; set; }
    }
}
