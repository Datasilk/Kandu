using System.Text;

namespace Kandu.Controllers
{
    public class Timeline : BoardPage
    {

        public override string Render(string body = "")
        {
            var html = new StringBuilder();

            if (PathParts.Length > 0)
            {
                //load timeline for board

            }

            return html.ToString();
        }
    }
}
