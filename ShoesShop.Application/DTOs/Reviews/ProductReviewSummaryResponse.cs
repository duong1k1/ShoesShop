using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.DTOs.Reviews;

public class ProductReviewSummaryResponse
{
    public long ProductId { get; set; }

    public double AverageRating { get; set; }

    public int TotalReviews { get; set; }

    public List<ReviewResponse> Reviews { get; set; } = new();
}
