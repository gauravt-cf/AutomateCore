using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace AutomateCore.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public SmtpEmailSender(EmailSettings emailSettings)
        {
            if (emailSettings == null)
                throw new ArgumentNullException(nameof(emailSettings));

            _emailSettings = emailSettings;
        }

        public EmailSendResult Send(EmailMessage message)
        {
            if (message == null)
                return EmailSendResult.Fail("Email message cannot be null.");

            var validationError = ValidateMessage(message);
            if (!string.IsNullOrEmpty(validationError))
                return EmailSendResult.Fail(validationError);

            try
            {
                using (var mailMessage = BuildMailMessage(message))
                using (var smtpClient = BuildSmtpClient())
                {
                    try
                    {
                        smtpClient.Send(mailMessage);
                        return EmailSendResult.Ok();
                    }
                    catch (SmtpException ex) when (
                        ex.StatusCode == SmtpStatusCode.MailboxBusy ||
                        ex.StatusCode == SmtpStatusCode.MailboxUnavailable ||
                        ex.StatusCode == SmtpStatusCode.TransactionFailed)
                    {
                        Thread.Sleep(5000);
                        try
                        {
                            smtpClient.Send(mailMessage);
                            return EmailSendResult.Ok();
                        }
                        catch (Exception retryEx)
                        {
                            return EmailSendResult.Fail("Retry failed: " + retryEx.Message);
                        }
                    }
                    catch (SmtpException ex)
                    {
                        return EmailSendResult.Fail("SMTP error: " + ex.Message);
                    }
                    catch (FormatException ex)
                    {
                        return EmailSendResult.Fail("Invalid email address format: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        return EmailSendResult.Fail("Unexpected error: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return EmailSendResult.Fail("Critical failure: " + ex.Message);
            }
        }

        private string ValidateMessage(EmailMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.From))
                return "Sender email address is required.";

            if (message.To == null || message.To.Count == 0)
                return "At least one recipient (To) address is required.";

            if (string.IsNullOrWhiteSpace(message.Subject))
                return "Email subject is required.";

            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpHost))
                return "SMTP host is not configured.";
            return string.Empty;
        }

        private MailMessage BuildMailMessage(EmailMessage message)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(message.From),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                Priority = MailPriority.Normal
            };

            foreach (var to in message.To.Distinct())
                mail.To.Add(new MailAddress(to));
            if (message.Cc != null && message.Cc.Count > 0)
            {
                foreach (var cc in message.Cc.Distinct())
                    mail.CC.Add(new MailAddress(cc));
            }
            if (message.Bcc != null && message.Bcc.Count > 0)
            {
                foreach (var bcc in message.Bcc.Distinct())
                    mail.Bcc.Add(new MailAddress(bcc));
            }
            if (message.Attachments != null && message.Attachments.Count > 0)
            {
                foreach (var att in message.Attachments)
                    mail.Attachments.Add(att);
            }

            if (!string.IsNullOrWhiteSpace(message.ReplyTo))
                mail.ReplyToList.Add(new MailAddress(message.ReplyTo));

            return mail;
        }

        private SmtpClient BuildSmtpClient()
        {
            var client = new SmtpClient(_emailSettings.SmtpHost);
            return client;
        }
    }
}