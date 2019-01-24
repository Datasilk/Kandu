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
                new Dictionary<string, object>()
                {
                    {"listId", card.listId },
                    {"boardId", card.boardId },
                    {"colors", card.colors },
                    {"name", card.name },
                    {"datedue", card.datedue == null ? DateTime.Now.AddYears(-100) : card.datedue },
                    {"description", card.description }
                }
            );
        }

        public static int Import(Models.Card card, bool merge = false)
        {
            return Sql.ExecuteScalar<int>(
                "Card_Import",
                new Dictionary<string, object>()
                {
                    {"listId", card.listId },
                    {"boardId", card.boardId },
                    {"colors", card.colors },
                    {"name", card.name },
                    {"datedue", card.datedue == null ? DateTime.Now.AddYears(-100) : card.datedue },
                    {"description", card.description },
                    {"merge", merge }
                }
            );
        }

        public static void Archive(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Archive",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId }
                }
            );
        }

        public static Models.Card GetDetails(int boardId, int cardId)
        {
            var list = Sql.Populate<Models.Card>(
                "Card_GetDetails",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId }
                }
            );
            if(list.Count > 0) { return list[0]; }
            return null;
        }

        public static void Restore(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Restore",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId }
                }
            );
        }

        public static void Delete(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Delete",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId }
                }
            );
        }

        public static List<Models.Card> GetList(int boardId, int listId = 0, int start = 1, int length = 2000)
        {
            return Sql.Populate<Models.Card>(
                "Card_GetList",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"listId", listId },
                    {"start", start },
                    {"length", length }
                }
            );
        }

        public static void Move(int boardId, int listId, int cardId, int[] cardIds)
        {
            Sql.ExecuteNonQuery("Card_Move",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"listId", listId },
                    {"cardId", cardId },
                    {"ids", string.Join(",", cardIds) }
                }
            );
        }

        public static void UpdateName(int boardId, int cardId, string name)
        {
            Sql.ExecuteNonQuery("Card_UpdateName",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId },
                    {"name", name }
                }
            );
        }

        public static void UpdateDescription(int boardId, int cardId, string description)
        {
            Sql.ExecuteNonQuery("Card_UpdateDescription",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId },
                    {"description", description }
                }
            );
        }
    }
}
