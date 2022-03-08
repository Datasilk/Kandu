using System;

namespace Kandu.Services
{
    public class Invitations : Service
    {
        public string Accept(string email, string publickey)
        {
            //validate invitation
            try
            {
                if(email == "") { return Error("Please provide an email address"); }
                var invitation = Query.Invitations.Accept(email, publickey);
                if (invitation == null)
                {
                    return Error("Invitation doesn't exist or is expired. Please request another invitation.");
                }
                var url = "/boards";
                switch (invitation.scope)
                {
                    case Models.Scope.Board:
                        //get board and redirect user to board
                        var board = Query.Boards.GetInfo(invitation.scopeId);
                        url = Common.Boards.GetUrl(board.boardId, board.name) + "#joined-board";
                        break;
                    case Models.Scope.List:
                        //get board from list and redirect user to board
                        var list = Query.Lists.GetBoard(invitation.scopeId);
                        url = Common.Boards.GetUrl(list.boardId, list.boardName) + "#joined-board";
                        break;
                    case Models.Scope.Card:
                        //get board from list and redirect user to board, then load card
                        var card = Query.Cards.GetBoard(invitation.scopeId);
                        url = Common.Boards.GetUrl(card.boardId, card.boardName) + "#card=" + invitation.scopeId;
                        break;
                    case Models.Scope.SecurityGroup:
                        url += "#joined-sg=" + invitation.scopeId;
                        break;
                    case Models.Scope.Team:
                        url += "#joined-team=" + invitation.scopeId;
                        break;
                    case Models.Scope.Organization:
                        url += "#joined-org=" + invitation.scopeId;
                        break;

                }
                return url;
            }
            catch(Exception ex)
            {
                Query.Logs.LogError(User.UserId, "Invitations/Accept", "", ex.Message, ex.StackTrace);
                return Error("Error occurred when trying to accept an invitation");
            }
        }
    }
}
