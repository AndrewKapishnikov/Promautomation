using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Store.DAL.Entities
{
    public class Post
    {
        [Key]
        public int Id  { get; set; }

        /// <summary>
        /// The heading of the post.
        /// </summary>
        [Required(ErrorMessage = "Title: Поле, обязательное для заполнения")]
        [StringLength(500, ErrorMessage = "Title: Длина поля не должна превышать 500 символов")]
        public string Title { get; set; }

        /// <summary>
        /// A brief paragraph about the post.
        /// </summary>
        [Required(ErrorMessage = "ShortDescription: Поле, обязательное для заполнения")]
        public string ShortDescription { get; set; }

        /// <summary>
        /// The complete post content.
        /// </summary>
        [Required(ErrorMessage = "Description: Поле, обязательное для заполнения")]
        public string Description { get; set; }

        /// <summary>
        /// The information about the post that has to be displayed in the &lt;meta&gt; tag (SEO).
        /// </summary>
        /// <remarks>
        /// Not sure Google still uses this for ranking but other search providers might be.
        /// </remarks>
        [Required(ErrorMessage = "Meta: Поле, обязательное для заполнения")]
        [StringLength(1000, ErrorMessage = "Meta: Длина поля не должна превышать 1000 символов")]
        public string Meta { get; set; }

        /// <summary>
        /// The url slug that is used to define the post address.
        /// </summary>
        [Required(ErrorMessage = "UrlSlug: Поле, обязательное для заполнения")]
        [StringLength(1000, ErrorMessage = "UrlSlug: Длина поля не должна превышать 1000 символов")]
        public string UrlSlug
        { get; set; }

        /// <summary>
        /// Flag to represent whether the article is published or not.
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// The post published date.
        /// </summary>
        [Required(ErrorMessage = "PostedOn: Поле, обязательное для заполнения")]
        public DateTime PostedOn
        { get; set; }

        /// <summary>
        /// The post's last modified date.
        /// </summary>
        public DateTime? Modified
        { get; set; }

        public string ImagePath { get; set; }

        public int NumberVisits { get; set; }

        [Required(ErrorMessage = "Topic: Поле, обязательное для заполнения")]
        public string Topic { get; set; }

        public string Subtopic { get; set; }
        public string Theme { get; set; }
        public string Subtheme { get; set; }

        /// <summary>
        /// Collection of tags labelled over the post.
        /// </summary>

        //[JsonIgnore]
        public virtual ICollection<Tag> Tags   { get; set; }

        
        //[JsonIgnore]
        public virtual ICollection<Category> Categorys { get; set; }

        public Post()
        {
            Tags = new List<Tag>();
            Categorys = new List<Category>();

        }
    }
}
