using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace MCCard.Controllers
{
    public class MCCardAPPController : BaseController
    {
        // GET: MCCardAPP
        public ActionResult Index()
        {
            Dictionary<string, string> imgs = common.LoadSubBanner(MCCard.Common.CommonFun.SubBanner.MCCardAppBanner, MCCard.Common.CommonFun.SubBanner.MCCardAppMobileBanner);
            ViewBag.BannerMobileImg = imgs.Values.First();
            ViewBag.BannerImg = imgs.Keys.First();//MCCardAPPNewsSubBanner1
            Dictionary<string, string> newsImgs = common.LoadSubBanner(MCCard.Common.CommonFun.SubBanner.MCCardAPPNewsSubBanner1, MCCard.Common.CommonFun.SubBanner.MCCardAPPNewsSubBanner);
            Dictionary<string, string> newsMobileImgs = common.LoadSubBanner(MCCard.Common.CommonFun.SubBanner.MCCardAPPNewsSubBanner3, MCCard.Common.CommonFun.SubBanner.MCCardAPPNewsSubBanner2);
            ViewBag.NewsBanner = newsImgs.Values.First();
            ViewBag.NewsBannerMobile1 = newsImgs.Keys.First();
            ViewBag.NewsBannerMobile2 = newsMobileImgs.Values.First();
            ViewBag.NewsBannerMobile3 = newsMobileImgs.Keys.First();
            return View();
        }
    }
}