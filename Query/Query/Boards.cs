using System.Collections.Generic;
using System.Linq;

namespace Query
{
    public static class Boards
    {
        public static int CreateBoard(Models.Board board, int userId)
        {
            return Sql.ExecuteScalar<int>(
                "Board_Create",
                new {board.orgId, board.teamId, userId, board.name, board.favorite, board.color, board.cardtype }
            );
        }

        public static void UpdateBoard(Models.Board board)
        {
            Sql.ExecuteNonQuery(
                "Board_Update",
                new { board.boardId, board.orgId, board.name, board.color, board.cardtype }
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


        public enum BoardsSort
        {
            FavsFirst = 0,
            AtoZ = 1
        }
        public static List<Models.Board> GetList(int userId, int orgId = 0, BoardsSort sort = BoardsSort.FavsFirst)
        {
            return Sql.Populate<Models.Board>(
                "Boards_GetList", new { userId, orgId, sort = (int)sort }
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
                "Boards_MemberIsPartOf", new { userId }
            );
        }

        public static int Import(Models.Board board, int userId, bool merge = false)
        {
            return Sql.ExecuteScalar<int>(
                "Board_Import",
                new { board.orgId, board.teamId, userId, board.name, board.favorite, board.color, merge }
            );
        }

        public static void Favorite(int boardId, int userId)
        {
            Sql.ExecuteNonQuery("Board_Favorite", new { boardId, userId });
        }

        public static void Unfavorite(int boardId, int userId)
        {
            Sql.ExecuteNonQuery("Board_Unfavorite", new { boardId, userId });
        }
    }
}
