using Log;
using MCCard.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace MCCard.Controllers
{
    public class CardAccountController : BaseController
    {
        string mid = Utility.WebConfig.MID;
        string tid = Utility.WebConfig.TID;
        string svc_lms = Utility.WebConfig.SVC_LMS;

        #region 卡片記名
        public ActionResult MyCardRegister()
        {
            Initialization();
            return PartialView("MyCardRegister");
        }

        public JsonResult RegisterCard(string CardNumber, string AuthenticationCode)
        {
            DateTime dt_Initial = DateTime.Now;
            common.InitLog("RegisterCard");

            string strSql = string.Empty;
            string cardMax = ConfigurationManager.AppSettings["CardRegMax"].ToString();
            List<IDataParameter> para = null;
            Hashtable ht = new Hashtable();
            DataTable dt = null;

            UserInfoLib UserInfo = common.GetSessionData();
            
            try
            {
                strSql = common.Select.PointCard(UserInfo.USN, ref para);
                dt = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);
                
                if (dt != null && dt.Rows.Count >= Convert.ToInt32(cardMax))
                {
                    return Json(new { Result = "您的點點卡記名卡數已超過"+ cardMax + "張上限" });
                }

                strSql = common.Select.PointCard(string.Empty, CardNumber, ref para);
                dt = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!dt.Rows[0]["CustID"].ToString().Trim().Equals(UserInfo.USN))
                    {
                        return Json(new { Result = "您登記的卡號已登記於其他帳號中，卡號是否輸入錯誤呢?" });
                    }
                    else
                    {
                        return Json(new { Result = "該卡號已被註冊" });
                    }
                }

                ht.Add("UID", "CardStyle".GenerateKey("CD").EncryptDES());
                ht.Add("CustID", UserInfo.USN);
                ht.Add("CardNumber", CardNumber.Trim());
                ht.Add("SecurityCode", AuthenticationCode.Trim());
                ht.Add("Source", "WEB");

                strSql = common.Insert.PointCard(ht, ref para);
                if (!common.DBConn.GeneralSqlCmd.ExecuteNonQuery(strSql, para).Equals(1))
                {
                    common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), CardNumber, "記名失敗", common.DateDiff(dt_Initial, DateTime.Now)));
                    return Json(new { Result = "系統發生錯誤,請聯繫相關人員" });
                }
                else
                {
                    common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), CardNumber, "記名成功", common.DateDiff(dt_Initial, DateTime.Now)));
                    return Json(new { Result = "", Card = GetCards().Where(x => x.CardID == CardNumber).FirstOrDefault() });
                }
            }
            catch (Exception ex)
            {
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), CardNumber, "記名失敗:" + ex.ToString(), common.DateDiff(dt_Initial, DateTime.Now)));
                return Json(new { Result = "系統發生錯誤,請聯繫相關人員" });
            }
            finally
            {
                common.CloseConn();
            }
        }
        #endregion

        #region 取消卡片記名
        public ActionResult MyCardCancel()
        {
            Initialization();
            if (Session[CommonFun.SessionName.UserInfo.ToString()] != null)
            {
                return PartialView("MyCardCancel", GetCards());
            }
            else {
                List<CardModel> cards = new List<CardModel>();
                return PartialView("MyCardCancel", cards);
            }
        }

        public JsonResult CancelCard(string uid)
        {
            DateTime dt_Initial = DateTime.Now;
            common.InitLog("CancelCard");
            List<IDataParameter> para = null;
            UserInfoLib UserInfo = common.GetSessionData();

            try
            {
                string strSql = common.Delete.PointCard(uid, ref para);
                if (!common.DBConn.GeneralSqlCmd.ExecuteNonQuery(strSql, para).Equals(1))
                {
                    common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), uid, "取消記名失敗:", common.DateDiff(dt_Initial, DateTime.Now)));
                    return Json(new { Result = "系統發生錯誤,請聯繫相關人員" });
                }
                else
                {
                    common.WriteLog(Mode.LogMode.DEBUG, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), uid, "取消記名成功:", common.DateDiff(dt_Initial, DateTime.Now)));
                    return Json(new { Result = "" });
                }
            }
            catch (Exception ex)
            {
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), uid, "取消記名失敗:" + ex.ToString(), common.DateDiff(dt_Initial, DateTime.Now)));
                return Json(new { Result = "系統發生錯誤,請聯繫相關人員" });
            }
            finally
            {
                common.CloseConn();
            }
        }
        #endregion

        #region 卡片消費紀錄
        public ActionResult MyCardExpenseRecords()
        {
            Initialization();
            ViewBag.PageSize = common.GetSystemSetting("PageSize");
            if (Session[CommonFun.SessionName.UserInfo.ToString()] != null)
            {
                return PartialView("MyCardExpenseRecords", GetCards());
            }
            else
            {
                List<CardModel> cards = new List<CardModel>();
                return PartialView("MyCardExpenseRecords", cards);
            }
        }

        public JsonResult ExpenseRecordsRead(string CardNumber)
        {
            DateTime dt_Initial = DateTime.Now;
            List<CardExpenseRecordsModel> records = new List<CardExpenseRecordsModel>();

            try
            {
                StringBuilder json_input = new StringBuilder();
                string call_time = DateTime.Now.ToString("yyyyMMddhhmmss");
                string mask = string.Format("Mc{0}Donalds", CardNumber + mid + tid + svc_lms + call_time).GetMD5();

                json_input = new StringBuilder();
                json_input.AppendLine("{");
                json_input.AppendLine(string.Format(" \"cardNo\" : \"{0}\"", CardNumber));
                json_input.AppendLine(string.Format(" ,\"mid\" : \"{0}\"", mid));
                json_input.AppendLine(string.Format(" ,\"tid\" : \"{0}\"", tid));
                json_input.AppendLine(string.Format(" ,\"expiryDate\" : \"{0}\"", svc_lms));
                json_input.AppendLine(string.Format(" ,\"callTime\" : \"{0}\"", call_time));
                json_input.AppendLine(string.Format(" ,\"mask\" : \"{0}\"", mask));
                json_input.AppendLine("}");

                string json_request = common.ECService.queryTxn(json_input.ToString());
                XmlDocument doc = JsonConvert.DeserializeXmlNode(json_request, "txnList");

                var TransactionDate = doc.SelectNodes(".//txnDateTime");
                var Stores = doc.SelectNodes(".//storeName");
                var ConsumptionCategory = doc.SelectNodes(".//txnID");
                var TradeName = doc.SelectNodes(".//txnName");
                var Balance = doc.SelectNodes(".//amount");
                var Points = doc.SelectNodes(".//bonus");

                for (int i = 0; i < TransactionDate.Count; i++)
                {
                    records.Add(new CardExpenseRecordsModel
                    {
                        TransactionDate = TransactionDate[i].InnerText,
                        Stores = Stores[i].InnerText,
                        ConsumptionCategory = ConsumptionCategory[i].InnerText,
                        TradeName = TradeName[i].InnerText,
                        Balance = Balance[i].InnerText,
                        Points = Points[i].InnerText,
                    });
                }
            }
            catch (Exception ex)
            {
                common.InitLog("ExpenseRecords");
                UserInfoLib UserInfo = common.GetSessionData();
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), CardNumber, "ExpenseRecords Error:" + ex.ToString(), common.DateDiff(dt_Initial, DateTime.Now)));
            }
            finally
            {
                common.ECService.Dispose(); common.ECService = null;
            }
            return Json(records);
        }
        #endregion

        #region 卡片掛失
        public ActionResult MyCardLose()
        {
            Initialization();
            ViewBag.ImportHtml = "本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。 本卡採無註冊、記名方式發售，若卡片遺失 、遭竊、詐取、毀損、滅失或遭第三者佔有等情事，加值金額視同現金遺失。恕無法提供金額賠償或掛失止付，卡片內之金額及點 數亦無掛失補發服務，請妥善保管使用。";
            if (Session[CommonFun.SessionName.UserInfo.ToString()] != null)
            {
                return PartialView("MyCardLose", GetCards());
            }
            else
            {
                List<CardModel> cards = new List<CardModel>();
                return PartialView("MyCardLose", cards);
            }
        }

        public JsonResult LoseCard(string CardNumber)
        {
            StringBuilder json_input = new StringBuilder();
            XmlDocument xmlDoc = null;
            DateTime dt_Initial = DateTime.Now;

            try
            {
                string call_time = DateTime.Now.ToString("yyyyMMddhhmmss");
                string mask = string.Format("Mc{0}Donalds", CardNumber + mid + tid + svc_lms + call_time).GetMD5();

                json_input = new StringBuilder();
                json_input.AppendLine("{");
                json_input.AppendLine(string.Format(" \"cardNo\" : \"{0}\"", CardNumber));
                json_input.AppendLine(string.Format(" ,\"mid\" : \"{0}\"", mid));
                json_input.AppendLine(string.Format(" ,\"tid\" : \"{0}\"", tid));
                json_input.AppendLine(string.Format(" ,\"expiryDate\" : \"{0}\"", svc_lms));
                json_input.AppendLine(string.Format(" ,\"callTime\" : \"{0}\"", call_time));
                json_input.AppendLine(string.Format(" ,\"mask\" : \"{0}\"", mask));
                json_input.AppendLine("}");

                string json_request = common.ECService.lost(json_input.ToString());
                xmlDoc = JsonConvert.DeserializeXmlNode(json_request);
                string xml_string = xmlDoc.OuterXml;

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}", xml_string));

                int code_id = Convert.ToInt32(xmlDoc.SelectSingleNode(".//codeID").InnerText.Trim());
                string code_description = xmlDoc.SelectSingleNode(".//codeDescription").InnerText.Trim();

                if (!code_id.Equals(0))
                {
                    common.InitLog("LoseCard");
                    UserInfoLib UserInfo = common.GetSessionData();
                    common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), CardNumber, "LoseCard Error: code_id=" + code_id, common.DateDiff(dt_Initial, DateTime.Now)));
                }
                return Json(new { code_id = code_id, code_description = code_description });
            }
            catch (Exception ex)
            {
                common.InitLog("LoseCard");
                UserInfoLib UserInfo = common.GetSessionData();
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 卡號：{1} , 狀態：{2} , 所花費時間：{3}", (UserInfo.UID), CardNumber, "LoseCard Error:" + ex.ToString(), common.DateDiff(dt_Initial, DateTime.Now)));
                return Json(new { code_id = "", code_description = ex.ToString() });
            }
            finally
            {
                common.ECService.Dispose(); common.ECService = null;
            }
        }
        #endregion

        #region 點數餘額轉置
        public ActionResult MyCardTranspose()
        {
            Initialization();
            if (Session[CommonFun.SessionName.UserInfo.ToString()] != null)
            {
                return PartialView("MyCardTranspose", GetCards());
            }
            else
            {
                List<CardModel> cards = new List<CardModel>();
                return PartialView("MyCardTranspose", cards);
            }
        }

        public JsonResult Transpose(string output_CardNo, string output_ValidateCode, string input_CardNo, string input_ValidateCode, string balance, string bonus, string input_CardStatus)
        {
            StringBuilder json_input = new StringBuilder();
            XmlDocument xmlDoc = null;
            DateTime dt_Initial = DateTime.Now;

            try
            {
                string call_time = DateTime.Now.ToString("yyyyMMddhhmmss");
                string mask = string.Format("Mc{0}Donalds", mid + tid + output_CardNo + output_ValidateCode + input_CardNo + input_ValidateCode + call_time + balance.ToString().PadLeft(8, '0') + bonus.ToString().PadLeft(8, '0') + input_CardStatus).GetMD5();

                json_input = new StringBuilder();
                json_input.AppendLine("{");
                json_input.AppendLine(string.Format(" \"mid\" : \"{0}\"", mid));
                json_input.AppendLine(string.Format(" ,\"tid\" : \"{0}\"", tid));
                json_input.AppendLine(string.Format(" ,\"FromCardNo\" : \"{0}\"", output_CardNo));
                json_input.AppendLine(string.Format(" ,\"FromValidateCode\" : \"{0}\"", output_ValidateCode));
                json_input.AppendLine(string.Format(" ,\"ToCardNo\" : \"{0}\"", input_CardNo));
                json_input.AppendLine(string.Format(" ,\"ToValidateCode\" : \"{0}\"", input_ValidateCode));
                json_input.AppendLine(string.Format(" ,\"callTime\" : \"{0}\"", call_time));
                json_input.AppendLine(string.Format(" ,\"Amount\" : \"{0}\"", balance.ToString().PadLeft(8, '0')));
                json_input.AppendLine(string.Format(" ,\"Bonus\" : \"{0}\"", bonus.ToString().PadLeft(8, '0')));
                //json_input.AppendLine(string.Format(" ,\"CouponList\" : \"{0}\"", string.Empty));
                json_input.AppendLine(string.Format(" ,\"ToCardStatus\" : \"{0}\"", input_CardStatus));//.PadLeft(2, '0')
                json_input.AppendLine(string.Format(" ,\"mask\" : \"{0}\"", mask));
                json_input.AppendLine("}");

                string json_request = common.ECService.balance_transfer(json_input.ToString());
                xmlDoc = JsonConvert.DeserializeXmlNode(json_request);

                string xml_string = xmlDoc.OuterXml;

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}", xml_string));

                int code_id = Convert.ToInt32(xmlDoc.SelectSingleNode(".//codeID").InnerText.Trim());
                string code_description = xmlDoc.SelectSingleNode(".//codeDescription").InnerText.Trim();

                if (!code_id.Equals(0))
                {
                    common.InitLog("Transpose");
                    UserInfoLib UserInfo = common.GetSessionData();
                    common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 狀態：{1} , 所花費時間：{2}", (UserInfo.UID), "Transpose Error: code_id=" + code_id, common.DateDiff(dt_Initial, DateTime.Now)));
                }
                return Json(new { code_id = code_id, code_description = code_description });
            }
            catch (Exception ex)
            {
                common.InitLog("Transpose");
                UserInfoLib UserInfo = common.GetSessionData();
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 狀態：{1} , 所花費時間：{2}", (UserInfo.UID), "Transpose Error:" + ex.ToString(), common.DateDiff(dt_Initial, DateTime.Now)));
                return Json(new { code_id = "", code_description = "" });
            }
            finally
            {
                common.ECService.Dispose(); common.ECService = null;
            }
        }
        #endregion

        private void Initialization()
        {
            Dictionary<string, string> imgs = common.LoadSubBanner(CommonFun.SubBanner.MyCardUserProfileBanner, CommonFun.SubBanner.MyCardUserProfileMobileBanner);
            ViewBag.BannerMobileImg = imgs.Values.First();
            ViewBag.BannerImg = imgs.Keys.First();
        }

        private List<CardModel> GetCards()
        {
            List<IDataParameter> para = null;
            DataTable dt = null;
            List<CardModel> cards = new List<CardModel>();
            DateTime dt_Initial = DateTime.Now;

            try
            {
                string strSql = common.Select.PointCard(common.GetSessionData().USN, ref para);
                dt = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var CardID = dt.Rows[i]["CardNumber"].ToString();
                        var UID = dt.Rows[i]["UID"].ToString();
                        var SecurityCode = dt.Rows[i]["SecurityCode"].ToString().DecryptDES();
                        var CardImg = "/wwwroot/Image/Card/" + dt.Rows[i]["IconPath"].ToString();
                        cards.Add(GenerateCard(CardID, CardImg, UID, SecurityCode));
                    }
                }
            }
            catch (Exception ex)
            {
                common.InitLog("GetCards");
                UserInfoLib UserInfo = common.GetSessionData();
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 狀態：{1} , 所花費時間：{2}", (UserInfo.UID), "GetCards Error:" + ex.ToString(), common.DateDiff(dt_Initial, DateTime.Now)));
            }
            finally
            {
                common.CloseConn();
            }
            return cards;
        }

        #region API
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CardID"></param>
        /// <param name="AuthenticationCode"></param>
        /// <returns></returns>
        public JsonResult CheckCard(string CardID, string AuthenticationCode)
        {
            StringBuilder json_input = new StringBuilder();
            XmlDocument xmlDoc = null;
            DateTime dt_Initial = DateTime.Now;
            try
            {
                if (!String.IsNullOrEmpty(CardID) && !String.IsNullOrEmpty(AuthenticationCode))
                {
                    string call_time = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string mask = string.Format("Mc{0}Donalds", CardID + mid + tid + svc_lms + call_time + AuthenticationCode).GetMD5();

                    json_input.AppendLine("{");
                    json_input.AppendLine(string.Format(" \"cardNo\" : \"{0}\"", CardID));
                    json_input.AppendLine(string.Format(" ,\"mid\" : \"{0}\"", mid));
                    json_input.AppendLine(string.Format(" ,\"tid\" : \"{0}\"", tid));
                    json_input.AppendLine(string.Format(" ,\"expiryDate\" : \"{0}\"", svc_lms));
                    json_input.AppendLine(string.Format(" ,\"callTime\" : \"{0}\"", call_time));
                    json_input.AppendLine(string.Format(" ,\"validateCode\" : \"{0}\"", AuthenticationCode));
                    json_input.AppendLine(string.Format(" ,\"mask\" : \"{0}\"", mask));
                    json_input.AppendLine("}");

                    string json_request = common.ECService.validateCard(json_input.ToString());
                    xmlDoc = JsonConvert.DeserializeXmlNode(json_request);
                    string xml_string = xmlDoc.OuterXml;
                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}", xml_string));

                    int code_id = Convert.ToInt32(xmlDoc.SelectSingleNode(".//codeID").InnerText.Trim());
                    string code_description = xmlDoc.SelectSingleNode(".//codeDescription").InnerText.Trim();

                    return Json(new { code_id = code_id, code_description = code_description });
                }
                return Json(new { code_id = "6", code_description = "請輸入卡號與驗證碼" });
            }
            catch (Exception ex)
            {
                common.InitLog("GetCards");
                UserInfoLib UserInfo = common.GetSessionData();
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 狀態：{1} , 所花費時間：{2}", (UserInfo.UID), "GetCards Error:" + ex.ToString(), common.DateDiff(dt_Initial, DateTime.Now)));
                return Json(new { Message = ex.Message });
            }
            finally
            {
                common.ECService.Dispose(); common.ECService = null;
            }
        }

        /// <summary>
        /// 取得該用戶已記名的卡片
        /// </summary>
        /// <param name="CardNumber"></param>
        /// <param name="IconPath"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        private CardModel GenerateCard(string CardNumber, string IconPath, string UID, string SecurityCode)
        {
            StringBuilder json_input = new StringBuilder();
            XmlDocument xmlDoc = null;
            CardModel card = new CardModel();
            DateTime dt_Initial = DateTime.Now;

            try
            {
                string call_time = DateTime.Now.ToString("yyyyMMddhhmmss");
                string mask = string.Format("Mc{0}Donalds", CardNumber + mid + tid + svc_lms + call_time).GetMD5();

                json_input = new StringBuilder();
                json_input.AppendLine("{");
                json_input.AppendLine(string.Format(" \"cardNo\" : \"{0}\"", CardNumber));
                json_input.AppendLine(string.Format(" ,\"mid\" : \"{0}\"", mid));
                json_input.AppendLine(string.Format(" ,\"tid\" : \"{0}\"", tid));
                json_input.AppendLine(string.Format(" ,\"expiryDate\" : \"{0}\"", svc_lms));
                json_input.AppendLine(string.Format(" ,\"callTime\" : \"{0}\"", call_time));
                json_input.AppendLine(string.Format(" ,\"mask\" : \"{0}\"", mask));
                json_input.AppendLine("}");

                string json_request = common.ECService.queryCard(json_input.ToString());
                xmlDoc = JsonConvert.DeserializeXmlNode(json_request);

                string xml_string = xmlDoc.OuterXml;

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}", xml_string));

                int bonus = Convert.ToInt32(xmlDoc.SelectSingleNode(".//bonus").InnerText.Trim());
                int balance = Convert.ToInt32(xmlDoc.SelectSingleNode(".//balance").InnerText.Trim());
                string status = xmlDoc.SelectSingleNode(".//status").InnerText.Trim();

                card.CardID = CardNumber;
                card.ID = UID;
                card.SecurityCode = SecurityCode;
                card.Points = bonus.ToString();
                card.Balance = balance.ToString();
                card.CardImg = IconPath;
                card.Status = status;
            }
            catch (Exception ex)
            {
                common.InitLog("GetCards");
                UserInfoLib UserInfo = common.GetSessionData();
                common.WriteLog(Mode.LogMode.ERROR, string.Format("ID：{0} , 狀態：{1} , 所花費時間：{2}", (UserInfo.UID), "GenerateCard Error:" + ex.ToString(), common.DateDiff(dt_Initial, DateTime.Now)));
            }
            finally
            {
                common.ECService.Dispose(); common.ECService = null;
            }
            return card;
        }
        #endregion 
    }

    public class UserInfoLib
    {
        public string USN { get; set; }
        public string UID { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Int16 Sex { get; set; }
        public string Birthday { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Township { get; set; }
        public string Address { get; set; }
        public Int16 MaritalStatus { get; set; }
        public Int16 Children { get; set; }
        public Int16 ChildrenYear { get; set; }
        public string Mail { get; set; }
        public string FacebookUID { get; set; }
        public string GooglePlusUID { get; set; }
        public Int16 ReceiveNews { get; set; }
        public Int16 Status { get; set; }
        public DateTime LastModifyTime { get; set; }
    }
    public class CardModel
    {
        public string ID { get; set; }
        public string CardID { get; set; }
        public string SecurityCode { get; set; }
        public string CardImg { get; set; }
        public string Points { get; set; }
        public string Balance { get; set; }
        public string Status { get; set; }
    }
    public class CardExpenseRecordsModel
    {
        public string TransactionDate { get; set; }
        public string Stores { get; set; }
        public string ConsumptionCategory { get; set; }
        public string TradeName { get; set; }
        public string Points { get; set; }
        public string Balance { get; set; }
    }
}