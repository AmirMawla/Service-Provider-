using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.MessageDto;
using ServiceProvider_BLL.Interfaces;
using System.Collections.Concurrent;

namespace SeeviceProvider_PL.Hubs
{
    [Authorize]
    public class MessageHub(IMessageRepository messageRepository) : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        private readonly IMessageRepository _messageRepository = messageRepository;

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User!.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections[userId] = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User!.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId) && UserConnections.TryRemove(userId, out _))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinOrderGroup(int orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }

        public async Task LeaveOrderGroup(int orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }

        public async Task SendMessage(MessageRequest messageDto)
        {
            var senderId = Context.User!.FindFirst("sub")?.Value;
            var senderRole = Context.User.IsInRole("Vendor") ? "Vendor" : "User";

            var result = await _messageRepository.SendMessageAsync(senderId!, senderRole, messageDto);

            if (!result.IsSuccess)
            {
                throw new HubException(result.ToProblem().ToString());
            }

            await Clients.Group($"Order_{messageDto.OrderId}").SendAsync("ReceiveMessage", result.Value);
        }
    }
}
