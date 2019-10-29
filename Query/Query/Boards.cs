using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Query
{
    public static class Boards
    {
        public static int CreateBoard(Models.Board board, int userId)
        {
            return Sql.ExecuteScalar<int>(
                "Board_Create",
                new {userId, board.teamId, board.favorite, board.name, board.security, board.color }
            );
        }

        public static void UpdateBoard(Models.Board board)
        {
            Sql.ExecuteNonQuery(
                "Board_Update",
                new { board.boardId, board.teamId, board.name, board.color }
            );
        }

        public static Models.Board GetDetails(int boardId)
        {
            var list = Sql.Populate<Models.Board>(
                "Board_GetDetails", new { boardId }
            );
            if(list.Count > 0) { return list[0]; }
            return null;
        }

        public static Models.Board GetInfo(int boardId)
        {
            var list = Sql.Populate<Models.Board>(
                "Board_GetInfo", new { boardId }
            );
            if (list.Count > 0) { return list[0]; }
            return null;
        }

        public static List<Models.Board> GetList(int userId)
        {
            return Sql.Populate<Models.Board>(
                "Boards_GetList", new { userId }
            );
        }

        public static Models.Board GetBoardAndLists(int boardId)
        {
            using (var sql = new Connection("Board_GetLists", new { boardId }))
            {
                var readers = sql.PopulateMultiple();
                var boards = readers.Read<Models.Board>().ToList();
                var lists = readers.Read<Models.List>().ToList();
                var cards = readers.Read<Models.Card>().ToList();

                for (var x = 0; x < boards.Count; x++)
                {
                    boards[x].lists = new List<Models.List>();
                    for (var y = 0; y < lists.Count; y++)
                    {
                        lists[y].cards = cards.Where((a) => { return a.listId == lists[y].listId; }).ToList();
                        boards[x].lists.Add(lists[y]);
                    }
                }

                return boards[0];
            }
        }

        public static bool MemberExists(int userId, int boardId)
        {
            return Sql.ExecuteScalar<int>(
                "Board_MemberExists",
                new { userId, boardId }
            ) == 1;
        }

        public static List<int> GetBoardsForMember(int userId)
        {
            return Sql.Populate<int>(
                "BoardMember_GetBoards", new { userId }
            );
        }

        public static int Import(Models.Board board, int userId, bool merge = false)
        {
            return Sql.ExecuteScalar<int>(
                "Board_Import",
                new { userId, board.teamId, board.favorite, board.name, board.security, board.color, merge }
            );
        }
    }
}
