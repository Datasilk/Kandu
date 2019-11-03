namespace Kandu.Controllers
{
    public class Board : Controller
    {
        public override string Render(string body = "")
        {
            //check security
            if(PathParts.Length < 2) { return Error(); }
            var boardId = int.Parse(PathParts[1]);
            if (User.userId == 0) { return AccessDenied<Login>(); }
            if (!User.CheckSecurity(boardId)) { return AccessDenied<Login>(); }

            //add client-side dependencies
            AddScript("/js/views/board/board.js?v=" + Server.Version);
            AddScript("/js/dashboard.js?v=" + Server.Version);
            AddCSS("/css/dashboard.css?v=" + Server.Version);

            var view = new Scaffold("/Views/Board/board.html");

            //load board details
            var colors = new Utility.Colors();
            var board = Query.Boards.GetBoardAndLists(boardId);
            BoardPage page;

            //add custom javascript for User Settings & Board info
            Scripts.Append("<script language=\"javascript\">" + 
                "S.board.id=" + board.boardId + ";" + 
                (User.allColor ? "S.head.allColor();" : "") + 
                "</script>");

            //choose which Lists Type to render
            switch (board.type)
            {
                default: 
                case Query.Models.Board.BoardType.kanban: //kanban
                    page = new Kanban();
                    page.Init(Context, Parameters, Path, PathParts);
                    break;
            }

            //dependancy injection
            page.board = board;

            //set background color of board
            view["color"] = "#" + board.color;
            view["color-dark"] = colors.ChangeHexBrightness(board.color, (float)-0.3);

            //transfer resources from page
            Scripts.Append(page.Scripts.ToString());
            Css.Append(page.Css.ToString());

            //render board lists
            view["content"] = page.Render();

            //load header
            LoadHeader(ref view);
           

            return base.Render(view.Render());
        }
    }

    public class BoardPage : Controller
    {
        public Query.Models.Board board;
        public Controller parent;
    }
}
