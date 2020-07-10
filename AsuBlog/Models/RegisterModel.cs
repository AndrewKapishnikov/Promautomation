using System.ComponentModel.DataAnnotations;

namespace AsuBlog.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [Display(Name = "Имя")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 40 символов")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [Display(Name = "Email")]
        [EmailAddress]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "Минимальная длина пароля - 6 символов!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [Display(Name = "Подтвердить пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

 
    }
}