using Microsoft.AspNetCore.Http;
namespace Kandu.Pages
{
    public class Board : Page
    {
        public Board(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            //check security
            if(path.Length < 2) { return Error(); }
            var boardId = int.Parse(path[1]);
            if (User.userId == 0) { return AccessDenied(true, new Login(context)); }
            if (!User.CheckSecurity(boardId)) { return AccessDenied(true, new Login(context)); }

            //add client-side dependencies
            AddScript("/js/views/board/board.js");
            AddScript("/js/dashboard.js");
            AddCSS("/css/dashboard.css");

            var scaffold = new Scaffold("/Views/Board/board.html", Server.Scaffold);

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
                    page = new Kanban(context);
                    break;
            }

            //dependancy injection
            page.board = board;

            //set background color of board
            scaffold.Data["color"] = "#" + board.color;
            scaffold.Data["color-dark"] = colors.ChangeHexBrightness(board.color, (float)-0.3);

            //transfer resources from page
            scripts.Append(page.scripts.ToString());
            headCss.Append(page.headCss.ToString());

            //render board lists
            scaffold.Data["content"] = page.Render(path);

            //load header
            LoadHeader(ref scaffold);
           

            return base.Render(path, scaffold.Render());
        }
    }

    public class BoardPage : Page
    {
        public BoardPage(HttpContext context) : base(context)
        {
        }

        public Query.Models.Board board;
        public Page parent;

    }
}
