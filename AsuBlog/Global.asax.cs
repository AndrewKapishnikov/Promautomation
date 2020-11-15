using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;
using AsuBlog.Util;
using Store.DAL.Entities;
using System;
using System.Web;
using AsuBlog.Controllers;
using Elmah;

namespace AsuBlog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Database.SetInitializer(new ApplicationDbInitializer());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            NinjectModule databaseModule = new UnitOfWorkModule("DefaultConnection"); 
            var kernel = new StandardKernel(databaseModule);
            //System.InvalidOperationException: 'Validation type names in unobtrusive client validation rules must be unique. 
            //The following validation type was seen more than once: required' - an exception is raised if I remove the bottom line
            kernel.Unbind<ModelValidatorProvider>();
            NinjectDependencyResolver ninjDepend = new NinjectDependencyResolver(kernel);

            DependencyResolver.SetResolver(ninjDepend);

            //If delete binding, then error Status: 'Internal Server Error'.Error code: 500
            ModelBinders.Binders.Add(typeof(Post), new PostModelBinder(kernel));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;
            var ex = Server.GetLastError();
            var status = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;

            if (httpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                httpContext.ClearError();
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = status;
                httpContext.Response.TrySkipIisCustomErrors = true;
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.Write("{ success: false, message: \"Произошла ошибка на сервере\" }");
                httpContext.Response.End();
            }
            else
            {
                var currentController = " ";
                var currentAction = " ";
                var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                if (currentRouteData != null)
                {
                    if (currentRouteData.Values["controller"] != null &&
                        !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                    {
                        currentController = currentRouteData.Values["controller"].ToString();
                    }

                    if (currentRouteData.Values["action"] != null &&
                        !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                    {
                        currentAction = currentRouteData.Values["action"].ToString();
                    }
                }

                var controller = new ErrorsController();
                var routeData = new RouteData();

                httpContext.ClearError();
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = status;
                httpContext.Response.TrySkipIisCustomErrors = true;

                routeData.Values["controller"] = "Errors";
                routeData.Values["action"] = status == 404 ? "NotFound" : "Index";

                controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
                ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
        }

        protected void Application_BeginRequest()
        {
            if (!Context.Request.IsSecureConnection && !Context.Request.IsLocal)
                Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
        }

        protected void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            if (e.Exception.Message == "The remote host closed the connection. The error code is 0x800704CD.")
                e.Dismiss();
        }

      
        protected void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            if (e.Exception.Message == "The remote host closed the connection. The error code is 0x800704CD.")
            {
                e.Dismiss();
            }
        }


    }
}
