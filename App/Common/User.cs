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
            if (request.User.IsAppOwner())
            {
                section.Show("app");
                request.AddScript("/js/views/app/app.js");
                request.AddCSS("/css/views/app/app.css");
            }
            html.Append(section.Render());
            return html.ToString();
        }
    }
}
