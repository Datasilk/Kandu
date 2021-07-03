using System;
using System.Net.Mail;

namespace Kandu.Core
{
    public static class Email
    {

        public static MailAddress From { get; set; } = new MailAddress("kandu@datasilk.io", "Kandu");

        /// <summary>
        /// Create a mail message used to be sent by Kandu. We do not define a From address since
        /// the preferred email client (Vendor.IVendorEmailClient) will supply a parameter for the 
        /// From address.
        /// </summary>
        /// <param name="to">The email address of the user you wish to send your email to</param>
        /// <param name="subject">The email subject line. This might be replaced before sending the email
        /// if the email message type used (Vendor.EmailType) allows a user-defined subject line.</param>
        /// <param name="body">The email HTML body</param>
        /// <param name="attachments">The relative path to any file attachments you wish to include. 
        /// If you want your email body to display image attachments, include an HTML <img/> tag with an 
        /// href that matches your attachment file path</param>
        /// <returns></returns>
        public static MailMessage Create(MailAddress from, MailAddress to, string subject, string body, string[] attachments = null)
        {
            var message = new MailMessage(from, to);
            if(attachments != null && attachments.Length > 0)
            {
                //include attachment data in message
            }
            message.Subject = subject;
            message.Body = body;
            return message;
        }

        /// <summary>
        /// Send an email using the preferred email client based on the type of email being sent
        /// </summary>
        /// <param name="message">The message being sent</param>
        /// <param name="type">The type of message to send. Known types are "signup" and "updatepass", 
        /// but vendor plugins can contain custom message types by using Vendor.IVendorEmails.</param>
        public static void Send(MailMessage message, string type)
        {
            Delegates.Email.Send(message, type);
        }
    }
}
