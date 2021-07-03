using System.Text;

namespace Kandu.Services
{
    public class Security : Service
    {
        public string Create(int orgId, string name)
        {
            if (!CheckSecurity(orgId, Models.Security.Keys.SecGroupCanCreate.ToString())) { return AccessDenied(); } //check security
            Query.Security.CreateGroup(orgId, name);
            return Success();
        }

        public string RefreshList(int orgId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var html = "<div class=\"grid-items\">" + Common.Security.RenderList(this, orgId);
            if (CheckSecurity(orgId, Models.Security.Keys.SecGroupCanCreate.ToString()))
            {
                var additem = new View("/Views/Security/add-item.html");
                var addbutton = additem.Render();
                html = html + addbutton;
            }
            return html + "</div>";
        }

        public string RenderGroupForm(int orgId, int groupId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var view = new View("/Views/Security/new-group.html");
            if (groupId != 0)
            {
                //Edit existing Security Group form
                var group = Query.Security.GroupDetails(groupId);
                if (!CheckSecurity(group.orgId, Models.Security.Keys.SecGroupCanCreate.ToString(), Models.Scope.SecurityGroup, groupId))
                {
                    return AccessDenied();
                }
                view["name"] = group.name;
                view["submit-label"] = "Update Security Group";
                view["submit-click"] = "S.security.add.submit('" + groupId + "')";
            }
            else
            {
                //Create Security Group form
                view["submit-label"] = "Create Security Group";
                view["submit-click"] = "S.security.add.submit()";
            }
            return view.Render();
        }

        public string Details (int groupId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var group = Query.Security.GroupInfo(groupId);
            var canEdit = CheckSecurity(group.orgId, Models.Security.Keys.SecGroupCanEditInfo.ToString(), Models.Scope.SecurityGroup, groupId);
            var tabHtml = new StringBuilder();
            var contentHtml = new StringBuilder();
            var view = new View("/Views/Security/details.html");
            var tab = new View("/Views/Shared/tab.html");
            var html = new StringBuilder();

            //load security keys tab
            tab["title"] = "Security Keys";
            tab["id"] = "keys";
            tab["onclick"] = "S.security.details.tabs.select('keys')";
            tab.Show("selected");
            tabHtml.Append(tab.Render());
            
            //load all available security keys


            contentHtml.Append("<div class=\"content-keys\">" + html.ToString() + "</div>");

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
