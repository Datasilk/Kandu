using System.Text;

namespace Kandu.Common
{
    public static class User
    {

        public static string RenderUserMenu()
        {
            var html = new StringBuilder();
            var section = new View("/Views/User/menu.html");
            html.Append(section.Render());
            return html.ToString();
        }
    }
}
