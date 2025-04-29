using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.MessageDto
{
    public record MessageRequest(
       string MessageText,
       string ReceiverId,
       int OrderId
    );

}
