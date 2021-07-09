using System;
using System.Collections.Generic;

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
            Sql.ExecuteNonQuery("Card_Archive",
                new { boardId, cardId }
            );
        }

        public static Models.CardBoard GetDetails(int boardId, int cardId)
        {
            var list = Sql.Populate<Models.CardBoard>(
                "Card_GetDetails",
                new { boardId, cardId }
            );
            if(list.Count > 0) { return list[0]; }
            return null;
        }

        public static void Restore(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Restore",
                new { boardId, cardId }
            );
        }

        public static void Delete(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Delete",
                new { boardId, cardId }
            );
        }

        public static List<Models.Card> GetList(int boardId, int listId = 0, int start = 1, int length = 2000, bool archivedOnly = false)
        {
            return Sql.Populate<Models.Card>(
                "Cards_GetList",
                new { boardId, listId, start, length, archivedOnly }
            );
        }

        public static List<Models.CardBoard> AssignedToMember(int userId, int orgId = 0, int start = 1, int length = 20, bool archivedOnly = false)
        {
            return Sql.Populate<Models.CardBoard>(
                "Cards_AssignedToMember",
                new { userId, orgId, start, length, archivedOnly }
            );
        }

        public static void Move(int boardId, int listId, int cardId, int[] cardIds)
        {
            Sql.ExecuteNonQuery("Card_Move",
                new { boardId, listId, cardId, ids = string.Join(",", cardIds) }
            );
        }

        public static void UpdateName(int boardId, int cardId, string name)
        {
            Sql.ExecuteNonQuery("Card_UpdateName",
                new { boardId, cardId, name }
            );
        }

        public static void UpdateDescription(int boardId, int cardId, string description)
        {
            Sql.ExecuteNonQuery("Card_UpdateDescription",
                new { boardId, cardId, description }
            );
        }

        public static void UpdateJson(int boardId, int cardId, string json)
        {
            Sql.ExecuteNonQuery("Card_UpdateJson",
                new { boardId, cardId, json }
            );
        }
    }
}
