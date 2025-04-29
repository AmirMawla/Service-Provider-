using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_DAL.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Description { get; set;} 
        public decimal Price { get; set; } 
        public string? MainImageUrl {  get; set; } = string.Empty ;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int SubCategoryId { get; set; }
        public string VendorId { get; set; } = string.Empty;
        public virtual SubCategory SubCategory { get; set; } = default!;
        public virtual Vendor Vendor { get; set; } = default!;
        public virtual ICollection<Banners>? Banners { get; set; }  = new List<Banners>();
        public virtual ICollection<OrderProduct>? OrderProducts { get; set; } = new List<OrderProduct>();
        public virtual ICollection<Review>? Reviews { get; set;} = new List<Review>();
        public virtual ICollection<CartProduct>? CartProducts { get; set; } = new List<CartProduct>();
    }
}
