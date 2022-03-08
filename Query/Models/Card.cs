using System;
using System.Collections.Generic;

namespace Query.Models
{
    public enum CardLayout : int
    {
        generic = 0,
        custom = 1
    }

    public class Card
    {
        public int cardId { get; set; }
        public int listId { get; set; }
        public int boardId { get; set; }
        public int orgId { get; set; }
        public int userIdAssigned { get; set; }
        public int sort { get; set; }
        public CardLayout layout { get; set; }
        public Board.BoardType boardType { get; set; }
        public bool archived { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime? datedue { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string colors { get; set; }
        public string assignedName { get; set; }
        public string description { get; set; }
        public string listName { get; set; }
        public bool listArchived { get; set; }
        public string json { get; set; }
    }

    public class CardDetails: Card
    {
        public string boardColor { get; set; }
        public string boardName { get; set; }
        public List<Label> labels { get; set; }
        public List<CardChecklistItem> checklist { get; set; }
        public List<CardComment> comments { get; set; }
    }

    public class CardBoard: Card
    {
        public string boardName { get; set; }
    }

    public class CardChecklistItem
    {
        public int itemId { get; set; }
        public int sort { get; set; }
        public bool isChecked { get; set; }
        public string label { get; set; }
    }

    public class CardComment
    {
        public int commentId { get; set; }
        public int userId { get; set; }
        public DateTime dateCreated { get; set; }
        public string comment { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public bool photo { get; set; }
        public bool hasflagged { get; set; }
    }

    public class CardMember
    {
        public int userId { get; set; }
        public string name { get; set; }
        public bool photo { get; set; }
    }
}
