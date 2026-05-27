using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Payments;

public class PaymentCallbackResult
{
    public bool IsValidSignature { get; set; }

    public bool IsSuccess { get; set; }

    public string OrderCode { get; set; } = string.Empty;

    public string? TransactionCode { get; set; }

    public string? ResponseCode { get; set; }

    public string? TransactionStatus { get; set; }

    public string RawData { get; set; } = string.Empty;
}
