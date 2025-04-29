using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.MessageDto
{
    public class MessageRequestValidator : AbstractValidator<MessageRequest>
    {
        public MessageRequestValidator() 
        {
            RuleFor(m => m.MessageText).NotEmpty().MaximumLength(1500);
            RuleFor(m => m.ReceiverId).NotEmpty();
            RuleFor(m => m.OrderId).GreaterThan(0);
        }
    }
}
