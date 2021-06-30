using System;
using System.Text;

namespace Kandu.Services
{
    public class Boards : Service
    {
        public string Create(string name, string color, int orgId)
        {
            if (!User.CheckSecurity(orgId, Models.Security.Keys.BoardCanCreate)) { return AccessDenied(); } //check security
            try
            {
                var boardId = Common.Platform.Boards.Create(this, name, color, orgId);
                return Success();
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
        }

        public string RenderForm(int boardId = 0, int orgId = 0)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var view = new View("/Views/Board/new-board.html");
            if(boardId != 0)
            {
                var board = Query.Boards.GetInfo(boardId);
                if (!CheckSecurity(board.orgId, Models.Security.Keys.BoardCanUpdate, Models.Security.Scope.Board, boardId))
                {
                    return AccessDenied();
                }
                view["name"] = board.name;
                view["color"] = "#0094ff";
                view["submit-label"] = "Update Board";
                view["submit-click"] = "S.boards.add.submit('" + boardId + "')";
            }
            else
            {
                view["color"] = "#0094ff";
                view["submit-label"] = "Create Board";
                view["submit-click"] = "S.boards.add.submit()";
            }

            if(orgId != 0)
            {
                if (!CheckSecurity(orgId, Models.Security.Keys.BoardCanUpdate))
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
                var orgs = Query.Organizations.UserIsPartOf(User.userId);
                var opts = new StringBuilder();
                foreach (var org in orgs)
                {
                    if (CheckSecurity(org.orgId, Models.Security.Keys.BoardCanCreate))
                    {
                        opts.Append("<option value=\"" + org.orgId + "\">" + org.name + "</option>");
                    }
                }
                view["org-options"] = opts.ToString();
                view.Show("no-id");
            }
            return view.Render();
        }

        public string RenderList()
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            return Common.Platform.Boards.RenderList(this);
        }

        public string Details(int boardId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            try
            {
                return Common.Platform.Boards.Details(boardId);
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
        }

        public string Update(int boardId, string name, string color, int orgId)
        {
            if (!User.CheckSecurity(orgId, Models.Security.Keys.BoardCanUpdate, Models.Security.Scope.Board, boardId)) { return AccessDenied(); } //check security
            try
            {
                Common.Platform.Boards.Update(this, boardId, name, color, orgId);
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
            return Common.Platform.Boards.RenderMenu(this, orgId, listOnly, subTitles, sort, buttonsInFront);
        }

        public string KeepMenuOpen(bool keepOpen)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            Common.Platform.Boards.KeepBoardsMenuOpen(this, keepOpen);
            return "";
        }

        public string AllColor(bool allColor)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            Common.Platform.Boards.UseAllColorScheme(this, allColor);
            return "";
        }
    }
}
