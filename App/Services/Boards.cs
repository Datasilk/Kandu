using System;
using System.Text;

namespace Kandu.Services
{
    public class Boards : Service
    {
        public string Create(string name, string color, int orgId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            try
            {
                var boardId = Common.Platform.Boards.Create(this, name, color, orgId);
                return Success();
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
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
            if (!CheckSecurity()) { return AccessDenied(); } //check security
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
