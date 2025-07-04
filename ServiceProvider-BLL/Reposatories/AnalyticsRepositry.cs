using Mapster;
using Microsoft.AspNetCore.Http;
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

        public async Task<Result<IEnumerable<VendorRevenueResponse>>> GetTopVendorsAsync(CancellationToken cancellationToken = default)
        {
            var topVendors = await _context.Users
                .Where(u => u.IsApproved)
                .Select(v => new
                {
                    Vendor = v,
                    Revenue = _context.Products!
                        .Where(p => p.VendorId == v.Id)
                        .SelectMany(p => p.OrderProducts)
                        .Where(op => op.Order.Payment.Status == PaymentStatus.Completed)
                        .Sum(op => (decimal?)op.Quantity * op.Product.Price) ?? 0m,
                    OrderCount = _context.Products
                        .Where(p => p.VendorId == v.Id)
                        .SelectMany(p => p.OrderProducts)
                        .Where(op => op.Order.Payment.Status == PaymentStatus.Completed)
                        .Select(op => op.OrderId)
                        .Distinct()
                        .Count()
                })
                .OrderByDescending(x => x.Revenue)
                .Take(15)
                .Select(x => new VendorRevenueResponse(
                    x.Vendor.Id,
                    x.Vendor.FullName,
                    x.Vendor.BusinessName,
                    x.Vendor.BusinessType,
                    x.Vendor.ProfilePictureUrl,
                    x.Vendor.CoverImageUrl,
                    x.Revenue,
                    x.OrderCount
                ))
                .ToListAsync(cancellationToken);

            return topVendors.Any()
                ? Result.Success(topVendors.AsEnumerable())
                : Result.Failure<IEnumerable<VendorRevenueResponse>>(
                    new Error("Not Found", "No vendors found", StatusCodes.Status404NotFound));
        }

        public async Task<Result<OverAllStatisticsResponse>> GetOverallStatisticsAsync()
        {
            var totalUsersCount = await _context.ApplicationUsers!.CountAsync();

            var vendors = await _userManager.GetUsersInRoleAsync("Vendor");

            var vendorsCount = vendors
                .Where(x => x.IsApproved)
                .Count();

            var OverAllRevenue = await _context.Payments!.Where(x => x.Status == PaymentStatus.Completed).SumAsync(x => x.TotalAmount);

            var response = new OverAllStatisticsResponse(
                totalUsersCount,
                vendorsCount,
                OverAllRevenue
            );

            return Result.Success(response);
        }
    }
}
