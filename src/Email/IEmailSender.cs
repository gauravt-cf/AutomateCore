using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Email
{
    public interface IEmailSender
    {
        EmailSendResult Send(EmailMessage message);
    }
}