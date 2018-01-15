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

        public int CreateCard(Models.Card card)
        {
            return Sql.ExecuteScalar<int>(
                "Card_Create",
                new Dictionary<string, object>()
                {
                    {"listId", card.listId },
                    {"boardId", card.boardId },
                    {"colors", card.colors },
                    {"name", card.name },
                    {"datedue", card.datedue },
                    {"description", card.description }
                }
            );
        }

        public void ArchiveCard(int cardId)
        {
            Sql.ExecuteNonQuery("Card_Archive",
                new Dictionary<string, object>()
                {
                    {"cardId", cardId }
                }
            );
        }

        public Models.Card GetCardDetails(int cardId)
        {
            var list = Sql.Populate<Models.Card>(
                "Card_GetDetails",
                new Dictionary<string, object>()
                {
                    {"cardId", cardId }
                }
            );
            if(list.Count > 0) { return list[0]; }
            return null;
        }

        public void RestoreCard(int cardId)
        {
            Sql.ExecuteNonQuery("Card_Restore",
                new Dictionary<string, object>()
                {
                    {"cardId", cardId }
                }
            );
        }

        public List<Models.Card> GetList(int boardId, int listId = 0, int start = 1, int length = 20)
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

        public void Sort(int listId, int[] cardIds)
        {
            Sql.ExecuteNonQuery("Cards_Sort",
                new Dictionary<string, object>()
                {
                    {"listId", listId },
                    {"ids", string.Join(",", cardIds) }
                }
            );
        }
    }
}
