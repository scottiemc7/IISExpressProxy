using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.IO;

namespace IISExpressProxy.Services
{
	abstract class EncryptingSettingsService : ISettingsService
	{
		#region ISettingsService

		public abstract string Get(string settingName);
		public abstract void Set(string settingName, string value);

		public string GetDecrypted(string settingName)
		{
			string encrypted = Get(settingName);
			return Decrypt(encrypted);
		}

		public void SetEncrypted(string settingName, string value)
		{
			Set(settingName, Encrypt(value));
		}

		#endregion

		private static byte[] SECRETKEY = new byte[32] { 159, 247, 143, 226, 125, 237, 116, 201, 180, 137, 51, 122, 55, 162, 143, 101, 79, 193, 71, 77, 216, 160, 219, 207, 232, 201, 6, 34, 246, 194, 202, 166 };
		private static byte[] SECRETIV = new byte[32] { 181, 122, 134, 112, 12, 223, 58, 46, 232, 78, 201, 74, 232, 219, 118, 13, 211, 177, 146, 233, 122, 43, 53, 205, 187, 68, 220, 25, 144, 5, 136, 14 };
		private string Encrypt(string value)
		{
			byte[] encrypted;
			using (RijndaelManaged myRijndael = new RijndaelManaged() { BlockSize = 256, Key = SECRETKEY, IV = SECRETIV })
			using (ICryptoTransform encryptor = myRijndael.CreateEncryptor(myRijndael.Key, myRijndael.IV))
			using (MemoryStream msEncrypt = new MemoryStream())
			using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
			{
				using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
					swEncrypt.Write(value);
				encrypted = msEncrypt.ToArray();
			}//end using

			return Convert.ToBase64String(encrypted);
		}

		private string Decrypt(string value)
		{
			using (RijndaelManaged myRijndael = new RijndaelManaged() { BlockSize = 256, Key = SECRETKEY, IV = SECRETIV })
			using (ICryptoTransform decryptor = myRijndael.CreateDecryptor(myRijndael.Key, myRijndael.IV))
			using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(value)))
			using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
			{
				using (StreamReader swDecrypt = new StreamReader(csDecrypt))
					return swDecrypt.ReadToEnd();
			}//end using
		}
	}
}
