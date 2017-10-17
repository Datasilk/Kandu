namespace Kandu.Pages
{
    public class Boards : Page
    {
        public Boards(Core KanduCore) : base(KanduCore)
        {
        }

        public override string Render(string[] path, string body = "")
        {
            //check for database reset
            var scaffold = new Scaffold(S, "/Boards/boards.html");
            scripts += "<script src=\"/js/boards/boards.js\"></script>";

            //load login page
            return base.Render(path, scaffold.Render());
        }
    }
}
