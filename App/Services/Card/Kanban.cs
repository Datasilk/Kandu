using Microsoft.AspNetCore.Http;

namespace Kandu.Services.Card
{
    public class Kanban : Service
    {

        public Kanban(HttpContext context) : base(context)
        {
        }

        public string LoadCardHtml(Query.Models.Card card)
        {
            if (!CheckSecurity()) { return AccessDenied(); }
            return Common.Platform.Card.Kanban.RenderCard(card);
        }

        public string Details(int boardId, int cardId)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            try
            {
                var results = Common.Platform.Card.Kanban.Details(boardId, cardId);
                return results.Item1.name + "|" + results.Item2;
            }
            catch (ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
        }

        public string Move(int boardId, int listId, int cardId, int[] cardIds)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            var query = new Query.Cards();
            query.Move(boardId, listId, cardId, cardIds);
            return Success();
        }
    }
}
