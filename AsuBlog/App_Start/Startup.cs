using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.AspNet.Identity;
using Store.DAL.Interfaces;
using AsuBlog.Interfaces;
using AsuBlog.Services;
using Store.DAL.Identity;

[assembly: OwinStartup(typeof(AsuBlog.App_Start.Startup))]

namespace AsuBlog.App_Start
{
    public class Startup
    {
        IUnitOfWorkCreator serviceCreator = new InitOfWorkCreator();
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<IUnitOfWork>(CreateUserService);
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.MapSignalR();
        }

        private IUnitOfWork CreateUserService()
        {
            return serviceCreator.CreateUnitOfWork("DefaultConnection");
        }
    }
}