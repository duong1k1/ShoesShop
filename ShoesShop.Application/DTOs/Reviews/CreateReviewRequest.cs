using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.DTOs.Reviews;

public class CreateReviewRequest
{
    public long ProductId { get; set; }

    public long OrderItemId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }
}