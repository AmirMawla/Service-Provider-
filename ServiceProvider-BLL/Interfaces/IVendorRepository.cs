﻿using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IVendorRepository : IBaseRepository<Vendor>
    {
        Task<Result<IEnumerable<VendorResponse>>> GetVendorsByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
    }
}
