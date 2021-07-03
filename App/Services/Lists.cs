using System.Collections.Generic;

namespace Kandu.Services
{
    public class Lists : Service
    {
        public string Create(int boardId, string name, int sort = 0)
        {
            //check security
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            
            var board = Query.Boards.GetDetails(boardId);
            int id;
            try
            {
                id = Common.Lists.Create(boardId, name, sort);
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
            switch (board.type)
            {
                case 0: //Kanban;
                    return Common.List.Kanban.RenderList(
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
                Common.Lists.Archive(listId);
            }
            catch (ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
            return Success();
        }
    }
}
