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

        public static void ClearVendorClients()
        {
            _vendorClients = null;
        }

        private static List<EmailAction> _emailtypes { get; set; }
        public static List<EmailAction> VendorActions
        {
            get
            {
                if(_emailtypes == null)
                {
                    _emailtypes  = new List<EmailAction>(Actions);
                    _emailtypes.AddRange(Core.Vendors.EmailTypes.Values);
                }
                return _emailtypes;
            }
        }

        public static EmailAction VendorAction(string action)
        {
            return VendorActions.Where(a => a.Key == action).FirstOrDefault();
        }

        public static View GetMessage(string key)
        {
            var action = VendorAction(key);
            return new View(action.TemplateFile);
        }

        public static Query.Models.EmailAction GetInfo(string action)
        {
            return Query.EmailActions.GetInfo(action);
        }

        private static Dictionary<string, Query.Models.EmailClientAction> _actions { get; set; }
        public static Query.Models.EmailClientAction GetActionConfig(string type)
        {
            if(_actions == null)
            {
                _actions = Query.EmailActions.GetList().ToDictionary(a => a.action, a => a);
            }
            return _actions[type] ?? null;
        }
        public static void ClearActions()
        {
            if (_actions != null) { _actions.Clear(); }
        }

        public static void Send(string from, string to, string subject, string body, string action)
        {
            Send(new MailMessage(from, to, subject, body), GetActionConfig(action));
        }

        public static void Send(MailMessage message, string action)
        {
            Send(message, GetActionConfig(action));
        }

        public static void SendMany(MailMessage[] messages, string action)
        {
            SendMany(messages, GetActionConfig(action));
        }

        public static void SendMany(MailMessage[] messages, Query.Models.EmailClientAction clientAction)
        {
            if (clientAction == null)
            {
                //log error, could not send email
                Query.Logs.LogError(0, "", "Email.Send", "Could not find Email Action \"" + clientAction.action + "\"", "");
                return;
            }
            var client = VendorClients.Where(a => a.Key == clientAction.key).FirstOrDefault();
            if (client == null)
            {
                //log error, could not send email
                Query.Logs.LogError(0, "", "Email.Send", "Could not find Email Client \"" + clientAction.key + "\"", "");
                return;
            }
            var _msg = "";
            client.SendMany(clientAction.config, messages, delegate (MailMessage message) {
                //only get RFC 2822 message if vendor plugin specifically requests it
                if (string.IsNullOrEmpty(_msg))
                {
                    _msg = GetRFC2822FormattedMessage(message);
                }
                return _msg;
            });
        }

        public static void Send(MailMessage message, Query.Models.EmailClientAction clientAction)
        {
            if (clientAction == null)
            {
                //log error, could not send email
                Query.Logs.LogError(0, "", "Email.Send", "Could not find Email Action \"" + clientAction.action + "\"", "");
                return;
            }
            var client = VendorClients.Where(a => a.Key == clientAction.key).FirstOrDefault();
            if (client == null)
            {
                //log error, could not send email
                Query.Logs.LogError(0, "", "Email.Send", "Could not find Email Client \"" + clientAction.key + "\"", "");
                return;
            }
            var _msg = "";
            client.Send(clientAction.config, message, delegate () {
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

        private static readonly List<EmailAction> Actions = new List<EmailAction>()
        {
            new EmailAction()
            {
                Key = "signup",
                Name = "Sign Up",
                Description = "",
                TemplateFile = "/Content/temp/emails/signup.html",
                UserDefinedSubject = true,
                UserDefinedBody = true,
                DefaultSubject = "{{app-name}} email verification"
            },
            new EmailAction()
            {
                Key="updatepass",
                Name = "Update Password",
                Description = "",
                TemplateFile = "/Content/temp/emails/update-pass.html",
                UserDefinedSubject = true,
                UserDefinedBody = true,
                DefaultSubject = "Reset your {{app-name}} account password"
            },
            new EmailAction()
            {
                Key="invite",
                Name = "Send Invitation",
                Description = "",
                TemplateFile = "/Content/temp/emails/invite.html",
                UserDefinedSubject = true,
                UserDefinedBody = true,
                DefaultSubject = "You are invited to participate in {{app-name}}"
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
                        Description = "The remote domain name or IP address where your email server resides.",
                        Placeholder = "mail.myserver.com"
                    }
                },
                {
                    "port",
                    new EmailClientParameter()
                    {
                        Name = "Port",
                        DataType = EmailClientDataType.Number,
                        Description = "Port number where your email server resides. Default is port 587.",
                        Placeholder = "587"
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
                        int.Parse(config["port"]) : 0, true);//config.ContainsKey("ssl") && config["ssl"] == "1");

                    //disable the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(config["user"], config["pass"]);
                    client.Send(msg);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Query.Logs.LogError(0, "", "Email.Smtps.Send", ex.Message, ex.StackTrace);
                    throw new Exception("Could not send message to " + message.To);
                }
            }

            public void SendMany(Dictionary<string, string> config, MailMessage[] messages, Func<MailMessage, string> GetRFC2822)
            {
                try
                {
                    //connect to email server
                    var client = new MailKit.Net.Smtp.SmtpClient();
                    client.Connect(config["domain"], config.ContainsKey("port") && config["port"] != "" ?
                        int.Parse(config["port"]) : 0, true);//config.ContainsKey("ssl") && config["ssl"] == "1");
                    //disable the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(config["user"], config["pass"]);

                    //send all messages
                    foreach (var message in messages)
                    {
                        try
                        {
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
                            client.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            Query.Logs.LogError(0, "", "Email.Smtps.Send", ex.Message, ex.StackTrace);
                            throw new Exception("Could not send message to " + message.To);
                        }
                    }
                    
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Query.Logs.LogError(0, config["domain"] + ":" + config["port"], "Email.Smtps.Send", ex.Message, ex.StackTrace);
                    throw new Exception("Could not connect to " + config["domain"] + ":" + config["port"]);
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
                        Description = "The remote domain name or IP address where your email server resides.",
                        Placeholder = "mail.myserver.com"
                    }
                },
                {
                    "port",
                    new EmailClientParameter()
                    {
                        Name = "Port",
                        DataType = EmailClientDataType.Number,
                        Description = "Port number where your email server resides. Default is port 587.",
                        Placeholder = "587"
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
                catch(Exception ex)
                {
                    Query.Logs.LogError(0, "", "Email.Smtp.Send", ex.Message, ex.StackTrace);
                }
             }

            public void SendMany(Dictionary<string, string> config, MailMessage[] messages, Func<MailMessage, string> GetRFC2822)
            {
                throw new NotImplementedException();
            }
        }
    }
}
