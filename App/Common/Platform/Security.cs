using System.Text;

namespace Kandu.Common.Platform
{
    public static class Security
    {
        public static string RenderList(Request request, int orgId)
        {
            var listItem = new View("/Views/Security/list-item.html");
            var html = new StringBuilder();
            var groups = Query.Security.GetGroups(orgId);
            foreach (var group in groups)
            {
                listItem.Clear();
                listItem.Bind(new { group });
                if (group.totalkeys != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.orgs.security.details(" + group.groupId + ", '" + group.name.Replace("'", "\\'").Replace("\"", "&quot;") + "')";
                listItem.Show("subtitle");
                html.Append(listItem.Render());
            }

            return html.ToString();
        }
    }
}
