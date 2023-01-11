using Store.DAL.Entities;

namespace AsuBlog.Models
{
    public class RouteCategoryModel
    {
        public Category TopicCategory { get; set; }
        public Category SubtopicCategory { get; set; }
        public Category ThemeCategory { get; set; }
        public Category SubthemeCategory { get; set; }

    }
}