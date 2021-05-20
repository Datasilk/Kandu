using System;

namespace Kandu.Common.Platform
{
    public static class Teams
    {
        public static int Create(Request request, int orgId, string name, string description = "")
        {
            try
            {
                return Query.Teams.Create(new Query.Models.Team()
                {
                    orgId = orgId,
                    name = name,
                    description = description
                }, request.User.userId);
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error creating new team");
            }
        }
    }
}
