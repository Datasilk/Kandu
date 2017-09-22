using System;

namespace Kandu.Query.Models
{
    class Team
    {
        public int teamId { get; set; }
        public int ownerId { get; set; }
        public bool security { get; set; }
        public string name { get; set; }
        public DateTime datecreated { get; set; }
        public string website { get; set; }
        public string description { get; set; }
    }
}
