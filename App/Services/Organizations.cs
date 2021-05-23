using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kandu.Services
{
    public class Organizations : Service
    {
        public string Create(string name, string description = "", string website = "")
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            try
            {
                if(website.Length > 0) 
                { 
                    website = "https://" + website.Replace("http://", "").Replace("https://", "");
                }
                var orgsOwned = Query.Organizations.Owned(User.userId);
                if(orgsOwned.Any(a => a.name.ToLower() == name.ToLower()))
                {
                    return Error("An organization with that name already exists");
                }
                var id = Common.Platform.Organizations.Create(this, name, description, website);
                Common.Platform.Teams.Create(this, id, "Managers", "Organization Managers");
                User.Security.Add(id, new Dictionary<string, bool>()
                {
                    {"owner", true}
                });
                User.Save(true);

                return id.ToString();
            }
            catch (ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
        }

        public string List(string security = "")
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var list = Query.Organizations.UserIsPartOf(User.userId);
            var html = new StringBuilder("{\"orgs\":[");
            var i = 0;
            list.ForEach((Query.Models.Organization o) =>
            {
                if(security == "" || (security != "" && User.CheckSecurity(o.orgId, security)))
                {
                    html.Append((i > 0 ? "," : "") + "{\"name\":\"" + o.name + "\", \"description\":\"" + o.description + "\",\"orgId\":\"" + o.orgId + "\"}");
                }
                i++;
            });
            html.Append("]}");
            return html.ToString();
        }

        public string RefreshListMenu()
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            return Common.Platform.Organizations.RenderOrgListModal(this);
        }

        public string Details(int orgId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var canEdit = CheckSecurity(orgId, Common.Platform.Security.Keys.OrgCanEdit.ToString());
            var tabHtml = new StringBuilder();
            var contentHtml = new StringBuilder();
            var view = new View("/Views/Organizations/details.html");
            var tab = new View("/Views/Organizations/tab.html");
            if (canEdit) 
            {
                view.Show("edit-org");
            }
            else
            {
                view.Show("view-org");
            }

            //load boards tab
            tab["title"] = "Boards";
            tab.Show("selected");
            tabHtml.Append(tab.Render());
            var html = Common.Platform.Boards.RenderBoardMenu(this, orgId, true, false);
            if(html == "")
            {
                //no boards
                var noboards = new View("/Views/Organizations/no-boards.html");
                noboards["orgId"] = orgId.ToString();
                html = noboards.Render();
            }
            contentHtml.Append("<div class=\"content-boards\">" + html + "</div>\n");
            view["tabs"] = tabHtml.ToString();
            view["content"] = contentHtml.ToString();
            return view.Render();
        }
    }
}
