using MCCard.Common;
using System.Web.Mvc;

namespace MCCard.Controllers
{
    public class BaseController : Controller
    {
        public CommonFun common = new CommonFun();
        public string cardPath = "/wwwroot/Image/Card/";
        public string iconPath = "/wwwroot/Image/ICON/";
        public string bannerPath = "/wwwroot/Image/Banner/";
        public string exhibitorsNewsPath = "/wwwroot/Image/ExhibitorsNews/";

        public BaseController()
        {
            //隱私權保護聲明
            ViewBag.copyright = common.GetSystemSetting("Copyright");
            //網站名稱
            ViewBag.PageTitle = common.GetSystemSetting("PageTitle");
            //連結
            ViewBag.FBLink = common.GetSystemSetting("FBLink");
            ViewBag.YouTubeLink = common.GetSystemSetting("YouTubeLink");
            ViewBag.IGLink = common.GetSystemSetting("IGLink");
        }
    }
}