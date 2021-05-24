using System;
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
            var sec = Enum.Parse<Common.Platform.Security.Keys>(security);
            list.ForEach((Query.Models.Organization o) =>
            {
                if(security == "" || (security != "" && User.CheckSecurity(o.orgId, sec)))
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
            var canEdit = CheckSecurity(orgId, Common.Platform.Security.Keys.OrgCanEdit);
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

            //load org info
            var org = Query.Organizations.GetInfo(orgId);
            view["name"] = org.name;
            view["description"] = org.description;
            view["website"] = org.website;

            //load boards tab
            tab["title"] = "Boards";
            tab["id"] = "boards";
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

            //load teams tab
            tab.Clear();
            tab["title"] = "Teams";
            tab["id"] = "teams";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-teams\"></div>\n");

            //load members tab
            tab.Clear();
            tab["title"] = "Members";
            tab["id"] = "members";
            tabHtml.Append(tab.Render());
            html = "";
            contentHtml.Append("<div class=\"content-members\"></div>\n");

            //load following tab
            tab.Clear();
            tab["title"] = "Following";
            tab["id"] = "following";
            tabHtml.Append(tab.Render());
            html = "";
            contentHtml.Append("<div class=\"content-following\"></div>\n");


            view["tabs"] = tabHtml.ToString();
            view["content"] = contentHtml.ToString();
            return view.Render();
        }

        public string Update(int orgId, string name, string description, string website)
        {
            if (website.Length > 0)
            {
                website = "https://" + website.Replace("http://", "").Replace("https://", "");
            }
            var orgsOwned = Query.Organizations.Owned(User.userId);
            if (orgsOwned.Any(a => a.name.ToLower() == name.ToLower() && a.orgId != orgId))
            {
                return Error("An organization with that name already exists");
            }

            try
            {
                Query.Organizations.Update(new Query.Models.Organization()
                {
                    orgId = orgId,
                    name = name,
                    description = description,
                    website = website
                });
                return Success();
            }
            catch (Exception)
            {
                return Error();
            }
        }
    }
}
