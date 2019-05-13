using System.Text;
using Microsoft.AspNetCore.Http;

namespace Kandu.Controllers
{
    public class Home : Controller
    {
        public Home(HttpContext context, Parameters parameters) : base(context, parameters)
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
