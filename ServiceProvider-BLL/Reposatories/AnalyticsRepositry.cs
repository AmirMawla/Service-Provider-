using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.AnalyticsDto.cs;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Reposatories
{
    public class AnalyticsRepositry(AppDbContext context, UserManager<Vendor> userManager) : IAnalyticsRepositry
    {
        private readonly AppDbContext _context = context;
        private readonly UserManager<Vendor> _userManager = userManager;

        public async Task<Result<TodaysStatsResponse>> GetTodaysStatsAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.Today;

            var newUsersCount = await _context.ApplicationUsers!
                .Where(x => x.RegistrationDate == today).CountAsync(cancellationToken);

            var orderCount = await _context.Orders!
                .Where(x => x.OrderDate == today)
                .CountAsync(cancellationToken);

            var RevenueToday = await _context.Payments!
                .Where(x => x.TransactionDate == today)
                .SumAsync(x => x.TotalAmount, cancellationToken);

            var vendors = await _userManager.GetUsersInRoleAsync("Vendor");

            var vendorsCount = vendors
                .Where(x => x.IsApproved)
                .Count();

            var stats = new TodaysStatsResponse(
                newUsersCount,
                orderCount,
                RevenueToday,
                vendorsCount
            );

            return Result.Success(stats);

        }

        public async Task<Result<IEnumerable<VendorRevenueResponse>>> GetTopVendorsAsync()
        {

            var vendorsQuery = _context.Users
                .Where(u => u.IsApproved)
                .Select(v => new
                {
                    v.Id,
                    v.FullName,
                    v.BusinessName,
                    v.BusinessType,
                    v.ProfilePictureUrl,
                    v.CoverImageUrl
                });

            var vendorList = await vendorsQuery.ToListAsync(); // Execute query in the database

            var vendorsRevenue = vendorList.Select(vendor => new VendorRevenueResponse
            (
                vendor.Id,
                vendor.FullName,
                vendor.BusinessName,
                vendor.BusinessType,
                vendor.ProfilePictureUrl,
                vendor.CoverImageUrl,
                _context.Products!
                    .Where(p => p.VendorId == vendor.Id)
                    .Join(_context.OrderProducts!,
                        product => product.Id,
                        orderProduct => orderProduct.ProductId,
                        (product, orderProduct) => new { product, orderProduct })
                    .Join(_context.Orders!,
                        po => po.orderProduct.OrderId,
                        order => order.Id,
                        (po, order) => new { po.product, po.orderProduct, order })
                    .Join(_context.Payments!,
                        poo => poo.order.Id,
                        payment => payment.OrderId,
                        (poo, payment) => payment.TotalAmount)
                    .Sum(),  // Now done in-memory

                _context.Products!
                    .Where(p => p.VendorId == vendor.Id)
                    .Join(_context.OrderProducts!,
                        product => product.Id,
                        orderProduct => orderProduct.ProductId,
                        (product, orderProduct) => orderProduct.OrderId)
                    .Distinct()
                    .Count()  // Now done in-memory
            ))
            .OrderByDescending(v => v.TotalRevenue)
            .Take(15)
            .ToList(); // Execute calculation in-memory

            

            return Result.Success(vendorsRevenue.Adapt<IEnumerable<VendorRevenueResponse>>());
        } 

        public async Task<Result<OverAllStatisticsResponse>> GetOverallStatisticsAsync()
        {
            var totalUsersCount = await _context.ApplicationUsers!.CountAsync();

            var vendors = await _userManager.GetUsersInRoleAsync("Vendor");

            var vendorsCount = vendors
                .Where(x => x.IsApproved)
                .Count();

            var OverAllRevenue = await _context.Payments!.SumAsync(x => x.TotalAmount);

            var response = new OverAllStatisticsResponse(
                totalUsersCount,
                vendorsCount,
                OverAllRevenue
            );

            return Result.Success(response);
        }
    }
}
