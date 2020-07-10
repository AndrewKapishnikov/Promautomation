using Store.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsuBlog.Models
{
    public class WidgetForPost
    {
        public Post Post { get; set; }
        public Category TopicCategory { get; set; }
        public Category SubtopicCategory { get; set; }
        public Category ThemeCategory { get; set; }
        public Category SubthemeCategory { get; set; }

    }
}