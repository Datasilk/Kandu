using System.Linq;
using System.Text;
using Kandu.Core;

namespace Kandu.Common
{
    public static class SecurityGroups
    {
        public static string RenderList(int orgId)
        {
            var listItem = new View("/Views/Security/list-item.html");
            var html = new StringBuilder();
            var groups = Query.Security.GetGroups(orgId);
            foreach (var group in groups)
            {
                listItem.Clear();
                listItem.Bind(new { group });
                if (group.totalkeys != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.orgs.security.details(" + group.groupId + ", " + orgId + ", '" + group.name.Replace("'", "\\'").Replace("\"", "&quot;") + "')";
                listItem.Show("subtitle");
                html.Append(listItem.Render());
            }

            return html.ToString();
        }

        public static string RenderListForUser(IRequest request, int userId)
        {
            var listItem = new View("/Views/Security/list-item.html");
            var titlebar = new View("/Views/Organizations/title-bar.html");
            var html = new StringBuilder();
            var groups = Query.Security.GetGroupsForUser(request.User.UserId, userId).OrderBy(a => a.orgId).ThenBy(a => a.groupId);
            var lastOrgId = 0;
            foreach (var group in groups)
            {
                if(lastOrgId != group.orgId)
                {
                    if(lastOrgId > 0) { html.Append("</div>"); }
                    lastOrgId = group.orgId;
                    titlebar.Clear();
                    titlebar["org-name"] = group.orgName;
                    titlebar["org-id"] = group.orgId.ToString();
                    if(group.ownerId == userId) { titlebar.Show("is-owner"); }
                    html.Append(titlebar.Render() + "\n<div class=\"grid-items\">");
                }
                listItem.Clear();
                listItem.Bind(new { group });
                if (group.totalkeys != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.user.security.details(" + group.groupId + ", " + group.orgId + ", '" + group.name.Replace("'", "\\'").Replace("\"", "&quot;") + "')";
                listItem.Show("subtitle");
                html.Append(listItem.Render());
            }
            html.Append("</div>");

            return html.ToString();
        }

        public static string RenderKeys(IRequest request, int groupId, bool canUpdate)
        {
            var viewKey = new View("/Views/Security/key.html");
            var html = new StringBuilder();
            var group = Query.Security.GroupDetails(groupId);

            if (canUpdate)
            {
                var additem = new View("/Views/Security/add-key.html");
                var addbutton = additem.Render();
                html.Append(addbutton);
            }

            foreach (var key in group.Keys)
            {
                viewKey.Clear();
                viewKey["key"] = key.key;
                viewKey["title"] = key.key;
                viewKey["groupid"] = groupId.ToString();
                viewKey["scope"] = key.scope.ToString();
                viewKey["scopeid"] = key.scopeId.ToString();
                viewKey["checked"] = key.enabled == true ? "checked" : "";
                if (canUpdate)
                {
                    viewKey.Show("can-update");
                }
                else
                {
                    viewKey.Show("cant-update");
                }
                html.Append(viewKey.Render());
            }

            return html.ToString();
        }
    }
}
