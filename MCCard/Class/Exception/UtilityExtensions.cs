using System;
using System.Xml;
using System.Text;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web.UI;
using System.Xml.Serialization;
using System.Security.Cryptography;
using Log;
using System.Security;

namespace MCCard
{
    public static class UtilityExtensions
    {
        #region RequestQueryString()
        /// <summary>
        /// 取得參數
        /// </summary>
        public static string RequestQueryString(this Page MyPage, string QueryName)
        {
            return MyPage.Request.QueryString[QueryName] != null ? MyPage.Request.QueryString[QueryName].Trim().Replace(StringFormatException.Mode.CrossSiteScripting) : string.Empty;
        }
        #endregion

        #region ResponseXml()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MyPage"></param>
        /// <param name="XmlDoc"></param>
        public static void ResponseXml(this Page MyPage, XmlDocument XmlDoc)
        {
            MyPage.Response.ClearContent();
            MyPage.Response.Clear();
            MyPage.Response.ContentType = "text/xml";
            MyPage.Response.Write(XmlDoc.OuterXml);
            MyPage.Response.Flush();
            MyPage.Response.End();
        }
        #endregion

        #region ResponseImage()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MyPage"></param>
        /// <param name="ImageByte"></param>
        public static void ResponseImage(this Page MyPage, byte[] ImageByte)
        {
            MyPage.Response.ClearContent();
            MyPage.Response.ContentType = "image/jpeg";
            MyPage.Response.BinaryWrite(ImageByte);
        }
        #endregion

        #region DeleteDirectory()
        /// <summary>
        /// 刪除整個目錄
        /// </summary>
        /// <param name="DirectoryPath"></param>
        public static void DeleteDirectory(this string DirectoryPath)
        {
            try
            {
                if (String.IsNullOrEmpty(DirectoryPath)) return;

                DirectoryInfo dir = new DirectoryInfo(DirectoryPath);

                if (!dir.Exists) return;

                if (dir.GetFiles().Length >= 0)
                {
                    foreach (FileInfo f in dir.GetFiles()) f.FullName.DeleteSigleFile();
                }

                if (dir.GetDirectories().Length > 0)
                {
                    foreach (DirectoryInfo d in dir.GetDirectories()) d.FullName.DeleteDirectory();
                }
                if (dir.GetFiles().Length.Equals(0)) dir.Delete();
            }
            catch { }
        }
        #endregion

        #region DeleteSigleFile()
        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool DeleteSigleFile(this string FilePath)
        {
            if (String.IsNullOrEmpty(FilePath) || !File.Exists(FilePath)) return true;

            int retryCount = 0;

            while (true)
            {
                FileInfo fileInfo = new FileInfo(FilePath);
                try
                {
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete(); return true;
                    }
                    else return false;
                }
                catch { retryCount++; System.Threading.Thread.Sleep(200); }
                finally
                {
                    if (fileInfo != null) fileInfo = null;
                }
                if (retryCount >= 3) break;
            }
            return false;
        }
        #endregion

        #region CreateSubDirectory()
        /// <summary>
        /// 產生子目錄
        /// </summary>
        /// <returns></returns>
        public static string CreateSubDirectory(this string DirectoryPath)
        {
            DateTime t = DateTime.Now;

            string subDirPath = System.IO.Path.Combine(t.ToString("yyyy"), t.ToString("MM"), t.ToString("dd"), t.ToString("HH"));

            string dirPath = System.IO.Path.Combine(DirectoryPath, subDirPath);

            if (!System.IO.Directory.Exists(dirPath)) System.IO.Directory.CreateDirectory(dirPath);

            return Path.Combine(DirectoryPath, subDirPath);
        }
        #endregion

        #region GetMaxString()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CheckStr"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static string GetMaxString(this string CheckStr, int MaxLength)
        {
            int len = 0;

            float charLen = 0;

            char[] chs = CheckStr.ToCharArray();

            StringBuilder sb = new StringBuilder();

            int count = 0;

            for (int i = 0; i < chs.Length; i++)
            {
                int charLength = System.Text.Encoding.UTF8.GetByteCount(chs[i].ToString());

                sb.Append(chs[i].ToString());

                if (charLength == 3)
                {
                    len++;

                    count += 2;
                }
                else
                {
                    if (charLen == 0.5)
                    {
                        charLen = 0;
                    }
                    else
                    {
                        charLen = 0.5f; len++;
                    }
                    count += 1;
                }
                if (count >= MaxLength) return sb.ToString();
            }
            return sb.ToString();
        }
        #endregion

        #region GetContentType()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public static string GetContentType(this string FileExtension)
        {
            string contentType = string.Empty;

            switch (FileExtension.ToLower().Trim())
            {
                case "m3u8": contentType = "application/x-mpegURL"; break;
                case "ts": contentType = "video/MP2T"; break;

                case "gif": contentType = "image/gif"; break;

                case "pdf": contentType = "application/pdf"; break;
                case "swf": contentType = "application/x-shockwave-flash"; break;

                case "mp4": contentType = "video/mp4"; break;
                case "mp2":
                case "mpa":
                case "mpe":
                case "mpeg":
                case "mpg":
                case "mpv2": contentType = "video/mpeg"; break;
                case "qt":
                case "mov": contentType = "video/quicktime"; break;
                case "wmv":
                case "avi": contentType = "video/x-ms-wmv"; break;
                case "lsf":
                case "lsx": contentType = "video/x-la-asf"; break;
                case "asf":
                case "asx": contentType = "video/x-ms-asf"; break;
                case "movie": contentType = "video/x-sgi-movie"; break;

                case "au":
                case "snd": contentType = "audio/basic"; break;
                case "rmi": contentType = "audio/mid"; break;
                case "mp3": contentType = "audio/mpeg"; break;
                case "aif":
                case "aiff":
                case "aifc": contentType = "audio/x-aiff"; break;
                case "m3u": contentType = "audio/x-mpegurl"; break;
                case "ra": contentType = "audio/x-pn-realaudio"; break;
                case "wav": contentType = "file	audio/x-wav"; break;

                default: contentType = "application/octet-stream"; break;
            }
            return contentType;
        }
        #endregion

        #region GetPartnerApplyStatusCH()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        public static string GetPartnerApplyStatusCH(this string Status)
        {
            if (Status.Equals("0")) return "待處理";
            else if (Status.Equals("1")) return "審核中";
            else if (Status.Equals("2")) return "預約中";
            else if (Status.Equals("3")) return "已完成";
            else if (Status.Equals("99")) return "取消";

            return Status;
        }
        #endregion

        #region GetMD5()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MyString"></param>
        /// <returns></returns>
        public static string GetMD5(this string MyString)
        {
            byte[] O = System.Text.Encoding.Default.GetBytes(MyString);

            MD5 M = MD5.Create();

            byte[] C = M.ComputeHash(O);

            string temp = "";

            foreach (byte ibyte in C)
            {
                temp += ibyte.ToString("x2");
            }
            temp = temp.Replace("-", "");

            return temp;
        }
        #endregion
    }
}