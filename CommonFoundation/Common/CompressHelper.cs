using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace CommonFoundation.Common
{
    public static class CompressHelper
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Compress(string value)
        {
            string compressString = string.Empty;
            try
            {
                MemoryStream mstream = new MemoryStream();
                GZipStream cstream = new GZipStream(mstream, CompressionMode.Compress, true);
                StreamWriter bwriter = new StreamWriter(cstream);
                bwriter.Write(value);
                bwriter.Close();
                cstream.Close();
                compressString = Convert.ToBase64String(mstream.ToArray());
                mstream.Close();
            }
            catch
            {

            }
            return compressString;

        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Decompress(string value)
        {
            string commonString = string.Empty;
            try
            {
                byte[] data = Convert.FromBase64String(value);
                MemoryStream mstream = new MemoryStream(data);
                GZipStream cstream = new GZipStream(mstream, CompressionMode.Decompress);
                StreamReader reader = new StreamReader(cstream);
                commonString = reader.ReadToEnd();
                mstream.Close();
                cstream.Close();
                reader.Close();
            }
            catch
            {
            }
            return commonString;

        }
    }
}