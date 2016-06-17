using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml;

namespace CommonFoundation.Common
{
    public class XmlToJSONParser
    {
        private static void OutputNode(string childname, object alChild, StringBuilder sbJSON, bool showNodeName)
        {
            if (alChild == null)
            {
                if (showNodeName)
                {
                    sbJSON.Append(SafeJSON(childname) + ": ");
                }
                sbJSON.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                {
                    sbJSON.Append(SafeJSON(childname) + ": ");
                }
                string s = (string)alChild;
                s = s.Trim();
                sbJSON.Append(SafeJSON(s));
            }
            else
            {
                XmlToJSONnode(sbJSON, (XmlElement)alChild, showNodeName);
            }
            sbJSON.Append(", ");
        }

        /// <summary>
        /// 包装s到json的string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SafeJSON(string s)
        {
            if ((s == null) || (s.Length == 0))
            {
                return "\"\"";
            }
            int length = s.Length;
            StringBuilder builder = new StringBuilder(length + 4);
            builder.Append('"');
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                switch (ch)
                {
                    case '\\':
                    case '"':
                    case '>':
                        builder.Append('\\');
                        builder.Append(ch);
                        break;

                    case '\b':
                        builder.Append(@"\b");
                        break;

                    case '\t':
                        builder.Append(@"\t");
                        break;

                    case '\n':
                        builder.Append(@"\n");
                        break;

                    case '\f':
                        builder.Append(@"\f");
                        break;

                    case '\r':
                        builder.Append(@"\r");
                        break;

                    default:
                        if (ch < ' ')
                        {
                            string str2 = new string(ch, 1);
                            string str = "000" + int.Parse(str2, NumberStyles.HexNumber);
                            builder.Append(@"\u" + str.Substring(str.Length - 4));
                        }
                        else
                        {
                            builder.Append(ch);
                        }
                        break;
                }
            }
            builder.Append('"');
            return builder.ToString();
        }

        private static void StoreChildNode(IDictionary childNodeNames, string nodeName, object nodeValue)
        {
            ArrayList list2;
            if (nodeValue is XmlElement)
            {
                XmlNode node = (XmlNode)nodeValue;
                if (node.Attributes.Count == 0)
                {
                    XmlNodeList childNodes = node.ChildNodes;
                    if (childNodes.Count == 0)
                    {
                        nodeValue = null;
                    }
                    else if ((childNodes.Count == 1) && (childNodes[0] is XmlText))
                    {
                        nodeValue = childNodes[0].InnerText;
                    }
                }
            }
            object obj2 = childNodeNames[nodeName];
            if (obj2 == null)
            {
                list2 = new ArrayList();
                childNodeNames[nodeName] = list2;
            }
            else
            {
                list2 = (ArrayList)obj2;
            }
            list2.Add(nodeValue);
        }

        /// <summary>
        /// 传入xml文件路径，得到jsonstring,不是normalstring, 不是jsonObj
        /// </summary>
        /// <param name="xmlPath">xml文件的路径</param>
        /// <param name="nodeXPath">要转换json的xml节点</param>
        /// <returns></returns>
        public static string XmlToJSON(string xmlPath, string nodeXPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNode xmlNode = xmlDoc.SelectSingleNode(nodeXPath);

            StringBuilder sbJSON = new StringBuilder();
            sbJSON.Append("{ ");
            XmlToJSONnode(sbJSON, xmlNode, true);
            sbJSON.Append("}");
            return sbJSON.ToString();
        }

        /// <summary>
        /// 把指定xmlNode的内容转换成JsonString
        /// </summary>
        /// <param name="sbJSON"></param>
        /// <param name="node"></param>
        /// <param name="showNodeName"></param>
        private static void XmlToJSONnode(StringBuilder sbJSON, XmlNode node, bool showNodeName)
        {
            if (showNodeName)
            {
                sbJSON.Append(SafeJSON(node.Name) + ": ");
            }
            sbJSON.Append("{");
            SortedList childNodeNames = new SortedList();
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    StoreChildNode(childNodeNames, attribute.Name, attribute.InnerText);
                }
            }
            foreach (XmlNode node2 in node.ChildNodes)
            {
                if (node2 is XmlText)
                {
                    StoreChildNode(childNodeNames, "value", node2.InnerText);
                }
                else if (node2 is XmlElement)
                {
                    StoreChildNode(childNodeNames, node2.Name, node2);
                }
            }
            foreach (string str in childNodeNames.Keys)
            {
                ArrayList list2 = (ArrayList)childNodeNames[str];
                if (list2.Count == 1)
                {
                    OutputNode(str, list2[0], sbJSON, true);
                }
                else
                {
                    sbJSON.Append(SafeJSON(str) + ": [ ");
                    foreach (object obj2 in list2)
                    {
                        OutputNode(str, obj2, sbJSON, false);
                    }
                    sbJSON.Remove(sbJSON.Length - 2, 2);
                    sbJSON.Append(" ], ");
                }
            }
            sbJSON.Remove(sbJSON.Length - 2, 2);
            sbJSON.Append(" }");
        }
    }
}