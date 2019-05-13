using Microsoft.AspNetCore.Http;
using System.Text;

namespace Kandu.Controllers
{
    public class Timeline : BoardPage
    {
        public Timeline(HttpContext context, Parameters parameters) : base(context, parameters)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var html = new StringBuilder();

            if (path.Length > 0)
            {
                //load timeline for board

            }

            return html.ToString();
        }
    }
}
