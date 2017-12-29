using System.Text;

namespace Kandu.Pages
{
    public class Timeline : Page
    {
        public Timeline(Core KanduCore) : base(KanduCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var html = new StringBuilder();

            if (path.Length > 0)
            {
                //load timeline for board

            }

            return base.Render(path, html.ToString());
        }
    }
}
