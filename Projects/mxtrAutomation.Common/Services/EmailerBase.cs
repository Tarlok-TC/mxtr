using System.Collections.Generic;
using System.Net.Mail;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Common.Services
{
    public abstract class EmailerBase : IEmailer
    {
        public abstract SmtpClient SmtpClient { get; }

        public IEnumerable<string> BccRecipients { get; set; }

        public virtual bool SendEmail(string senderEmail, string senderName, IEnumerable<string> recipientEmails, string subject, string body, bool isBodyHtml)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            recipientEmails.ForEach(message.To.Add);

            if (!BccRecipients.IsNullOrEmpty())
                BccRecipients.ForEach(message.Bcc.Add);

            SmtpClient.Send(message);
            return true;
        }

        public virtual bool SendEmailWithAttachment(string senderEmail, string senderName, IEnumerable<string> recipientEmails, string subject, string body, bool isBodyHtml, Attachment attachment)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            message.Attachments.Add(attachment);

            recipientEmails.ForEach(message.To.Add);

            if (!BccRecipients.IsNullOrEmpty())
                BccRecipients.ForEach(message.Bcc.Add);

            SmtpClient.Send(message);
            return true;
        }
    }
}
