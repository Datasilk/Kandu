using System.Collections.Generic;
using System.Linq;

namespace Kandu.Query
{
    public class Boards : QuerySql
    {
        public int CreateBoard(Models.Board board, int userId)
        {
            return Sql.ExecuteScalar<int>(
                "Board_Create",
                new Dictionary<string, object>()
                {
                    {"userId", userId },
                    {"teamId", board.teamId },
                    {"favorite", board.favorite },
                    {"name", board.name },
                    {"security", board.security },
                    {"color", board.color }
                }
            );
        }

        public void UpdateBoard(Models.Board board)
        {
            Sql.ExecuteNonQuery(
                "Board_Update",
                new Dictionary<string, object>()
                {
                    {"boardId", board.boardId },
                    {"teamId", board.teamId },
                    {"name", board.name },
                    {"color", board.color }
                }
            );
        }

        public Models.Board GetDetails(int boardId)
        {
            var list = Sql.Populate<Models.Board>(
                "Board_GetDetails",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId }
                }
            );
            if(list.Count > 0) { return list[0]; }
            return null;
        }

        public Models.Board GetInfo(int boardId)
        {
            var list = Sql.Populate<Models.Board>(
                "Board_GetInfo",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId }
                }
            );
            if (list.Count > 0) { return list[0]; }
            return null;
        }

        public List<Models.Board> GetList(int userId)
        {
            return Sql.Populate<Models.Board>(
                "Boards_GetList",
                new Dictionary<string, object>()
                {
                    {"userId", userId }
                }
            );
        }

        public Models.Board GetBoardAndLists(int boardId)
        {
            var readers = Sql.PopulateMultiple(
                "Board_GetLists",
                new Dictionary<string, object>()
                {
                    {"boardId", boardId }
                }
            );

            var boards = readers.Read<Models.Board>().ToList();
            var lists = readers.Read<Models.List>().ToList();
            var cards = readers.Read<Models.Card>().ToList();

            for(var x = 0; x < boards.Count; x++)
            {
                boards[x].lists = new List<Models.List>();
                for (var y = 0; y < lists.Count; y++)
                {
                    lists[y].cards = cards.Where((a) => { return a.listId == lists[y].listId; }).ToList();
                    boards[x].lists.Add(lists[y]);
                }
            }
            Sql.EndQuery();
            return boards[0];
        }

        public bool MemberExists(int userId, int boardId)
        {
            return Sql.ExecuteScalar<int>(
                "Board_MemberExists",
                new Dictionary<string, object>()
                {
                    {"userId", userId },
                    {"boardId", boardId }
                }
            ) == 1;
        }

        public List<int> GetBoardsForMember(int userId)
        {
            return Sql.Populate<int>(
                "BoardMember_GetBoards",
                new Dictionary<string, object>()
                {
                    {"userId", userId }
                }
            );
        }

        public int Import(Models.Board board, int userId, bool merge = false)
        {
            return Sql.ExecuteScalar<int>(
                "Board_Import",
                new Dictionary<string, object>()
                {
                    {"userId", userId },
                    {"teamId", board.teamId },
                    {"favorite", board.favorite },
                    {"name", board.name },
                    {"security", board.security },
                    {"color", board.color },
                    {"merge", merge }
                }
            );
        }
    }
}
