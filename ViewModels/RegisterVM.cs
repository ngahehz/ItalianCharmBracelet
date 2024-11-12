using System.ComponentModel.DataAnnotations;

namespace ItalianCharmBracelet.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Username")]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string Id { get; set; }


        [Display(Name = "Password")]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Họ tên")]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string FirstName { get; set; }

        [Display(Name = "Họ tên")]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string LastName { get; set; }

        public bool Gender { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DoB { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 ký tự")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 ký tự")]
        [RegularExpression("^[0-9]{9,12}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Cell { get; set; }


        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }


        public string? Img { get; set; }


        public int Role { get; set; }


        public bool State { get; set; }


        public string? RandomKey { get; set; }
    }
}
