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
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardCanView.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }
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
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }
            Query.Cards.Move(boardId, listId, cardId, cardIds);
            return Success();
        }


        public string MoveChecklistItem(int cardId, int[] itemIds)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }
            Query.Cards.SortChecklist(cardId, User.UserId, itemIds);
            return Success();
        }
    }
}
