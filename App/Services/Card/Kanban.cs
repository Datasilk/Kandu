namespace Kandu.Services.Card
{
    public class Kanban : Service
    {
        public string LoadCardHtml(Query.Models.Card card)
        {
            if (!CheckSecurity()) { return AccessDenied(); }
            return Common.Card.Kanban.RenderCard(this, card);
        }

        public string Details(int boardId, int cardId)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            try
            {
                var results = Common.Card.Kanban.RenderDetails(this, boardId, cardId, User.UserId);
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
            Query.Cards.Move(boardId, listId, cardId, cardIds);
            return Success();
        }


        public string MoveChecklistItem(int boardId, int cardId, int[] itemIds)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            Query.Cards.SortChecklist(cardId, User.UserId, itemIds);
            return Success();
        }
    }
}
