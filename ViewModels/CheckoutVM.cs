using System.ComponentModel.DataAnnotations;

namespace ItalianCharmBracelet.ViewModels
{
    public class CheckoutVM
    {
        [Display(Name = "Tên người nhận")]
        [Required(ErrorMessage = "*")]
        public string Name { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "*")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^(?:\+84|0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5]|9[0-4|6-9])\d{7}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Cell { get; set; }

        public string? Note { get; set; }

        public string PaymentMethod { get; set; }
    }
}
