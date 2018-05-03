using Microsoft.AspNetCore.Http;
using System;
using Utility;


namespace Kandu.Services
{
    public class Cards : Service
    {
        public Cards(HttpContext context) : base(context)
        {
        }

        #region "Create, Archive, Restore, & Delete"

        public string Create(int boardId, int listId, string name, string description = "", DateTime? dateDue = null, string colors = "")
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            Query.Models.Card card;
            try
            {
                card = Common.Platform.Cards.Create(boardId, listId, name, description, dateDue, colors);
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }

            //load Card html
            try
            {
                var boards = new Query.Boards();
                var board = boards.GetDetails(boardId);
                card.boardType = board.type;
                return GetCard(boardId, card);
            }
            catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
        }

        public string Archive(int boardId, int cardId)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }

            var query = new Query.Cards();
            try
            {
                query.Archive(boardId, cardId);
            }
            catch (Exception) {
                return Error();
            }
            return Success();
        }

        public string Restore(int boardId, int cardId)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }

            var query = new Query.Cards();
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
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }

            var query = new Query.Cards();
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
        #endregion

        #region "Render Card HTML based on Board Type"

        public string GetCard(int boardId, int cardId)
        {
            var query = new Query.Cards();
            var card = query.GetDetails(boardId, cardId);
            return GetCard(boardId, card);
        }

        public string GetCard(int boardId, Query.Models.Card card)
        {
            //get card HTML based on board type (kanban, timeline, etc)
            switch (card.boardType)
            {
                case Query.Models.Board.BoardType.kanban:
                    return Common.Platform.Card.Kanban.RenderCard(card);
            }
            return "";
        }
        #endregion

        #region "Update Card Information"
        public string UpdateDescription(int boardId, int cardId, string description)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }

            //check description for malicious input
            if (Malicious.IsMalicious(description, Malicious.InputType.TextOnly) == true)
            {
                return Error(); 
            }

            var query = new Query.Cards();
            try
            {
                query.UpdateDescription(boardId, cardId, description);
                return GetCard(boardId, cardId);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        #endregion
    }
}
