using Store.DAL.EF;
using Store.DAL.Entities;
using Store.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace Store.DAL.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private ApplicationContext db;

        public CategoryRepository(ApplicationContext context)
        {
            this.db = context;
        }
        public IEnumerable<Category> GetAll()
        {
            return db.Categorys;
            //return db.Categorys.OrderBy<Category,string>(p => p.Name); //string это тип переменной Name
        }

        public Category Get(int id)
        {
            return db.Categorys.FirstOrDefault<Category>(p => p.Id == id);
        }
        public Category Get(string slug)
        {
            return db.Categorys.FirstOrDefault<Category>(p => p.UrlSlug.Equals(slug));
        }

        public int TotalItems()
        {
            return db.Categorys.Count<Category>();
        }

        public int Create(Category item)
        {
            Category category = db.Categorys.Add(item);
            db.SaveChanges();
            return category.Id;
        }

        public void Update(Category item)
        {
            db.Entry<Category>(item).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            Category category = db.Categorys.Find(id);
            if (category != null)
            {
                db.Categorys.Remove(category);
                db.SaveChanges();
            }
        }

    }
}
