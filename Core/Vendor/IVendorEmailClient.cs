using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Kandu.Vendor
{
    /// <summary>
    /// Specify a custom email client that can be used to send emails to Kandu users, 
    /// such as signup authentication & forgotten password emails.
    /// </summary>
    public interface IVendorEmailClient
    {
        /// <summary>
        /// Used to identify this email client (e.g. "send-grid")
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Human-readable name for your email client (e.g. "Send Grid")
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A list of parameters used to configure the email client with. Include a parameter for your From address (if applicable)
        /// since emails sent using Kandu's email platform does not provide a way to include a From address.
        /// </summary>
        Dictionary<string, EmailClientParameter> Parameters { get; set; }

        /// <summary>
        /// Executed when Kandu starts running to give the email client a chance to do stuff
        /// </summary>
        void Init();

        /// <summary>
        /// Executed when Kandu is requesting to send an email via your email client
        /// </summary>
        /// <param name="config">the user configuration for this email client based on the given Parameters dictionary</param>
        /// <param name="message">Mail Message which includes all information about the email; to, from, subject, body, etc.</param>
        /// <param name="GetRFC2822">The RFC 2822 formatted email. Some clients require that it be Base64 URL encoded.</param>
        void Send(Dictionary<string, string> config, MailMessage message, Func<string> GetRFC2822);
    }
}
