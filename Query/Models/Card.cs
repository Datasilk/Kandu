using System;

namespace Query.Models
{
    public class Card
    {
        public enum CardLayout : int
        {
            generic = 0,
            custom = 1
        }

        public int cardId { get; set; }
        public int listId { get; set; }
        public int boardId { get; set; }
        public int sort { get; set; }
        public CardLayout layout { get; set; }
        public Board.BoardType boardType { get; set; }
        public bool archived { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime? datedue { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string colors { get; set; }
        public string description { get; set; }
        public string listName { get; set; }
        public bool listArchived { get; set; }
        public string json { get; set; }
    }

    public class CardBoard: Card
    {
        public string boardColor { get; set; }
        public string boardName { get; set; }
    }
}
