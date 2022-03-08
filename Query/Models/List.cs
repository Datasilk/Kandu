using System.Collections.Generic;

namespace Query.Models
{
    public class List
    {
        public int listId { get; set; }
        public int boardId { get; set; }
        public string name { get; set; }
        public int sort { get; set; }
        public string cardtype { get; set; }
        public List<Card> cards { get; set; }
    }

    public class ListBoard: List
    {
        public string boardName { get; set; }
    }
}
