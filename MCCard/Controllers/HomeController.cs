using System.Collections.Generic;
using System.Web.Mvc;
using MCCard.Common;
using System.Data;
using MultiOAuth.Core;

namespace MCCard.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        { 
            List<NewsModel> news = new List<NewsModel>();
            List<string> desktopImg = new List<string>();
            List<string> mobileImg = new List<string>();
            try
            {
                string sql = common.Select.HomeBanner();
                DataTable homeData = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(sql);
                for (int d = 0; d < homeData.Rows.Count; d++)
                {
                    if (homeData.Rows[d]["Type"].ToString() == "4")
                    {
                        ViewBag.BannerMobileImg = bannerPath + homeData.Rows[d]["BannerPath"].ToString();
                    }
                    else if (homeData.Rows[d]["Type"].ToString() == "0")
                    {
                        ViewBag.BannerImg = bannerPath + homeData.Rows[d]["BannerPath"].ToString();
                        ViewBag.BannerUrl = homeData.Rows[d]["UrlLink"].ToString();
                    }
                }
                List<IDataParameter> p = new List<IDataParameter>();
                sql = common.Select.HomeSubBanner("1", ref p);
                DataTable homeSubData = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(sql, p);
                for (int d = 0; d < homeSubData.Rows.Count; d++)
                {
                    ViewBag.SubBannerImg = bannerPath + homeSubData.Rows[d]["BannerPath"].ToString();
                    ViewBag.Title = homeSubData.Rows[d]["Subject"].ToString();
                    ViewBag.Description = homeSubData.Rows[d]["Body"].ToString();
                }
                p.Clear();
                sql = common.Select.HomeSubBanner("5", ref p);
                DataTable homeSubMobileData = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(sql, p);
                for (int d = 0; d < homeSubMobileData.Rows.Count; d++)
                {
                    if (homeSubMobileData.Rows[d]["Seq"].ToString() == "0")
                    {
                        ViewBag.SubBannerMobileImg1 = bannerPath + homeSubMobileData.Rows[d]["BannerPath"].ToString();
                    }
                    else if (homeSubMobileData.Rows[d]["Seq"].ToString() == "1")
                    {
                        ViewBag.SubBannerMobileImg2 = bannerPath + homeSubMobileData.Rows[d]["BannerPath"].ToString();
                        ViewBag.TitleMobile = homeSubMobileData.Rows[d]["Subject"].ToString();
                        ViewBag.DescriptionMobile = homeSubMobileData.Rows[d]["Body"].ToString();
                    }
                }
                p.Clear();
                sql = common.Select.News();
                DataTable homeNewsData = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(sql);
                for (int d = 0; d < homeNewsData.Rows.Count; d++)
                {
                    news.Add(new NewsModel
                    {
                        Title = homeNewsData.Rows[d]["Subject"].ToString(),
                        Url = homeNewsData.Rows[d]["UrlLink"].ToString()
                    });
                }
                sql = common.Select.ExhibitorsNews("0", ref p);
                DataTable desktopImgData = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(sql, p);
                for (int d = 0; d < desktopImgData.Rows.Count; d++)
                {
                    desktopImg.Add(exhibitorsNewsPath + desktopImgData.Rows[d]["IconPath"].ToString());
                }
                p.Clear();
                sql = common.Select.ExhibitorsNews("1", ref p);
                DataTable mobileImgData = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(sql, p);
                for (int d = 0; d < mobileImgData.Rows.Count; d++)
                {
                    mobileImg.Add(exhibitorsNewsPath + mobileImgData.Rows[d]["IconPath"].ToString());
                }
                HomeModel data = new HomeModel();
                data.News = news;
                data.DesktopImg = desktopImg;
                data.MobileImg = mobileImg;
                ViewData.Model = data;
            }
            catch (System.Exception ex)
            {
                common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
            }
            finally
            {
                common.CloseConn();
            }
            //ViewBag.BannerMobileImg = "/wwwroot/Image/Banner/105AC5F060314222446.png";
            //ViewBag.BannerImg = "/wwwroot/Image/Banner/105AC5F060314443617.png";
            //ViewBag.BannerUrl = "/CardIntroduction/CardList";
            //ViewBag.SubBannerMobileImg1 = "/wwwroot/Image/Banner/105AC5F060314340683.png";
            //ViewBag.SubBannerMobileImg2 = "/wwwroot/Image/Banner/105AC5F060314897380.png";
            //ViewBag.SubBannerImg = "/wwwroot/Image/Banner/105AC5F060313896208.png";
            //List<NewsModel> news = new List<NewsModel>();
            //news.Add(new NewsModel
            //{
            //    Title = "新聞四",
            //    Url = "http://www.mcdonalds.com.tw/tw/ch/index.html"
            //});
            //news.Add(new NewsModel
            //{
            //    Title = "新聞一",
            //    Url = "http://www.mcdonalds.com.tw/tw/ch/index.html"
            //});
            //news.Add(new NewsModel
            //{
            //    Title = "新聞二",
            //    Url = "http://www.mcdonalds.com.tw/tw/ch/index.html"
            //});
            //List<string> desktopImg = new List<string>();
            //List<string> mobileImg = new List<string>();
            //desktopImg.Add("/wwwroot/Image/ExhibitorsNews/EXN591C2020315000813.png");
            //desktopImg.Add("/wwwroot/Image/ExhibitorsNews/EXN591C2020315000183.png");
            //desktopImg.Add("/wwwroot/Image/ExhibitorsNews/EXN591C2020315000153.png");
            //desktopImg.Add("/wwwroot/Image/ExhibitorsNews/EXN591C2020315001012.png");
            //mobileImg.Add("/wwwroot/Image/ExhibitorsNews/EXN591C2020315000920.png");
            //mobileImg.Add("/wwwroot/Image/ExhibitorsNews/EXN591C2020315000296.png");
            //mobileImg.Add("/wwwroot/Image/ExhibitorsNews/EXN591C2020315000743.png");
            //mobileImg.Add("/wwwroot/Image/ExhibitorsNews/EXN591C2020315000204.png");
            //HomeModel data = new HomeModel();
            //data.News = news;
            //data.DesktopImg = desktopImg;
            //data.MobileImg = mobileImg;
            //ViewData.Model = data;
            //ViewBag.Title = "麥當勞點點APP";
            //ViewBag.TitleMobile = "麥當勞點點APP";
            //ViewBag.Description = "只要設天氣報報，必送麥當勞優惠卷或歡樂貼，集滿6枚歡樂貼，即可兌換優惠，方便查詢點點交易明細、餘額和點數！";
            //ViewBag.DescriptionMobile = "只要設天氣報報，必送麥當勞優惠卷或歡樂貼，集滿6枚歡樂貼，即可兌換優惠，方便查詢點點交易明細、餘額和點數！";
            return View();
        }
    }
    public class NewsModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
    public class HomeModel
    {
        public List<NewsModel> News { get; set; }
        public List<string> DesktopImg { get; set; }
        public List<string> MobileImg { get; set; }
    }
}