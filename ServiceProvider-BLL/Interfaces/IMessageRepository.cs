using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.MessageDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<Result<MessageResponse>> SendMessageAsync(string senderId, string senderRole, MessageRequest messageDto, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<MessageResponse>>> GetConversationAsync(string userId, string userRole, string otherUserId, int orderId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<MessageResponse>>> GetUserConversationsAsync(string userId, string userRole, CancellationToken cancellationToken = default);
        Task<Result> MarkMessageAsReadAsync(int messageId, string userId, CancellationToken cancellationToken = default);
    }
}
