using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DAL.Entities
{
    public class Tag
    {
        [Key]
        public int Id
        { get; set; }

        [Required(ErrorMessage = "Name: Поле, обязательное для заполнения")]
        [StringLength(500, ErrorMessage = "Name: Длина поля не должна превышать 500 символов")]
        public string Name
        { get; set; }

        [Required(ErrorMessage = "UrlSlug: Поле, обязательное для заполнения")]
        [StringLength(500, ErrorMessage = "UrlSlug: Длина поля не должна превышать 500 символов")]
        public string UrlSlug { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }

      
        public Tag()
        {
            Posts = new List<Post>();
        }
    }
}
