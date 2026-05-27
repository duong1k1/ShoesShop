using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.DTOs.Payments;

public class CheckoutRequest
{
    public string PaymentMethod { get; set; } = string.Empty;

    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public decimal ShippingFee { get; set; }

    public string? Note { get; set; }
}
