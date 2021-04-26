using System;
using System.Collections.Generic;

namespace Query.Models
{
    public class Board
    {
        public enum BoardType: int
        {
            kanban = 0,
            timeline = 1
        }

        public int boardId { get; set; }
        public int teamId { get; set; }
        public int orgId { get; set; }
        public BoardType type { get; set; }
        public bool favorite { get; set; }
        public bool archived { get; set; }
        public string name { get; set; }
        public string orgName { get; set; }
        public string teamName { get; set; }
        public string teamDescription { get; set; }
        public string color { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime lastmodified { get; set; }
        public List<List> lists { get; set; }
    }
}
