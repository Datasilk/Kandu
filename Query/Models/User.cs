using System;

namespace Query.Models
{
    public class User
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool photo { get; set; }
        public bool active { get; set; }
        public DateTime datecreated { get; set; }
        public int lastboard { get; set; }
        public string lastboardName { get; set; }
        public bool keepmenu { get; set; }
        public bool allcolor { get; set; }
    }
}
