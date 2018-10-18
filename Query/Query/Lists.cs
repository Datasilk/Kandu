using System.Collections.Generic;

namespace Query
{
    public static class Lists
    {
        public static int CreateList(Models.List list)
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

        public static int Import(Models.List list, bool merge = false)
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

        public static List<Models.List> GetListsForBoard(int boardId)
        {
            return Sql.Populate<Models.List>(
                "Lists_GetList",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId }
                }
            );
        }

        public static Models.List GetDetails(int listId)
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

        public static void Move(int boardId, int[] cardIds)
        {
            Sql.ExecuteNonQuery("List_Move",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId },
                    {"ids", string.Join(",", cardIds) }
                }
            );
        }

        public static void Archive(int listId)
        {
            Sql.ExecuteNonQuery("List_Archive",
                new Dictionary<string, object>()
                {
                    {"listId", listId }
                }
            );
        }
    }
}
