using System.Text;
using Kandu.Core;

namespace Kandu.Common
{
    public static class User
    {

        public static string RenderUserMenu(IRequest request)
        {
            var html = new StringBuilder();
            var section = new View("/Views/User/menu.html");
            section["org-id"] = request.User.OrgId.ToString();
            section["user-id"] = request.User.UserId.ToString();
            section["username"] = request.User.Name;
            html.Append(section.Render());
            return html.ToString();
        }
    }
}
