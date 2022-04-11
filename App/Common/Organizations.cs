using System;
using System.Collections.Generic;
using System.Text;
using Kandu.Core;

namespace Kandu.Common
{
    public static class Organizations
    {

        public static string RenderList(IRequest request, string onclick = "S.orgs.details.show")
        {
            var section = new View("/Views/Organizations/list.html");
            section["list"] = RenderListItems(request, onclick);
            return section.Render();
        }

        public static string RenderListItems(IRequest request, string onclick = "S.orgs.details.show")
        {
            var html = new StringBuilder();
            var item = new View("/Views/Organizations/list-item.html");
            var orgs = Query.Organizations.UserIsPartOf(request.User.UserId);
            foreach (var org in orgs)
            {
                item.Clear();
                item.Bind(new { org });
                item["onclick"] = (onclick != "" ? onclick : "S.orgs.details.show") + "(" + org.orgId + ", '" + org.name + "');S.head.user.hide();";
                html.Append(item.Render());
            }
            return html.ToString();
        }

        public static int Create(IRequest request, string name, string description, string website)
        {
            try
            {
                var id = Query.Organizations.Create(new Query.Models.Organization()
                {
                    ownerId = request.User.UserId,
                    name = name,
                    description = description,
                    website = website,
                    isprivate = false,
                    cardtype = "" //empty by default
                });

                //create new team for organization
                Teams.Create(request, id, "Managers", "Organization Managers");

                //add "owner" security to current user
                request.User.Keys.Add(id, new List<SecurityKey>()
                {
                    new SecurityKey()
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
