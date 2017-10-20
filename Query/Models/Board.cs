using System;

namespace Kandu.Query.Models
{
    public class Board
    {
        public int boardId { get; set; }
        public int teamId { get; set; }
        public bool favorite { get; set; }
        public bool archived { get; set; }
        public string name { get; set; }
        public short security { get; set; }
        public string color { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime lastmodified { get; set; }
    }
}
