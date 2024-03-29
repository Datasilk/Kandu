﻿using System.Collections.Generic;
using System.Linq;

namespace Query
{
    public static class Lists
    {
        public static int CreateList(Models.List list)
        {
            return Sql.ExecuteScalar<int>("List_Create",new { list.boardId, list.name, list.sort, list.cardtype });
        }

        public static int Import(Models.List list, bool merge = false)
        {
            return Sql.ExecuteScalar<int>("List_Import", new { list.boardId, list.name, list.sort, merge });
        }

        public static List<Models.List> GetListsForBoard(int boardId)
        {
            return Sql.Populate<Models.List>("Lists_GetList", new { boardId });
        }

        public static Models.List GetDetails(int listId)
        {
            return Sql.Populate<Models.List>("Lists_GetDetails", new { listId }).FirstOrDefault();
        }

        public static Models.ListBoard GetBoard(int listId)
        {
            return Sql.Populate<Models.ListBoard>("Lists_GetBoard", new { listId }).FirstOrDefault();
        }

        public static void Move(int boardId, int[] cardIds)
        {
            Sql.ExecuteNonQuery("List_Move",new { boardId, ids = string.Join(",", cardIds) });
        }

        public static void Archive(int listId)
        {
            Sql.ExecuteNonQuery("List_Archive", new { listId });
        }
    }
}
