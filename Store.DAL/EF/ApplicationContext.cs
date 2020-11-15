using Microsoft.AspNet.Identity.EntityFramework;
using Store.DAL.Entities;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Store.DAL.EF
{
    public class ApplicationContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(string connectionString)
               :base(connectionString)
        {
        }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Chat> Chats { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasMany(c => c.Posts)
                .WithMany(s => s.Categorys)
                .Map(t => t.MapLeftKey("CategoryId")
                .MapRightKey("PostId")
                .ToTable("CategoryPost"));

            modelBuilder.Entity<Tag>().HasMany(c => c.Posts)
              .WithMany(s => s.Tags)
              .Map(t => t.MapLeftKey("TagId")
              .MapRightKey("PostId")
              .ToTable("TagPost"));

            //Remember to call the base class for the migration to work
            base.OnModelCreating(modelBuilder);

        }

    }

    /// <summary>
    ///The factory must be added in order for the Migration to work
    /// </summary>
    public class MigrationsContextFactory : IDbContextFactory<ApplicationContext>
    {
        public ApplicationContext Create()
        {
            return new ApplicationContext("DefaultConnection");
        }
    }

    /// <summary>
    /// Database initializer
    /// </summary>
    public class ApplicationDbInitializer: DropCreateDatabaseAlways<ApplicationContext>
    {
        protected override void Seed(ApplicationContext context)
        {
            //context.Prod.Add(new Prod() { ProdId = 1, Name = "Product 1", Price = 10 });
            //context.Prod.Add(new Prod() { ProdId = 2, Name = "Product 2", Price = 20 });
          
            base.Seed(context);
        }
    }
}
