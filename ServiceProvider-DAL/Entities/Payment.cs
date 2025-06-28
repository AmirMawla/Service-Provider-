using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_DAL.Entities
{
    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4,
        Cancelled = 5
    }
    public class Payment
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public PaymentStatus Status { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public int OrderId { get; set; }

        public virtual Order Order { get; set; } = default!;

    }
}
