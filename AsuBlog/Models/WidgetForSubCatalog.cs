using Store.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsuBlog.Models
{
    public class WidgetForSubCatalog
    {
       public List<Category> Categories { get; set; } 
       public string TopicUrlSlug { get; set; }
       public string SubTopicUrlSlug { get; set; }
       public string ThemeUrlSlug { get; set; }

    }
}