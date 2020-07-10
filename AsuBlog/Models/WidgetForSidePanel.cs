using Store.DAL.Entities;

using System.Collections.Generic;


namespace AsuBlog.Models
{
    public class WidgetForSidePanel
    {
        public ICollection<Post> LastPosts { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}