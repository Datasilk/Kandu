using System;

namespace Kandu.Common.Platform
{
    public static class Teams
    {
        public static int Create(Datasilk.Request request, string name, string description = "")
        {
            try
            {
                var query = new Query.Teams();
                return query.CreateTeam(new Query.Models.Team()
                {
                    name = name,
                    description = description,
                    ownerId = request.User.userId,
                    website = "",
                    security = true
                });
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error creating new team");
            }
        }
    }
}
