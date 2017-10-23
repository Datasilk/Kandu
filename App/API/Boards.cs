using System.Collections.Generic;

namespace Kandu.Services
{
    public class Boards : Service
    {
        public Boards(Core KanduCore) : base(KanduCore)
        {
        }

        public string Create(string name, string color, int teamId)
        {
            if (S.User.userId == 0) { return AccessDenied(); } //check security
            var query = new Query.Boards(S.Server.sqlConnection);
            query.CreateBoard(new Query.Models.Board()
            {
                name = name,
                security = 1,
                color = color,
                teamId = teamId
            });
            return Success();
        }

        public string Details(int boardId)
        {
            var query = new Query.Boards(S.Server.sqlConnection);
            var board = query.GetBoardDetails(boardId);
            return S.Util.Serializer.WriteObjectToString( 
                new Dictionary<string,Dictionary<string,string>>(){
                    {
                        "board", new Dictionary<string,string>(){
                            { "name", board.name },
                            { "color", "#" + board.color },
                            {"teamId", board.teamId.ToString() }
                        }
                    }
                }
            );
        }

        public string Update(int boardId, string name, string color, int teamId)
        {
            if (S.User.userId == 0) { return AccessDenied(); } //check security
            var query = new Query.Boards(S.Server.sqlConnection);

            //check if user has access to board
            if (!query.MemberExists(S.User.userId, boardId)) { return AccessDenied(); }
            
            //finally, update board
            query.UpdateBoard(new Query.Models.Board()
            {
                name = name,
                boardId = boardId,
                color = color,
                teamId = teamId
            });
            return Success();
        }


    }
}
