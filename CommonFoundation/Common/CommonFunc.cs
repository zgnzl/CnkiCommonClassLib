using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data;
using System.Web.Script.Serialization;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CommonFoundation.Common
{
    public static class CommonFunc
    {
        private static Regex Reg_not = new Regex(@"(?<not>not.*)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static Regex Reg_yzbs = new Regex(@"not \s*[(]{0,1}\s*引证标识\s*=\s*0\s*[)]{0,1}", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static Regex Reg_groupby = new Regex(@"group\s*by\s*.*", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static Regex Reg_orderby = new Regex(@"order\s*by\s*.*", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static Regex Reg_Int = new Regex(@"^[0-9]\d*$", RegexOptions.Compiled | RegexOptions.Singleline);

        #region Check
        /// <summary>
        /// 检验int
        /// </summary>
        /// <param name="str">字符</param>
        /// <returns></returns>
        public static bool CheckInt(string str)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(str))
            {
                if (Reg_Int.IsMatch(str))
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 检验int
        /// </summary>
        /// <param name="str">字符</param>
        /// <param name="max">最大</param>
        /// <param name="min">最小</param>
        /// <returns></returns>
        public static bool CheckInt(string str, int min, int max)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(str))
            {
                if (Reg_Int.IsMatch(str))
                {
                    int i = FormatInt(str);
                    if (i <= max && i >= min)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 检查多个字符串是否为空, or 条件
        /// 有一个不为空即为true
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckStringByOr(string[] strs)
        {
            bool ret = false;
            if (strs != null && strs.Length > 0)
            {
                foreach (string str in strs)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 检查多个字符串是否为空,and 条件
        /// 有一个为空即为false
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckStringByAnd(string[] strs)
        {
            bool ret = true;
            if (strs != null && strs.Length > 0)
            {
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 检查年字段
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool CheckYear(string year)
        {
            return !string.IsNullOrEmpty(year) && year.Length == 4 && CheckInt(year, 0, DateTime.Now.Year);
        }
        #endregion

        #region Format
        /// <summary>
        /// 格式化string到string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatString(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str;
        }

        /// <summary>
        /// 格式化string到int，空为0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int FormatInt(string str)
        {
            return (string.IsNullOrEmpty(str) || !CheckInt(str)) ? 0 : Convert.ToInt32(str);
        }
        /// <summary>
        /// 格式化string到double，空为0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double FormatDouble(string str)
        {
            return string.IsNullOrEmpty(str) ? 0.0 : Convert.ToDouble(str);
        }
        /// <summary>
        /// 格式化string到Bool，空为false
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool FormatBool(string str)
        {
            return (!string.IsNullOrEmpty(str) && str.ToUpper() == "Y") ? true : false;
        }
        /// <summary>
        /// 格式化string到Bool,key比较
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trueKey"></param>
        /// <returns></returns>
        public static bool FormatBool(string str, string trueKey)
        {
            return (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(trueKey) && str.ToUpper() == trueKey.ToUpper()) ? true : false;
        }
        #endregion

        #region String
        /// <summary>
        /// 分割字符串，通过;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitString(string str)
        {
            return SplitString(str, new char[] { ';' });
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitString(string str, char splitChar)
        {
            return SplitString(str, new char[] { splitChar });
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string[] SplitString(string str, char[] chars)
        {
            string[] ret = null;
            if (!string.IsNullOrEmpty(str))
            {
                ret = str.Split(chars, StringSplitOptions.RemoveEmptyEntries);
            }
            return ret;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static List<string> SplitString(string str, string splitStr)
        {
            List<string> ret = null;
            if (!string.IsNullOrEmpty(str))
            {
                string[] array = str.Split(new string[] { splitStr }, StringSplitOptions.RemoveEmptyEntries);
                if (array != null)
                {
                    ret = array.ToList();
                }
            }
            return ret;
        }
        /// <summary>
        /// 取子串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="idx"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubString(string str, int idx, int length)
        {
            return SubString(str, idx, length, string.Empty);
        }
        /// <summary>
        /// 取子串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="idx"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubString(string str, int idx, int length, string sign)
        {
            string ret = string.Empty;

            if (!string.IsNullOrEmpty(str) && idx >= 0 && length > 0)
            {
                ret = str;
                if (str.Length > idx + length)
                {
                    ret = string.Format("{0}{1}", str.Substring(idx, length),sign);
                }
            }

            return ret;
        }
        /// <summary>
        /// 判断字符串，空赋值默认
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static string GetDefaultStr(string str, string defaultVal)
        {
            string ret = defaultVal;
            if (!string.IsNullOrEmpty(str))
            {
                ret = str;
            }
            return ret;
        }
        /// <summary>
        /// 包裹引号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string WrapQuotes(string str)
        {
            return string.Format("\"{0}\"", str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="longTxt"></param>
        /// <returns></returns>
        public static string ShortCode(string longTxt)
        {
            return ShortCodeArray(longTxt)[0];
        }
        /// <summary>
        /// md5压缩字符串
        /// </summary>
        /// <param name="longTxt"></param>
        /// <returns></returns>
        public static string[] ShortCodeArray(string longTxt)
        {
            //可以自定义生成MD5加密字符传前的混合KEY   
            string key = "Fi9pl6";
            //要使用生成URL的字符   
            string[] chars = new string[]{   
                "a","b","c","d","e","f","g","h",   
                "i","j","k","l","m","n","o","p",   
                "q","r","s","t","u","v","w","x",   
                "y","z","0","1","2","3","4","5",   
                "6","7","8","9","A","B","C","D",   
                "E","F","G","H","I","J","K","L",   
                "M","N","O","P","Q","R","S","T",   
                "U","V","W","X","Y","Z"  
  
             };
            //对传入网址进行MD5加密   
            string hex = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key + longTxt, "md5");

            string[] resUrl = new string[4];

            for (int i = 0; i < 4; i++)
            {
                //把加密字符按照8位一组16进制与0x3FFFFFFF进行位与运算   
                int hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(i * 8, 8), 16);
                string outChars = string.Empty;
                for (int j = 0; j < 6; j++)
                {
                    //把得到的值与0x0000003D进行位与运算，取得字符数组chars索引   
                    int index = 0x0000003D & hexint;
                    //把取得的字符相加   
                    outChars += chars[index];
                    //每次循环按位右移5位   
                    hexint = hexint >> 5;
                }
                //把字符串存入对应索引的输出数组   
                resUrl[i] = outChars;
            }

            return resUrl;
        }

        /// <summary>
        /// 获取str中，指定pattern中的数组，通过正则表达式
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string GetNumByRegexPattern(this string str, string pattern)
        {
            string num = "";
            Regex rgx = new Regex(string.Format(@"{0}:((.)+?),", pattern), RegexOptions.IgnoreCase); //匹配任意字符，非贪婪模式
            MatchCollection matches = rgx.Matches(str);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Groups.Count >= 2)
                    {
                        num = match.Groups[1].Value;
                    }
                    else
                    {
                        num = "0";
                    }
                    break;
                }

            }
            else
            {
                num = "0";
            }

            return num;
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public static string StrCompress(string inPut)
        {
            string compressString=string.Empty;
            try
            {
                MemoryStream mstream = new MemoryStream();
                GZipStream cstream = new GZipStream(mstream, CompressionMode.Compress, true);
                StreamWriter bwriter = new StreamWriter(cstream);
                bwriter.Write(inPut);
                bwriter.Close();
                cstream.Close();
                compressString = Convert.ToBase64String(mstream.ToArray());
                mstream.Close();
            }
            catch { }
            return compressString;
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="inPunt"></param>
        /// <returns></returns>
        public static string StrDecompress(string inPunt)
        {
            string commonString = string.Empty;
            try
            {
                byte[] data = Convert.FromBase64String(inPunt);
                MemoryStream mstream = new MemoryStream(data);
                GZipStream cstream = new GZipStream(mstream, CompressionMode.Decompress);
                StreamReader reader = new StreamReader(cstream);
                commonString = reader.ReadToEnd();
                mstream.Close();
                cstream.Close();
                reader.Close();
            }
            catch { }
            return commonString;
        }
        #endregion

        #region 获取时间
        /// <summary>
        /// 获得当前时间，格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public static string GetTimeNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion

        #region 文件路径
        /// <summary>
        /// 获得日志存储路径
        /// </summary>
        /// <returns></returns>
        public static string GetDirPath(string logBasePath)
        {
            return string.Format("{0}\\{1}", logBasePath, DateTime.Now.ToString("yyyy-MM"));
        }

        /// <summary>
        /// 获得日志文件全路径
        /// </summary>
        /// <returns></returns>
        public static string GetFileFullPath(string dirPath)
        {
            return string.Format("{0}\\{1}.txt", dirPath, DateTime.Now.ToString("yyyy-MM-dd"));
        }
        /// <summary>
        /// 获得日志文件全路径
        /// </summary>
        /// <returns></returns>
        public static string GetErrorFileFullPath(string dirPath)
        {
            return string.Format("{0}\\{1}_error.txt", dirPath, DateTime.Now.ToString("yyyy-MM-dd"));
        }
        /// <summary>
        /// 获得日志文件全路径：关于错误的日志
        /// </summary>
        /// <returns></returns>
        public static string GetLogErrorFileFullPath(string dirPath)
        {
            return string.Format("{0}\\{1}.txt", dirPath, DateTime.Now.ToString("yyyy-MM-dd")+"Error");
        }
        #endregion

        #region 获得本机IP
        /// <summary>
        /// 获得本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            string ip = string.Empty;
            foreach (IPAddress iPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = iPAddress.ToString();
                    break;
                }
            }
            return ip;
        }
        #endregion

        #region Json转换
        /// <summary>
        /// 泛型转换为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ConvertToJson<T>(T obj)
        {
            string jsonStr = string.Empty;
            if (obj != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                jsonStr = serializer.Serialize(obj);
            }
            return jsonStr;
        }

        /// <summary>
        /// Json转换为Object
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T ConvertToGeneric<T>(string jsonStr)
        {
            object obj = null;
            if (!string.IsNullOrEmpty(jsonStr))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                obj = serializer.Deserialize<T>(jsonStr);

            }
            return (T)obj;
        }
        #endregion

        #region 字符串与EnCoding互转
        /// <summary>
        /// 字符串转EnCoding
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Encoding StringToEnCoding(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                str = str.ToUpper();
                if (Encoding.ASCII.HeaderName.ToUpper() == str)
                {
                    return Encoding.ASCII;
                }
                else if (Encoding.UTF8.HeaderName.ToUpper() == str)
                {
                    return Encoding.UTF8;
                }
                else if (Encoding.Unicode.HeaderName.ToUpper() == str)
                {
                    return Encoding.Unicode;
                }
                else if (Encoding.UTF32.HeaderName.ToUpper() == str)
                {
                    return Encoding.UTF32;
                }
                else
                {
                    return Encoding.Default;
                }
            }

            return null;
        }
        /// <summary>
        /// Encoding转string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EnCodingToString(Encoding encoding)
        {
            return encoding == null ? string.Empty : encoding.HeaderName.ToUpper();
        }
        #endregion
    }
}