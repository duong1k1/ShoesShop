using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using ShoesShop.Application.DTOs.Carts;

namespace ShoesShop.Application.Validators.Carts;

public class AddCartItemRequestValidator : AbstractValidator<AddCartItemRequest>
{
    public AddCartItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId không hợp lệ.");

        RuleFor(x => x.Size)
            .NotEmpty()
            .WithMessage("Size không được để trống.")
            .MaximumLength(20)
            .WithMessage("Size không được vượt quá 20 ký tự.");

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Màu không được để trống.")
            .MaximumLength(50)
            .WithMessage("Màu không được vượt quá 50 ký tự.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Số lượng phải lớn hơn 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Không được thêm quá 100 sản phẩm trong một lần.");
    }
}
