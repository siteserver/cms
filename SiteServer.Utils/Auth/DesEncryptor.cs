using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SiteServer.Utils.Auth
{
	/// <summary>
	/// Version: 2.0
	/// LastEditDate: 16:14 2005-10-6
	/// </summary>
	public class DesEncryptor
	{
		#region 私有成员
		/// <summary>
		/// 输入字符串
		/// </summary>
		private string _inputString;
		/// <summary>
		/// 输出字符串
		/// </summary>
		private string _outString;
		/// <summary>
		/// 输入文件路径
		/// </summary>
		private string _inputFilePath;
		/// <summary>
		/// 输出文件路径
		/// </summary>
		private string _outFilePath;
		/// <summary>
		/// 加密密钥
		/// </summary>
		private string _encryptKey;
		/// <summary>
		/// 解密密钥
		/// </summary>
		private string _decryptKey;
		/// <summary>
		/// 提示信息
		/// </summary>
		private string _noteMessage;
		#endregion

		#region 公共属性
		/// <summary>
		/// 输入字符串
		/// </summary>
		public string InputString
		{
			get { return _inputString; }
			set { _inputString = value; }
		}
		/// <summary>
		/// 输出字符串
		/// </summary>
		public string OutString
		{
			get { return _outString; }
			set { _outString = value; }
		}
		/// <summary>
		/// 输入文件路径
		/// </summary>
		public string InputFilePath
		{
			get { return _inputFilePath; }
			set { _inputFilePath = value; }
		}
		/// <summary>
		/// 输出文件路径
		/// </summary>
		public string OutFilePath
		{
			get { return _outFilePath; }
			set { _outFilePath = value; }
		}
		/// <summary>
		/// 加密密钥
		/// </summary>
		public string EncryptKey
		{
			get { return _encryptKey; }
			set { _encryptKey = value; }
		}
		/// <summary>
		/// 解密密钥
		/// </summary>
		public string DecryptKey
		{
			get { return _decryptKey; }
			set { _decryptKey = value; }
		}
		/// <summary>
		/// 错误信息
		/// </summary>
		public string NoteMessage
		{
			get { return _noteMessage; }
			set { _noteMessage = value; }
		}
		#endregion

		#region DES加密字符串
		/// <summary>
		/// 加密字符串
		/// 注意:密钥必须为８位
		/// </summary>
		public void DesEncrypt()
		{
		    byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
			try
			{
                var byKey = Encoding.UTF8.GetBytes(_encryptKey.Length > 8 ? _encryptKey.Substring(0, 8) : _encryptKey);
				var des = new DESCryptoServiceProvider();
				var inputByteArray = Encoding.UTF8.GetBytes(_inputString);
				var ms = new MemoryStream();
				var cs = new CryptoStream(ms, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				_outString = Convert.ToBase64String(ms.ToArray());
			}
			catch (Exception error)
			{
				_noteMessage = error.Message;
			}
		}
		#endregion

		#region DES解密字符串
		/// <summary>
		/// 解密字符串
		/// </summary>
		public void DesDecrypt()
		{
		    byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
		    try
			{
				var byKey = Encoding.UTF8.GetBytes(_decryptKey.Substring(0, 8));
				var des = new DESCryptoServiceProvider();
				var inputByteArray = Convert.FromBase64String(_inputString);
				var ms = new MemoryStream();
				var cs = new CryptoStream(ms, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				Encoding encoding = new UTF8Encoding();
				_outString = encoding.GetString(ms.ToArray());
			}
			catch (Exception error)
			{
				_noteMessage = error.Message;
			}
		}
		#endregion

		#region DES加密文件
		/// <summary>
		/// DES加密文件
		/// </summary>
		public void FileDesEncrypt()
		{
		    byte[] iv = { 0x12, 0x44, 0x16, 0xEE, 0x88, 0x15, 0xDD, 0x41 };//在向量中放入一些随机数据
			try
			{
				var byKey = Encoding.UTF8.GetBytes(_encryptKey.Substring(0, 8));
				var fin = new FileStream(_inputFilePath, FileMode.Open, FileAccess.Read);
				var fout = new FileStream(_outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
				fout.SetLength(0);
				//Create variables to help with read and write.
				var bin = new byte[100]; //This is intermediate storage for the encryption.
				long rdlen = 0;              //This is the total number of bytes written.
				var totlen = fin.Length;    //This is the total length of the input file.
			    DES des = new DESCryptoServiceProvider();
				var encStream = new CryptoStream(fout, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);


				//Read from the input file, then encrypt and write to the output file.
				while (rdlen < totlen)
				{
					var len = fin.Read(bin, 0, 100);                     //This is the number of bytes to be written at a time.
					encStream.Write(bin, 0, len);
					rdlen = rdlen + len;
				}

				encStream.Close();
				fout.Close();
				fin.Close();


			}
			catch (Exception error)
			{
				_noteMessage = error.Message;

			}
		}
		#endregion

		#region DES解密文件
		/// <summary>
		/// 解密文件
		/// </summary>
		public void FileDesDecrypt()
		{
		    byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
			try
			{
				var byKey = Encoding.UTF8.GetBytes(_decryptKey.Substring(0, 8));
				var fin = new FileStream(_inputFilePath, FileMode.Open, FileAccess.Read);
				var fout = new FileStream(_outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
				fout.SetLength(0);
				//Create variables to help with read and write.
				var bin = new byte[100]; //This is intermediate storage for the encryption.
				long rdlen = 0;              //This is the total number of bytes written.
				var totlen = fin.Length;    //This is the total length of the input file.
			    DES des = new DESCryptoServiceProvider();
				var encStream = new CryptoStream(fout, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);


				//Read from the input file, then encrypt and write to the output file.
				while (rdlen < totlen)
				{
					var len = fin.Read(bin, 0, 100); //This is the number of bytes to be written at a time.
					encStream.Write(bin, 0, len);
					rdlen = rdlen + len;
				}

				encStream.Close();
				fout.Close();
				fin.Close();
			}
			catch (Exception error)
			{
				_noteMessage = error.Message;
			}
		}
		#endregion

		#region MD5
		/// <summary>
		/// MD5 Encrypt
		/// </summary>
		/// <returns>md5 Encrypt string</returns>
		public void Md5Encrypt()
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			var result = md5.ComputeHash(Encoding.Default.GetBytes(_inputString));
			_outString = Encoding.Default.GetString(result);
		}
		#endregion
	}
}

