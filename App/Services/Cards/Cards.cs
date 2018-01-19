using System;
using System.Collections.Generic;
using System.Linq;

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
            var id = query.CreateCard(card);
            card.cardId = id;

            //load Card html
            var boards = new Query.Boards(S.Server.sqlConnectionString);
            var board = boards.GetBoardDetails(boardId);
            var html = "";
            switch (board.type)
            {
                case Query.Models.Board.BoardType.kanban:
                    var service = new Card.Kanban(S);
                    html = service.LoadCardHtml(card);
                    break;
            }
            
            return Success() + "|" + html;
        }
    }
}
