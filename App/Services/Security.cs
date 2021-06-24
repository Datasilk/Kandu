using System.Text;

namespace Kandu.Services
{
    public class Security : Service
    {
        public string RefreshList(int orgId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var html = Common.Platform.Security.RenderList(this, orgId);
            if (CheckSecurity(orgId, Common.Platform.Security.Keys.SecGroupCanCreate))
            {
                var additem = new View("/Views/Security/add-item.html");
                var addbutton = additem.Render();
                html = html + addbutton;
            }
            return html;
        }

        public string Details (int groupId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var group = Query.Security.GroupInfo(groupId);
            var canEdit = CheckSecurity(group.orgId, Common.Platform.Security.Keys.SecGroupCanEditInfo, Common.Platform.Security.Scope.SecurityGroup, groupId);
            var tabHtml = new StringBuilder();
            var contentHtml = new StringBuilder();
            var view = new View("/Views/Security/details.html");
            var tab = new View("/Views/Shared/tab.html");
            var html = new StringBuilder();

            //load security keys tab
            tab["title"] = "Security Keys";
            tab["id"] = "keys";
            tab["onclick"] = "S.teams.details.tabs.select('keys')";
            tab.Show("selected");
            tabHtml.Append(tab.Render());
            //TODO: load security keys
            contentHtml.Append("<div class=\"content-members\">" + html.ToString() + "</div>");

            view["name"] = group.name;
            view["tabs"] = tabHtml.ToString();
            view["content"] = contentHtml.ToString();
            if (canEdit)
            {
                view.Show("can-edit");
            }
            else
            {
                view.Show("no-edit");
            }
            return view.Render();
        }
    }
}
