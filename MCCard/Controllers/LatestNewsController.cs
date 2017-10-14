using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Data;

namespace MCCard.Controllers
{
    public class LatestNewsController : BaseController
    {
        public ActionResult Index()
        {
            Dictionary<string, string> imgs = common.LoadSubBanner(MCCard.Common.CommonFun.SubBanner.LatestNewsBanner, MCCard.Common.CommonFun.SubBanner.LatestNewsMobileBanner);
            ViewBag.MobileImg = imgs.Values.First();
            ViewBag.Img = imgs.Keys.First();   
            string strSql = string.Empty;
            DataTable dt_latest_news_type11 = null , dt_latest_news_type12 = null;

            var model = new List<LatestNewsModel>();
            try
            {
                strSql = common.Select.LatestNewsBannerPC();
                dt_latest_news_type11 = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql);
                strSql = common.Select.LatestNewsBannerMB();
                dt_latest_news_type12 = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql);
                                
                //For PC
                //for (int i = 0; i < dt_latest_news_type11.Rows.Count; i++)
                //{

                //    model.Add(new LatestNewsModel
                //    {
                //        Subject = dt_latest_news_type11.Rows[i]["Subject"].ToString(),
                //        Body = dt_latest_news_type11.Rows[i]["Body"].ToString(),
                //        UrlLink = dt_latest_news_type11.Rows[i]["UrlLink"].ToString(),
                //        BannerPath = dt_latest_news_type11.Rows[i]["BannerPath"].ToString(),
                //        Type = dt_latest_news_type11.Rows[i]["Type"].ToString()
                //    });
                //}
                
                //For Moible device
                //for (int i = 0; i < dt_latest_news_type12.Rows.Count; i++)
                //{

                //    model.Add(new LatestNewsModel
                //    {
                //        Subject = dt_latest_news_type12.Rows[i]["Subject"].ToString(),
                //        Body = dt_latest_news_type12.Rows[i]["Body"].ToString(),
                //        UrlLink = dt_latest_news_type12.Rows[i]["UrlLink"].ToString(),
                //        BannerPath = dt_latest_news_type12.Rows[i]["BannerPath"].ToString(),
                //        Type = dt_latest_news_type12.Rows[i]["Type"].ToString()
                //    });
                //}
                //ViewData.Model = model;

                ViewBag.NewsImg1 = bannerPath + dt_latest_news_type11.Rows[0]["BannerPath"].ToString();
                ViewBag.NewsImg2 = bannerPath + dt_latest_news_type11.Rows[1]["BannerPath"].ToString();
                ViewBag.NewsImg3 = bannerPath + dt_latest_news_type11.Rows[2]["BannerPath"].ToString();

                ViewBag.NewsMobileImg1 = bannerPath + dt_latest_news_type12.Rows[0]["BannerPath"].ToString(); ;
                ViewBag.NewsMobileImg2 = bannerPath + dt_latest_news_type12.Rows[1]["BannerPath"].ToString(); ;
                ViewBag.NewsMobileImg3 = bannerPath + dt_latest_news_type12.Rows[2]["BannerPath"].ToString(); ;

                ViewBag.NewsTitle1 = dt_latest_news_type11.Rows[0]["Subject"].ToString();
                ViewBag.NewsTitle2 = dt_latest_news_type11.Rows[1]["Subject"].ToString();
                ViewBag.NewsTitle3 = dt_latest_news_type11.Rows[2]["Subject"].ToString();

                ViewBag.NewsDescription1 = dt_latest_news_type11.Rows[0]["Body"].ToString();
                ViewBag.NewsDescription2 = dt_latest_news_type11.Rows[1]["Body"].ToString();
                ViewBag.NewsDescription3 = dt_latest_news_type11.Rows[2]["Body"].ToString();

                ViewBag.Newshref1 = dt_latest_news_type11.Rows[0]["UrlLink"].ToString();
                ViewBag.Newshref2 = dt_latest_news_type11.Rows[1]["UrlLink"].ToString();
                ViewBag.Newshref3 = dt_latest_news_type11.Rows[2]["UrlLink"].ToString();

            }
            catch (System.Exception ex)
            {
                common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
            }
            finally
            {
                common.CloseConn();
            }

            return View();
        }
                
    }

    public class LatestNewsModel
    {

        public string Subject { get; set; }
        public string Body { get; set; }
        public string UrlLink { get; set; }
        public string BannerPath { get; set; }
        public string Type { get; set; }
    }

}