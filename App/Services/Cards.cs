using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using Utility.Strings;


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
                    return Common.Card.Kanban.RenderCard(this, card);
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

        #region "Checklist"
        public string AddCheckList(int boardId, int cardId)
        {
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            Query.Cards.AddChecklistItem(cardId, User.UserId, "", false);
            var card = Query.Cards.GetDetails(boardId, cardId, User.UserId);
            return Common.Card.Kanban.RenderChecklist(card);
        }

        public string NewCheckListItem(int boardId, int cardId)
        {
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            var item = Query.Cards.AddChecklistItem(cardId, User.UserId, "", false);
            var viewItem = new View("/Views/Card/Kanban/Details/checklist-item.html");
            viewItem["id"] = item.itemId.ToString();
            viewItem["text"] = Web.HtmlEncode(item.label);
            return viewItem.Render();
        }

        public string UpdateCheckListItemLabel(int boardId, int cardId, int itemId, string label)
        {
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            try
            {
                Query.Cards.UpdateChecklistItemLabel(itemId, cardId, User.UserId, label);
            }catch(Exception)
            {
                return Error("Could not update existing checklist item");
            }
            return Success();
        }

        public string UpdateCheckListItemChecked(int boardId, int cardId, int itemId, bool ischecked)
        {
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            try
            {
                Query.Cards.UpdateChecklistItemChecked(itemId, cardId, User.UserId, ischecked);
            }
            catch (Exception)
            {
                return Error("Could not update existing checklist item");
            }
            return Success();
        }

        public string DeleteCheckListItem(int boardId, int cardId, int itemId)
        {
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            try
            {
                Query.Cards.RemoveChecklistItem(itemId, cardId, User.UserId);
            }
            catch (Exception)
            {
                return Error("Could not delete checklist item");
            }
            return Success();
        }

        public string GetCheckList(int boardId, int cardId)
        {
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, new string[] { Security.Keys.CardCanUpdate.ToString(), Security.Keys.CardFullAccess.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(board.orgId, new string[] {
                    Security.Keys.BoardCanView.ToString(), Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString()
                }, Models.Scope.Board, boardId)
            ) { return AccessDenied(); }

            var card = Query.Cards.GetDetails(boardId, cardId, User.UserId);
            return Common.Card.Kanban.RenderChecklist(card);
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

        #region "Share"
        public string FindInvites(int cardId, string search)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            //find member from search param
            var members = new List<Query.Models.Member>();
            var results = Query.Organizations.GetMembers(card.orgId, 1, 10, search);
            if(results != null && results.Count > 0)
            {
                members.AddRange(results);
            }
            else if (search.IsEmail())
            {
                //search is for an unknown email address
                members.Add(new Query.Models.Member()
                {
                    name = search
                });
            }

            return JsonResponse(members.Select(a => new {id = a.userId, a.name, a.photo}));
        }

        public string BatchInvite(int cardId, string invites, bool canupdate = false, bool canpostcomment = false)
        {
            var card = Query.Cards.GetInfo(cardId);
            if (!User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString() }, Models.Scope.Card, cardId)
                || !User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, card.boardId)
            ) { return AccessDenied(); }

            var keys = new List<string>();
            if (canupdate) { keys.Add(Security.Keys.CardCanUpdate.ToString()); }
            if (canpostcomment) { keys.Add(Security.Keys.CardCanComment.ToString()); }
            try
            {
                var failed = Common.Invitations.Send(this, invites.Split(',').ToList(), card.orgId, Models.Scope.Card, cardId, new string[] { "join" });
                //if (failed.Length > 0)
                //{
                //    return Error(string.Join(",", failed));
                //}
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }
            
            return Success();
        }
        #endregion
    }
}
