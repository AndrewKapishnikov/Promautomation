using Store.DAL.EF;
using Store.DAL.Entities;
using Store.DAL.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;
using Store.DAL.Identity;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace Store.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationContext db;

        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;
        private CategoryRepository categoryRepository;
        private TagRepository tagRepository;
        private PostRepository postRepository;
        private ChatRepository chatRepository;


        public IRepository<Category> Categorys
        {
            get
            {
                if (categoryRepository == null)
                    categoryRepository = new CategoryRepository(db);
                return categoryRepository;
            }
        }

        public IRepository<Tag> Tags
        {
            get
            {
                if (tagRepository == null)
                    tagRepository = new TagRepository(db);
                return tagRepository;
            }
        }
        public IRepository<Chat> Chats
        {
            get
            {
                if (chatRepository == null)
                    chatRepository = new ChatRepository(db);
                return chatRepository;
            }
        }

        public PostRepository Posts
        {
            get
            {
                if (postRepository == null)
                    postRepository = new PostRepository(db);
                return postRepository;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString"></param>
        public UnitOfWork(string connectionString)
        {
            db = new ApplicationContext(connectionString);
            
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true,
                
            };

            // Configure validation logic for passwords
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            var provider = new DpapiDataProtectionProvider("SampleAppName");
            userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                                                provider.Create("SampleTokenName"));
            userManager.EmailService = new EmailService();

            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
     
        }

        public ApplicationUserManager UserManager
        {
            get { return userManager; }
        }
        public ApplicationRoleManager RoleManager
        {
            get { return roleManager; }
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }
        public void Save()
        {
            db.SaveChanges();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    userManager.Dispose();
                    roleManager.Dispose();
                }
                this.disposed = true;
            }
        }
    }
}
