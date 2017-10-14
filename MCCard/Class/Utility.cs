using System;
using System.Configuration;
using System.Text;

namespace MCCard
{
    public static class Utility
    {
        private const string UnReservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Url Encoding
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (UnReservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        #region WebConfig
        /// <summary>
        /// 
        /// </summary>
        public class WebConfig
        {
            #region LogPath
            /// <summary>
            /// 
            /// </summary>
            public static string LogPath
            {
                get
                {
                    if (ConfigurationManager.AppSettings["LogPath"] == null) throw new Exception("Web.Config Error : LogPath");

                    return ConfigurationManager.AppSettings["LogPath"].Trim();
                }
            }
            #endregion

            #region MID
            /// <summary>
            /// 
            /// </summary>
            public static string MID
            {
                get
                {
                    if (ConfigurationManager.AppSettings["MID"] == null) throw new Exception("Web.Config Error : MID");

                    return ConfigurationManager.AppSettings["MID"].Trim();
                }
            }
            #endregion

            #region TID
            /// <summary>
            /// 
            /// </summary>
            public static string TID
            {
                get
                {
                    if (ConfigurationManager.AppSettings["TID"] == null) throw new Exception("Web.Config Error : TID");

                    return ConfigurationManager.AppSettings["TID"].Trim();
                }
            }
            #endregion

            #region SVC_LMS
            /// <summary>
            /// 
            /// </summary>
            public static string SVC_LMS
            {
                get
                {
                    if (ConfigurationManager.AppSettings["SVC_LMS"] == null) throw new Exception("Web.Config Error : SVC_LMS");

                    return ConfigurationManager.AppSettings["SVC_LMS"].Trim();
                }
            }
            #endregion
        }
        #endregion
    }
}