using System;

namespace Query.Models
{
    public class Team
    {
        public int teamId { get; set; }
        public int orgId { get; set; }
        public string name { get; set; }
        public DateTime datecreated { get; set; }
        public string description { get; set; }
        public int totalMembers { get; set; }
        public int? groupId { get; set; } //default security group for new team members
    }
}
