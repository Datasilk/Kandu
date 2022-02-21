using System;
using System.Collections.Generic;
using System.Linq;

namespace Query
{
    public static class Cards
    {
        public static int Create(Models.Card card)
        {
            return Sql.ExecuteScalar<int>(
                "Card_Create",
                new
                {
                    card.listId,
                    card.boardId,
                    card.layout,
                    card.colors,
                    card.name,
                    card.type,
                    datedue = card.datedue == null ? DateTime.Now.AddYears(-100) : card.datedue,
                    card.description,
                    card.json
                }
            );
        }

        public static int Import(Models.Card card, bool merge = false)
        {
            return Sql.ExecuteScalar<int>(
                "Card_Import",
                new
                {
                    card.listId,
                    card.boardId,
                    card.layout,
                    card.colors,
                    card.name,
                    card.type,
                    datedue = card.datedue == null ? DateTime.Now.AddYears(-100) : card.datedue,
                    card.description,
                    card.json,
                    merge
                }
            );
        }

        public static void Archive(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Archive",new { boardId, cardId });
        }

        public static Models.CardDetails GetDetails(int boardId, int cardId)
        {
            using (var conn = new Connection("Card_GetDetails", new { boardId, cardId }))
            {
                var reader = conn.PopulateMultiple();
                var details = reader.ReadFirst<Models.CardDetails>();
                if(details != null)
                {
                    details.labels = reader.Read<Models.Label>().ToList();
                    details.checklist = reader.Read<Models.CardChecklistItem>().ToList();
                    details.comments = reader.Read<Models.CardComment>().ToList();
                }
                return details;
            }
        }

        public static void Restore(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Restore",new { boardId, cardId });
        }

        public static void Delete(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Delete",new { boardId, cardId });
        }

        public static Models.Card GetInfo(int cardId)
        {
            return Sql.Populate<Models.Card>("Card_GetInfo", new { cardId }).FirstOrDefault();
        }

        public static List<Models.Card> GetList(int boardId, int listId = 0, int start = 1, int length = 2000, bool archivedOnly = false)
        {
            return Sql.Populate<Models.Card>("Cards_GetList",new { boardId, listId, start, length, archivedOnly });
        }

        public static List<Models.CardDetails> AssignedToMember(int userId, int orgId = 0, int start = 1, int length = 20, bool archivedOnly = false)
        {
            return Sql.Populate<Models.CardDetails>(
                "Cards_AssignedToMember",
                new { userId, orgId, start, length, archivedOnly }
            );
        }

        public static List<Models.CardMember> Members(int cardId)
        {
            return Sql.Populate<Models.CardMember>("Card_GetMembers", new { cardId }).Distinct().ToList();
        }

        public static void Move(int boardId, int listId, int cardId, int[] cardIds)
        {
            Sql.ExecuteNonQuery("Card_Move",new { boardId, listId, cardId, ids = string.Join(",", cardIds) });
        }

        public static void UpdateName(int boardId, int cardId, string name)
        {
            Sql.ExecuteNonQuery("Card_UpdateName",new { boardId, cardId, name });
        }

        public static void UpdateDescription(int boardId, int cardId, string description)
        {
            Sql.ExecuteNonQuery("Card_UpdateDescription",new { boardId, cardId, description });
        }

        public static void UpdateJson(int boardId, int cardId, string json)
        {
            Sql.ExecuteNonQuery("Card_UpdateJson",new { boardId, cardId, json });
        }

        public static void UpdateAssignedTo(int cardId, int userId, int userIdAssigned)
        {
            Sql.ExecuteNonQuery("Card_UpdateAssignedTo",new { cardId, userId, userIdAssigned });
        }

        public static void UpdateDueDate(int cardId, int userId, DateTime? duedate)
        {
            Sql.ExecuteNonQuery("Card_UpdateDueDate",new { cardId, userId, duedate });
        }

        public static int AddComment(int cardId, int userId, string comment)
        {
            return Sql.ExecuteScalar<int>("Card_Comment_Add", new { cardId, userId, comment });
        }

        public static void UpdateComment(int commentId, int cardId, int userId, string comment)
        {
            Sql.ExecuteNonQuery("Card_Comment_Update", new { commentId, cardId, userId, comment });
        }

        public static void RemoveComment(int commentId, int cardId, int userId)
        {
            Sql.ExecuteNonQuery("Card_Comment_Remove", new { commentId, cardId, userId });
        }

        public static Models.CardComment GetComment(int cardId, int commentId)
        {
            return Sql.Populate<Models.CardComment>("Card_Comment_Get", new { cardId, commentId }).FirstOrDefault();
        }
    }
}
