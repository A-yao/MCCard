using System.Collections.Generic;
using System.Web.Mvc;
using MCCard.Common;
using System.Web;
using System.Data;
using System;
using XMail;
using Log;
using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace MCCard.Controllers
{
    public class AuthorityController : BaseController
    {
        //登入
        public JsonResult Login(string uid, string pwd)
        {
            bool result = false;
            common.InitLog("UserLogin");
            try
            {
                if (!String.IsNullOrEmpty(uid) && !String.IsNullOrEmpty(pwd))
                {
                    List<IDataParameter> para = null;
                    string strSql = common.Select.UserByUID(uid, pwd, ref para);
                    DataTable dt = null;
                    dt = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);
                    common.CloseConn();

                    if (dt != null && dt.Rows.Count.Equals(1))
                    {
                        if (SaveCookie(dt.Rows[0]["USN"].ToString().Trim()))
                        {
                            result = common.SetUserInfo(dt.Rows[0]["USN"].ToString().Trim());
                        }
                    }
                }
                common.WriteLog(Mode.LogMode.DEBUG, string.Format("{0} Login {1} ", ("ID：" + uid), (result ? "Success" : "Fail")));
            }
            catch (Exception ex)
            {
                common.WriteLog(Mode.LogMode.ERROR, string.Format("{0} Login {1} ", ("ID：" + uid), "Fail, error message:" + ex.Message.ToString()));
            }
            return Json(new { Result = result });
        }

        //登出
        public JsonResult Logout()
        {
            common.InitLog("UserLogout");
            common.WriteLog(Mode.LogMode.DEBUG, string.Format("{0} Logout", ("ID：" + common.GetSessionData().UID)));

            Session[CommonFun.SessionName.UserInfo.ToString()] = null;
            return Json(new { Result = Session[CommonFun.SessionName.UserInfo.ToString()] == null });
        }

        //驗證登入狀態(判斷Session有沒有值)
        public JsonResult CheckLoginStatus()
        {
            return Json(new { Result = Session[CommonFun.SessionName.UserInfo.ToString()] != null });
        }

        //確認密碼
        public JsonResult CheckPW(string uid, string pwd)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(uid) && !String.IsNullOrEmpty(pwd))
            {
                List<IDataParameter> para = null;
                string strSql = common.Select.UserByUID(uid, pwd, ref para);
                DataTable dt = null;
                dt = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);
                common.CloseConn();

                if (dt != null && dt.Rows.Count.Equals(1))
                {
                    result = true;
                }
            }
            return Json(new { Result = result });
        }

        //取得個人資料
        public JsonResult GetUserInfo()
        {
            return Json(new { Result = common.GetSessionData() });
        }

        //檢查登入驗證碼
        public JsonResult CheckCaptchaLogin(string captcha)
        {
            return Json(new { Result = new CommonFun().CheckCaptcha(captcha, "Login") });
        }

        public ActionResult GetImage()
        {
            return File(new Captcha().GetCaptcha(), "image/jpg");
        }
        public ActionResult GetImageLogin()
        {
            return File(new Captcha().GetCaptcha("Login"), "image/jpg");
        }

        public JsonResult CheckCaptcha(string captcha)
        {
            return Json(new { Result = new CommonFun().CheckCaptcha(captcha) });
        }

        //檢查使用者註冊狀態
        public JsonResult CheckUser(string uid)
        {
            List<IDataParameter> para = null;
            int returnCode = -1;
            try
            {
                if (!String.IsNullOrEmpty(uid))
                {
                    string strSql = common.Select.UserByUID(uid, string.Empty, ref para);
                    DataTable dt = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);

                    if (dt != null && dt.Rows.Count > 0) returnCode = Convert.ToInt16(dt.Rows[0]["Status"].ToString());
                }
                return Json(new { Result = returnCode });
            }
            catch (Exception ex)
            {
                common.InitLog("CheckUser");
                common.WriteLog(Mode.LogMode.ERROR, string.Format("CheckUser Exception: {0}", ex.Message.ToString()));
                return Json(new { Result = returnCode });
            }
            finally
            {
                para = null;
                common.CloseConn();
            }
        }

        //忘記密碼
        public JsonResult ForgetPW(string uid)
        {
            common.InitLog("ForgetPW");
            List<IDataParameter> para = null;
            bool check = false;
            try
            {
                if (!String.IsNullOrEmpty(uid))
                {
                    string strSql = common.Select.UserByUID(uid, string.Empty, ref para);

                    DataTable dt = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);

                    if (dt != null || !dt.Rows.Count.Equals(0))
                    {
                        string server_ip = common.GetSystemSetting("MailServerIP");
                        string login_id = common.GetSystemSetting("MailServerLoginID");
                        string password = common.GetSystemSetting("MailServerPassword");
                        string from = common.GetSystemSetting("MailServerFrom");
                        string subject = common.GetSystemSetting("MailServerRegisterSubject");
                        int port = Convert.ToInt16(common.GetSystemSetting("MailServerPort"));
                        bool ssl = common.GetSystemSetting("MailServerSSL").Trim().ToUpper().Equals("Y");

                        string url = ConfigurationManager.AppSettings["MacURL"];
                        string MailTemplete = ConfigurationManager.AppSettings["MailTemplate"];//MailTemplete名稱
                        string strPath = "";
                        strPath = Server.MapPath("~/wwwroot/contents/" + MailTemplete);
                        System.IO.TextReader txtRead = new System.IO.StreamReader(strPath, System.Text.Encoding.Default);
                        string mail_body = txtRead.ReadToEnd();
                        mail_body = mail_body.Replace("[UID]", uid);
                        mail_body = mail_body.Replace("[URL]", url);
                        mail_body = mail_body.Replace("[LastName]", dt.Rows[0]["FirstName"].ToString().DecryptDES());
                        mail_body = mail_body.Replace("[LINK]", string.Format("Member/UserProfile?forgotpassword={0}", string.Format("{0}|{1}", dt.Rows[0]["UID"].ToString(), dt.Rows[0]["Password"].ToString()).EncryptDES()));

                        MailMessage resetMail = new MailMessage(from, dt.Rows[0]["Mail"].ToString().DecryptDES());

                        resetMail.Subject = subject;
                        resetMail.Body = mail_body;
                        resetMail.IsBodyHtml = true;
                        resetMail.Priority = MailPriority.High;

                        #region 設定附件檔案(Attachment)                        
                        string strFilePath = Server.MapPath("~/wwwroot/Image/desktop_img/ic_mlogo.png");

                        System.Net.Mail.Attachment attachment1 =
                           new System.Net.Mail.Attachment(strFilePath);
                        attachment1.Name = System.IO.Path.GetFileName(strFilePath);
                        attachment1.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        System.Net.Mail.Attachment attachment2 =
                           new System.Net.Mail.Attachment(Server.MapPath("~/wwwroot/Image/mailbox/btn_bottom.png"));
                        attachment2.Name = System.IO.Path.GetFileName(Server.MapPath("~/wwwroot/Image/mailbox/btn_bottom.png"));
                        attachment2.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        System.Net.Mail.Attachment attachment3 =
                           new System.Net.Mail.Attachment(Server.MapPath("~/wwwroot/Image/mailbox/btn_left.png"));
                        attachment3.Name = System.IO.Path.GetFileName(Server.MapPath("~/wwwroot/Image/mailbox/btn_left.png"));
                        attachment3.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        System.Net.Mail.Attachment attachment4 =
                           new System.Net.Mail.Attachment(Server.MapPath("~/wwwroot/Image/mailbox/btn_right.png"));
                        attachment4.Name = System.IO.Path.GetFileName(Server.MapPath("~/wwwroot/Image/mailbox/btn_right.png"));
                        attachment4.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        System.Net.Mail.Attachment attachment5 =
                           new System.Net.Mail.Attachment(Server.MapPath("~/wwwroot/Image/mailbox/btn_top.png"));
                        attachment5.Name = System.IO.Path.GetFileName(Server.MapPath("~/wwwroot/Image/mailbox/btn_top.png"));
                        attachment5.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        System.Net.Mail.Attachment attachment6 =
                           new System.Net.Mail.Attachment(Server.MapPath("~/wwwroot/Image/mailbox/email_bottom.png"));
                        attachment6.Name = System.IO.Path.GetFileName(Server.MapPath("~/wwwroot/Image/mailbox/email_bottom.png"));
                        attachment6.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        System.Net.Mail.Attachment attachment7 =
                           new System.Net.Mail.Attachment(Server.MapPath("~/wwwroot/Image/mailbox/email_left.png"));
                        attachment7.Name = System.IO.Path.GetFileName(Server.MapPath("~/wwwroot/Image/mailbox/email_left.png"));
                        attachment7.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        System.Net.Mail.Attachment attachment8 =
                           new System.Net.Mail.Attachment(Server.MapPath("~/wwwroot/Image/mailbox/email_right.png"));
                        attachment8.Name = System.IO.Path.GetFileName(Server.MapPath("~/wwwroot/Image/mailbox/email_right.png"));
                        attachment8.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        System.Net.Mail.Attachment attachment9 =
                           new System.Net.Mail.Attachment(Server.MapPath("~/wwwroot/Image/mailbox/email_top.png"));
                        attachment9.Name = System.IO.Path.GetFileName(Server.MapPath("~/wwwroot/Image/mailbox/email_top.png"));
                        attachment9.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;

                        // 設定該附件為一個內嵌附件(Inline Attachment)
                        attachment1.ContentDisposition.Inline = true;
                        attachment1.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        attachment2.ContentDisposition.Inline = true;
                        attachment2.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        attachment3.ContentDisposition.Inline = true;
                        attachment3.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        attachment4.ContentDisposition.Inline = true;
                        attachment4.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        attachment5.ContentDisposition.Inline = true;
                        attachment5.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        attachment6.ContentDisposition.Inline = true;
                        attachment6.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        attachment7.ContentDisposition.Inline = true;
                        attachment7.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        attachment8.ContentDisposition.Inline = true;
                        attachment8.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        attachment9.ContentDisposition.Inline = true;
                        attachment9.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;

                        resetMail.Attachments.Add(attachment1);
                        resetMail.Attachments.Add(attachment2);
                        resetMail.Attachments.Add(attachment3);
                        resetMail.Attachments.Add(attachment4);
                        resetMail.Attachments.Add(attachment5);
                        resetMail.Attachments.Add(attachment6);
                        resetMail.Attachments.Add(attachment7);
                        resetMail.Attachments.Add(attachment8);
                        resetMail.Attachments.Add(attachment9);
                        #endregion

                        SmtpClient smtp = new SmtpClient(server_ip, port);
                        smtp.Credentials = new System.Net.NetworkCredential(login_id, password);
                        smtp.EnableSsl = ssl;
                        smtp.Send(resetMail);
                        check = true;
                    }
                }
                common.WriteLog(Mode.LogMode.DEBUG, string.Format("ID：" + uid + "ForgetPW Send Email {0}", (check ? "Success" : "Fail")));
                return Json(new { Result = check });
            }
            catch (Exception ex)
            {
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ForgetPW Exception {0}", "ID：" + uid, "Message" + ex.Message.ToString()));
                return Json(new { Result = false });
            }
            finally
            {
                common.CloseConn();
            }
        }

        #region 社群網站帳號單一登入
        //註冊
        public JsonResult RegisterCommunityInfo(string community, string usn, string id, string name)
        {
            //檢查帳號是否已被登記
            if (GetCommunityUser(community, id).Rows.Count >= 1)
            {
                return Json(new { Result = false, Message = "該" + community + "帳戶已被連結，請連結其他組帳號" });
            }
            string strSql = string.Empty;
            List<IDataParameter> para = null;
            usn = usn == null ? common.GetSessionData().USN : usn;
            try
            {
                switch (community)
                {
                    case "FaceBook":
                        strSql = common.Update.UserFacebook(usn, id, name, ref para);
                        break;
                    case "Google":
                        strSql = common.Update.UserGoogle(usn, id, name, ref para);
                        break;
                }
                if (common.DBConn.GeneralSqlCmd.ExecuteNonQuery(strSql, para).Equals(1))
                {
                    return Json(new { Result = true });
                }
                return Json(new { Result = false, Message = "該" + community + "帳戶連結失敗" });
            }
            catch (Exception ex)
            {
                common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
                return Json(new { Result = false, Message = "該" + community + "帳戶連結失敗" });
            }
            finally
            {
                para = null;
                common.CloseConn();
            }
        }
        //登入
        public JsonResult CommunityLogin(string community, string id)
        {
            DataTable dt = GetCommunityUser(community, id);
            if (dt != null && dt.Rows.Count.Equals(1))
            {
                if (SaveCookie(dt.Rows[0]["USN"].ToString().Trim()))
                {
                    common.SetUserInfo(dt.Rows[0]["USN"].ToString().Trim());
                    return Json(new { Result = true });
                }
                else
                {
                    return Json(new { Result = false, Message = "Cookie寫入失敗" });
                }
            }
            else
            {
                return Json(new { Result = false, Message = "您的帳號尚未與" + community + "帳戶連結" });
            }
        }

        public DataTable GetCommunityUser(string community, string id)
        {
            string strSql = string.Empty;
            List<IDataParameter> para = null;
            switch (community)
            {
                case "FaceBook":
                    strSql = common.Select.UserFacebook(id, ref para);
                    break;
                case "Google":
                    strSql = common.Select.UserGoogle(id, ref para);
                    break;
            }
            return common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);
        }

        private bool SaveCookie(string USN)
        {
            try
            {
                HttpCookie cookie = Request.Cookies["MCCard"];

                if (cookie == null)
                {
                    cookie = new HttpCookie("MCCard");
                    DateTime dt = DateTime.Now;
                    TimeSpan ts = new TimeSpan(365, 0, 0, 0, 0);
                    cookie.Expires = dt.Add(ts);
                    cookie.Values.Add("USN", USN);
                }
                else
                {
                    if (cookie.Values["USN"] != null) cookie.Values.Remove("USN");

                    DateTime dt = DateTime.Now;
                    TimeSpan ts = new TimeSpan(365, 0, 0, 0, 0);
                    cookie.Expires = dt.Add(ts);
                    cookie.Values.Add("USN", USN);
                }
                Response.AppendCookie(cookie);
                return true;
            }
            catch (System.Exception ex) { common.WriteLog(string.Format("SaveCookie.Exception::\r\n{0}", ex.ToString())); }

            return false;
        }
        #endregion
        public ContentResult LoginInfo()
        {
            StringBuilder sb = new StringBuilder();
            //註冊與登入
            string display_RegSign = "";
            //登入後
            string display_Login = "";
            //使用者資訊
            string UserInfo = "";
            if (Session[CommonFun.SessionName.UserInfo.ToString()] == null)
            {
                display_Login = "style='display:none'";
            }
            else
            {
                display_RegSign = "style='display:none'";
                UserInfoLib userInfoLib = common.GetSessionData();
                
                UserInfo += "<a id='btn_Menu_Member' href='' class='dropdown-oggle nav-text' data-toggle='dropdown' role='button' aria-haspopup='true' aria-expanded='false'>";
                UserInfo += "Hi " + userInfoLib.LastName + userInfoLib.FirstName + "</a>";
                UserInfo += "<ul class='dropdown-menu' style='margin-left: auto; '><li class='arrow_t_int DesktopView'></li><li class='arrow_t_out DesktopView'></li>";
                UserInfo += "<li><a id='btn_Menu_Member_Data_Maintain' class='nav-ext-dropdown' href='/Member/UserProfile'>修改個人資料</a></li>";
                UserInfo += "<hr class='meun_hr' />";
                UserInfo += "<li><a onclick='Logout(); return false' href='' class='nav-text-dropdown' >登出</a></li></ul>";
            }
            //編輯時請連同 Login.cshtml 異動 
            sb.Append("<li id='liLogin' " + display_RegSign + ">");
            sb.Append("<a id='btn_Menu_Member_Register' style='display: inline-block' class='nav-text-yellow' href='/SignUp'>");
            sb.Append("<img class='nav-signup'>新用戶註冊");
            sb.Append("</a>");
            sb.Append("<a id='btn_Menu_Member_Login' href='#' class='nav-text-yellow' style='display: inline-block; margin-left: 0px;'>");
            sb.Append("<img class='nav-signin'>報報APP會員登入");
            sb.Append("</a>");
            sb.Append("</li>");


            sb.Append("<li id='liuserInfo' class='dropdown' " + display_Login + " + >");
            sb.Append(UserInfo);           
            sb.Append("</ li > ");

            return Content(sb.ToString());
        }
    }
}