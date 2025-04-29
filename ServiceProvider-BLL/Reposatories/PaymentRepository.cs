using Mapster;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return Result.Failure<PaginatedList<TransactionResponse>>(new Error("Not Found","No transactions found",StatusCodes.Status404NotFound));


            var transactions = await PaginatedList<TransactionResponse>.CreateAsync(query, request.PageNumer, request.PageSize, cancellationToken);

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
    }
}
