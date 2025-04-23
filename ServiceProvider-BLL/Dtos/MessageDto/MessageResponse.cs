using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.MessageDto
{
    public record MessageResponse(
       int Id,
       string MessageText,
       DateTime MessageDate,
       bool IsRead,
       string SenderId,
       string SenderName,
       string SenderRole,
       string ReceiverId,
       string ReceiverName,
       int OrderId
    );
}
