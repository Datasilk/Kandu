namespace Kandu.Services
{
    public class Boards : Service
    {
        public string Create(string name, string color, int teamId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            try
            {
                var boardId = Common.Platform.Boards.Create(this, name, color, teamId);
                return Success();
            }catch(ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
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

        public string Update(int boardId, string name, string color, int teamId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            try
            {
                Common.Platform.Boards.Update(this, boardId, name, color, teamId);
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

        public string BoardsMenu(int orgId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            return Common.Platform.Boards.RenderBoardMenu(this, orgId);
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
