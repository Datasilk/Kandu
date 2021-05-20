using System.Text;

namespace Kandu.Common.Platform
{
    public static class Organizations
    {

        public static string RenderOrgListModal(Request request)
        {
            var html = new StringBuilder();
            var section = new View("/Views/Organizations/list.html");
            var item = new View("/Views/Organizations/list-item.html");
            var orgs = Query.Organizations.UserIsPartOf(request.User.userId);
            foreach(var org in orgs)
            {
                item.Clear();
                item.Bind(new { org });
                html.Append(item.Render());
            }
            section["list"] = html.ToString();
            return section.Render();
        }
    }
}
