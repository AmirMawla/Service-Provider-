using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.MessageDto;
using ServiceProvider_BLL.Errors;
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
    public class MessageRepository : BaseRepository<Message> , IMessageRepository
    {
        private readonly AppDbContext _context;
        //private readonly IHubContext<MessageHub> _hubContext;

        public MessageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Result<MessageResponse>> SendMessageAsync(string senderId, string senderRole, MessageRequest messageDto,CancellationToken cancellationToken = default)
        {
            var senderType = senderRole.ToLower() == "user" ? SenderType.User : SenderType.Vendor;

            // Load the order and validate it includes both sender and receiver
            var order = await _context.Orders!
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Vendor)
                .FirstOrDefaultAsync(o => o.Id == messageDto.OrderId,cancellationToken);

            if (order == null)
                return Result.Failure<MessageResponse>(OrderErrors.OrderNotFound);

            // Validate sender and receiver relationship to the order
            if (senderType == SenderType.User)
            {
                // Validate user is order owner
                if (order.ApplicationUserId != senderId)
                    return Result.Failure<MessageResponse>(OrderErrors.UserNotOrderOwner);

                // Validate vendor has products in this order
                var vendorExistsInOrder = order.OrderProducts
                    .Any(op => op.Product.VendorId == messageDto.ReceiverId);

                if (!vendorExistsInOrder)
                    return Result.Failure<MessageResponse>(OrderErrors.VendorNotAssociatedWithThisOrder);
            }
            else // SenderType.Vendor
            {
                // Validate vendor has products in this order
                var vendorExistsInOrder = order.OrderProducts
                    .Any(op => op.Product.VendorId == senderId);

                if (!vendorExistsInOrder)
                    return Result.Failure<MessageResponse>(OrderErrors.VendorNotAssociatedWithThisOrder);

                // Validate receiver is the order owner
                if (order.ApplicationUserId != messageDto.ReceiverId)
                    return Result.Failure<MessageResponse>(OrderErrors.UserNotOrderOwner);
            }

            // Get sender and receiver info
            string senderName = "";
            string receiverName = "";

            if (senderType == SenderType.User) 
            {
                var sender = await _context.ApplicationUsers!.FindAsync(senderId);
                var receiver = await _context.Users.FindAsync(messageDto.ReceiverId);

                if (sender == null || receiver == null)
                    return Result.Failure<MessageResponse>(MessageErrors.SenderOrReceiverNotFound);
                senderName = sender.FullName;
                receiverName = receiver.FullName;
            }

            else 
            {
                var sender = await _context.Users.FindAsync(senderId);
                var receiver = await _context.ApplicationUsers!.FindAsync(messageDto.ReceiverId);

                if (sender == null || receiver == null)
                    return Result.Failure<MessageResponse>(MessageErrors.SenderOrReceiverNotFound);
                senderName = sender.FullName;
                receiverName = receiver.FullName;
            }

            // Create and save the message
            var message = new Message
            {
                MessageText = messageDto.MessageText,
                ApplicationUserId = senderType == SenderType.User ? senderId : messageDto.ReceiverId,
                VendorId = senderType == SenderType.Vendor ? senderId : messageDto.ReceiverId,
                OrderId = messageDto.OrderId,
                SenderType = senderType,
                MessageDate = DateTime.UtcNow
            };

            _context.Messages!.Add(message);
            await _context.SaveChangesAsync(cancellationToken);

            // Build DTO for broadcasting
            var messageDtoResult = new MessageResponse(
                message.Id,
                message.MessageText,
                message.MessageDate,
                message.IsRead ,
                senderId,
                senderName,
                senderRole,
                messageDto.ReceiverId,
                receiverName,
                messageDto.OrderId
            );

            // Send to group associated with the order
            //await _hubContext.Clients.Group($"Order_{messageDto.OrderId}")
            //    .SendAsync("ReceiveMessage", messageDtoResult);

            return Result.Success(messageDtoResult);
        }

        public async Task<Result<IEnumerable<MessageResponse>>> GetConversationAsync(string userId, string userRole, string otherUserId, int orderId, CancellationToken cancellationToken = default)
        {
            var messages = await _context.Messages!
                .Where(m => m.OrderId == orderId &&
                            ((m.SenderType == SenderType.User && m.ApplicationUserId == userId && m.VendorId == otherUserId) ||
                             (m.SenderType == SenderType.Vendor && m.VendorId == userId && m.ApplicationUserId == otherUserId)))
                .AsNoTracking()
                .Select(m => new MessageResponse(
                    m.Id,
                    m.MessageText,
                    m.MessageDate,
                    m.IsRead ,
                    m.SenderType == SenderType.User ? m.ApplicationUserId : m.VendorId,
                    m.SenderType == SenderType.User ? m.User.FullName : m.Vendor.FullName!,
                    m.SenderType.ToString(),
                    m.SenderType == SenderType.User ? m.VendorId : m.ApplicationUserId,
                    m.SenderType == SenderType.User ? m.Vendor.FullName! : m.User.FullName,
                    m.OrderId
                ))
                .ToListAsync(cancellationToken);

            if (!messages.Any())
                return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.NoMessagesFound);

            return Result.Success<IEnumerable<MessageResponse>>(messages);
        }

        public async Task<Result<IEnumerable<MessageResponse>>> GetUserConversationsAsync(string userId, string userRole, CancellationToken cancellationToken = default)
        {
            var isUser = userRole.ToLower() == "user";
            var messages = await _context.Messages!
                .Where(m => (isUser && m.ApplicationUserId == userId) || (!isUser && m.VendorId == userId))
                .AsNoTracking()
                .GroupBy(m => new { m.OrderId, m.VendorId, m.ApplicationUserId })
                .Select(g => g.OrderByDescending(m => m.MessageDate).First())
                .Select(m => new MessageResponse(
                    m.Id,
                    m.MessageText,
                    m.MessageDate,
                    m.IsRead ,
                    m.SenderType == SenderType.User ? m.ApplicationUserId : m.VendorId,
                    m.SenderType == SenderType.User ? m.User.FullName : m.Vendor.FullName,
                    m.SenderType.ToString(),
                    m.SenderType == SenderType.User ? m.VendorId : m.ApplicationUserId,
                    m.SenderType == SenderType.User ? m.Vendor.FullName! : m.User.FullName,
                    m.OrderId
                ))
                .ToListAsync(cancellationToken);

            if (!messages.Any())
                return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.NoMessagesFound);


            return Result.Success<IEnumerable<MessageResponse>>(messages);
        }

        public async Task<Result> MarkMessageAsReadAsync(int messageId, string userId, CancellationToken cancellationToken = default)
        {
            var message = await _context.Messages!
            .Include(m => m.User)
            .Include(m => m.Vendor)
            .FirstOrDefaultAsync(m => m.Id == messageId,cancellationToken);
            

            if (message == null)
                return Result.Failure(MessageErrors.NoMessagesFound);

            if (message.ApplicationUserId != userId && message.VendorId != userId)
                return Result.Failure(MessageErrors.UnAuthorized);

            message.IsRead = true;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }

    }
}
