using System;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Kandu.Pages
{
    public class Kanban : BoardPage
    {
        public Kanban(HttpContext context) : base(context)
        {
            //load page resources
            scripts.Append("<script src=\"/js/views/board/kanban/kanban.js\"></script>");
            headCss.Append("<link type=\"text/css\" rel=\"stylesheet\" href=\"/css/views/board/kanban/kanban.css\">");
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var html = new StringBuilder();

            if(path.Length > 1)
            {
                //load kanban lists for board
                var boardId = int.Parse(path[1]);
                var scaffold = new Scaffold("Views/Board/Kanban/kanban.html", Server.Scaffold);
                var htmlists = new StringBuilder();
                foreach(var list in board.lists)
                {
                    htmlists.Append(Common.Platform.List.Kanban.RenderList(list, list.cards) + "\n");
                }

                var colors = new Utility.Colors();

                scaffold.Data["name"] = board.name;
                try
                {
                    scaffold.Data["color-hover"] = colors.FromHexToRgba("#" + board.color, 1);
                }
                catch (Exception)
                {
                }
                scaffold.Data["lists"] = htmlists.ToString();
                html.Append(scaffold.Render());
            }

            return html.ToString();
        }
    }
}
