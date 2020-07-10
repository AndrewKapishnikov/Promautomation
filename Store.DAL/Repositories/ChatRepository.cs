using Store.DAL.EF;
using Store.DAL.Entities;
using Store.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DAL.Repositories
{
    class ChatRepository: IRepository<Chat>
    {
        private ApplicationContext db;

        public ChatRepository(ApplicationContext context)
        {
            this.db = context;
        }
        public IEnumerable<Chat> GetAll()
        {
            return db.Chats.Include(p => p.User).OrderBy(p => p.Id); //возможно здесь стоит дописать .ToList()
        }

        public Chat Get(int id)
        {
            return db.Chats.FirstOrDefault<Chat>(p => p.Id == id);
        }

        public Chat Get(string message)
        {
            return db.Chats.FirstOrDefault<Chat>(p => p.Message.Equals(message));
        }

        public int TotalItems()
        {
            return db.Chats.Count<Chat>();
        }

        public int Create(Chat item)
        {
            Chat chat = db.Chats.Add(item);
            db.SaveChanges();
            return chat.Id;
        }

        public void Update(Chat item)
        {
            db.Entry<Chat>(item).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            Chat chat = db.Chats.Find(id);
            if (chat != null)
            {
                db.Chats.Remove(chat);
                db.SaveChanges();
            }
        }
    }
}
