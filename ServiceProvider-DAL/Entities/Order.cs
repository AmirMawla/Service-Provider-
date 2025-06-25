using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_DAL.Entities
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } 
        public OrderStatus Status { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;

        public virtual ApplicationUser User { get; set; } = default!;
        public virtual ICollection<Message>? Messages { get; set; } = new List<Message>();
        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        public virtual ICollection<Shipping>? Shippings { get; set; } = new List<Shipping>();
        public virtual Payment Payment { get; set; } = default!;

    }
   
}
