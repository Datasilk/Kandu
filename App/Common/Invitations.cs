using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kandu.Core;
using Utility.Strings;

namespace Kandu.Common
{
    public static class Invitations
    {
        /// <summary>
        /// Sends out invitations via email client to people who will be given access to a
        /// specific security key upon invitation
        /// </summary>
        /// <param name="people">Either userId or valid email address</param>
        /// <param name="scopeId">ID related to scope</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static List<Query.Models.FailedInvite> Send(IRequest request, List<string> people, int orgId, Models.Scope scope, int scopeId, string[] keys)
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
                //create public key
                invite.publickey = Generate.NewId(16);
                emails.Add(invite);
            }

            //save invitations into the database and retrieve any failed invitations
            var failed = Query.Invitations.InvitePeople(request.User.UserId, scopeId, scope, keys, emails.Select(a => new Query.Models.Xml.Invites.Invite()
            {
                UserId = a.userId,
                Email = a.email,
                PublicKey = string.IsNullOrEmpty(a.publickey) ? "" : a.publickey
            }).ToList());

            //get sentence based on scope, scopeId, and security key
            var scopeItem = Query.Security.GetScopeItem((int)scope, scopeId);
            var keyItem = Core.Vendors.Keys.SelectMany(a => a.Keys).Where(a => a.Value == keys[0]).FirstOrDefault();
            var action = Email.GetInfo("invite");
            if(action == null)
            {
                throw new Exception("The \"invite\" email action has not yet been set up in your " + Server.Name + " Settings > Email tab.");
            }
            var message = Email.GetMessage("invite");
            var subject = new View(new ViewOptions() { Html = action.subject });
            var keyItemLabel = keyItem != null ? keyItem.Label.ToLower() : "";
            var scopeKey = (keys.Length > 1 ? keyItemLabel + " & " + (keys.Length - 1) + " more type(s) of" : keyItemLabel) + " access for the ";
            var scopeText = "";
            switch (scope)
            {
                case Models.Scope.All:
                    scopeText = "application-wide administrator access";
                    break;
                case Models.Scope.Organization:
                    scopeText = scopeKey + " organization \"<b>" + scopeItem.title + "</b>\"";
                    break;
                case Models.Scope.SecurityGroup:
                    scopeText = scopeKey + "security group \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
                case Models.Scope.Team:
                    scopeText = (keyItemLabel == "" ? "joining the " : scopeKey) + 
                        "team \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
                case Models.Scope.Board:
                    scopeText = scopeKey + "board \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
                case Models.Scope.List:
                    scopeText = scopeKey + "list \"<b>" + scopeItem.title + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
                case Models.Scope.Card:
                    scopeText = scopeKey + "card \"<b>" + scopeItem.title.MaxChars(32, "...") + "</b>\" in organization \"<b>" + scopeItem.orgName + "</b>\"";
                    break;
            }

            var task = new Task(new Action(() => {
                //send an email out to each person using a separate thread
                foreach (var person in emails)
                {
                    if (failed.Any(a => person.email != "" && person.email == a.email))
                    {
                        //skip all failed invitations (duplicate invites)
                        continue;
                    }

                    //email subject
                    subject.Clear();
                    subject["app-name"] = Server.Name;
                    subject["name"] = person.name;
                    subject["scope"] = scopeText;

                    //email body
                    message.Clear();
                    message["email"] = person.email;
                    message["app-name"] = Server.Name;
                    message["orgid"] = orgId.ToString();
                    message["name"] = person.name;
                    message["scope"] = scopeText;
                    message["invite-url"] = App.Host + "invitation?pk=" + person.publickey;

                    Email.Send(action.fromAddress, person.email, subject.Render(), message.Render(), "invite");
                }
            }));
            task.Start();
            
            return failed;
        }
    }
}
