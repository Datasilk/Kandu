using System.Collections.Generic;

namespace Kandu.Query.Models
{
    public class List
    {
        public int listId { get; set; }
        public int boardId { get; set; }
        public string name { get; set; }
        public int sort { get; set; }
        public List<Card> cards { get; set; }
    }
}
