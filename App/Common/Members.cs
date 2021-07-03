using System;
using System.Text;

namespace Kandu.Common
{
    public static class Members //Members of an organization
    {
        public static string RenderList(Core.IRequest request, int orgId, int page = 1, int length = 10, string search = "")
        {
            var listItem = new View("/Views/Members/list-item.html");
            var html = new StringBuilder();
            var members = Query.Organizations.GetMembers(orgId, page, length, search);
            foreach (var member in members)
            {
                listItem.Clear();
                listItem.Bind(new { member });
                listItem["click"] = "S.members.details.show(" + member.userId + ")";
                html.Append(listItem.Render());
            }

            return html.ToString();
        }
    }
}
