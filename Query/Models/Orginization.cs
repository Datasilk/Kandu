using System;

namespace Query.Models
{
    public class Orginization
    {
        public int orgId { get; set; }
        public int ownerId { get; set; }
        public string name { get; set; }
        public DateTime datecreated { get; set; }
        public string website { get; set; }
        public string description { get; set; }
        public bool banner { get; set; }
        public bool photo { get; set; }
        public bool enabled { get; set; }
        public bool isprivate { get; set; }
    }
}
