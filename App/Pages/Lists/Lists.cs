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
            var html = new StringBuilder();
            var query = new Query.Boards(S.Server.sqlConnectionString);
            try
            {

                //choose which Lists Type to render
                var board = query.GetBoardDetails(int.Parse(path[1]));
                Page page;
                switch (board.type)
                {
                    default: 
                    case 0: //kanban
                        page = new Kanban(S);
                        break;
                    case 1: //timeline
                        page = new Timeline(S);
                        break;
                }

                //render board lists
                html.Append(page.Render(path));
                
            }
            catch(Exception ex) { }
           

            return base.Render(path, html.ToString());
        }
    }
}
