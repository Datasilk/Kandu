using System;
using System.Collections.Generic;

namespace Kandu.Query.Models
{
    public class Card
    {
        public enum CardType: int
        {
            _default = 0
        }

        public int cardId { get; set; }
        public int listId { get; set; }
        public int boardId { get; set; }
        public Board.BoardType boardType { get; set; }
        public int sort { get; set; }
        public CardType type { get; set; }
        public bool archived { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime? datedue { get; set; }
        public string name { get; set; }
        public string colors { get; set; }
        public string description { get; set; }
    }
}
