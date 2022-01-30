using System;
using Utility;


namespace Kandu.Services
{
    public class Cards : Service
    {
        #region "Create, Archive, Restore, & Delete"

        public string Create(int boardId, int listId, string name, string description = "", DateTime? dateDue = null, string colors = "", string type = "")
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            Query.Models.Card card;
            try
            {
                card = Common.Cards.Create(boardId, listId, name, description, dateDue, colors, type);
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }

            //load Card html
            try
            {
                var board = Query.Boards.GetDetails(boardId);
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
            
            try
            {
                Query.Cards.Archive(boardId, cardId);
            }
            catch (Exception) {
                return Error();
            }
            return Success();
        }

        public string Restore(int boardId, int cardId)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }
            
            try
            {
                Query.Cards.Restore(boardId, cardId);
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
            
            try
            {
                Query.Cards.Delete(boardId, cardId);
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
            var card = Query.Cards.GetDetails(boardId, cardId);
            return GetCard(boardId, card);
        }

        public string GetCard(int boardId, Query.Models.Card card)
        {
            //get card HTML based on board type (kanban, timeline, etc)
            switch (card.boardType)
            {
                case Query.Models.Board.BoardType.kanban:
                    return Common.Card.Kanban.RenderCard(this, card);
            }
            return "";
        }
        #endregion

        #region "Update Card Information"
        public string UpdateName(int boardId, int cardId, string name)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }

            //check description for malicious input
            if (Malicious.IsMalicious(name, Malicious.InputType.TextOnly) == true)
            {
                return Error();
            }

            try
            {
                Query.Cards.UpdateName(boardId, cardId, name);
                return GetCard(boardId, cardId);
            }
            catch (Exception)
            {
                return Error();
            }
        }

        public string UpdateDescription(int boardId, int cardId, string description)
        {
            if (!User.CheckSecurity(boardId)) { return AccessDenied(); }

            //check description for malicious input
            if (Malicious.IsMalicious(description, Malicious.InputType.TextOnly) == true)
            {
                return Error(); 
            }
            
            try
            {
                Query.Cards.UpdateDescription(boardId, cardId, description);
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
