using ItalianCharmBracelet.ViewModels;

namespace ItalianCharmBracelet.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnpaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
