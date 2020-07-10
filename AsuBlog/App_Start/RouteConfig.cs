using System.Web.Mvc;
using System.Web.Routing;

namespace AsuBlog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              "Catalog1",
              "catalog/{topic}/{subtopic}/{theme}/{subtheme}",
             defaults: new { controller = "Blog", action = "Catalog" }
            );
            routes.MapRoute(
             "Catalog2",
             "catalog/{topic}/{subtopic}/{theme}",
            defaults: new { controller = "Blog", action = "Catalog" }
            );
            routes.MapRoute(
             "Catalog3",
             "catalog/{topic}/{subtopic}",
             defaults: new { controller = "Blog", action = "Catalog" }
            );
            routes.MapRoute(
             "Catalog4",
             "catalog/{topic}",
             defaults: new { controller = "Blog", action = "Catalog" }
            );

            routes.MapRoute(
              "Post",
              "post/{year}/{month}/{title}",
             defaults: new { controller = "Blog", action = "Post" }
            //new { year = @"\d{4}", month = @"\d{2}", day = @"\d{2}" //
            );
            routes.MapRoute(
               name: "Account",
               url: "account/{action}",
               defaults: new { controller = "Account", action = "login"}
            );

            routes.MapRoute(
               "Tag",
               "tag/{tag}",
               new { controller = "Blog", action = "Tag" }
             );

             routes.MapRoute(
             name: "Admin",
             url: "admin/{action}",
             defaults: new { controller = "Admin", action = "Manage" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{action}",
                defaults: new { controller = "Blog", action = "Index" }
            );
        }
    }
}
