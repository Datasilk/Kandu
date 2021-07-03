using System;
using System.Collections.Generic;
using System.Text;

namespace Kandu.Common
{
    public static class Organizations
    {

        public static string RenderOrgListModal(Core.IRequest request)
        {
            var html = new StringBuilder();
            var section = new View("/Views/Organizations/list.html");
            var item = new View("/Views/Organizations/list-item.html");
            var orgs = Query.Organizations.UserIsPartOf(request.User.UserId);
            foreach(var org in orgs)
            {
                item.Clear();
                item.Bind(new { org });
                html.Append(item.Render());
            }
            section["list"] = html.ToString();
            return section.Render();
        }

        public static int Create(Core.IRequest request, string name, string description, string website)
        {
            try
            {
                var id = Query.Organizations.Create(new Query.Models.Organization()
                {
                    ownerId = request.User.UserId,
                    name = name,
                    description = description,
                    website = website,
                    isprivate = false
                });

                //create new team for organization
                Teams.Create(request, id, "Managers", "Organization Managers");

                //add "owner" security to current user
                request.User.Keys.Add(id, new List<Core.Security>()
                {
                    new Core.Security()
                    {
                        Key = "Owner",
                        Enabled = true
                    }
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
