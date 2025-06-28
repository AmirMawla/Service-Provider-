using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_DAL.Entities
{
    public enum ShippingStatus{
       Preparing ,
       OutForDelivery,
       Delivered,
       Pending,
       Cancelled
    }
    public class Shipping
    {

        public int OrderId { get; set; }
        public string VendorId { get; set; } = string.Empty;

        public DateTime EstimatedDeliveryDate { get; set; }

        public ShippingStatus Status { get; set; }

        public virtual Order Order { get; set; } = default!;

        public virtual Vendor Vendor { get; set; } = default!;
    }
}
