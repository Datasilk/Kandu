using System;

namespace Kandu.Services
{
    public class Cards : Service
    {
        public Cards(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public string Create(int boardId, int listId, string name, string description = "", DateTime? dateDue = null, string colors = "")
        {
            if (!UserInfo.CheckSecurity(boardId)) { return AccessDenied(); }

            var query = new Query.Cards(S.Server.sqlConnectionString);
            var card = new Query.Models.Card()
            {
                boardId = boardId,
                listId = listId,
                name = name,
                colors = colors,
                description = description,
                datedue = dateDue,
                datecreated = DateTime.Now
            };
            var id = query.Create(card);
            card.cardId = id;

            //load Card html
            var boards = new Query.Boards(S.Server.sqlConnectionString);
            var board = boards.GetDetails(boardId);
            card.boardType = board.type;
            
            return Success() + "|" + GetCard(boardId, card);
        }

        public string Archive(int boardId, int cardId)
        {
            if (!UserInfo.CheckSecurity(boardId)) { return AccessDenied(); }

            var query = new Query.Cards(S.Server.sqlConnectionString);
            try
            {
                query.Archive(boardId, cardId);
            }
            catch (Exception) {
                return Error(); }
            
            return Success();
        }

        public string Restore(int boardId, int cardId)
        {
            if (!UserInfo.CheckSecurity(boardId)) { return AccessDenied(); }

            var query = new Query.Cards(S.Server.sqlConnectionString);
            try
            {
                query.Restore(boardId, cardId);
                return GetCard(boardId, cardId);
            }
            catch (Exception)
            {
                return Error();
            }
        }

        public string Delete(int boardId, int cardId)
        {
            if (!UserInfo.CheckSecurity(boardId)) { return AccessDenied(); }

            var query = new Query.Cards(S.Server.sqlConnectionString);
            try
            {
                query.Delete(boardId, cardId);
                return Success();
            }
            catch (Exception)
            {
                return Error();
            }
        }

        public string GetCard(int boardId, int cardId)
        {
            var query = new Query.Cards(S.Server.sqlConnectionString);
            var card = query.GetDetails(boardId, cardId);
            return GetCard(boardId, card);
        }

        public string GetCard(int boardId, Query.Models.Card card)
        {
            //get card HTML based on board type (kanban, timeline, etc)
            switch (card.boardType)
            {
                case Query.Models.Board.BoardType.kanban:
                    var service = new Card.Kanban(S);
                    return service.LoadCardHtml(card);
            }
            return "";
        }
    }
}
