using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using ShoesShop.Application.DTOs.Payments;
using ShoesShop.Domain.Constants;

namespace ShoesShop.Application.Validators.Payments;

public class CheckoutRequestValidator : AbstractValidator<CheckoutRequest>
{
    public CheckoutRequestValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Phương thức thanh toán không được để trống.")
            .Must(x =>
                x == DomainConstants.PaymentMethod.COD ||
                x == DomainConstants.PaymentMethod.VNPay ||
                x == DomainConstants.PaymentMethod.Momo)
            .WithMessage("Phương thức thanh toán chỉ được là COD, VNPay hoặc Momo.");

        RuleFor(x => x.RecipientName)
            .NotEmpty()
            .WithMessage("Tên người nhận không được để trống.")
            .MaximumLength(150)
            .WithMessage("Tên người nhận không được vượt quá 150 ký tự.");

        RuleFor(x => x.RecipientPhone)
            .NotEmpty()
            .WithMessage("Số điện thoại người nhận không được để trống.")
            .MaximumLength(20)
            .WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty()
            .WithMessage("Địa chỉ giao hàng không được để trống.")
            .MaximumLength(500)
            .WithMessage("Địa chỉ giao hàng không được vượt quá 500 ký tự.");

        RuleFor(x => x.ShippingFee)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Phí giao hàng không được âm.");

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .WithMessage("Ghi chú không được vượt quá 500 ký tự.");
    }
}
