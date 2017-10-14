using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Xml;
using System.IO.Compression;
using Log;

namespace MCCard
{
    /// <summary>
    /// 2016/05/06-1
    /// </summary>
    public static class GenerateKeyExtensions
    {
        /// <summary>
        /// AESD VI
        /// </summary>
        private static byte[] AESDKeys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        #region EncryptAES()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EncryptString"></param>
        /// <param name="EncryptKey"></param>
        /// <returns></returns>
        public static string EncryptAES(this string EncryptString, string EncryptKey)
        {
            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            try
            {
                EncryptKey = EncryptKey.PadRight(32, ' ');

                rijndaelProvider.Key = Encoding.UTF8.GetBytes(EncryptKey.Substring(0, 32));

                rijndaelProvider.IV = AESDKeys;

                ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

                byte[] inputData = Encoding.UTF8.GetBytes(EncryptString);

                byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Convert.ToBase64String(encryptedData);
            }
            catch (System.Exception ex) { throw new Exception("AESEncrypt : " + ex.Message); }
            finally
            {
                rijndaelProvider = null;
            }
        }
        #endregion

        #region DecryptAES()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DecryptString"></param>
        /// <param name="DecryptKey"></param>
        /// <returns></returns>
        public static string DecryptAES(this string DecryptString, string DecryptKey)
        {
            RijndaelManaged rijndaelProvider = null;
            try
            {
                DecryptKey = DecryptKey.PadRight(32, ' ');

                rijndaelProvider = new RijndaelManaged();

                rijndaelProvider.Key = Encoding.UTF8.GetBytes(DecryptKey);

                rijndaelProvider.IV = AESDKeys;

                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                byte[] inputData = Convert.FromBase64String(DecryptString);

                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (System.Exception ex) { throw new Exception("AESEncrypt : " + ex.Message); }
            finally
            {
                rijndaelProvider = null;
            }
        }
        #endregion

        #region EncryptShift()
        /// <summary>
        /// 字串移位加密  
        /// </summary>
        /// <param name="str">待加密字串</param>
        /// <returns>加密後的字串</returns>
        public static string EncryptShift(this string Input)
        {
            try
            {
                string temp = "";
                int intTemp;
                char[] charTemp = Input.ToCharArray();
                for (int i = 0; i < charTemp.Length; i++)
                {
                    intTemp = charTemp[i] + 1; charTemp[i] = (char)intTemp; temp += charTemp[i];
                }
                return temp;
            }
            catch (System.Exception ex) { throw new Exception("ShiftEncrypt : " + ex.Message); }
        }
        #endregion

        #region DecryptShift()
        /// <summary>
        /// 字串移位解密
        /// </summary>
        /// <param name="str">待解密字串</param>
        /// <returns>解密後的字串</returns>
        public static string DecryptShift(this string Input)
        {
            try
            {
                string temp = "";
                int intTemp;
                char[] charTemp = Input.ToCharArray();
                for (int i = 0; i < charTemp.Length; i++)
                {
                    intTemp = charTemp[i] - 1; charTemp[i] = (char)intTemp; temp += charTemp[i];
                }
                return temp;
            }
            catch (System.Exception ex) { throw new Exception("ShiftDecrypt : " + ex.Message); }
        }
        #endregion

        #region GenerateKey()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string GenerateKey(this string ID, string Prefix)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            StringBuilder sb = new StringBuilder();
            try
            {
                string key = string.Format("{0}{1}", ID, Guid.NewGuid());

                byte[] bytValue = Encoding.UTF8.GetBytes(key.EncryptShift().EncryptAES("!(**)@)^"));

                byte[] bytHash = md5.ComputeHash(bytValue);

                md5.Clear();

                for (int i = 0; i < bytHash.Length; i++) sb.Append(bytHash[i].ToString("X").PadLeft(2, '0'));

                string result = string.Format("{0}{1}{2}", Prefix, sb.ToString().Substring(0, 30 - Prefix.Length - 3), new Random().Next(1, 999).ToString().PadLeft(3, '0'));

                return result.ToUpper();
            }
            catch (System.Exception ex) { throw new Exception("MD5Encrypt.Exception::{0}" + ex.Message); }
            finally
            {
                md5 = null; sb = null;
            }
        }
        #endregion
    }
}