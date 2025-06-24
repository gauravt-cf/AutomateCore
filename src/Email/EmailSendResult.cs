using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Email
{
    public class EmailSendResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static EmailSendResult Ok()
        {
            return new EmailSendResult { Success = true };
        }

        public static EmailSendResult Fail(string error)
        {
            return new EmailSendResult { Success = false, ErrorMessage = error };
        }
    }
}
