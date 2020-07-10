using Store.DAL.EF;
using Store.DAL.Entities;
using Store.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace Store.DAL.Repositories
{
    public class TagRepository : IRepository<Tag>
    {
        private ApplicationContext db;

        public TagRepository(ApplicationContext context)
        {
            this.db = context;
        }
        public IEnumerable<Tag> GetAll()
        {
            return db.Tags.OrderBy<Tag, string>(p => p.Name); //возможно здесь стоит дописать .ToList()
        }

        public Tag Get(int id)
        {
            return db.Tags.FirstOrDefault<Tag>(p => p.Id == id);
        }
        public Tag Get(string slug)
        {
            return db.Tags.FirstOrDefault<Tag>(p => p.UrlSlug.Equals(slug));
        }

        public int TotalItems()
        {
            return db.Tags.Count<Tag>();
        }

        public int Create(Tag item)
        {
            Tag tag = db.Tags.Add(item);
            db.SaveChanges();
            return tag.Id;
        }

        public void Update(Tag item)
        {
            db.Entry<Tag>(item).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            Tag tag = db.Tags.Find(id);
            if (tag != null)
            {
                db.Tags.Remove(tag);
                db.SaveChanges();
            }
        }

    }
}
