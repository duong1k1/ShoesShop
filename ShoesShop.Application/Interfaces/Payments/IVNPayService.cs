using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Payments;

public interface IVNPayService
{
    CreatePaymentResult CreatePaymentUrl(CreatePaymentRequest request);

    PaymentCallbackResult VerifyCallback(Dictionary<string, string> queryParams);
}
