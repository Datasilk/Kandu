using System;
using System.Collections.Generic;
using System.Linq;

namespace Query
{
    public static class Invitations
    {
        public static List<Models.FailedInvite> InvitePeople(int invitedBy, int scopeId = 0, Kandu.Models.Scope scope = Kandu.Models.Scope.All, string[] keys = null, List<Models.Xml.Invites.Invite> invites = null)
        {
            var inviteList = new Models.Xml.Invites() { List = invites.ToArray() };
            return Sql.Populate<Models.FailedInvite>("Invite_People_Batch", new { 
                invitedBy, scopeId, scope = (int)scope, keys = keys == null ? "" : string.Join(',', keys), 
                invites = Common.Serializer.ToXmlDocument(inviteList).OuterXml.Replace("encoding=\"utf-8\"", "") 
            });
        }

        public static Models.Invitation Accept(string email, string publickey)
        {
            return Sql.Populate<Models.Invitation>("Invite_Accept", new {email, publickey}).FirstOrDefault();
        }
    }
}
