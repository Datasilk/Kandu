using Microsoft.AspNetCore.Http;

namespace Kandu.Controllers
{
    public class Board : Controller
    {
        public Board(HttpContext context, Parameters parameters) : base(context, parameters)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            //check security
            if(path.Length < 2) { return Error(); }
            var boardId = int.Parse(path[1]);
            if (User.userId == 0) { return AccessDenied(new Login(context, parameters)); }
            if (!User.CheckSecurity(boardId)) { return AccessDenied(new Login(context, parameters)); }

            //add client-side dependencies
            AddScript("/js/views/board/board.js?v=" + Server.Version);
            AddScript("/js/dashboard.js?v=" + Server.Version);
            AddCSS("/css/dashboard.css?v=" + Server.Version);

            var scaffold = new Scaffold("/Views/Board/board.html");

            //load board details
            var colors = new Utility.Colors();
            var board = Query.Boards.GetBoardAndLists(boardId);
            BoardPage page;

            //add custom javascript for User Settings & Board info
            scripts.Append("<script language=\"javascript\">" + 
                "S.board.id=" + board.boardId + ";" + 
                (User.allColor ? "S.head.allColor();" : "") + 
                "</script>");

            //choose which Lists Type to render
            switch (board.type)
            {
                default: 
                case Query.Models.Board.BoardType.kanban: //kanban
                    page = new Kanban(context, parameters);
                    break;
            }

            //dependancy injection
            page.board = board;

            //set background color of board
            scaffold["color"] = "#" + board.color;
            scaffold["color-dark"] = colors.ChangeHexBrightness(board.color, (float)-0.3);

            //transfer resources from page
            scripts.Append(page.scripts.ToString());
            css.Append(page.css.ToString());

            //render board lists
            scaffold["content"] = page.Render(path);

            //load header
            LoadHeader(ref scaffold);
           

            return base.Render(path, scaffold.Render());
        }
    }

    public class BoardPage : Controller
    {

        public Query.Models.Board board;
        public Controller parent;

        public BoardPage(HttpContext context, Parameters parameters) : base(context, parameters)
        {
        }
    }
}
