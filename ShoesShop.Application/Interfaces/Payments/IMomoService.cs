using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Payments;

public interface IMomoService
{
    Task<CreatePaymentResult> CreatePaymentAsync(CreatePaymentRequest request);

    PaymentCallbackResult VerifyCallback(Dictionary<string, string> data);
}
