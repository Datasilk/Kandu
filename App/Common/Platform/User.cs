using System.Text;

namespace Kandu.Common.Platform
{
    public static class User
    {

        public static string RenderUserMenu(Request request)
        {
            var html = new StringBuilder();
            var section = new View("/Views/User/menu.html");
            html.Append(section.Render());
            return html.ToString();
        }
    }
}
