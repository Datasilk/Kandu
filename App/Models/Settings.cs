using System.Collections.Generic;

namespace Kandu.Models
{
    public class Settings
    {
        public Email Email { get; set; } = new Email();
    }

    public class Email
    {
        public Smtp Smtp { get; set; } = new Smtp();
        public List<EmailAction> Actions { get; set; } = new List<EmailAction>()
        {
            new EmailAction() { Type = "signup", Subject = "Welcome to Kandu!" },
            new EmailAction() { Type = "forgotpass", Subject = "Kandu Password Reset" },
            new EmailAction() { Type = "newsletter" }
        };
    }

    public class Smtp
    {
        public string Domain { get; set; } = "";
        public int Port { get; set; } = 25;
        public bool SSL { get; set; } = false;
        public string From { get; set; } = "";
        public string FromName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class EmailAction
    {
        public string Type { get; set; }
        public string Client { get; set; }
        public string Subject { get; set; }
    }
}
