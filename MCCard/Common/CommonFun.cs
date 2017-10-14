using MCCard.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Log;
using System.Net;
using System.Configuration;

namespace MCCard.Common
{
    public class CommonFun : Page
    {
        string strSql = string.Empty;
        List<IDataParameter> para = null;
        DataTable dt = null;

        //API
        #region ECService
        /// <summary>
        /// 
        /// </summary>
        public MCCard_EC_Service.MCCard_EC _ECService = null;
        /// <summary>
        /// 
        /// </summary>
        public MCCard_EC_Service.MCCard_EC ECService
        {
            get
            {
                if (_ECService == null)
                {
                    _ECService = new MCCard_EC_Service.MCCard_EC();

                    _ECService.Url = ConfigurationManager.AppSettings["MCCard_EC_Service"].ToString().Trim();

                    _ECService.Discover();
                }
                return _ECService;
            }
            set { _ECService = value; }
        }
        #endregion

        //Email 格式
        #region MailFormat

        public string GetMailTemplate()
        {
            return "<table border=\"0\" width=\"1138\" height=\"640\" cellspacing=\"0\" cellpadding=\"0\">\n"
            + "	<tr>\n"
            //+ "		<td width=\"24\" rowspan=\"3\"><img border=\"0\" src=\"[URL]/wwwroot/Image/mailbox/email_left.png\" width=\"24\" height=\"640\"></td>\n"
            //+ "		<td height=\"24\" valign=\"top\" width=\"1090\"><img border=\"0\" src=\"[URL]/wwwroot/Image/mailbox/email_top.png\" width=\"1090\" height=\"24\"></td>\n"
            //+ "		<td width=\"24\" rowspan=\"3\"><img border=\"0\" src=\"[URL]/wwwroot/Image/mailbox/email_right.png\" width=\"24\" height=\"640\"></td>\n"
            + "	</tr>\n"
            + "	<tr>\n"
            + "		<td>\n"
            + "		<table border=\"0\" width=\"100%\" id=\"table1\" cellspacing=\"0\" cellpadding=\"0\" height=\"100%\">\n"
            + "			<tr>\n"
            + "				<td height=\"30%\" width=\"7%\">&nbsp;</td>\n"
            + "				<td height=\"30%\" width=\"86%\"><img src=\"[URL]/wwwroot/Image/desktop_img/ic_mlogo.png\" style=\"width:86px;height:86px\"></td>\n"
            + "				<td height=\"30%\" width=\"7%\">&nbsp;</td>\n"
            + "			</tr>\n"
            + "			<tr>\n"
            + "				<td height=\"53%\" width=\"7%\">&nbsp;</td>\n"
            + "				<td height=\"53%\" width=\"86%\">\n"
            + "					<div><font face=\"Noto Sans TC\" size=\"5\" color=\"#000\">Hi [LastName],</font></p></div>\n"
            + "					<div style=\"font-family:'Noto Sans TC', sans-serif;font-size:24px;color:#8c8c8c;\">\n"
            + "						<font face=\"Noto Sans TC\" size=\"5\" color=\"#8c8c8c\">[BODY]</font>\n"
            + "						<p>\n"
            + "						<table cellspacing=\"0\" cellpadding=\"0\" height=\"86\">\n"
            + "							<tr>\n"
            //+ "								<td width=\"24\" rowspan=\"3\"><img border=\"0\" src=\"[URL]/wwwroot/Image/mailbox/btn_left.png?t=1\" width=\"24\" height=\"86\"></td>\n"
            //+ "								<td width=\"180\" valign=\"top\"><img border=\"0\" src=\"[URL]/wwwroot/Image/mailbox/btn_top.png?t=1\" width=\"180\" height=\"22\"></td>\n"
            //+ "								<td width=\"24\" rowspan=\"3\"><img border=\"0\" src=\"[URL]/wwwroot/Image/mailbox/btn_right.png?t=1\" width=\"24\" height=\"86\"></td>\n"
            + "							</tr>\n"
            + "							<tr>\n"
            + "								<td height=\"42\" align=\"center\" width=\"180\"><a target=\"_blank\" href=\"[URL]/[LINK]\"><font face=\"Noto Sans TC\" size=\"5\" color=\"#000\">[BUTTON]</font></a></td>\n"
            + "							</tr>\n"
            + "							<tr>\n"
            //+ "								<td width=\"180\" valign=\"bottom\"><img border=\"0\" src=\"[URL]/wwwroot/Image/mailbox/btn_bottom.png?t=1\" width=\"180\" height=\"22\"></td>\n"
            + "							</tr>\n"
            + "						</table>\n"
            + "					</div>\n"
            + "				</td>\n"
            + "				<td height=\"53%\" width=\"7%\">&nbsp;</td>\n"
            + "			</tr>\n"
            + "			<tr>\n"
            + "				<td height=\"17%\" width=\"7%\">&nbsp;</td>\n"
            + "				<td height=\"17%\" width=\"86%\">&nbsp;</td>\n"
            + "				<td height=\"17%\" width=\"7%\">&nbsp;</td>\n"
            + "			</tr>\n"
            + "		</table>\n"
            + "		</td>\n"
            + "	</tr>\n"
            + "	<tr>\n"
            //+ "		<td height=\"24\" valign=\"bottom\" width=\"1090\"><img border=\"0\" src=\"[URL]/Images/mailbox/email_bottom.png\" width=\"1090\" height=\"24\"></td>\n"
            + "	</tr>\n"
            + "</table>\n";
        }
        #endregion

        //資料庫連線
        #region DBConn
        /// <summary>
        /// 資料庫物件介面
        /// </summary>
        private DBLib.IDBConn _DBConn = null;
        /// <summary>
        /// 資料庫物件介面
        /// </summary>
        public DBLib.IDBConn DBConn
        {
            get
            {
                if (_DBConn == null)
                {
                    _DBConn = new DBLib.DBConn(new DBLibUtility.Mode.FreeMode()
                    {
                        APMode = DBLibUtility.Mode.APMode.Web,
                        DBMode = DBLibUtility.Mode.DBMode.MSSQL,
                        ConnectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString().DecryptDES()
                    });
                }
                return _DBConn;
            }
            set { _DBConn = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public void CloseConn()
        {
            if (_DBConn != null)
            {
                DBConn.Dispose(); DBConn = null;
            }
        }
        #endregion

        //資料庫交易
        #region DBConnTransac
        /// <summary>
        /// 資料庫物件介面
        /// </summary>
        private DBLib.IDBTransacConn _DBConnTransac = null;
        /// <summary>
        /// 資料庫物件介面
        /// </summary>
        public DBLib.IDBTransacConn DBConnTransac
        {
            get
            {
                if (_DBConnTransac == null)
                {
                    _DBConnTransac = new DBLib.DBConnTransac(new DBLibUtility.Mode.FreeMode()
                    {
                        APMode = DBLibUtility.Mode.APMode.Web,
                        DBMode = DBLibUtility.Mode.DBMode.MSSQL,
                        ConnectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString().DecryptDES()
                    });
                }
                return _DBConnTransac;
            }
            set { _DBConnTransac = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public void CloseConnTransac()
        {
            if (_DBConnTransac != null)
            {
                DBConnTransac.Dispose(); DBConnTransac = null;
            }
        }
        #endregion

        //紀錄
        #region Log
        /// <summary>
        /// Log 介面
        /// </summary>
        public ILog MyLog = null;
        /// <summary>
        /// Log 介面
        /// </summary>
        public void InitLog(string LogName)
        {
            if (MyLog == null)
            {
                MyLog = new WebLog(LogName);
                MyLog.MaxSize = 2;
                MyLog.DirectoryPath = Utility.WebConfig.LogPath;
                try
                {
                    DateTime now_date_time = DateTime.Now;
                    string check_clear_path = Path.Combine(MyLog.DirectoryPath, "clear.flag");
                    if (!File.Exists(check_clear_path)) File.WriteAllText(check_clear_path, now_date_time.ToString("yyyy/MM/dd HH:mm:ss"));
                    DateTime date_time = DateTime.Parse(File.ReadAllText(check_clear_path).Trim());
                    bool clear = DateTime.Compare(DateTime.Now, date_time.AddDays(14)) > 0;

                    if (clear)
                    {
                        MyLog.Delete(14);

                        MyLog.DeleteBackup(14);

                        File.WriteAllText(check_clear_path, now_date_time.ToString("yyyy/MM/dd HH:mm:ss"));

                        MyLog.WriteLog(Mode.LogMode.INFO, string.Format("ClearLog::{0}", now_date_time.ToString("yyyy/MM/dd HH:mm:ss")));
                    }
                    MyLog.WriteLog(Mode.LogMode.INFO, string.Format("[{0}]Start Ver.{1}", GetClientIPv4(), System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));
                }
                catch (System.Exception ex) { MyLog.WriteLog(Mode.LogMode.ERROR, string.Format("ClearLog.Exception::{0}", ex.ToString())); }
            }
        }
        /// <summary>
        /// 寫入成 Log
        /// </summary>
        /// <param name="Message">Log 訊息</param>
        public void WriteLog(string Message)
        {
            MyLog.WriteLog(Mode.LogMode.INFO, string.Format("[{0}] {1}", IPv4, Message));
        }
        /// <summary>
        /// 寫入成 Log
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="Message">Log 訊息</param>
        public void WriteLog(Mode.LogMode Mode, string Message)
        {
            if (MyLog != null)
            {
                MyLog.WriteLog(Mode, string.Format("[{0}] {1}", IPv4, Message));
            }
        }
        #endregion

        #region IPv4
        /// <summary>
        /// 
        /// </summary>
        private string _IPv4 = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string IPv4
        {
            get
            {
                if (String.IsNullOrEmpty(_IPv4)) _IPv4 = GetClientIPv4();

                return _IPv4;
            }
        }
        #endregion

        //連結資料庫
        #region SqlCommand
        /// <summary>
        /// Sql Command (Select)
        /// </summary>
        public SqlCommand.Select Select
        {
            get
            {
                return new SqlCommand.Select();
            }
        }
        /// <summary>
        /// Sql Command (Update)
        /// </summary>
        public SqlCommand.Update Update
        {
            get
            {
                return new SqlCommand.Update();
            }
        }
        /// <summary>
        /// Sql Command (Delete)
        /// </summary>
        public SqlCommand.Delete Delete
        {
            get
            {
                return new SqlCommand.Delete();
            }
        }
        /// <summary>
        /// Sql Command (Insert)
        /// </summary>
        public SqlCommand.Insert Insert
        {
            get
            {
                return new SqlCommand.Insert();
            }
        }
        #endregion

        #region Session 
        public enum SessionName
        {
            UserInfo,
            Captcha,
            CaptchaLogin,
            ZipCode,
            Register,
            PageName,
            ReturnPageName,
            Profile,
            LoginMode,
            ProblemType,
            ProblemDetail,
            ProblemLink
        }

        //取得當前Session的資料
        public UserInfoLib GetSessionData()
        {
            return (UserInfoLib)Session[SessionName.UserInfo.ToString()];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="USN"></param>
        /// <returns></returns>
        public bool SetUserInfo(string USN)
        {
            Session[CommonFun.SessionName.UserInfo.ToString()] = GetUserInfo(USN, false);
            return Session[CommonFun.SessionName.UserInfo.ToString()] != null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public UserInfoLib GetUserInfo(string USN, bool IsUID)
        {
            DataTable dt = null;

            List<IDataParameter> para = null;
            try
            {
                string strSql = IsUID ? Select.UserByUID(USN, string.Empty, ref para) : Select.UserByUSN(USN, string.Empty, ref para);

                dt = DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);

                if (dt == null || dt.Rows.Count.Equals(0)) return null;

                return new UserInfoLib()
                {
                    USN = dt.Rows[0]["USN"].ToString().Trim(),
                    UID = dt.Rows[0]["UID"].ToString().Trim().DecryptDES(),
                    Password = dt.Rows[0]["Password"].ToString().Trim().DecryptDES(),
                    LastName = dt.Rows[0]["LastName"].ToString().Trim().DecryptDES(),
                    FirstName = dt.Rows[0]["FirstName"].ToString().Trim().DecryptDES(),
                    Sex = Convert.ToInt16(dt.Rows[0]["Sex"].ToString().Trim()),
                    Birthday = dt.Rows[0]["Birthday"].ToString().Trim(),
                    Zipcode = dt.Rows[0]["Zipcode"].ToString().Trim(),
                    City = dt.Rows[0]["City"].ToString().Trim().DecryptDES(),
                    Township = dt.Rows[0]["Township"].ToString().Trim().DecryptDES(),
                    Address = dt.Rows[0]["Address"].ToString().Trim().DecryptDES(),
                    MaritalStatus = Convert.ToInt16(dt.Rows[0]["MaritalStatus"].ToString().Trim()),
                    Children = Convert.ToInt16(dt.Rows[0]["Children"].ToString().Trim()),
                    ChildrenYear = Convert.ToInt16(dt.Rows[0]["ChildrenYear"].ToString().Trim()),
                    Mail = dt.Rows[0]["Mail"].ToString().Trim().DecryptDES(),
                    FacebookUID = dt.Rows[0]["FacebookUID"].ToString().Trim(),//.DecryptDES()
                    GooglePlusUID = dt.Rows[0]["GooglePlusUID"].ToString().Trim(),//.DecryptDES()
                    ReceiveNews = Convert.ToInt16(dt.Rows[0]["ReceiveNews"].ToString().Trim()),
                    Status = Convert.ToInt16(dt.Rows[0]["Status"].ToString().Trim()),
                    LastModifyTime = Convert.ToDateTime(dt.Rows[0]["LastModifyTime"].ToString().Trim())
                };
            }
            catch (System.Exception ex) { WriteLog(Mode.LogMode.ERROR, string.Format("GetUserInfo.Exception::{0}", ex.ToString())); }
            finally
            {
                para = null; dt = null; CloseConn();
            }
            return null;
        }
        #endregion 

        //取得Excel
        #region GetDataTableFromCsv()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CSVPath"></param>
        /// <returns></returns>
        public DataTable GetDataTableFromCsv(string CSVPath)
        {
            string header = "Yes";
            string pathOnly = Path.GetDirectoryName(CSVPath);
            string fileName = Path.GetFileName(CSVPath);
            string sql = @"SELECT * FROM [" + fileName + "]";

            DataTable dataTable = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly + ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
            }
            return dataTable;
        }
        #endregion

        #region CreateNewXmlFormat()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="XmlString"></param>
        /// <returns></returns>
        public string CreateNewXmlFormat(string XmlString)
        {
            return string.Format("<?xml version=\"1.0\" encoding=\"utf-8\" ?>{0}", XmlString);
        }
        #endregion

        #region GetClientIPv4()
        /// <summary>
        /// 取得客戶端主機 IPv4 位址
        /// </summary>
        /// <returns></returns>
        public string GetClientIPv4()
        {
            string ipv4 = String.Empty;

            foreach (IPAddress ip in Dns.GetHostAddresses(GetClientIP()))
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = ip.ToString(); break;
                }
            }
            if (ipv4 != String.Empty) return ipv4;

            foreach (IPAddress ip in Dns.GetHostEntry(GetClientIP()).AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = ip.ToString(); break;
                }

            }
            return ipv4;
        }
        #endregion

        #region GetClientIP()
        /// <summary>
        /// 取得客戶端主機位址
        /// </summary>
        public string GetClientIP()
        {
            if (null == HttpContext.Current.Request.ServerVariables["HTTP_VIA"])
            {
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
        }
        #endregion

        #region RequestQueryString()
        /// <summary>
        /// 取得參數
        /// </summary>
        public string RequestQueryString(string QueryName)
        {
            return Request.QueryString[QueryName] != null ? Server.UrlDecode(Request.QueryString[QueryName].Trim()).Replace(StringFormatException.Mode.CrossSiteScripting) : string.Empty;
        }
        #endregion

        //取得Config參數
        #region Config
        public string GetSystemSetting(string SystemName)
        {
            string result = string.Empty;

            DataTable dtSys = Session["SystemSetting"] != null ? (DataTable)Session["SystemSetting"] : null;
            try
            {
                if (dtSys == null)
                {
                    dtSys = DBConn.GeneralSqlCmd.ExecuteToDataTable(string.Format("Select s.name, s.comment From SystemSetting s Where 1 = 1"));

                    Session["SystemSetting"] = dtSys;

                    CloseConn();
                }
                if (dtSys == null) return string.Empty;

                DataRow[] dr = dtSys.Select(string.Format("name = '{0}'", SystemName));

                result = dr != null && dr.Length > 0 ? dr[0]["comment"].ToString().Trim() : string.Empty;
            }
            catch (System.Exception ex) { throw new Exception(string.Format("GetSystemSetting.Exception::{0}", ex.ToString())); }

            return result;
        }
        #endregion

        //橫幅
        #region LoadSubBanner()
        /// <summary>
        /// 
        /// </summary>
        public enum SubBanner
        {
            UserProfileBanner,
            CardListBanner,
            CardListMobileBanner,
            CardListSubBanner,
            CardListSubMobileBanner,
            CardIntroductionBanner,
            CardIntroductionMobileBanner,
            CommonProblemBanner,
            CommonProblemMobileBanner,
            LatestNewsBanner,
            LatestNewsMobileBanner,
            MCCardAppBanner,
            MCCardAppMobileBanner,
            MCCardAPPNewsSubBanner,
            MCCardAPPNewsSubBanner1,
            MCCardAPPNewsSubBanner2,
            MCCardAPPNewsSubBanner3,
            MyCardUserProfileBanner,
            MyCardUserProfileMobileBanner
        }

        public Dictionary<string, string> LoadSubBanner(SubBanner SubBannerMode, SubBanner SubMobileBannerMode)
        {
            try
            {
                string banner_name_pc = GetSystemSetting(SubBannerMode.ToString());
                string image_path_pc = String.IsNullOrEmpty(banner_name_pc) ? "/wwwroot/Image/Banner/?" : string.Format("/wwwroot/Image/Banner/{0}", banner_name_pc);

                string banner_name_mobile = GetSystemSetting(SubMobileBannerMode.ToString());
                string image_path_mobile = String.IsNullOrEmpty(banner_name_mobile) ? "/wwwroot/Image/Banner/?" : string.Format("/wwwroot/Image/Banner/{0}", banner_name_mobile);

                return new Dictionary<string, string>() {
                       { image_path_pc, image_path_mobile }
                    };
            }
            catch (Exception ex)
            {
                WriteLog(Mode.LogMode.ERROR, ex.ToString());
                return new Dictionary<string, string>() {
                       { "", "" }
                    };
            }
        }
        #endregion

        //圖形驗證碼
        #region Captcha
        public bool CheckCaptcha(string captcha, string root = null)
        {
            try
            {
                string captchaSession = (Session[SessionName.Captcha.ToString()] ?? "").ToString(); ;
                if (root == "Login")
                {
                    captchaSession = (Session[SessionName.CaptchaLogin.ToString()] ?? "").ToString(); ;
                }
                bool check = captchaSession.ToLower().Equals(captcha.ToLower());
                return check;
            }
            catch (Exception ex)
            {
                InitLog("Common");
                WriteLog(Log.Mode.LogMode.ERROR, string.Format("Common.Exception::\r\n{0}", ex));
                return false;
            }
            finally
            {
                para = null; dt = null;
                CloseConn();
            }
        }
        #endregion

        //從Excel取得地址
        #region GetAddress
        public List<AddressData> GetAddress()
        {
            var root = HttpContext.Current.Server.MapPath("");
            root = root.Substring(0, root.LastIndexOf('\\'));

            DataTable table = GetDataTableFromCsv(Path.Combine(root, "wwwroot\\contents", "zipcode3.csv"));
            List<AddressData> list = new List<AddressData>();
            list = (from l in table.AsEnumerable() /* Assume Linq to SQL */
                    select new AddressData()
                    {
                        City = l.Field<string>("City"),
                        Township = l.Field<string>("Township"),
                        Zipcode = l.Field<int>("Zipcode"),
                    }).ToList<AddressData>();
            return list;
        }
        #endregion

        //計算時間
        public string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = ts.Minutes > 0 ? ts.Minutes.ToString() + "分鐘又" : "" + ts.Seconds.ToString() + "." + ts.Milliseconds.ToString() + "秒";
            return dateDiff;
        }

        //以下未完整整理放置位子
        public int checkmail(string usn, string mail)
        {
            int returnCode = -1;
            try
            {
                if (!String.IsNullOrEmpty(usn) && !String.IsNullOrEmpty(mail))
                {
                    strSql = Select.EmailVerify(usn, mail, ref para);

                    dt = DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);

                    if (dt != null && dt.Rows.Count > 0) returnCode = Convert.ToInt16(dt.Rows[0]["Status"].ToString());
                }
                //xmlString = string.Format("<EmailVerify><Result>{0}</Result></EmailVerify>", returnCode.ToString());
                return returnCode;
            }
            catch (System.Exception ex)
            {
                InitLog("Common");

                WriteLog(Log.Mode.LogMode.ERROR, string.Format("Common.Exception::\r\n{0}", ex));
                return returnCode;
            }
            finally
            {
                para = null; dt = null;
                CloseConn();
            }
        }
    }
    public class ShortSmsSubmitReq
    {
        public string SysId = string.Empty;
        public string SrcAddress = string.Empty;
        public string DestAddress = string.Empty;
        public string SmsBody = string.Empty;
        public string DrFlag = string.Empty;
        public string FirstFailFlag = string.Empty;
    }
}

