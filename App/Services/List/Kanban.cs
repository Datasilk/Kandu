using Microsoft.AspNetCore.Http;

namespace Kandu.Services.List
{
    public class Kanban: Service
    {
        public Kanban(HttpContext context) : base(context)
        {
        }

        public string LoadList(int listId)
        {
            var list = Query.Lists.GetDetails(listId);
            if (!User.CheckSecurity(list.boardId)) { return AccessDenied(); }
            return Common.Platform.List.Kanban.RenderList(list, Query.Cards.GetList(list.boardId, listId, 1, 100));
        }

        public string Move(int boardId, int[] listIds)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            Query.Lists.Move(boardId, listIds);
            return Success();
        }
    }
}
