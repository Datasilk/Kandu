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

        public int Import(Models.List list, bool merge = false)
        {
            return Sql.ExecuteScalar<int>(
                "List_Import",
                new Dictionary<string, object>()
                {
                    {"boardId", list.boardId },
                    {"name", list.name },
                    {"sort", list.sort },
                    {"merge", merge }
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

        public Models.List GetDetails(int listId)
        {
            var lists = Sql.Populate<Models.List>(
                "Lists_GetDetails",
                new Dictionary<string, object>()
                {
                    {"listId", listId }
                }
            );
            if(lists.Count > 0) { return lists[0]; }
            return null;
        }
    }
}
