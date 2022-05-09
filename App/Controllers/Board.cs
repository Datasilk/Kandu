namespace Kandu.Controllers
{
    public class Board : Controller
    {
        public override string Render(string body = "")
        {
            //check security
            if(PathParts.Length < 2) { return Error(); }
            var boardId = int.Parse(PathParts[1]);
            if (User.UserId == 0) { return AccessDenied<Login>(); }
            if (!User.CheckSecurity(boardId)) { return AccessDenied<Login>(); }

            //add client-side dependencies
            AddScript("/js/views/board/board.js?v=" + Server.Version);
            AddScript("/js/dashboard.js?v=" + Server.Version);
            AddCSS("/css/dashboard.css?v=" + Server.Version);

            var view = new View("/Views/Board/board.html");
            var board = Query.Boards.GetBoardAndLists(boardId);
            var org = Query.Organizations.GetInfo(board.orgId);

            //add theme-related resources
            if (org.customJs)
            {
                AddScript("/themes/orgs/" + org.orgId + "/theme.js", "themejs");
            }
            if (org.customCss)
            {
                AddCSS("/themes/orgs/" + org.orgId + "/theme.css", "themecss");
            }


            //load board details
            var colors = new Utility.Colors();
            BoardPage page;

            //add custom javascript for User Settings & Board info
            Scripts.Append("<script language=\"javascript\">" + 
                "S.board.id=" + board.boardId + ";" + 
                (User.AllColor ? "S.head.allColor();" : "S.board.color = '" + board.color + "';") + 
                "</script>");

            //choose which Lists Type to render
            switch (board.type)
            {
                default: 
                case Query.Models.Board.BoardType.kanban: //kanban
                    page = new Kanban
                    {
                        Context = Context,
                        Parameters = Parameters,
                        Path = Path,
                        PathParts = PathParts
                    };
                    page.Init();
                    break;
            }

            //dependancy injection
            page.board = board;

            //set background color of board
            view["color"] = "#" + board.color;
            view["color-dark"] = colors.ChangeHexBrightness(board.color, (float)-0.3);

            //transfer resources from page
            Scripts.Append(page.Scripts);
            Css.Append(page.Css);

            //render board lists
            view["content"] = page.Render();
            Title = board.name + " - Kandu";

            //load header
            LoadHeader(ref view, HasMenu.Board);
           

            return base.Render(view.Render());
        }
    }

    public class BoardPage : Controller
    {
        public Query.Models.Board board;
        public Controller parent;
    }
}
