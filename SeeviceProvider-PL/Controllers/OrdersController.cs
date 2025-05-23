﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.OrderDto;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Entities;
using System.Security.Claims;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IUnitOfWork orderRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _orderRepositry = orderRepositry;

        [HttpGet("{id}")]
        [Authorize(Roles ="Admin,MobileUser")]
        [ProducesResponseType(typeof(OrderResponseV2),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetOrder([FromRoute] int id , CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var result = await _orderRepositry.Orders.GetOrderAsync(id,currentUserId!,isAdmin, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
       
        [HttpGet("users")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(PaginatedList<OrderResponseV2>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserOrders([FromQuery] string? userId,[FromQuery] RequestFilter request,CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "MobileUser")
            {
                // users can only access their own orders
                if (!string.IsNullOrEmpty(userId) && userId != currentUserId)
                    return Forbid();

                userId = currentUserId;
            }
            else if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required for admin users");
            }

            var result = await _orderRepositry.Orders.GetUserOrdersAsync(userId!,request, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("all-orders")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PaginatedList<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllOrders([FromQuery] RequestFilter request,CancellationToken cancellationToken)
        { 

            var result = await _orderRepositry.Orders.GetAllOrderAsync(request, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("vendors/{vendorId}")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        [ProducesResponseType(typeof(PaginatedList<OrdersOfVendorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetVendorOrders([FromRoute]string? vendorId,[FromQuery] RequestFilter request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "Vendor")
            {
                // Vendors can only access their own reviews
                if (!string.IsNullOrEmpty(vendorId) && vendorId != currentUserId)
                    return Forbid();

                vendorId = currentUserId;
            }
            else if (string.IsNullOrEmpty(vendorId))
            {
                return BadRequest("Vendor ID is required for admin users");
            }

            var result = await _orderRepositry.Orders.GetVendorsOrders(vendorId!, request,cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("vendors/top-five-recent-orders")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(typeof(IEnumerable<RecentOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTopFiveRecentVendorOrders(int count = 5,CancellationToken cancellationToken = default)
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _orderRepositry.Orders.GetTopFiveRecentOrdersAsync(vendorId!, count, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("")]
        [Authorize(Roles = "MobileUser")]
        [ProducesResponseType(typeof(OrderResponseV2), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddOrder ([FromBody] OrderRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _orderRepositry.Orders.AddOrderAsync(userId!,request, cancellationToken);

            return result.IsSuccess
                ?  CreatedAtAction(nameof(GetOrder), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        [ProducesResponseType(typeof(OrderResponseV2), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, [FromBody] UpdateOrderStatusRequest request , CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var result = await _orderRepositry.Orders.UpdateOrderStatusAsync(id,currentUserId!,isAdmin, request , cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

     

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request , CancellationToken cancellationToken)
        {
            var result = await _orderRepositry.Orders.CheckoutAsync(request , cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

    }
}
