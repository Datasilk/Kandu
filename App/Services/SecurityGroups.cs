using System.Text;
using System.Linq;

namespace Kandu.Services
{
    public class SecurityGroups : Service
    {
        public string Create(int orgId, string name)
        {
            if (!CheckSecurity(orgId, Security.Keys.SecGroupCanCreate.ToString())) { return AccessDenied(); } //check security
            Query.Security.CreateGroup(orgId, name);
            return Success();
        }

        public string RefreshList(int orgId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var html = Common.SecurityGroups.RenderList(orgId);
            if (CheckSecurity(orgId, Security.Keys.SecGroupCanCreate.ToString()))
            {
                var additem = new View("/Views/Security/add-item.html");
                var addbutton = additem.Render();
                html = addbutton + html;
            }
            return "<div class=\"grid-items\">" + html + "</div>";
        }

        public string RefreshListForUser(int userId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            return Common.SecurityGroups.RenderListForUser(this, userId);
        }

        public string RenderGroupForm(int orgId, int groupId)
        {
            //form for creating a new security group
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var view = new View("/Views/Security/new-group.html");
            if (groupId != 0)
            {
                //Edit existing Security Group form
                var group = Query.Security.GroupDetails(groupId);
                if (!CheckSecurity(group.orgId, Security.Keys.SecGroupCanCreate.ToString(), Models.Scope.SecurityGroup, groupId))
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
            var canEdit = CheckSecurity(group.orgId, Security.Keys.SecGroupCanEditInfo.ToString(), Models.Scope.SecurityGroup, groupId);
            var canUpdateKeys = CheckSecurity(group.orgId, Security.Keys.SecGroupCanUpdateKeys.ToString(), Models.Scope.SecurityGroup, groupId);
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
            contentHtml.Append("<div class=\"row pad-top content-keys\">" + Common.SecurityGroups.RenderKeys(this, groupId, canUpdateKeys) + "</div>");

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

        public string Update(int groupId, string name)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var group = Query.Security.GroupInfo(groupId);
            var canEdit = CheckSecurity(group.orgId, Security.Keys.SecGroupCanEditInfo.ToString(), Models.Scope.SecurityGroup, groupId);
            if (!canEdit) { return AccessDenied(); }
            Query.Security.UpdateGroup(groupId, name);
            return Success();
        }

        public string ShowAddKey(int groupId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var group = Query.Security.GroupInfo(groupId);
            var canEdit = CheckSecurity(group.orgId, Security.Keys.SecGroupCanEditInfo.ToString(), Models.Scope.SecurityGroup, groupId);
            var canUpdateKeys = CheckSecurity(group.orgId, Security.Keys.SecGroupCanUpdateKeys.ToString(), Models.Scope.SecurityGroup, groupId);
            if (!canUpdateKeys) { return AccessDenied(); }
            var view = new View("/Views/Security/new-key.html");
            var html = new StringBuilder();
            var allkeys = Core.Vendors.Keys.SelectMany(a => a.Keys).Where(a =>
            {
                if(a.RequiredKeys != null && a.RequiredKeys.Length > 0)
                {
                    var passed = true;
                    foreach(var key in a.RequiredKeys)
                    {
                        if (!CheckSecurity(group.orgId, key)){ passed = false; }
                    }
                    if(passed == false && !CheckSecurity(group.orgId, a.Value)) {
                        passed = false;
                    }
                    return passed;
                }
                return false;
            }).ToList();
            view["name"] = group.name;
            view["key-options"] = string.Join('\n',
                allkeys.Select(a => "<option value=\"" + a.Value + "\" data-title=\"" + a.Description + "\"" + 
                        (a.ScopeTypes != null ? " data-scopes=\"" + string.Join(',', a.ScopeTypes.Select(a => (int)a)) + "\"" : "") +
                        ">" + a.Label + "</option>"));
            return view.Render();
        }

        public string AddKey(int groupId, string key, int scope, int scopeId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var group = Query.Security.GroupInfo(groupId);
            var canEdit = CheckSecurity(group.orgId, Security.Keys.SecGroupCanUpdateKeys.ToString(), Models.Scope.SecurityGroup, groupId);
            if (!canEdit) { return AccessDenied(); }
            Query.Security.UpdateKey(group.orgId, groupId, key, true, scope, scopeId);
            return Success();
        }

        public string SaveKey(int groupId, string key, bool ischecked, int scope, int scopeid)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var group = Query.Security.GroupInfo(groupId);
            var canEdit = CheckSecurity(group.orgId, Security.Keys.SecGroupCanUpdateKeys.ToString(), Models.Scope.SecurityGroup, groupId);
            if (!canEdit) { return AccessDenied(); }
            Query.Security.UpdateKey(group.orgId, groupId, key, ischecked, scope, scopeid);
            return Success();
        }

        public string RemoveKey(int groupId, string key, int scope, int scopeId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var group = Query.Security.GroupInfo(groupId);
            var canEdit = CheckSecurity(group.orgId, Security.Keys.SecGroupCanUpdateKeys.ToString(), Models.Scope.SecurityGroup, groupId);
            if (!canEdit) { return AccessDenied(); }
            Query.Security.RemoveKey(group.orgId, groupId, key, scope, scopeId);
            return Success();
        }

        public string GetScopeItems(int groupId, string key, int scope)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var group = Query.Security.GroupInfo(groupId);
            var canEdit = CheckSecurity(group.orgId, Security.Keys.SecGroupCanUpdateKeys.ToString(), Models.Scope.SecurityGroup, groupId);
            if (!canEdit) { return AccessDenied(); }
            if(scope == 0) { return ""; }
            return string.Join("", Query.Security.GetScopeItems(group.orgId, groupId, key, scope)
                .Select(a => "<option value=\"" + a.id + "\">" + a.title + "</option>\n"));
        }
    }
}
