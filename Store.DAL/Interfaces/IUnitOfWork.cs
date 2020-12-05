using Store.DAL.Entities;
using Store.DAL.Identity;
using Store.DAL.Repositories;
using System;
using System.Threading.Tasks;

namespace Store.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; }
        ApplicationRoleManager RoleManager { get; }
        IRepository<Category> Categorys { get; }
        IRepository<Tag> Tags { get; }
        IRepository<Post> Posts { get; }
        IRepository<Chat> Chats { get; }
        Task SaveAsync();
        void Save();
    }
}
