using System;
using System.Collections.Generic;
using System.Text;

namespace Kandu.Common.Platform
{
    public static class Organizations
    {

        public static string RenderOrgListModal(Request request)
        {
            var html = new StringBuilder();
            var section = new View("/Views/Organizations/list.html");
            var item = new View("/Views/Organizations/list-item.html");
            var orgs = Query.Organizations.UserIsPartOf(request.User.userId);
            foreach(var org in orgs)
            {
                item.Clear();
                item.Bind(new { org });
                html.Append(item.Render());
            }
            section["list"] = html.ToString();
            return section.Render();
        }

        public static int Create(Request request, string name, string description, string website)
        {
            try
            {
                var id = Query.Organizations.Create(new Query.Models.Organization()
                {
                    ownerId = request.User.userId,
                    name = name,
                    description = description,
                    website = website,
                    isprivate = false
                });

                //create new team for organization
                Teams.Create(request, id, "Managers", "Organization Managers");

                //add "owner" security to current user
                request.User.Security.Add(id, new Dictionary<string, bool>()
                {
                    {"Owner", true}
                });
                request.User.Save(true);

                return id;
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error creating new organization");
            }
        }
    }
}
