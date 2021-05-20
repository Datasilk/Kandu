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
    }
}
