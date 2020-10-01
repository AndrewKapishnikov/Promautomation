using Store.DAL.Entities;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.Owin;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Store.DAL.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
                : base(store)
        {
        }
        public IList<ApplicationUser> GetUsersForAdminPanel(int pageNo, int pageSize, string sortColumn, bool sortByAscending)
        {
            IList<ApplicationUser> users;
            
            switch (sortColumn)
            {
                case "UserName":
                    if (sortByAscending)
                    {
                        users = Users.OrderBy(p => p.UserName)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    }
                    else
                    {
                        users = Users.OrderByDescending(p => p.UserName)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    break;
                case "Email":
                    if (sortByAscending)
                    {
                        users = Users.OrderBy(p => p.Email)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    else
                    {
                        users = Users.OrderByDescending(p => p.Email)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                    }
                    break;
               
                default:
                    users = Users.OrderByDescending(p => p.Id)
                                    .Skip(pageNo * pageSize)
                                    .Take(pageSize)
                                    .ToList();
                    break;
            }

            return users;
        }
    }


    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }


    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
      
            var from = "kapishnikovaa@yandex.ru";
            var pass = "e**********";

            SmtpClient client = new SmtpClient("smtp.yandex.ru", 25);

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, pass);
            client.EnableSsl = true;

            var mail = new MailMessage(from, message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            return client.SendMailAsync(mail);
        }
    }
}
