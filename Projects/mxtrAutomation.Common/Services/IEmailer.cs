using System.Collections.Generic;
using System.Net.Mail;

namespace mxtrAutomation.Common.Services
{
    public interface IEmailer
    {
        bool SendEmail(string senderEmail, string senderName, IEnumerable<string> recipientEmails, string subject, string body, bool isBodyHtml);
        bool SendEmailWithAttachment(string senderEmail, string senderName, IEnumerable<string> recipientEmails, string subject, string body, bool isBodyHtml, Attachment attachment);
    }
}
