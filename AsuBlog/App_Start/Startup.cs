using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
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
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "388590986103-b00rphg3n0s7usvhp5rp9b9ih8dg6eg6.apps.googleusercontent.com",
                ClientSecret = "VDMcQgOE3CpH5Cuj83OC5vRA"
            });
            app.MapSignalR();
        }

        private IUnitOfWork CreateUserService()
        {
            return serviceCreator.CreateUnitOfWork("DefaultConnection");
        }
    }
}