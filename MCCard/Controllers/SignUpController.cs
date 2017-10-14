using MCCard.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace MCCard.Controllers
{
    public class SignUpController : BaseController
    {
        //static List<AddressData> addressData = new List<AddressData>();
        public ActionResult Index()
        {
            //addressData = new CommonFun().GetAddress(false);
            return View();
        }

        public JsonResult SendSMS(string ump)
        {
            string result_code = "99999", result_text = string.Empty, message_id = string.Empty;

            if (!String.IsNullOrEmpty(ump))
            {
                XmlHelper xmlHelper = new XmlHelper();
                String result = String.Empty;
                try
                {
                    common.InitLog("SMS");

                    if (!ump.StartsWith("+886") && !ump.StartsWith("886"))
                    {
                        ump = string.Format("886{0}", ump.Remove(0, 1));
                    }
                    //WriteLog(Log.Mode.LogMode.ERROR, string.Format("UMP::{0}", ump));

                    string sms_body = common.GetSystemSetting("SMSBody");
                    string SMSURL = ConfigurationManager.AppSettings["SMSURL"].ToString();

                    ShortSmsSubmitReq xml_in = new ShortSmsSubmitReq();
                    xml_in.SysId = ConfigurationManager.AppSettings["SYSID"].ToString();
                    xml_in.SrcAddress = ConfigurationManager.AppSettings["SourceAddress"].ToString();
                    xml_in.DestAddress = ump;
                    xml_in.SmsBody = Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(sms_body));
                    xml_in.DrFlag = "true";
                    xml_in.FirstFailFlag = "false";

                    string xmlPaser = xmlHelper.toXmlString(xml_in);

                    String content = "xml=" + HttpUtility.UrlEncode(xmlPaser.Substring(1, xmlPaser.Length - 1));

                    byte[] bs = Encoding.UTF8.GetBytes(content);

                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(SMSURL);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = bs.Length;

                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }

                    using (WebResponse wr = req.GetResponse())
                    {
                        Stream streamResponse = wr.GetResponseStream();

                        using (StreamReader streamRead = new StreamReader(streamResponse, Encoding.Default))
                        {
                            result = streamRead.ReadToEnd();
                        }
                    }
                    //WriteLog(Log.Mode.LogMode.INFO, result);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(result);
                    result_code = xmlDoc.SelectSingleNode(".//ResultCode").InnerText.Trim();
                    result_text = xmlDoc.SelectSingleNode(".//ResultText").InnerText.Trim();
                    message_id = xmlDoc.SelectSingleNode(".//MessageId").InnerText.Trim();
                }
                catch (Exception ex)
                {
                    result_code = "99999";

                    result_text = "簡訊傳送失敗";

                    common.WriteLog(Log.Mode.LogMode.ERROR, string.Format("SMS.Exception::\r\n{0}", ex));
                }
            }
            else
            {
                result_text = string.Format("請輸入電話號碼");
            }

            return Json(new
            {
                Result = new SMS()
                {
                    Code = result_code,
                    Result = message_id,
                    Message = result_code.Equals("00000") ? "簡訊傳送成功" : result_text
                }
            });
        }

        public JsonResult smsauthcode(string uid, string code)
        {
            bool check = false;
            List<IDataParameter> para = null;

            try
            {
                common.InitLog("SMSAuth");
                common.WriteLog(Log.Mode.LogMode.ERROR, string.Format("UID::{0},CODE:{1}", uid, code));

                if (!String.IsNullOrEmpty(uid) && !String.IsNullOrEmpty(code))
                {
                    if (code == "12345678") check = true;//測試用
                    else
                    {
                        string strSql = common.Select.SMSVerify(uid, code, ref para);
                        common.WriteLog(Log.Mode.LogMode.DEBUG, strSql.EncryptDES());
                        check = common.DBConn.GeneralSqlCmd.ExecuteScalar(strSql, para);
                    }
                }
            }
            catch (Exception ex)
            {
                common.WriteLog(Log.Mode.LogMode.ERROR, string.Format("SMS.Exception::\r\n{0}", ex));
            }
            finally
            {
                common.CloseConn();
            }
            return Json(new { Result = check });
        }

        public JsonResult CreateUser(UserInfo info)
        {
            List<IDataParameter> para = null;
            try
            {
                Hashtable ht = new Hashtable();
                string usn = "UserID".GenerateKey("U");
                ht.Add("USN", usn);
                ht.Add("UID", info.UID);
                ht.Add("Password", info.Password);
                ht.Add("LastName", info.LastName);
                ht.Add("FirstName", info.FirstName);
                ht.Add("Sex", info.Sex);
                ht.Add("Birthday", info.Birthday);
                ht.Add("Mail", info.Mail);
                ht.Add("Zipcode", info.Zipcode);
                ht.Add("City", info.City);
                ht.Add("Township", info.Township);
                ht.Add("Address", info.Address);
                ht.Add("MaritalStatus", info.MaritalStatus);
                ht.Add("Children", info.Children);
                ht.Add("ChildrenYear", info.ChildrenYear);
                ht.Add("ReceiveNews", info.ReceiveNews);

                string strSql = common.Insert.User(ht, ref para);
                //發Email
                if (common.DBConn.GeneralSqlCmd.ExecuteNonQuery(strSql, para).Equals(1))
                {
                    //SendMail sendMail = null;
                    //string server_ip = GetSystemSetting("MailServerIP");
                    //string login_id = GetSystemSetting("MailServerLoginID");
                    //string password = GetSystemSetting("MailServerPassword");
                    //string from = GetSystemSetting("MailServerFrom");
                    //string subject = GetSystemSetting("MailServerRegisterSubject");
                    //int port = Convert.ToInt16(GetSystemSetting("MailServerPort"));
                    //bool ssl = GetSystemSetting("MailServerSSL").Trim().ToUpper().Equals("Y");

                    ////string url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");
                    //string url = "http://210.63.32.119/MCCard";

                    //string body = string.Format("感謝您成為麥當勞電子會員！您所註冊的電子會員帳號為{0}，<p />請立即點擊下方按鈕完成驗證並啟用您的電子會員帳號，開始享受更多專屬優惠！<p />", info.UID);

                    //string mail_body = MailFormat.Replace("[URL]", url);

                    //mail_body = mail_body.Replace("[LastName]", info.FirstName);
                    //mail_body = mail_body.Replace("[BODY]", body);
                    //mail_body = mail_body.Replace("[LINK]", string.Format("RegisterStep4.aspx?uid={0}", info.UID.EncryptDES()));
                    //mail_body = mail_body.Replace("[BUTTON]", "立刻驗證");

                    //sendMail = new SendMail(server_ip, login_id, password, port);
                    //sendMail.EnableSsl = ssl;
                    //sendMail.From = from;
                    //sendMail.Address = new string[] { info.Mail };
                    //sendMail.Subject = subject;
                    //sendMail.IsBodyHtml = true;
                    //sendMail.Body = mail_body;

                    //sendMail.Send();

                    //Session.RemoveAll();                   
                }
                return Json(new { Result = true, USN = usn });
            }
            catch (Exception ex)
            {
                common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
                return Json(new { Result = false });
            }
            finally
            {
                para = null;
                common.CloseConn();
            }
        }

        //public JsonResult GetCity()
        //{
        //    List<AddressData> result = addressData.GroupBy(test => test.City)
        //              .Select(grp => grp.First())
        //              .ToList();
        //    return Json(new { Result = result });
        //}

        //public JsonResult GetTown(string city)
        //{
        //    return Json(new { Result = addressData.Where(x => x.City == city).ToList() });
        //}

        //public JsonResult GetZipCode(string city, string town)
        //{
        //    return Json(new { Result = addressData.Where(x => x.City == city && x.Township == town).ToList() });
        //}
    }
     

    public class UserInfo
    {
        public string UID { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Sex { get; set; }
        public string Birthday { get; set; }
        public string Mail { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Township { get; set; }
        public string Address { get; set; }
        public string MaritalStatus { get; set; }
        public string Children { get; set; }
        public string ChildrenYear { get; set; }
        public string ReceiveNews { get; set; }
    }

    public class SMS
    {
        public string Code { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }

    }
}