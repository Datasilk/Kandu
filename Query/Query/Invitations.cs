using System;
using System.Collections.Generic;

namespace Query
{
    public static class Invitations
    {
        public static string[] InvitePeople(int invitedBy, int scopeId = 0, Kandu.Models.Scope scope = Kandu.Models.Scope.All, string message = "", List<Models.Xml.Invites.Invite> invites = null)
        {
            var inviteList = new Models.Xml.Invites() { List = invites.ToArray() };
            var errors = Sql.ExecuteScalar<string>("Invite_People_Batch", new { invitedBy, scopeId, scope = (int)scope, message, invites = Common.Serializer.ToXmlDocument(inviteList).OuterXml });
            return string.IsNullOrEmpty(errors) ? new string[] { } : errors.Split(",", StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
