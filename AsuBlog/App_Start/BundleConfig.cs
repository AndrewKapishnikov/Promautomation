using System.Web.Optimization;

namespace AsuBlog
{
    public class BundleConfig
    {
       
        public static void RegisterBundles(BundleCollection bundles)
        {   
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/tagcanvas").Include("~/Scripts/jquery.tagcanvas.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/general").Include("~/Scripts/general.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalR").Include("~/Scripts/jquery.signalR-2.4.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/chat").Include("~/Scripts/chat.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/pagedlist.css",
                      "~/Content/site.css",
                      "~/Content/catalogstyle.css"));

            bundles.Add(new StyleBundle("~/MainPage/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/mainstyle.css"));
           //--------------------AdminBundles--------------------------------------------
           
           //---------------------css-------------------------
           
            var jqueryUI = new StyleBundle("~/Content/themes/simple/jqueryui")
                                .Include("~/Content/themes/simple/jqueryuicustom/css/sunny/jquery-ui.css");
            bundles.Add(jqueryUI);


            var manageCssBundle = new StyleBundle("~/Scripts/jqgrid/css/bundle").Include("~/Scripts/jqgrid/css/ui.jqgrid.css");
            bundles.Add(manageCssBundle);

            var jqueryUICssBundle = new StyleBundle("~/Content/themes/simple/jqueryuicustom/css/sunny/bundle")
                                   .Include("~/Content/themes/simple/jqueryuicustom/css/sunny/jquery-ui.css");
            bundles.Add(jqueryUICssBundle);

            // login page bundles
            var loginCssBundle = new StyleBundle("~/Content/themes/simple/admin").Include("~/Content/themes/simple/admin.css");
            bundles.Add(loginCssBundle);

            //---------------------css------------------------

            var jqueryB = new ScriptBundle("~/jqueryB", "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.2.min.js")
                             .Include("~/Scripts/jquery-1.8.2.js");
            bundles.Add(jqueryB);

            // manage page bundles
            var jqueryUIBundle = new ScriptBundle("~/jqueryui", "http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.1/jquery-ui.min.js").Include("~/Scripts/jquery-ui.js");
            bundles.Add(jqueryUIBundle);
       
            var tinyMceBundle = new ScriptBundle("~/Scripts/tiny_mce/js").Include("~/Scripts/tiny_mce/tiny_mce.js");
            bundles.Add(tinyMceBundle);

            var manageJsBundle = new ScriptBundle("~/manage/js").Include("~/Scripts/jqgrid/js/jquery.jqGrid.js").Include("~/Scripts/jqgrid/js/i18n/grid.locale-ru.js").Include("~/Scripts/admin.js");
            bundles.Add(manageJsBundle);


        }
    }
}
