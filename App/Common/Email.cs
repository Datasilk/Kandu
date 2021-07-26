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
            new Smtps(),
            new Smtp()
        };

        private static List<IVendorEmailClient> _vendorClients { get; set; }

        public static List<IVendorEmailClient> VendorClients { 
            get  {
                if(_vendorClients == null)
                {
                    _vendorClients = new List<IVendorEmailClient>();
                    _vendorClients.AddRange(Email.Clients);
                    _vendorClients.AddRange(Core.Vendors.EmailClients.Values.OrderBy(a => a.Name));
                }                
                return _vendorClients;
            } 
        }

        public static List<EmailType> VendorActions
        {
            get
            {
                var emailActions = new List<EmailType>();
                emailActions.AddRange(Types);
                emailActions.AddRange(Core.Vendors.EmailTypes.Values);
                return emailActions;
            }
        }

        private class Action : Query.Models.EmailClientAction
        {
            public EmailType VendorAction { get; set; }
        }

        private static Dictionary<string, Query.Models.EmailClientAction> _actions { get; set; }
        public static Query.Models.EmailClientAction GetActionConfig(string type)
        {
            if(_actions == null)
            {
                _actions = Query.EmailActions.GetList().ToDictionary(a => a.key, a => a);
            }
            if (_actions.ContainsKey(type)) { return _actions[type]; }
            return null;
        }


        public static void Send(MailMessage message, string type)
        {
            var action = GetActionConfig(type);
            if(action == null)
            {
                //log error, could not send email
                //Query.Logs.LogError(0, "", "Email.Send", "Could not find Email Action Type \"" + type + "\"", "");
                return;
            }
            var client = VendorClients.Where(a => a.Key == action.key).FirstOrDefault();
            if (client == null)
            {
                //log error, could not send email
                //Query.Logs.LogError(0, "", "Email.Send", "Could not find Email Client \"" + action.Client + "\"", "");
                return;
            }
            var _msg = "";
            client.Send(action.config, message, delegate() {
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

        private static readonly List<EmailType> Types = new List<EmailType>()
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

        public class Smtps : IVendorEmailClient
        {
            public string Key { get; set; } = "smtps";
            public string Name { get; set; } = "SMTPS Server";

            public void Init() { }

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

            public void Send(Dictionary<string, string> config, MailMessage message, Func<string> GetRFC2822)
            {
                try
                {
                    var client = new MailKit.Net.Smtp.SmtpClient();
                    var msg = new MimeMessage();
                    msg.From.Add(new MailboxAddress(message.From.DisplayName, message.From.Address));
                    foreach (var to in message.To)
                    {
                        msg.To.Add(new MailboxAddress(to.DisplayName, to.Address));
                    }
                    msg.Subject = message.Subject;

                    msg.Body = new TextPart("html")
                    {
                        Text = message.Body
                    };

                    client.Connect(config["domain"], config.ContainsKey("port") && config["port"] != "" ?
                        int.Parse(config["port"]) : 0, config["ssl"] == "1");

                    //disable the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(config["user"], config["pass"]);
                    client.Send(msg);
                    client.Disconnect(true);
                }
                catch (Exception)
                {
                    //Query.Logs.LogError(0, "", "Email.Smtp.Send", ex.Message, ex.StackTrace);
                }
            }
        }

        public class Smtp : IVendorEmailClient
        {
            public string Key { get; set; } = "smtp";
            public string Name { get; set; } = "SMTP Server";

            public void Init() { }

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
                },
                {
                    "ssl",
                    new EmailClientParameter()
                    {
                        Name = "Use SSL",
                        DataType = EmailClientDataType.Boolean,
                        Description = "Whether or not the connection to your email server uses SSL."
                    }
                }
            };

            public void Send(Dictionary<string, string> config, MailMessage message, Func<string> GetRFC2822)
            {
                try
                {
                    var client = new MailKit.Net.Smtp.SmtpClient();
                    var msg = new MimeMessage();
                    msg.From.Add(new MailboxAddress(message.From.DisplayName, message.From.Address));
                    foreach(var to in message.To)
                    {
                        msg.To.Add(new MailboxAddress(to.DisplayName, to.Address));
                    }
                    msg.Subject = message.Subject;

                    msg.Body = new TextPart("html")
                    {
                        Text = message.Body
                    };

                    client.Connect(config["domain"], config.ContainsKey("port") && config["port"] != "" ? 
                        int.Parse(config["port"]) : 0, config["ssl"] == "1");

                    //disable the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(config["user"], config["pass"]);
                    client.Send(msg);
                    client.Disconnect(true);
                }
                catch(Exception)
                {
                    //Query.Logs.LogError(0, "", "Email.Smtp.Send", ex.Message, ex.StackTrace);
                }
             }
        }
    }
}
