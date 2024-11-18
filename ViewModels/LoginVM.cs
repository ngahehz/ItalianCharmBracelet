using System.ComponentModel.DataAnnotations;

namespace ItalianCharmBracelet.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(20, ErrorMessage = "Username can not be more than 20 characters")]
        //cần phải check trong database nữa, xem sql nó có bao nhiêu ký tự
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

}
