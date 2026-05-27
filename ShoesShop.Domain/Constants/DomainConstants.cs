using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Domain.Constants;

public static class DomainConstants
{
    public static class CartStatus
    {
        public const string Active = "Active";
        public const string CheckedOut = "CheckedOut";
        public const string Abandoned = "Abandoned";
    }

    public static class ProductStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Draft = "Draft";
        public const string Deleted = "Deleted";
    }

    public static class ProductVariantStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Deleted = "Deleted";
    }

    public static class PaymentMethod
    {
        public const string COD = "COD";
        public const string VNPay = "VNPay";
        public const string Momo = "Momo";
    }

    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Failed = "Failed";
        public const string Refunded = "Refunded";
        public const string Cancelled = "Cancelled";
    }

    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Processing = "Processing";
        public const string Shipping = "Shipping";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";
        public const string Returned = "Returned";
    }

    public static class PaymentProvider
    {
        public const string COD = "COD";
        public const string VNPay = "VNPay";
        public const string Momo = "Momo";
    }

    public static class PaymentTransactionStatus
    {
        public const string Pending = "Pending";
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
    }
}