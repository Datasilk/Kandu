using System.Collections.Generic;

namespace Query.Models
{
    public class SecurityKey
    {
        public int orgId { get; set; }
        public int groupId { get; set; }
        public string key { get; set; }
        public bool enabled { get; set; }
        public int scope { get; set; }
        public int scopeId { get; set; } = 0;
        public string scopeType { get; set; } = "";
        public string scopeItem { get; set; } = "";
    }

    public class SecurityGroup
    {
        public int groupId { get; set; }
        public int orgId { get; set; }
        public string name { get; set; }
        public string orgName { get; set; }
        public int ownerId { get; set; }
        public int totalkeys { get; set; }
    }

    public class SecurityDetails: SecurityGroup
    {
        public List<SecurityKey> Keys { get; set; }
        public List<User> Users { get; set; }
    }
}
