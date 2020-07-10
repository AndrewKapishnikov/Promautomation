using System.Collections.Generic;


namespace Store.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        int TotalItems();
        T Get(int id);
        T Get(string slug);
        int Create(T item);
        void Update(T item);
        void Delete(int id);
   
    }
}
