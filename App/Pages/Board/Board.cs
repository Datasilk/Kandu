using System;
using System.Text;

namespace Kandu.Pages
{
    public class Board : Page
    {
        public Board(Core KanduCore) : base(KanduCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            //check security
            if(path.Length < 2) { return Error(); }
            var boardId = int.Parse(path[1]);
            if (S.User.userId == 0) { return AccessDenied(true, new Login(S)); }
            if (!UserInfo.CheckSecurity(boardId)) { return AccessDenied(true, new Login(S)); }

            //add client-side dependencies
            AddScript("/js/pages/board/board.js");
            AddScript("/js/dashboard.js");
            AddCSS("/css/dashboard.css");

            var scaffold = new Scaffold("/Pages/Board/board.html", S.Server.Scaffold);
            var query = new Query.Boards(S.Server.sqlConnectionString);
            

            //load board details
            var colors = new Utility.Colors();
            var board = query.GetBoardAndLists(boardId);
            BoardPage page;

            //add custom javascript for User Settings & Board info
            scripts += "<script language=\"javascript\">" + 
                "S.board.id=" + board.boardId + ";" + 
                (UserInfo.Settings.allColor ? "S.head.allColor();" : "") + 
                "</script>";

            //choose which Lists Type to render
            switch (board.type)
            {
                default: 
                case Query.Models.Board.BoardType.kanban: //kanban
                    page = new Kanban(S);
                    break;
            }

            //dependancy injection
            page.board = board;

            //set background color of board
            scaffold.Data["color"] = "#" + board.color;
            scaffold.Data["color-dark"] = colors.ChangeHexBrightness(board.color, (float)-0.3);

            //transfer resources from page
            scripts += page.scripts;
            headCss += page.headCss;

            //render board lists
            scaffold.Data["content"] = page.Render(path);

            //load header
            LoadHeader(ref scaffold);
           

            return base.Render(path, scaffold.Render());
        }
    }

    public class BoardPage : Page
    {
        public BoardPage(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public Query.Models.Board board;
        public Page parent;

    }
}
