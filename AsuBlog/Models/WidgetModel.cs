using Store.DAL.Entities;
using Store.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;


namespace AsuBlog.Models
{
    public class WidgetModel
    {
        public WidgetModel(IUnitOfWork blogRepository)
        {
            Categories = blogRepository.Categorys.GetAll().ToList();
            Tags = blogRepository.Tags.GetAll().ToList();
            Posts = blogRepository.Posts.GetAll().ToList();
        }

        public IList<Category> Categories
        { get; private set; }

        public IList<Tag> Tags
        { get; private set; }

        public IList<Post> Posts
        { get; private set; }
    }

}