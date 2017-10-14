using System.Web.Optimization;

namespace MCCard
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/JS").Include(
                     "~/wwwroot/lib/jquery/jquery-3.1.1.js",
                     "~/wwwroot/lib/boostrap/js/bootstrap.min.js",
                     "~/wwwroot/Scripts/utility-jquery.js",
                     "~/wwwroot/Scripts/picturefill.js",
                     "~/wwwroot/Scripts/Site.js"
                     ));

            bundles.Add(new StyleBundle("~/bundles/CSS").Include(
                 "~/wwwroot/lib/boostrap/css/bootstrap.min.css",
                 "~/wwwroot/lib/boostrap/css/bootstrap-theme.min.css",
                 "~/wwwroot/css/main.css"));
        }
    }
}
