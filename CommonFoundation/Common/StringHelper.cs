using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Common
{
    public class StringHelper
    {
        /// <summary>
        /// 返回特殊符号连接的字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string ListToString(List<string> list)
        {
            return String.Join("$$", list.ToArray()); ;
        }
        /// <summary>
        /// 将特殊符号连接的字符串变为List集合
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> ListToString(string str)
        {
            string[] result=str.Split(new string[]{"$$"}, StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(result);
        }
        /// <summary>
        /// 在做网站的时候，用到了去除html标签的问题，用正则匹配到html标签，然后replace即可。
        /// </summary>
        /// <param name="html">html</param>
        /// <param name="length">Length参数可以根据传入值取固定长度的值。用于生成文章摘要比较方便。</param>
        /// <returns></returns>
        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }


    }
}
