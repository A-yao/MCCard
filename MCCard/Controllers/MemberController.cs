using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace MCCard.Controllers
{
    public class MemberController : BaseController
    {
        public ActionResult UserProfile()
        {
            Dictionary<string, string> imgs = common.LoadSubBanner(Common.CommonFun.SubBanner.MyCardUserProfileBanner, Common.CommonFun.SubBanner.MyCardUserProfileMobileBanner);
            ViewBag.MobileImg = imgs.Values.First();
            ViewBag.Img = imgs.Keys.First();

            bool result = false;
            string forgotpassword = Request.QueryString["forgotpassword"];
            if (!String.IsNullOrEmpty(forgotpassword))
            {
                try
                {                    
                    forgotpassword = forgotpassword.DecryptDES();
                    string[] infos = forgotpassword.Split('|');
                    string uid = infos[0].DecryptDES();

                    List<IDataParameter> para = null;
                    string strSql = common.Select.UserByUID(uid, string.Empty, ref para);

                    DataTable dt = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql, para);

                    common.CloseConn();

                    if (dt != null && dt.Rows.Count.Equals(1))
                    {
                        common.SetUserInfo(dt.Rows[0]["USN"].ToString().Trim());
                    }

                    
                }
                catch (System.Exception ex)
                {
                    common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
                }
            }
            return View();
        }

        #region 修改個人資料
        public JsonResult UpdateUserProfile(UserInfoLib UserInfo)
        {
            List<IDataParameter> para = null;
            Hashtable ht = new Hashtable();
            try
            {
                string USN = common.GetSessionData().USN;
                ht.Add("USN", USN);
                ht.Add("Password", UserInfo.Password);
                ht.Add("LastName", UserInfo.LastName);
                ht.Add("FirstName", UserInfo.FirstName);
                ht.Add("Sex", UserInfo.Sex);
                ht.Add("Zipcode", UserInfo.Zipcode);
                ht.Add("City", UserInfo.City);
                ht.Add("Township", UserInfo.Township);
                ht.Add("Address", UserInfo.Address ?? "");
                ht.Add("MaritalStatus", UserInfo.MaritalStatus);
                ht.Add("Children", UserInfo.Children);
                ht.Add("ChildrenYear", UserInfo.ChildrenYear);
                ht.Add("Mail", UserInfo.Mail);
                ht.Add("ReceiveNews", UserInfo.ReceiveNews);

                string strSql = common.Update.User(ht, ref para);
                if (common.DBConn.GeneralSqlCmd.ExecuteNonQuery(strSql, para).Equals(1))
                {
                    //SetSession
                    common.SetUserInfo(USN);
                }
                return Json(new { Result = common.DBConn.GeneralSqlCmd.ExecuteNonQuery(strSql, para).Equals(1) });
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
        #endregion
    }
}