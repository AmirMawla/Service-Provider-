using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ProductDto;
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
        Task<Result<PaginatedList<VendorResponse>>> GetAllProviders(RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<PaginatedList<VendorResponse>>> GetAllProvidersForMobile(RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<VendorResponse>> GetProviderDetails(string providerId,CancellationToken cancellationToken = default);
        Task<Result<PaginatedList<VendorRatingResponse>>> GetVendorsRatings(string vendorId, RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<ProductsOfVendorDto>>> GetProviderMenuAsync(string providerId , CancellationToken cancellationToken);
        Task<Result<IEnumerable<TopVendorResponse>>> GetTopVendorsByOrders(CancellationToken cancellationToken = default);
        Task<Result<VendorBusinessTypeRespons>> GetAllVendorsBusinessTypes(CancellationToken cancellationToken = default);
        Task<VendorDashboardResponse> GetVendorDashboard(CancellationToken cancellationToken = default);
        Task<Result> UpdateVendorAsync(string id,UpdateVendorResponse vendorDto, CancellationToken cancellationToken = default);
        Task<Result> ChangeVendorPasswordAsync(string id, ChangeVendorPasswordRequest request);
        Task<Result> DeleteVendorAsync(string vendorId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VendorResponse>>> GetPendingVendorsAsync(CancellationToken cancellationToken = default);
        Task<Result> ApproveVendorAsync(string vendorId, CancellationToken cancellationToken = default);
        Task<Result> DeactivateVendorAsync(string vendorId, CancellationToken cancellationToken = default);
    }
}
