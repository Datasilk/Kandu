using System;
using System.Collections.Generic;

namespace Kandu.Query
{
    public class Lists : QuerySql
    {
        public Lists(string connectionString) : base(connectionString)
        {
        }

        public int CreateList(Models.List list)
        {
            return Sql.ExecuteScalar<int>(
                "List_Create",
                new Dictionary<string, object>()
                {
                    {"boardId", list.boardId },
                    {"name", list.name },
                    {"sort", list.sort }
                }
            );
        }

        public List<Models.List> GetListsForBoard(int boardId)
        {
            return Sql.Populate<Models.List>(
                "Lists_GetList",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId }
                }
            );
        }
    }
}
