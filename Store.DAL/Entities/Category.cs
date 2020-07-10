
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.DAL.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }

        [Required(ErrorMessage = "Name: Поле, обязательное для заполнения")]
        [StringLength(500, ErrorMessage = "Name: Длина не должна превышать 500 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "UrlSlug: Поле, обязательное для заполнения")]
        [StringLength(500, ErrorMessage = "UrlSlug: Длина не должна превышать 500 символов")]
        public string UrlSlug { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "BoolArticle: Поле, обязательное для заполнения")]
        public bool BoolArticle { get; set; }

        [Required(ErrorMessage = "Level: Поле, обязательное для заполнения")]
        public int Level { get; set; }

        public string FullUrl { get; set; }

        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }

        public Category()
        {
            Posts = new List<Post>();
        }

    }
    
}
