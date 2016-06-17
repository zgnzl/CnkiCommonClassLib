using System;
using System.Security.Cryptography;  
using System.Text;

namespace CommonFoundation.Common
{
	/// <summary>
	/// DES加密/解密类。
	/// </summary>
    public static class DESEncrypt
	{
		#region 加密
		/// <summary> 
		/// 加密数据 
		/// </summary> 
		/// <param name="Text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Encrypt(string Text,string sKey) 
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(sKey))
			{
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				byte[] inputByteArray;
				inputByteArray = Encoding.Default.GetBytes(Text);
				des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
				des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
				System.IO.MemoryStream ms = new System.IO.MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				StringBuilder ret = new StringBuilder();
				foreach (byte b in ms.ToArray())
				{
					ret.AppendFormat("{0:X2}", b);
				}
				str = ret.ToString();
			}
			return str;
		} 

		#endregion

		#region 解密
		/// <summary> 
		/// 解密数据 
		/// </summary> 
		/// <param name="Text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Decrypt(string Text,string sKey) 
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(sKey))
			{
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				int len;
				len = Text.Length / 2;
				byte[] inputByteArray = new byte[len];
				int x, i;
				for (x = 0; x < len; x++)
				{
					i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
					inputByteArray[x] = (byte)i;
				}
				des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
				des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
				System.IO.MemoryStream ms = new System.IO.MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				str= Encoding.Default.GetString(ms.ToArray());
			}
			return str;
		} 
 
		#endregion 


	}
}
