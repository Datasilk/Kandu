using System;

namespace Kandu.Common.Platform
{
    public static class Teams
    {
        public static int Create(Request request, string name, string description = "")
        {
            try
            {
                return Query.Teams.CreateTeam(new Query.Models.Team()
                {
                    name = name,
                    description = description,
                    ownerId = request.User.userId
                });
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error creating new team");
            }
        }
    }
}
