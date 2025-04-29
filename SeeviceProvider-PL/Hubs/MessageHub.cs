using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.MessageDto;
using ServiceProvider_BLL.Interfaces;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace SeeviceProvider_PL.Hubs
{
    [Authorize]
    public class MessageHub(IMessageRepository messageRepository) : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        private readonly IMessageRepository _messageRepository = messageRepository;

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections[userId] = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId) && UserConnections.TryRemove(userId, out _))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinOrderGroup(int orderId)
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = Context.User!.IsInRole("Vendor") ? "Vendor" : "User";

            // Validate that this user has permission to join this order group
            var hasAccess = await ValidateOrderAccess(userId!, userRole, orderId);

            if (!hasAccess)
            {
                throw new HubException("You don't have permission to access this order conversation.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
            await Clients.Caller.SendAsync("GroupJoined", $"Joined Order_{orderId}");
        }

        public async Task LeaveOrderGroup(int orderId)
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = Context.User!.IsInRole("Vendor") ? "Vendor" : "User";

            // Validate that this user has permission to join this order group
            var hasAccess = await ValidateOrderAccess(userId!, userRole, orderId);

            if (!hasAccess)
            {
                throw new HubException("You don't have permission to access this order conversation.");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }

        public async Task SendMessage(MessageRequest messageDto)
        {
            var senderId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            var senderRole = Context.User!.IsInRole("Vendor") ? "Vendor" : "User";

            var result = await _messageRepository.SendMessageAsync(senderId!, senderRole, messageDto);

            if (!result.IsSuccess)
            {
                throw new HubException(result.ToProblem().ToString());
            }

            await Clients.Group($"Order_{messageDto.OrderId}").SendAsync("ReceiveMessage", result.Value);
        }

        private async Task<bool> ValidateOrderAccess(string userId, string userRole, int orderId)
        {
            // Get the order with related data
            var order = await _messageRepository.GetOrderWithRelationsAsync(orderId);

            if (order == null)
                return false;

            if (userRole == "User")
            {
                // For users, check if they're the order owner
                return order.ApplicationUserId == userId;
            }
            else // Vendor
            {
                // For vendors, check if they have products in this order
                return order.OrderProducts.Any(op => op.Product.VendorId == userId);
            }
        }
    }
}
