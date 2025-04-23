using Microsoft.AspNetCore.Http;
using SeeviceProvider_BLL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Errors
{
    public static class MessageErrors
    {
        public static readonly Error SenderOrReceiverNotFound = new("Not Found", "Sender or receiver not found .", StatusCodes.Status404NotFound);
        public static readonly Error NoMessagesFound = new("Not Found", "No messages found .", StatusCodes.Status404NotFound);
        public static readonly Error UnAuthorized = new("UnAuthorized", "Unauthorized user or vendor .", StatusCodes.Status403Forbidden);
    }
}
