using System;
using System.Net;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using mxtrAutomation.Common.Services;
using mxtrAutomation.Common.Utils;

namespace mxtrAutomation.Common.Services
{
    public class DevEmailer : EmailerBase
    {
        public override SmtpClient SmtpClient
        {
            get
            {
                return
                    new SmtpClient(ConfigManager.AppSettings["SMTPServer"], Convert.ToInt32(ConfigManager.AppSettings["SMTPPort"]))
                    {
                        Credentials = new NetworkCredential(ConfigManager.AppSettings["SMTPUsername"], ConfigManager.AppSettings["SMTPPassword"]),
                        EnableSsl = false
                    };
            }
            //get
            //{
            //    return
            //        new SmtpClient("smtp.gmail.com", 587)
            //        {
            //            Credentials = new NetworkCredential("noreply@marketsmart360.com", "aqp34nlr"),
            //            EnableSsl = true
            //        };
            //}
        }

        public DevEmailer()
        {
            BccRecipients = new[] { ConfigurationManager.AppSettings["DefaultBCCEmail"] };
        }

        public override bool SendEmail(string senderEmail, string senderName, IEnumerable<string> recipientEmails, string subject, string body, bool isBodyHtml)
        {
            try
            {
                base.SendEmail(senderEmail, senderName, recipientEmails, subject, body, isBodyHtml);
                return true;
            }
            catch (SmtpException e)
            {
                //Log4Net.Log(LogLevel.ERROR, "SendEmail() failed with status code {0}.  SendEmail(\"{0}\", \"{1}\", \"{2}\", \"{3}\", body, \"{5}\")".With(e.StatusCode, senderEmail, senderName, recipientEmails.ToString(","), subject, body, isBodyHtml));
                return false;
            }
            catch (Exception e)
            {
                //Log4Net.Log(LogLevel.ERROR, e, "Exception in SendEmail(\"{0}\",\"{1}\",\"{2}\",\"{3}\", body,\"{5}\")".With(senderEmail, senderName, recipientEmails.ToString(","), subject, body, isBodyHtml));
                return false;
            }
        }

        public override bool SendEmailWithAttachment(string senderEmail, string senderName, IEnumerable<string> recipientEmails, string subject, string body, bool isBodyHtml, Attachment attachment)
        {
            try
            {
                base.SendEmailWithAttachment(senderEmail, senderName, recipientEmails, subject, body, isBodyHtml, attachment);
                return true;
            }
            catch (SmtpException e)
            {
                //Log4Net.Log(LogLevel.ERROR, "SendEmailWithAttachment() failed with status code {0}.  SendEmail(\"{0}\", \"{1}\", \"{2}\", \"{3}\", body, \"{5}\")".With(e.StatusCode, senderEmail, senderName, recipientEmails.ToString(","), subject, body, isBodyHtml));
                return false;
            }
            catch (Exception e)
            {
                //Log4Net.Log(LogLevel.ERROR, e, "Exception in SendEmailWithAttachment(\"{0}\",\"{1}\",\"{2}\",\"{3}\", body,\"{5}\")".With(senderEmail, senderName, recipientEmails.ToString(","), subject, body, isBodyHtml));
                return false;
            }
        }
    }
}
