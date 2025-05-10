using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.SearchDto;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Reposatories
{
    public class SearchRepository(AppDbContext context) : ISearchRepository
    {
        private readonly AppDbContext _context = context;

        //public async Task<Result<IEnumerable<GlobalSearchResponse>>> GlobalSearchAsync(string searchTerm, int maxResultsPerType = 50, CancellationToken cancellationToken = default)
        //{
        //    // Input validation
        //    if (string.IsNullOrWhiteSpace(searchTerm))
        //        return Result.Failure<IEnumerable<GlobalSearchResponse>>(
        //            new Error("Validation", "Search term cannot be empty", StatusCodes.Status400BadRequest));

        //    // Normalize search term once
        //    var normalizedTerm = $"%{searchTerm.Trim().ToLower()}%";

        //    try
        //    {
        //        // Using a dictionary to organize and track search tasks
        //        var searchTasks = new Dictionary<string, Task<List<GlobalSearchResponse>>>
        //        {
        //            ["Vendor"] = SearchVendorsAsync(normalizedTerm, maxResultsPerType, cancellationToken),
        //            ["Product"] = SearchProductsAsync(normalizedTerm, maxResultsPerType, cancellationToken),
        //            ["Category"] = SearchCategoriesAsync(normalizedTerm, maxResultsPerType, cancellationToken),
        //            ["SubCategory"] = SearchSubCategoriesAsync(normalizedTerm, maxResultsPerType, cancellationToken)
        //        };

        //        // Wait for all tasks to complete
        //        await Task.WhenAll(searchTasks.Values);

        //        // Combine and sort results
        //        var results = searchTasks.Values
        //            .SelectMany(task => task.Result)
        //            .OrderBy(x => x.Type)
        //            .ThenBy(x => x.NameEn)
        //            .ToList();

        //        return results.Any()
        //            ? Result.Success<IEnumerable<GlobalSearchResponse>>(results)
        //            : Result.Failure<IEnumerable<GlobalSearchResponse>>(
        //                new Error("Search", "No results found", StatusCodes.Status404NotFound));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result.Failure<IEnumerable<GlobalSearchResponse>>(
        //            new Error("Search", $"An error occurred while searching: {ex.Message}", StatusCodes.Status500InternalServerError));
        //    }

        //    // Helper methods for each search type
        //    async Task<List<GlobalSearchResponse>> SearchVendorsAsync(string term, int limit, CancellationToken ct)
        //    {


        //        return await _context.Users
        //            .Where(u => u.BusinessName != null && (
        //                EF.Functions.Like(u.FullName.ToLower(), term) ||
        //                EF.Functions.Like(u.BusinessName.ToLower(), term) ||
        //                EF.Functions.Like(u.BusinessType.ToLower(), term)))
        //            .Select(u => new GlobalSearchResponse(
        //                "Vendor",
        //                u.Id,
        //                null,
        //                null,
        //                u.FullName,
        //                u.BusinessName,
        //                u.BusinessType,
        //                null,
        //                null,
        //                null))
        //            .Take(limit)
        //            .AsNoTracking()
        //            .ToListAsync(ct);
        //    }

        //    async Task<List<GlobalSearchResponse>> SearchProductsAsync(string term, int limit, CancellationToken ct)
        //    {


        //        return await _context.Products!
        //            .Where(p =>
        //                EF.Functions.Like(p.NameEn.ToLower(), term) ||
        //                EF.Functions.Like(p.NameAr.ToLower(), term) ||
        //                EF.Functions.Like(p.Description!.ToLower(), term))
        //            .Select(p => new GlobalSearchResponse(
        //                "Product",
        //                p.Id.ToString(),
        //                p.NameEn,
        //                p.NameAr,
        //                null,
        //                null,
        //                null,
        //                p.SubCategory.Category.NameEn,
        //                p.SubCategory.Category.NameAr,
        //                p.MainImageUrl))
        //            .Take(limit)
        //            .AsNoTracking()
        //            .ToListAsync(ct);
        //    }

        //    async Task<List<GlobalSearchResponse>> SearchCategoriesAsync(string term, int limit, CancellationToken ct)
        //    {


        //        return await _context.Categories!
        //            .Where(c =>
        //                EF.Functions.Like(c.NameEn.ToLower(), term) ||
        //                EF.Functions.Like(c.NameAr.ToLower(), term))
        //            .Select(c => new GlobalSearchResponse(
        //                "Category",
        //                c.Id.ToString(),
        //                c.NameEn,
        //                c.NameAr,
        //                null,
        //                null,
        //                null,
        //                null,
        //                null,
        //                c.ImageUrl))
        //            .Take(limit)
        //            .AsNoTracking()
        //            .ToListAsync(ct);
        //    }

        //    async Task<List<GlobalSearchResponse>> SearchSubCategoriesAsync(string term, int limit, CancellationToken ct)
        //    {


        //        return await _context.SubCategories!
        //            .Where(sc =>
        //                EF.Functions.Like(sc.NameEn.ToLower(), term) ||
        //                EF.Functions.Like(sc.NameAr.ToLower(), term))
        //            .Select(sc => new GlobalSearchResponse(
        //                "SubCategory",
        //                sc.Id.ToString(),
        //                sc.NameEn,
        //                sc.NameAr,
        //                null,
        //                null,
        //                null,
        //                sc.Category.NameEn,
        //                sc.Category.NameAr,
        //                sc.ImageUrl))
        //            .Take(limit)
        //            .AsNoTracking()
        //            .ToListAsync(ct);
        //    }
        //}
        public async Task<Result<IEnumerable<GlobalSearchResponse>>> GlobalSearchAsync(string searchTerm, int maxResultsPerType = 50, CancellationToken cancellationToken = default)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Result.Failure<IEnumerable<GlobalSearchResponse>>(
                    new Error("Validation", "Search term cannot be empty", StatusCodes.Status400BadRequest));

            // Normalize search term once
            var normalizedTerm = $"%{searchTerm.Trim().ToLower()}%";

            try
            {
                // Execute searches sequentially
                var vendorResults = await SearchVendorsAsync(normalizedTerm, maxResultsPerType, cancellationToken);
                var productResults = await SearchProductsAsync(normalizedTerm, maxResultsPerType, cancellationToken);
                var categoryResults = await SearchCategoriesAsync(normalizedTerm, maxResultsPerType, cancellationToken);
                var subCategoryResults = await SearchSubCategoriesAsync(normalizedTerm, maxResultsPerType, cancellationToken);

                // Combine results
                var results = new List<GlobalSearchResponse>();
                results.AddRange(vendorResults);
                results.AddRange(productResults);
                results.AddRange(categoryResults);
                results.AddRange(subCategoryResults);

                // Sort combined results
                var orderedResults = results
                    .OrderBy(x => x.Type)
                    .ThenBy(x => x.NameEn)
                    .ToList();

                return orderedResults.Any()
                    ? Result.Success<IEnumerable<GlobalSearchResponse>>(orderedResults)
                    : Result.Failure<IEnumerable<GlobalSearchResponse>>(
                        new Error("Search", "No results found", StatusCodes.Status404NotFound));
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<GlobalSearchResponse>>(
                    new Error("Search", $"An error occurred while searching: {ex.Message}", StatusCodes.Status500InternalServerError));
            }

            // Helper methods remain unchanged
            async Task<List<GlobalSearchResponse>> SearchVendorsAsync(string term, int limit, CancellationToken ct)
            {
                return await _context.Users
                    .Where(u => u.BusinessName != null && (
                        EF.Functions.Like(u.FullName.ToLower(), term) ||
                        EF.Functions.Like(u.BusinessName.ToLower(), term) ||
                        EF.Functions.Like(u.BusinessType.ToLower(), term)))
                    .Select(u => new GlobalSearchResponse(
                        "Vendor",
                        u.Id,
                        null,
                        null,
                        u.FullName,
                        u.BusinessName,
                        u.BusinessType,
                        null,
                        null,
                        null))
                    .Take(limit)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            async Task<List<GlobalSearchResponse>> SearchProductsAsync(string term, int limit, CancellationToken ct)
            {
                return await _context.Products!
                    .Where(p =>
                        EF.Functions.Like(p.NameEn.ToLower(), term) ||
                        EF.Functions.Like(p.NameAr.ToLower(), term) ||
                        EF.Functions.Like(p.Description!.ToLower(), term))
                    .Select(p => new GlobalSearchResponse(
                        "Product",
                        p.Id.ToString(),
                        p.NameEn,
                        p.NameAr,
                        null,
                        null,
                        null,
                        p.SubCategory.Category.NameEn,
                        p.SubCategory.Category.NameAr,
                        p.MainImageUrl))
                    .Take(limit)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            async Task<List<GlobalSearchResponse>> SearchCategoriesAsync(string term, int limit, CancellationToken ct)
            {
                return await _context.Categories!
                    .Where(c =>
                        EF.Functions.Like(c.NameEn.ToLower(), term) ||
                        EF.Functions.Like(c.NameAr.ToLower(), term))
                    .Select(c => new GlobalSearchResponse(
                        "Category",
                        c.Id.ToString(),
                        c.NameEn,
                        c.NameAr,
                        null,
                        null,
                        null,
                        null,
                        null,
                        c.ImageUrl))
                    .Take(limit)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            async Task<List<GlobalSearchResponse>> SearchSubCategoriesAsync(string term, int limit, CancellationToken ct)
            {
                return await _context.SubCategories!
                    .Where(sc =>
                        EF.Functions.Like(sc.NameEn.ToLower(), term) ||
                        EF.Functions.Like(sc.NameAr.ToLower(), term))
                    .Select(sc => new GlobalSearchResponse(
                        "SubCategory",
                        sc.Id.ToString(),
                        sc.NameEn,
                        sc.NameAr,
                        null,
                        null,
                        null,
                        sc.Category.NameEn,
                        sc.Category.NameAr,
                        sc.ImageUrl))
                    .Take(limit)
                    .AsNoTracking()
                    .ToListAsync(ct);
            }
        }
    }
}
