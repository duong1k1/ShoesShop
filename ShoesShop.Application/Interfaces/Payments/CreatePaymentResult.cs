using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Payments;

public class CreatePaymentResult
{
    public string RequestId { get; set; } = string.Empty;

    public string PaymentUrl { get; set; } = string.Empty;

    public string? RawResponse { get; set; }
}
