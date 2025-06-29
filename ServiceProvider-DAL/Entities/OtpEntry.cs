using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_DAL.Entities
{
    public class OtpEntry
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string HashedOtp { get; set; } = default!;
        public DateTime Expiry { get; set; }

    }
}
