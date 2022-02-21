using System;
using Utility;


namespace Kandu.Services
{
    public class Cards : Service
    {
        #region "Create, Archive, Restore, & Delete"

        public string Create(int boardId, int listId, string name, string description = "", DateTime? dateDue = null, string colors = "", string type = "")
        {
            //check security
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            //create new card
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
            //check security
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            //archive card
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
            //check security
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            //restore archived card
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
            //check security
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            //delete card
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
            var card = Query.Cards.GetDetails(boardId, cardId, User.UserId);
            return GetCard(boardId, card);
        }

        public string GetCard(int boardId, Query.Models.Card card)
        {
            //check security
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardCanView.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, card.cardId)
                || !User.CheckSecurity(board.orgId, new string[] { 
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardsCanViewAll.ToString() 
                }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            //get card HTML based on board type (kanban, timeline, etc)
            switch (card.boardType)
            {
                case Query.Models.Board.BoardType.kanban:
                    return Common.Card.Kanban.RenderCardDetails(this, card);
            }
            return "";
        }
        #endregion

        #region "Update Card Information"
        public string UpdateName(int boardId, int cardId, string name)
        {
            //check security
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

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
            //check security
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            //check description for malicious input
            if (Malicious.IsMalicious(description, Malicious.InputType.TextOnly) == true)
            {
                return Error(); 
            }
            
            //save description
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

        public string UpdateDueDate(int cardId, string duedate)
        {
            //check security
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }


            //save due date
            try
            {
                DateTime? date = null;
                if(duedate != "")
                {
                    date = DateTime.Parse(duedate);
                }
                Query.Cards.UpdateDueDate(cardId, User.UserId, date);
            }
            catch (Exception)
            {
                return Error();
            }
            return Success();
        }
        #endregion

        #region "Assigned To"

        public string GetMembers(int cardId)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanView.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanView.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            return JsonResponse(Query.Cards.Members(cardId));
        }

        public string UpdateAssignedTo(int cardId, int userId)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            Query.Cards.UpdateAssignedTo(cardId, User.UserId, userId);
            return GetAssignedTo(cardId);
        }

        public string GetAssignedTo(int cardId)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanView.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanView.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }
            if(card.userIdAssigned > 0)
            {
                var view = new View("/Views/Card/Kanban/Details/assigned-to.html");
                view["assigned-userid"] = card.userIdAssigned.ToString();
                view["assigned-name"] = card.assignedName;
                view["org-id"] = card.orgId.ToString();
                return view.Render();
            }
            return "";
        }
        #endregion

        #region "Comments"
        public string AddComment(int cardId, string comment)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardCanComment.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString(), Security.Keys.BoardCanComment.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            try
            {
                //check for malicious text
                if (Malicious.IsMalicious(comment, Malicious.InputType.TextOnly) == true)
                {
                    return Error();
                }
                //add comment
                var commentId = Query.Cards.AddComment(cardId, User.UserId, comment);

                //render new comment
                var view = new View("/Views/Card/Kanban/Details/comment.html");
                return Common.Cards.RenderComment(view, commentId, User.UserId, card.orgId, User.Name, comment, User.Photo, DateTime.Now, true, false);
            }
            catch (Exception)
            {
                return Error();
            }
        }

        public string GetComment(int cardId, int commentId)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanView.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanView.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            return Query.Cards.GetComment(cardId, commentId, User.UserId).comment;
        }

        public string UpdateComment(int cardId, int commentId, string comment)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardCanComment.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString(), Security.Keys.BoardCanComment.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            try
            {
                //check for malicious text
                if (Malicious.IsMalicious(comment, Malicious.InputType.TextOnly) == true)
                {
                    return Error();
                }
                //check for comment ownership
                var commentobj = Query.Cards.GetComment(cardId, commentId, User.UserId);
                if(commentobj.userId != User.UserId)
                {
                    return Error("You do not own this comment");
                }
                Query.Cards.UpdateComment(commentId, cardId, User.UserId, comment);

                //render comment
                var view = new View("/Views/Card/Kanban/Details/comment.html");
                return Common.Cards.RenderComment(view, commentId, User.UserId, card.orgId, User.Name, comment, User.Photo, DateTime.Now, true, commentobj.hasflagged);
            }
            catch (Exception)
            {
                return Error();
            }
        }

        public string DeleteComment(int cardId, int commentId)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardCanComment.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString(), Security.Keys.BoardCanComment.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            try
            {
                //check for comment ownership
                var commentobj = Query.Cards.GetComment(cardId, commentId, User.UserId);
                if (commentobj.userId != User.UserId)
                {
                    return Error("You do not own this comment");
                }
                Query.Cards.RemoveComment(commentId, cardId, User.UserId);
                return Success();
            }
            catch (Exception)
            {
                return Error();
            }
        }

        public string FlagComment(int cardId, int commentId)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanView.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanView.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            try
            {
                //check for comment ownership
                var commentobj = Query.Cards.GetComment(cardId, commentId, User.UserId);
                if (commentobj.userId == User.UserId)
                {
                    return Error("You cannot flag your own comment");
                }
                Query.Cards.FlagComment(commentId, cardId, User.UserId);
                return Success();
            }
            catch (Exception)
            {
                return Error();
            }
        }
        #endregion
    }
}
