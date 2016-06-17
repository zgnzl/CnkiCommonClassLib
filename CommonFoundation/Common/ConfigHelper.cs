using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonFoundation.Common
{
    public class ConfigHelper
    {
        /// <summary>
        /// 获得config值
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetValue(string Key)
        {
            string Value = System.Configuration.ConfigurationManager.AppSettings[Key];
            if (!string.IsNullOrEmpty(Value))
            {
                return Value;
            }
            return string.Empty;
        }
        /// <summary>
        /// 获得config值，默认值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static string GetString(string Key, string DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                return Setting;
            }
            return DefaultValue;
        }
        /// <summary>
        /// 获得config值，默认值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static bool GetBool(string Key, bool DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                switch (Setting.ToLower())
                {
                    case "false":
                    case "0":
                    case "n":
                        return false;
                    case "true":
                    case "1":
                    case "y":
                        return true;
                }
            }
            return DefaultValue;
        }
        /// <summary>
        /// 获得config值，默认值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static int GetInt(string Key, int DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                int i;
                if (int.TryParse(Setting, out i))
                {
                    return i;
                }
            }
            return DefaultValue;
        }
        /// <summary>
        /// 获得config值，默认值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static double GetDouble(string Key, double DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                double d;
                if (double.TryParse(Setting, out d))
                {
                    return d;
                }
            }
            return DefaultValue;
        }
        /// <summary>
        /// 获得config值，默认值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static byte GetByte(string Key, byte DefaultValue)
        {
            string Setting = GetValue(Key);
            if (!string.IsNullOrEmpty(Setting))
            {
                byte b;
                if (byte.TryParse(Setting, out b))
                {
                    return b;
                }
            }
            return DefaultValue;
        }
    }
}
