using FluentValidation;
using ShoesShop.Application.DTOs.Reviews;

namespace ShoesShop.Application.Validators.Reviews;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId không hợp lệ.");

        RuleFor(x => x.OrderItemId)
            .GreaterThan(0)
            .WithMessage("OrderItemId không hợp lệ.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating phải nằm trong khoảng từ 1 đến 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Nội dung đánh giá không được vượt quá 2000 ký tự.");
    }
}
