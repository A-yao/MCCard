using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace MCCard.Controllers
{
    public class CardIntroductionController : BaseController
    {
        public ActionResult Rebate()
        {
            GetRebate();
            return View("Rebate");
        }

        public ActionResult CardIntroductionAPP()
        {
            GetRebate();
            return View("RebateBasic");
        }

        public void GetRebate()
        {
            try
            {
                Dictionary<string, string> imgs = common.LoadSubBanner(Common.CommonFun.SubBanner.CardIntroductionBanner, Common.CommonFun.SubBanner.CardIntroductionMobileBanner);
                ViewBag.BannerMobileImg = imgs.Values.First();
                ViewBag.BannerImg = imgs.Keys.First();
                //common.Select.LoadExhibitorsNews(ExhibitorsNewsMode.Index);
                ViewBag.Description = common.GetSystemSetting("CardIntroductionTitle");

                List<string> goods = new List<string>();
                for (int i = 0; i < 8; i++)
                {
                    goods.Add(iconPath + common.GetSystemSetting("CardIntroductionIcon" + (i + 1)));
                }
                ViewData.Model = goods;
            }
            catch (System.Exception ex)
            {
                common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
            }
            finally
            {
                common.CloseConn();
            }
        }

        public ActionResult CardList()
        {
            try
            {
                Dictionary<string, string> imgs = common.LoadSubBanner(Common.CommonFun.SubBanner.CardListBanner, Common.CommonFun.SubBanner.CardListMobileBanner);
                ViewBag.BannerMobileImg = imgs.Values.First();
                ViewBag.BannerImg = imgs.Keys.First();

                Dictionary<string, string> SubImgs = common.LoadSubBanner(Common.CommonFun.SubBanner.CardListSubBanner, Common.CommonFun.SubBanner.CardListSubMobileBanner);
                ViewBag.SubBannerMobileImg = SubImgs.Values.First();
                ViewBag.SubBannerImg = SubImgs.Keys.First();
                //最新點點卡 開放時間
                ViewBag.CardListOpenYear = ConfigurationManager.AppSettings["CardListOpenYear"].ToString();
                ViewBag.CardListOpenMonth = ConfigurationManager.AppSettings["CardListOpenMonth"].ToString();
                ViewBag.CardListOpenDay = ConfigurationManager.AppSettings["CardListOpenDay"].ToString();

                //CardStyle
                List<CardStyleModel> style = new List<CardStyleModel>();
                string sql = common.Select.CardStyle();
                DataTable cardData = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(sql);
                for (int d = 0; d < cardData.Rows.Count; d++)
                {
                    style.Add(new CardStyleModel
                    {
                        UID = cardData.Rows[d]["UID"].ToString(),
                        Subject = cardData.Rows[d]["Subject"].ToString(),
                        IconPath = cardPath + cardData.Rows[d]["IconPath"].ToString(),
                        Pricing = cardData.Rows[d]["Pricing"].ToString()
                    });
                }
                var s = from data in style
                        group data by data.IconPath into g
                        select g.First();

                ViewData.Model = s.ToList<CardStyleModel>();
            }
            catch (System.Exception ex)
            {
                common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
            }
            finally
            {
                common.CloseConn();
            }
            return PartialView("CardList");
        }


        public ActionResult CardDetail(string ID)
        {
            try
            {
                Dictionary<string, string> imgs = common.LoadSubBanner(Common.CommonFun.SubBanner.CardListBanner, Common.CommonFun.SubBanner.CardListMobileBanner);
                ViewBag.BannerMobileImg = imgs.Values.First();
                ViewBag.BannerImg = imgs.Keys.First();

                List<IDataParameter> para = null;
                string strSQL = common.Select.CardStyle(ID, ref para);
                DataTable cardData = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSQL, para);

                ViewBag.CardImg = cardPath + cardData.Rows[0]["IconPath"].ToString();
                ViewBag.CardTitle = cardData.Rows[0]["Subject"].ToString();
                ViewBag.CardDescription = cardData.Rows[0]["Body"].ToString();
                ViewBag.CardPrice = cardData.Rows[0]["Pricing"].ToString();
            }
            catch (System.Exception ex)
            {
                common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
            }
            finally
            {
                common.CloseConn();
            }
            return PartialView("CardDetail");
        }
    }

    public class CardStyleModel
    {
        public string UID { get; set; }
        public string Subject { get; set; }
        public string IconPath { get; set; }
        public string Pricing { get; set; }
    }
}