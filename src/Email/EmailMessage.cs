using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AutomateCore.Email
{
    public class EmailMessage
    {
        public string From { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string ReplyTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public char Delimiter { get; set; }

        public EmailMessage()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            Attachments = new List<Attachment>();
            From = "";
            ReplyTo = "";
            Subject = "";
            Body = "";
            Delimiter = ';';
        }
        public void AddTo(string to)
        {
            if (string.IsNullOrWhiteSpace(to))
                return;

            if (Delimiter == default(char))
                throw new ArgumentException("Delimiter is not set.");

            var addresses = to.Split(Delimiter)
                              .Select(addr => addr.Trim())
                              .Where(addr => !string.IsNullOrWhiteSpace(addr));
            To.AddRange(addresses);
        }
        public void AddCc(string cc)
        {
            if (string.IsNullOrWhiteSpace(cc))
                return;

            if (Delimiter == default(char))
                throw new ArgumentException("Delimiter is not set.");

            var addresses = cc.Split(Delimiter)
                              .Select(addr => addr.Trim())
                              .Where(addr => !string.IsNullOrWhiteSpace(addr));

            Cc.AddRange(addresses);
        }
        public void AddBcc(string bcc)
        {
            if (string.IsNullOrWhiteSpace(bcc))
                return;

            if (Delimiter == default(char))
                throw new ArgumentException("Delimiter is not set.");

            var addresses = bcc.Split(Delimiter)
                               .Select(addr => addr.Trim())
                               .Where(addr => !string.IsNullOrWhiteSpace(addr));

            Bcc.AddRange(addresses);
        }
    }
}
