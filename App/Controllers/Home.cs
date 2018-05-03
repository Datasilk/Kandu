using System.Text;
using Microsoft.AspNetCore.Http;

namespace Kandu.Pages
{
    public class Home : Page
    {
        public Home(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var html = new StringBuilder();
            html.Append(Redirect("/login"));
            return base.Render(path, html.ToString(), metadata);
        }
    }
}
