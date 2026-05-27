using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Payments;

public class CreatePaymentRequest
{
    public long OrderId { get; set; }

    public string OrderCode { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string OrderInfo { get; set; } = string.Empty;

    public string ClientIpAddress { get; set; } = string.Empty;
}
