using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MimeKit;
using Kandu.Vendor;

namespace Kandu.Common
{
    public static class Email
    {
        public static List<IVendorEmailClient> Clients { get; set; } = new List<IVendorEmailClient>()
        {
            new Smtp()
        };

        public static void Send(MailMessage message, string type)
        {
            var config = Settings.Load();
            var action = config.Email.Actions.Where(a => a.Type == type).FirstOrDefault();
            if(action == null)
            {
                //log error, could not send email
                //Query.Logs.LogError(0, "", "Email.Send", "Could not find Email Action Type \"" + type + "\"", "");
                return;
            }
            var client = Clients.Where(a => a.Key == action.Client).FirstOrDefault() ??
                Core.Vendors.EmailClients.Values.Where(a => a.Key == action.Client).FirstOrDefault();
            if (client == null)
            {
                //log error, could not send email
                //Query.Logs.LogError(0, "", "Email.Send", "Could not find Email Client \"" + action.Client + "\"", "");
                return;
            }
            var _msg = "";
            client.Send(message, delegate() {
                //only get RFC 2822 message if vendor plugin specifically requests it
                if (string.IsNullOrEmpty(_msg))
                {
                    _msg = GetRFC2822FormattedMessage(message);
                }
                return _msg; 
            });
        }

        public static string GetRFC2822FormattedMessage(MailMessage message)
        {
            return MimeMessage.CreateFromMailMessage(message).ToString();
        }

        public static List<EmailType> Types = new List<EmailType>()
        {
            new EmailType()
            {
                Key = "signup",
                Name = "Sign Up",
                Description = "",
                TemplateFile = "signup.html",
                UserDefinedSubject = true
            },
            new EmailType()
            {
                Key="updatepass",
                Name = "Update Password",
                Description = "",
                TemplateFile = "update-pass.html",
                UserDefinedSubject = true
            }
        };

        public class Smtp : IVendorEmailClient
        {
            public string Key { get; set; } = "smtp";
            public string Name { get; set; } = "SMTP Server";
            public Dictionary<string, EmailClientParameter> Parameters { get; set; } = new Dictionary<string, EmailClientParameter>()
            {
                {
                    "domain", 
                    new EmailClientParameter()
                    {
                        Name = "Host",
                        DataType = EmailClientDataType.Text,
                        Description = "The remote domain name or IP address where your email server resides."
                    }
                },
                {
                    "port",
                    new EmailClientParameter()
                    {
                        Name = "Port",
                        DataType = EmailClientDataType.Number,
                        Description = "Port number where your email server resides. Default is port 25."
                    }
                },
                {
                    "ssl",
                    new EmailClientParameter()
                    {
                        Name = "Use SSL",
                        DataType = EmailClientDataType.Boolean,
                        Description = "Whether or not the connection to your email server uses SSL."
                    }
                },
                {
                    "from",
                    new EmailClientParameter()
                    {
                        Name = "From Address",
                        DataType = EmailClientDataType.Text,
                        Description = "The email address used to send emails to your users with on behalf of your website."
                    }
                },
                {
                    "from-name",
                    new EmailClientParameter()
                    {
                        Name = "From Name",
                        DataType = EmailClientDataType.Text,
                        Description = "The name of the person sending emails on behalf of your website."
                    }
                },
                {
                    "user",
                    new EmailClientParameter()
                    {
                        Name = "Username / Email",
                        DataType = EmailClientDataType.Text,
                        Description = "The username or email used to authenticate before sending an email via SMTP."
                    }
                },
                {
                    "pass",
                    new EmailClientParameter()
                    {
                        Name = "Password",
                        DataType = EmailClientDataType.Password,
                        Description = "The password used to authenticate before sending an email via SMTP."
                    }
                }
            };

            public Dictionary<string, string> GetConfig()
            {
                var config = Settings.Load();
                return new Dictionary<string, string>()
                {
                    { "domain", config.Email.Smtp.Domain },
                    { "port", config.Email.Smtp.Port.ToString() },
                    { "ssl", config.Email.Smtp.SSL ? "1" : "0" },
                    { "from", config.Email.Smtp.From },
                    { "from-name", config.Email.Smtp.FromName },
                    { "user", config.Email.Smtp.Username },
                    { "pass", config.Email.Smtp.Password }
                };
            }

            public void Init()
            {
                
            }

            public void SaveConfig(Dictionary<string, string> parameters)
            {
                var config = Settings.Load();
                int.TryParse(parameters["port"], out var port);
                var ssl = parameters["ssl"] ?? "";
                var pass = parameters["pass"] ?? "";
                config.Email.Smtp.Domain = parameters["domain"] ?? "";
                config.Email.Smtp.Port = port;
                config.Email.Smtp.SSL = ssl.ToLower() == "true";
                config.Email.Smtp.From = parameters["from"];
                config.Email.Smtp.FromName = parameters["from-name"];
                config.Email.Smtp.Username = parameters["user"];
                if (pass != "" && pass.Any(a => a != '*'))
                {
                    config.Email.Smtp.Password = parameters["pass"];
                }
                Settings.Save(config);
            }

            public void Send(MailMessage message, Func<string> GetRFC2822)
            {
                try
                {
                    var config = Settings.Load().Email.Smtp;
                    var client = new MailKit.Net.Smtp.SmtpClient();
                    var msg = new MimeMessage();
                    msg.From.Add(new MailboxAddress(config.FromName, config.From));
                    foreach(var to in message.To)
                    {
                        msg.To.Add(new MailboxAddress(to.DisplayName, to.Address));
                    }
                    msg.Subject = message.Subject;

                    msg.Body = new TextPart("html")
                    {
                        Text = message.Body
                    };

                    client.Connect(config.Domain, config.Port, config.SSL);
                    //disable the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(config.Username, config.Password);
                    client.Send(msg);
                    client.Disconnect(true);
                }
                catch(Exception ex)
                {
                    //Query.Logs.LogError(0, "", "Email.Smtp.Send", ex.Message, ex.StackTrace);
                }
             }
        }
    }
}
