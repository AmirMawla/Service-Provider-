using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_DAL.Entities
{
    public class Banners
    {
        public string? Description { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; } 
        public string? DiscountCode { get; set; }
        public int ProductId { get; set; }
        public string VendorId { get; set; } = string.Empty;
        public virtual Product? Product { get; set; }
        public virtual Vendor? Vendor { get; set; }

    }
}
