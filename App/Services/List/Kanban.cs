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
            var query = new Query.Lists();
            var list = query.GetDetails(listId);
            if (!User.CheckSecurity(list.boardId)) { return AccessDenied(); }
            var cards = new Query.Cards();
            return Common.Platform.List.Kanban.RenderList(list, cards.GetList(list.boardId, listId, 1, 100));
        }
    }
}
