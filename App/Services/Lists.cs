using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Kandu.Services
{
    public class Lists : Service
    {
        public Lists(HttpContext context) : base(context)
        {
        }

        public string Create(int boardId, string name, int sort = 0)
        {
            //check security
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }

            var boards = new Query.Boards();
            var board = boards.GetDetails(boardId);
            int id;
            try
            {
                id = Common.Platform.Lists.Create(boardId, name, sort);
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
            switch (board.type)
            {
                case 0: //Kanban;
                    return Common.Platform.List.Kanban.RenderList(
                        new Query.Models.List() { boardId = boardId, name = name, sort = sort, listId = id },
                        new List<Query.Models.Card>()
                    );
            }
            return Error();
        }

        public string Archive(int boardId, int listId)
        {
            //check security
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }

            try
            {
                Common.Platform.Lists.Archive(listId);
            }
            catch (ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
            return Success();
        }
    }
}
