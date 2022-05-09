using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
                var orgsOwned = Query.Organizations.Owned(User.UserId);
                if(orgsOwned.Any(a => a.name.ToLower() == name.ToLower()))
                {
                    return Error("An organization with that name already exists");
                }
                var id = Common.Organizations.Create(this, name, description, website);

                return id.ToString();
            }
            catch (ServiceErrorException ex)
            {
                return Error(ex.Message);
            }
        }

        public string List()
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var list = Query.Organizations.UserIsPartOf(User.UserId);
            var html = new StringBuilder("{\"orgs\":[");
            var i = 0;
            list.ForEach((Query.Models.Organization o) =>
            {
                if(User.CheckSecurity(o.orgId, Security.Keys.BoardCanCreate.ToString()))
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
            return Common.Organizations.RenderList(this);
        }

        public string Details(int orgId)
        {
            if (!CheckSecurity() || !User.IsInOrganization(orgId)) { return AccessDenied(); } //check security
            
            var canEdit = CheckSecurity(orgId, Security.Keys.OrgCanEditInfo.ToString(), Models.Scope.Organization, orgId);
            var tabHtml = new StringBuilder();
            var contentHtml = new StringBuilder();
            var view = new View("/Views/Organizations/details.html");
            var tab = new View("/Views/Shared/tab.html");
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
            if(org.website != "") { view.Show("has-website"); }

            //load boards tab
            tab["title"] = "Boards";
            tab["id"] = "boards";
            tab["onclick"] = "S.orgs.details.tabs.select('boards')";
            tab.Show("selected");
            tabHtml.Append(tab.Render());
            var html = Common.Boards.RenderSideBar(this, orgId, true, false);
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
            tab["onclick"] = "S.orgs.details.tabs.select('teams')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-teams\"></div>\n");

            //load members tab
            tab.Clear();
            tab["title"] = "Members";
            tab["id"] = "members";
            tab["onclick"] = "S.orgs.details.tabs.select('members')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-members\"></div>\n");

            //load security tab
            if(CheckSecurity(orgId, Security.Keys.SecGroupsCanViewAll.ToString()) ||
                CheckSecurity(orgId, Security.Keys.SecGroupCanCreate.ToString()) ||
                CheckSecurity(orgId, Security.Keys.SecGroupCanUpdateKeys.ToString()) ||
                CheckSecurity(orgId, Security.Keys.SecGroupCanAddUsers.ToString()) ||
                CheckSecurity(orgId, Security.Keys.SecGroupCanRemoveUsers.ToString()) ||
                CheckSecurity(orgId, Security.Keys.SecGroupCanEditInfo.ToString()))
            {
                tab.Clear();
                tab["title"] = "Security";
                tab["id"] = "security";
                tab["onclick"] = "S.orgs.details.tabs.select('security')";
                tabHtml.Append(tab.Render());
                contentHtml.Append("<div class=\"content-security\"></div>\n");
            }

            if(CheckSecurity(orgId, Security.Keys.OrgCanEditSettings.ToString()))
            {
                //load settings tab
                tab.Clear();
                tab["title"] = "Settings";
                tab["id"] = "settings";
                tab["onclick"] = "S.orgs.details.tabs.select('settings')";
                tabHtml.Append(tab.Render());
                contentHtml.Append("<div class=\"content-settings pad-top\"></div>\n");
            }

            if (CheckSecurity(orgId, Security.Keys.OrgCanEditTheme.ToString()))
            {
                //load theme tab
                tab.Clear();
                tab["title"] = "Theme";
                tab["id"] = "theme";
                tab["onclick"] = "S.orgs.details.tabs.select('theme')";
                tabHtml.Append(tab.Render());
                contentHtml.Append("<div class=\"content-theme pad-top\"></div>\n");
            }

            view["tabs"] = tabHtml.ToString();
            view["content"] = contentHtml.ToString();
            return view.Render();
        }

        public string Update(int orgId, string name, string description, string website)
        {
            if(!CheckSecurity(orgId, new string[] { Security.Keys.OrgCanEditInfo.ToString() }, Models.Scope.Organization, orgId)) { return AccessDenied(); }
            if (website.Length > 0)
            {
                website = "https://" + website.Replace("http://", "").Replace("https://", "");
            }
            var orgsOwned = Query.Organizations.Owned(User.UserId);
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

        #region "Organization Settings"

        public string RefreshSettings(int orgId)
        {
            if (!CheckSecurity(orgId, new string[] { Security.Keys.OrgCanEditSettings.ToString() }, Models.Scope.Organization, orgId)) { return AccessDenied(); } //check security
            var org = Query.Organizations.GetInfo(orgId);
            var groups = Query.Security.GetGroups(orgId, User.UserId);
            var view = new View("/Views/Organizations/settings.html");
            view.Bind(new { org });
            if (org.groupId == null || org.groupId <= 0)
            {
                view["groups"] = "<option value=\"0\" selected>Select A Security Group...</option>\n";
            }
            view["groups"] += string.Join("\n", groups.Select(a => "<option value=\"" + a.groupId + "\"" + (org.groupId == a.groupId ? " selected" : "") + ">" + a.name + "</option>"));

            //default card type
            var html = new StringBuilder();
            var cards = new List<string>() { "Basic" };
            cards.AddRange(Core.Vendors.Cards.Keys);
            html.Append("<option value=\"\">No Default</option>\n");
            foreach (var key in cards)
            {
                html.Append("<option value=\"" + key + "\" " + (org.cardtype == key ? "selected" : "") + ">" + key + "</option>\n");
            }
            view["cardtypes"] = html.ToString();

            //TODO: Render vendor plugin partial views for organization settings 
            return view.Render();
        }

        public string SaveSettings(int orgId, Dictionary<string, string> parameters)
        {
            if (!CheckSecurity(orgId, new string[] { Security.Keys.OrgCanEditSettings.ToString() }, Models.Scope.Organization, orgId)) { return AccessDenied(); } //check security
            var groupId = 0;
            var cardtype = parameters.ContainsKey("org_cardtype") ? parameters["org_cardtype"] : "";
            if (parameters.ContainsKey("org_groupid")) int.TryParse(parameters["org_groupid"], out groupId);
            try
            {
                //save org settings
                Query.Organizations.UpdateSettings(orgId, groupId, cardtype);
            }
            catch (Exception)
            {
                return Error("Error saving organization settings");
            }
            try
            {
                //send parameters to all related Partial Views 
                var data = new Dictionary<string, object>()
                {
                    {"orgId", orgId }
                };
                Common.PartialViews.Save(this, parameters, Vendor.PartialViewKeys.Organization_Settings, data);
            }
            catch (Exception)
            {
                return Error("Error saving plugin data for organization settings");
            }
            return Success();
        }

        #endregion

        #region "Organization Theme"

        public string RefreshTheme(int orgId)
        {
            if (!CheckSecurity(orgId, new string[] { Security.Keys.OrgCanEditTheme.ToString() }, Models.Scope.Organization, orgId)) { return AccessDenied(); } //check security
            var org = Query.Organizations.GetInfo(orgId);
            var path = "/themes/orgs/" + orgId + "/";
            var view = new View("/Views/Organizations/theme.html");
            view["css-file"] = org.customCss ? "<a href=\"" + path + "theme.css\" target=\"_blank\">" + path + "theme.css</a>" : "";
            view["js-file"] = org.customJs ? "<a href=\"" + path + "theme.js\" target=\"_blank\">" + path + "theme.js</a>" : "";
            //get list of files from wwwroot/themes
            if(Directory.Exists(App.MapPath("/wwwroot" + path)))
            {
                var viewItem = new View("/Views/Organizations/resource-item.html");
                var exclude = new List<string> { "theme.css", "theme.js" };
                var files = new DirectoryInfo(App.MapPath("/wwwroot" + path)).GetFiles().Where(a => !exclude.Contains(a.Name)).ToList();
                if(files.Count > 0)
                {
                    var html = new StringBuilder();
                    foreach (var file in files)
                    {
                        viewItem.Clear();
                        viewItem["id"] = file.Name;
                        viewItem["url"] = path + file.Name;
                        viewItem["filename"] = file.Name;
                        html.Append(viewItem.Render());
                    }
                    view["resources"] = Common.Accordion.Render("Theme Resources", html.ToString(), "resources", "icon-file-jpg", "", true);
                }
            }
            return view.Render();
        }

        public string DeleteThemeResource(int orgId, string filename)
        {
            if (!CheckSecurity(orgId, new string[] { Security.Keys.OrgCanEditTheme.ToString() }, Models.Scope.Organization, orgId)) { return AccessDenied(); } //check security
            var path = "/themes/orgs/" + orgId + "/";
            try
            {
                File.Delete(App.MapPath("/wwwroot/" + path + filename));
            }
            catch(Exception ex)
            {
                return Error("Could not delete resource");
            }
            return Success();
        }

        #endregion
    }
}
