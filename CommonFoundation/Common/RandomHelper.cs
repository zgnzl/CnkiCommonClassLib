using System;
using System.Text;

namespace CommonFoundation.Common
{
    /// <summary>
    /// 随机数据方法类
    /// </summary>
    public static class RandomHelper
    {
        #region GetIntRandom
        /// <summary>
        /// 生成随即數字
        /// </summary>
        /// <returns></returns>
        public static int GetIntRandom()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        #endregion

        #region GetStringRandom
        /// <summary>
        /// 生成字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="strSource">指定字符表，默认a~z</param>
        /// <returns></returns>
        public static string GetStringRandom(int length, string strSource)
        {
            if (string.IsNullOrEmpty(strSource))
            {
                strSource = "abcdefghijklmnopqrstuvwxyz";
            }
            int strLen = strSource.Length;
            StringBuilder sbPwd = new StringBuilder();
            Random random = new Random(GetIntRandom());
            for (int i = 0; i < length; i++)
            {
                sbPwd.Append(strSource.Substring(random.Next(0, strLen), 1));
            }
            return sbPwd.ToString();
        }
        #endregion
    }
}
