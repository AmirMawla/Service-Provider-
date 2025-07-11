﻿using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.PaymentDto;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ServiceProvider_BLL.Reposatories
{
    public class PaymentRepository: BaseRepository<Payment> , IPaymentRepository 
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedList<TransactionResponse>>> GetAllTransactions(RequestFilter request , CancellationToken cancellationToken = default)
        {
            var query = _context.Payments!
                .OrderByDescending(x => x.TransactionDate)
                .AsNoTracking();

            if (!query.Any())
                return Result.Failure<PaginatedList<TransactionResponse>>(new Error("Not Found","No transactions found",StatusCodes.Status404NotFound));

            if (request.Statuses != null && request.Statuses.Any())
            {
                // Convert string statuses to the enum type (e.g., "Completed" → PaymentStatus.Completed)
                var statusEnums = request.Statuses
                    .Select(s => Enum.Parse<PaymentStatus>(s, ignoreCase: true))
                    .ToList();

                query = query.Where(x => statusEnums.Contains(x.Status));
            }


            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Order.User.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.OrderId.ToString(), searchTerm)
                );
            }
             
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            if (request.DateFilter.HasValue && request.DateFilter != null) 
            {
                query = query.Where(x => DateOnly.FromDateTime(x.TransactionDate) == request.DateFilter.Value);
            }

            if (request.PaymentMethods != null && request.PaymentMethods.Any()) 
            {
                var paymentMethods = request.PaymentMethods.ToList();

                query = query.Where(x => paymentMethods.Contains(x.PaymentMethod));
            }

                var source = query.Select(x => new TransactionResponse(
                    x.Id,
                    x.TotalAmount,
                    x.TransactionDate,
                    x.Status.ToString(),
                    x.PaymentMethod,
                    x.OrderId,
                    x.Order.User.FullName
                ));
            var transactions = await PaginatedList<TransactionResponse>.CreateAsync(source, request.PageNumer, request.PageSize, cancellationToken);

            return Result.Success(transactions);
        }

        public async Task<Result<PaginatedList<VendorTransactionResponse>>> GetVendorTransactionsAsync(RequestFilter request,string vendorId, CancellationToken cancellationToken = default)
        {
            var query = _context.Payments!
                .Where(p => p.Order.OrderProducts.Any(op => op.Product.VendorId == vendorId))
                .OrderByDescending(p => p.TransactionDate)
                .AsNoTracking();

            if (!query.Any())
                return Result.Failure<PaginatedList<VendorTransactionResponse>>(new Error("Not Found", "No transactions found", StatusCodes.Status404NotFound));

            if (request.Statuses != null && request.Statuses.Any())
            {
                // Convert string statuses to the enum type (e.g., "Completed" → PaymentStatus.Completed)
                var statusEnums = request.Statuses
                    .Select(s => Enum.Parse<PaymentStatus>(s, ignoreCase: true))
                    .ToList();

                query = query.Where(x => statusEnums.Contains(x.Status));
            }


            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Order.User.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.OrderId.ToString(), searchTerm)
                );
            }

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            if (request.DateFilter.HasValue && request.DateFilter != null)
            {
                query = query.Where(x => DateOnly.FromDateTime(x.TransactionDate) == request.DateFilter.Value);
            }

            if (request.PaymentMethods != null && request.PaymentMethods.Any())
            {
                var paymentMethods = request.PaymentMethods.ToList();

                query = query.Where(x => paymentMethods.Contains(x.PaymentMethod));
                
            }

            var source = query.Select(x => new VendorTransactionResponse(
              x.OrderId,
              x.Order.User.FullName,
              x.Order.User.Email,
              x.Order.OrderProducts.Where(op => op.Product.VendorId == vendorId)
              .Sum(op => op.Quantity*op.Product.Price),
              x.PaymentMethod,
              x.Status.ToString(),
              x.TransactionDate,
              x.Order.OrderProducts.Where(op => op.Product.VendorId == vendorId).Select(op => new VendorProductTransactionResponse(
                op.ProductId,
                op.Product.NameEn,
                op.Product.NameAr,
                op.Product.Price,
                op.Quantity
              )).ToList()
            ));

            var transactions = await PaginatedList<VendorTransactionResponse>.CreateAsync(source, request.PageNumer, request.PageSize, cancellationToken);

            return Result.Success(transactions);
        }

        public async Task<Result<PaginatedList<TransactionResponse>>> GetUserTransactions(string userId,RequestFilter request, CancellationToken cancellationToken = default)
        {
            var query = _context.Payments!
                .Where(x => x.Order.ApplicationUserId == userId)
                .OrderByDescending(x => x.TransactionDate)
                .Select(x => new TransactionResponse(
                    x.Id,
                    x.TotalAmount,
                    x.TransactionDate,
                    x.Status.ToString(),
                    x.PaymentMethod,
                    x.OrderId,
                    x.Order.User.FullName
                ))
                .AsNoTracking();

            if (!query.Any())
                return Result.Failure<PaginatedList<TransactionResponse>>(new Error("Not Found", "No transactions found", StatusCodes.Status404NotFound));


            var transactions = await PaginatedList<TransactionResponse>.CreateAsync(query, request.PageNumer, request.PageSize, cancellationToken);

            return Result.Success(transactions);
        }

        public async Task<Result<int>> GetTotalTransactionsCountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _context.Payments!.CountAsync(cancellationToken);
                return Result.Success(count);
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(new Error("Error", "Failed to retrieve transactions count.", StatusCodes.Status400BadRequest));
            }
        }

        public async Task<Result<PaymentStatsResponse>> GetPaymentStatsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var totalTransactions = await _context.Payments!.CountAsync(cancellationToken);

                var completedTransactions = await _context.Payments!
                    .Where(x => x.Status == PaymentStatus.Completed)
                    .CountAsync(cancellationToken);

                var pendingTransactions = await _context.Payments!
                    .Where(x => x.Status == PaymentStatus.Pending)
                    .CountAsync(cancellationToken);

                var totalRevenue = await _context.Payments!
                    .SumAsync(x => x.TotalAmount, cancellationToken: cancellationToken);

                var response = new PaymentStatsResponse(
                    totalTransactions,
                    completedTransactions,
                    pendingTransactions,
                    totalRevenue
                );

                return Result.Success(response);
            }
            catch
            {
                return Result.Failure<PaymentStatsResponse>(new Error("Error", "Failed to retrieve required data.", StatusCodes.Status400BadRequest));
            }
        }

        public async Task<Result<IEnumerable<VendorRevenueByPaymentMethod>>> GetVendorRevenueByPaymentMethod(string vendorId,CancellationToken cancellationToken = default)
        {
            // Predefined list of payment methods
            var allPaymentMethods = new List<string>
            {
                "visa",
                "mastercard",
                "amex",
                "jcb",
                "discover",
                "union pay"
            };

            // Get existing revenue data
            var revenueData = await _context.OrderProducts!
                .Include(op => op.Product)
                .Include(op => op.Order)
                    .ThenInclude(o => o.Payment)
                .Where(op =>
                    op.Product.VendorId == vendorId &&
                    op.Order.Payment.Status == PaymentStatus.Completed)
                .GroupBy(op => op.Order.Payment.PaymentMethod)
                .Select(g => new
                {
                    PaymentMethod = g.Key,
                    Revenue = g.Sum(op => op.Quantity * op.Product.Price)
                })
                .ToListAsync(cancellationToken);

            // Create dictionary for efficient lookup
            var revenueDict = revenueData
                .ToDictionary(
                    x => x.PaymentMethod.ToLower(),
                    x => x.Revenue
                );

            // Create result with all payment methods
            var result = allPaymentMethods
                .Select(method => new VendorRevenueByPaymentMethod(
                    method,
                    revenueDict.TryGetValue(method.ToLower(), out var revenue)
                        ? revenue
                        : 0
                ))
                .OrderByDescending(x => x.Revenue)
                .ToList();

            return result.Any()
                ? Result.Success(result.AsEnumerable())
                : Result.Failure<IEnumerable<VendorRevenueByPaymentMethod>>(
                    new Error("Not Found", "No revenue data found", StatusCodes.Status404NotFound));
        }

        public async Task<Result<IEnumerable<VendorRevenueByPaymentMethod>>> GetAllRevenueByPaymentMethod(CancellationToken cancellationToken = default)
        {
            var allPaymentMethods = new List<string>
            {
                "visa",
                "mastercard",
                "american express",
                "jcb",
                "discover",
                "union pay"
            };

            var payments = await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                .Where(p => p.Status == PaymentStatus.Completed)
                .ToListAsync(cancellationToken);

            var revenueByPayment = payments
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new VendorRevenueByPaymentMethod(
                    g.Key,
                    g.Sum(p => p.TotalAmount)
                ))
                .OrderByDescending(x => x.Revenue)
                .ToList();

            var revenueDict = revenueByPayment
            .GroupBy(x => x.PaymentMethod.ToLower())
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Revenue)
            );

            var result = allPaymentMethods
              .Select(method => new VendorRevenueByPaymentMethod(
                  method,
                  revenueDict.TryGetValue(method.ToLower(), out var revenue)
                      ? revenue
                      : 0
              ))
              .OrderByDescending(x => x.Revenue)
              .ToList();

            return result.Any()
                ? Result.Success(result.Adapt<IEnumerable<VendorRevenueByPaymentMethod>>())
                : Result.Failure<IEnumerable<VendorRevenueByPaymentMethod>>(
                    new Error("Not Found", "No revenue data found", StatusCodes.Status404NotFound));
        }
    }
}
