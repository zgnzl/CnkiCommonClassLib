using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CommonFoundation.Common
{
    public static class SessionHelper
    {
        /// <summary>
        /// 设置session值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Obj"></param>
        public static void SetSessionValue(string Key, object Obj)
        {
            if ((Obj != null))
            {
                if (HttpContext.Current.Session[Key] == null)
                {
                    HttpContext.Current.Session[Key] = Obj;
                }
            }
        }
        /// <summary>
        /// 获得session值
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static object GetSessionValue(string Key)
        {
            return HttpContext.Current.Session[Key];
        }
        /// <summary>
        /// 移除session值
        /// </summary>
        /// <param name="Key"></param>
        public static void RemoveSessionValue(string Key)
        {
            if ((HttpContext.Current.Session[Key] != null))
            {
                HttpContext.Current.Session[Key] = null;
            }
        }
    }
}
