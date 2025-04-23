using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Interfaces;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController(IUnitOfWork messageRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _messageRepositry = messageRepositry;

        [HttpGet("conversation/{otherUserId}/{orderId}")]
        public async Task<IActionResult> GetConversation(string otherUserId, int orderId)
        {
            var userId = User.FindFirst("sub")?.Value;
            var userRole = User.IsInRole("Vendor") ? "Vendor" : "User";
            var result = await _messageRepositry.Messages.GetConversationAsync(userId!, userRole, otherUserId, orderId);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = User.FindFirst("sub")?.Value;
            var userRole = User.IsInRole("Vendor") ? "Vendor" : "User";
            var result = await _messageRepositry.Messages.GetUserConversationsAsync(userId!, userRole);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPut("read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userId = User.FindFirst("sub")?.Value;
            var result = await _messageRepositry.Messages.MarkMessageAsReadAsync(messageId, userId);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }
    }
}
