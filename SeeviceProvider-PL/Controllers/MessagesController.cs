using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.MessageDto;
using ServiceProvider_BLL.Interfaces;
using System.Security.Claims;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController(IUnitOfWork messageRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _messageRepositry = messageRepositry;

        [HttpGet("conversation/{otherUserId}/{orderId}")]
        [ProducesResponseType(typeof(IEnumerable<MessageResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConversation(string otherUserId, int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.IsInRole("Vendor") ? "Vendor" : "User";
            var result = await _messageRepositry.Messages.GetConversationAsync(userId!, userRole, otherUserId, orderId);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("conversations")]
        [ProducesResponseType(typeof(IEnumerable<MessageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConversations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.IsInRole("Vendor") ? "Vendor" : "User";
            var result = await _messageRepositry.Messages.GetUserConversationsAsync(userId!, userRole);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPut("read/{messageId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _messageRepositry.Messages.MarkMessageAsReadAsync(messageId, userId!);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }
    }
}
