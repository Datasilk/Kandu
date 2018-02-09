using System;
using System.Collections.Generic;

namespace Kandu.Services
{
    public class Lists : Service
    {
        public Lists(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public string Create(int boardId, string name, int sort = 0)
        {
            //check security
            if (!UserInfo.CheckSecurity(boardId)) { return AccessDenied(); }

            var query = new Query.Lists(S.Server.sqlConnectionString);
            try
            {
                var id = query.CreateList(
                    new Query.Models.List()
                    {
                        boardId=boardId,
                        name=name,
                        sort=sort
                    }
                );
                var boards = new Query.Boards(S.Server.sqlConnectionString);
                var board = boards.GetDetails(boardId);
                var html = "";
                switch (board.type)
                {
                    case 0: //Kanban
                        var kanban = new List.Kanban(S);
                        html = kanban.LoadListHtml(
                            new Query.Models.List() { boardId = boardId, name = name, sort = sort, listId = id }, 
                            new List<Query.Models.Card>()
                        );
                        break;
                }
                return Success() + "|" + html;
            }catch(Exception)
            {
                return Error();
            }
        }
    }
}
