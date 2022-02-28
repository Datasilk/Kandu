using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility.Strings;
using Kandu.Core;

namespace Kandu.Common
{
    public static class Invitations
    {
        /// <summary>
        /// Sends out invitations via email client to people who will be given access to a
        /// specific security key upon invitation
        /// </summary>
        /// <param name="people"></param>
        /// <param name="scopeId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static string[] Send(IRequest request, List<string> people, int orgId, Models.Scope scope, int scopeId, string key, string message)
        {
            var emails = new List<Models.Invitations.Person>();
            foreach (var person in people)
            {
                var invite = new Models.Invitations.Person();
                if (int.TryParse(person, out var result))
                {
                    //invite person by userId
                    invite.userId = result;
                    if (invite.userId > 0)
                    {
                        var user = Query.Users.GetInfo(invite.userId);
                        invite.email = user.email;
                        invite.name = user.name;
                    }
                }
                else if (person.IsEmail())
                {
                    //invite person by email
                    invite.email = person;
                }
                emails.Add(invite);
            }

            //save invitations into the database and retrieve any failed invitations
            var failed = Query.Invitations.InvitePeople(request.User.UserId, scopeId, scope, emails.Select(a => new Query.Models.Xml.Invites.Invite()
            {
                UserId = a.userId,
                Email = a.email,
                PublicKey = string.IsNullOrEmpty(a.publickey) ? "" : a.publickey
            }).ToList());

            //get sentence based on scope, scopeId, and security key
            var scopeItem = Query.Security.GetScopeItem((int)scope, scopeId);
            var keyItem = Core.Vendors.Keys.SelectMany(a => a.Keys).Where(a => a.Value == key).FirstOrDefault();
            var scopeText = "";
            switch (scope)
            {
                case Models.Scope.All:
                    scopeText = "application-wide administrator access";
                    break;
                case Models.Scope.Organization:
                    scopeText = keyItem.Label + " access for the organization \"<b>" + scopeItem.title + "</b>\"";
                    break;
                case Models.Scope.SecurityGroup:
                    scopeText = keyItem.Label + " access for the security group \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
                case Models.Scope.Team:
                    scopeText = keyItem.Label + " access for the team \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
                case Models.Scope.Board:
                    scopeText = keyItem.Label + " access for the board \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
                case Models.Scope.List:
                    scopeText = keyItem.Label + " access for the list \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
                case Models.Scope.Card:
                    scopeText = keyItem.Label + " access for the card \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
            }

            var task = new Task(new System.Action(() => {
                //send an email out to each person using a separate thread
                foreach (var person in emails)
                {
                    if (failed.Any(a => (person.email != "" && person.email == a) || (person.userId > 0 && person.userId.ToString() == a)))
                    {
                        //skip all failed invitations (duplicate invites)
                        continue;
                    }

                    Email.Send(new System.Net.Mail.MailMessage("", person.email)
                    {
                            Body = message.Replace("{{app-name}}", Server.Name)
                            .Replace("{{name}}", person.name)
                            .Replace("{{public-key}}", person.publickey)
                            .Replace("{{scope}}", scopeText)

                    }, "invitation");
        }
            }));
            
            return failed;
        }
    }
}
