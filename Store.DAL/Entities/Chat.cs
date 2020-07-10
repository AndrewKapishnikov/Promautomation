using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.DAL.Entities
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Message: Поле, обязательное для заполнения")]
        public string Message { get; set; }

        [Required(ErrorMessage = "DateMessage: Поле, обязательное для заполнения")]
        public DateTime DateMessage
        { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
