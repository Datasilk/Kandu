using System;
using System.Collections.Generic;
using System.Text;

namespace Kandu.Query
{
    public class Cards : QuerySql
    {
        public Cards(string connectionString) : base(connectionString)
        {
        }

        public int Create(Models.Card card)
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

        public int Import(Models.Card card, bool merge = false)
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

        public void Archive(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Archive",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId }
                }
            );
        }

        public Models.Card GetDetails(int boardId, int cardId)
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

        public void Restore(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Restore",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId }
                }
            );
        }

        public void Delete(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Delete",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"cardId", cardId }
                }
            );
        }

        public List<Models.Card> GetList(int boardId, int listId = 0, int start = 1, int length = 2000)
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

        public void Move(int boardId, int listId, int cardId, int[] cardIds)
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

        public void UpdateDescription(int boardId, int cardId, string description)
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
