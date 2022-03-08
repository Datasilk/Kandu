using System;

namespace Query.Models
{
    public class Invitation
    {
        public int userId { get; set; }
        public int scopeId { get; set; }
        public Kandu.Models.Scope scope { get; set; }
        public string email { get; set; }
        public string publickey { get; set; }
        public string keys { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime? accepted { get; set; }
        public string message { get; set; }
    }

    public class FailedInvite
    {
        public string email { get; set; }
        public string name { get; set; }
    }
}
