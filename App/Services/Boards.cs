using System.Collections.Generic;
using System.Text;

namespace Kandu.Services
{
    public class Boards : Service
    {
        public string Create(string name, string color, int orgId, string cardtype = "")
        {
            //check security
            if (!User.CheckSecurity(orgId, Security.Keys.BoardCanCreate.ToString())
                || !User.CheckSecurity(orgId, Security.Keys.BoardsFullAccess.ToString())) { return AccessDenied(); }
            
            //create new board
            try
            {
                var boardId = Common.Boards.Create(this, name, color, orgId, cardtype);
                return Success();
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
        }

        public string RenderForm(int boardId = 0, int orgId = 0)
        {
            //check security
            if (!User.CheckSecurity(orgId, Security.Keys.BoardCanCreate.ToString())
                   || !User.CheckSecurity(orgId, Security.Keys.BoardsFullAccess.ToString())) { return AccessDenied(); }
            
            //render form used for creating new boards
            var view = new View("/Views/Board/new-board.html");
            var defaultCardType = "";
            if(boardId != 0)
            {
                var board = Query.Boards.GetInfo(boardId);
                defaultCardType = board.cardtype;
                if (!CheckSecurity(board.orgId, Security.Keys.BoardCanUpdate.ToString(), Models.Scope.Board, boardId))
                {
                    return AccessDenied();
                }
                view["name"] = board.name;
                view["color"] = "#0094ff";
                view["submit-label"] = "Update Board";
                view["submit-click"] = "S.boards.add.submit(" + orgId + ", " + boardId + ")";
            }
            else
            {
                view["color"] = "#0094ff";
                view["submit-label"] = "Create Board";
                view["submit-click"] = "S.boards.add.submit(" + orgId + ")";
            }

            if(orgId != 0)
            {
                if (!CheckSecurity(orgId, Security.Keys.BoardCanUpdate.ToString()))
                {
                    return AccessDenied();
                }
                var org = Query.Organizations.GetInfo(orgId);
                view["org-options"] = "<option value=\"" + org.orgId + "\">" + org.name + "</option>";
                view["org-name"] = org.name;
                view.Show("has-id");
            }
            else
            {
                var orgs = Query.Organizations.UserIsPartOf(User.UserId);
                var opts = new StringBuilder();
                foreach (var org in orgs)
                {
                    if (CheckSecurity(org.orgId, Security.Keys.BoardCanCreate.ToString()))
                    {
                        opts.Append("<option value=\"" + org.orgId + "\">" + org.name + "</option>");
                    }
                }
                view["org-options"] = opts.ToString();
                view.Show("no-id");
            }

            //TODO: get card types from vendors
            var cardTypes = new List<string>() { "Basic" };
            var html = new StringBuilder();
            foreach(var card in cardTypes)
            {
                html.Append("<option value=\"" + card + "\"" + (defaultCardType == card ? " selected" : "") + ">" + card + "</option>");
            }
            view["cardtypes"] = html.ToString();

            return view.Render();
        }

        public string RenderList()
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            return Common.Boards.RenderList(User.UserId, this);
        }

        public string Details(int boardId)
        { 
            //check security
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(board.orgId, Security.Keys.BoardCanView.ToString(), Models.Scope.Board, boardId)
                || !User.CheckSecurity(board.orgId, new string[]{Security.Keys.BoardsCanViewAll.ToString(), Security.Keys.BoardsFullAccess.ToString()})
                ) 
            { return AccessDenied(); }

            //view details about a board
            try
            {
                return Common.Boards.Details(boardId);
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
        }

        public string Update(int boardId, string name, string color, int orgId, string cardtype)
        {
            var board = Query.Boards.GetInfo(boardId);
            if (!User.CheckSecurity(orgId, Security.Keys.BoardCanUpdate.ToString(), Models.Scope.Board, boardId)
                || !User.CheckSecurity(board.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString() })
                ) { return AccessDenied(); } //check security
            try
            {
                Common.Boards.Update(this, boardId, name, color, orgId, cardtype);
            }
            catch (ServiceDeniedException)
            {
                return AccessDenied();
            }
            catch (ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
            return Success();
        }

        public string BoardsMenu(int orgId, bool subTitles = false, bool listOnly = true, int sort = 0, bool buttonsInFront = false)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            return Common.Boards.RenderSideBar(this, orgId, listOnly, subTitles, sort, buttonsInFront);
        }

        public string KeepMenuOpen(bool keepOpen)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            Common.Boards.KeepBoardsMenuOpen(this, keepOpen);
            return "";
        }

        public string AllColor(bool allColor)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            Common.Boards.UseAllColorScheme(this, allColor);
            return "";
        }
    }
}
