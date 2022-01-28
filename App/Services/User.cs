using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kandu.Services
{
    public class User : Service
    {
        #region "Authentication"
        public string Authenticate(string email, string password)
        {
            var encrypted = Query.Users.GetPassword(email);
            try
            {
                if (!DecryptPassword(email, password, encrypted)) { return Error(); }
            }
            catch (Exception)
            {
                return Error();
            }
            var user = Query.Users.AuthenticateUser(email, encrypted);
            if (user != null)
            {
                User.LogIn(user.userId, user.orgId, user.email, user.name, user.datecreated, "", user.photo);
                User.Save(true);

                if (user.lastboard == 0)
                {
                    return "boards";
                }
                return "board/" + user.lastboard + "/" + user.lastboardName.Replace(" ", "-").ToLower();
            }
            return Error("Incorrect email and/or password");
        }

        public string SaveAdminPassword(string password)
        {
            if (Server.ResetPass == true)
            {
                var update = false; //security check
                var emailAddr = "";
                var adminId = 1;
                if (Server.ResetPass == true)
                {
                    //securely change admin password
                    //get admin email address from database
                    emailAddr = Query.Users.GetEmail(adminId);
                    if (emailAddr != "" && emailAddr != null) { update = true; }
                }
                if (update == true)
                {
                    Query.Users.UpdatePassword(adminId, EncryptPassword(emailAddr, password));
                    Server.ResetPass = false;
                }
                return Success();
            }
            return Error();
        }

        public string CreateAdminAccount(string name, string email, string password)
        {
            if (Server.HasAdmin == false && App.Environment == Environment.development)
            {
                //validate email
                if (!Utility.Strings.Validation.IsEmail(email))
                {
                    return Error("Please provide a valid email address");
                }
                //validate password strength
                try
                {
                    User.ValidatePassword(password);
                }
                catch (Exception ex)
                {
                    return Error(ex.Message);
                }
                //check name
                if (string.IsNullOrEmpty(name))
                {
                    return Error("Please provide your name");
                }

                Query.Users.CreateUser(new Query.Models.User()
                {
                    name = name,
                    email = email,
                    password = EncryptPassword(email, password)
                });
                Server.HasAdmin = true;
                Server.ResetPass = false;
                return "success";
            }
            return Error();
        }

        public string CreateAccount(string name, string email, string password)
        {
            //validate email
            if (!Utility.Strings.Validation.IsEmail(email)) { 
                return Error("Please provide a valid email address"); 
            }
            //validate password strength
            try
            {
                User.ValidatePassword(password);
            }catch(Exception ex)
            {
                return Error(ex.Message);
            }
            //check name
            if (string.IsNullOrEmpty(name))
            {
                return Error("Please provide your name");
            }

            //finally, create user
            Query.Users.CreateUser(new Query.Models.User()
            {
                name = name,
                email = email,
                password = EncryptPassword(email, password)
            });
            Server.HasAdmin = true;
            Server.ResetPass = false;
            return "success";
        }

        public void LogOut()
        {
            User.LogOut();
        }

        private static string EncryptPassword(string email, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(email + Server.Salt + password, Server.BcryptWorkFactor);
        }

        private static bool DecryptPassword(string email, string password, string encrypted)
        {
            return BCrypt.Net.BCrypt.Verify(email + Server.Salt + password, encrypted);
        }
        #endregion

        #region "Details"

        public string Details(int orgId, int userId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var user = Query.Users.GetInfo(userId);
            var canEdit = userId == User.UserId;
            var tabHtml = new StringBuilder();
            var contentHtml = new StringBuilder();
            var view = new View("/Views/User/details.html");
            var tab = new View("/Views/Shared/tab.html");

            //load cards tab
            tab["title"] = "Cards";
            tab["id"] = "cards";
            tab["onclick"] = "S.user.details.tabs.select('cards')";
            tab.Show("selected");
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-cards kanban\">" + RenderCards(orgId, userId, 1, 20) + "</div>");

            //load boards tab
            tab.Clear();
            tab["title"] = "Boards";
            tab["id"] = "boards";
            tab["onclick"] = "S.user.details.tabs.select('boards')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-boards pad-top\"></div>");

            //load teams tab
            tab.Clear();
            tab["title"] = "Teams";
            tab["id"] = "teams";
            tab["onclick"] = "S.user.details.tabs.select('teams')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-teams pad-top\"></div>");

            //load account tab
            if(userId == User.UserId)
            {
                tab.Clear();
                tab["title"] = "Account";
                tab["id"] = "account";
                tab["onclick"] = "S.user.details.tabs.select('account')";
                tabHtml.Append(tab.Render());
                contentHtml.Append("<div class=\"content-account pad-top\"></div>");
            }

            //load security tab
            tab.Clear();
            tab["title"] = "Security Groups";
            tab["id"] = "security";
            tab["onclick"] = "S.user.details.tabs.select('security')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-security pad-top\"></div>");

            //load email settings tab
            tab.Clear();
            tab["title"] = "Emails";
            tab["id"] = "email-settings";
            tab["onclick"] = "S.user.details.tabs.select('email-settings')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-email-settings pad-top\"></div>");

            view["name"] = user.name;
            view["email"] = user.email;
            if (canEdit || CheckSecurity(orgId, Security.Keys.OrgCanViewMemberEmailAddr.ToString(), Models.Scope.Organization, orgId))
            {
                view.Show("has-email");
            }
            view["tabs"] = tabHtml.ToString();
            view["content"] = contentHtml.ToString();
            return view.Render();
        }

        public string RenderCards(int orgId, int userId, int start = 1, int length = 20)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var view = new View("/Views/User/cards.html");
            var user = Query.Users.GetInfo(userId);
            view["username"] = user.name;
            var cards = Common.Card.Kanban.RenderCardsForMember(this, userId, orgId, start, length);
            if(cards.Count > 0)
            {
                var html = new StringBuilder();
                html.Append("<div class=\"col six\"><div class=\"list\"><div class=\"items\">");
                for (var x = 0; x < cards.Count / 2; x++)
                {
                    html.Append(cards[x]);
                }
                html.Append("</div></div></div>");
                if (cards.Count > 1)
                {
                    html.Append("<div class=\"col six\"><div class=\"list\"><div class=\"items\">");
                    for (var x = cards.Count / 2; x < cards.Count; x++)
                    {
                        html.Append(cards[x]);
                    }
                    html.Append("</div></div></div>");
                }
                view["cards"] = html.ToString();
            }
            else
            {
                //no cards found
                view.Show("no-cards");
            }
            
            return view.Render();
        }

        public string RefreshBoards(int userId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            return Common.Boards.RenderList(this);
        }

        public string RefreshTeams(int userId)
        { 
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var listItem = new View("/Views/Teams/list-item.html");
            var titlebar = new View("/Views/Organizations/title-bar.html");
            var html = new StringBuilder();
            var teams = Query.Teams.GetAllForUser(User.UserId, userId).OrderBy(a => a.orgId).ThenBy(a => a.groupId);
            var lastOrgId = 0;
            foreach (var team in teams)
            {
                if (lastOrgId != team.orgId)
                {
                    if (lastOrgId > 0) { html.Append("</div>"); }
                    lastOrgId = team.orgId;
                    titlebar.Clear();
                    titlebar["org-name"] = team.orgName;
                    titlebar["org-id"] = team.orgId.ToString();
                    if (team.ownerId == userId) { titlebar.Show("is-owner"); }
                    html.Append(titlebar.Render() + "\n<div class=\"grid-items\">");
                }
                listItem.Clear();
                listItem.Bind(new { team });
                listItem.Show("subtitle");
                if (team.totalMembers != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.user.teams.details(" + team.orgId + ", " + team.teamId + ", '" + team.name.Replace("'", "\\'").Replace("\"", "&quot;") + "')";
                listItem.Show("subtitle");
                html.Append(listItem.Render());
            }
            if(teams.Count() > 0)
            {
                html.Append("</div>");
            }
            else
            {
                html.Append("This user is not a part of any teams");
            }

            return html.ToString();
        }

        public string RefreshAccount(int userId)
        {
            if (userId != User.UserId) { return AccessDenied(); } //check security
            var view = new View("/Views/User/account.html");
            var user = Query.Users.GetInfo(userId);
            view.Bind(new { user });
            return view.Render();
        }

        public string RefreshEmailSettings(int userId)
        {
            if (userId != User.UserId) { return AccessDenied(); } //check security
            var view = new View("/Views/User/email-settings.html");
            var html = new StringBuilder();

            //get email settings from plugins
            var partials = Common.PartialViews.GetList(Vendor.PartialViewKeys.User_EmailSettings);
            if(partials.Count > 0)
            {
                //render each partial view
                var data = new Dictionary<string, object>()
                {
                    {"userId", userId }
                };
                foreach (var partial in partials)
                {
                    html.Append(partial.Render(this, data));
                }
            }
            
            view.Show("no-settings");
            return view.Render();
        }

        public string UpdateInfo(int userId, string name, string email, string oldpass, string newpass1, string newpass2)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            if(userId == User.UserId)
            {
                //only users can change their own user information
                var user = Query.Users.GetInfo(userId);

                if (email != "" && user.email != email)
                {
                    //validate email
                    if (!Utility.Strings.Validation.IsEmail(email))
                    {
                        return Error("Please provide a valid email address");
                    }
                    //authenticate user
                    if (!DecryptPassword(user.email, oldpass, user.password)) { 
                        return Error("Incorrect password"); 
                    }

                    Query.Users.UpdateEmail(userId, email, EncryptPassword(email, oldpass));
                }

                if(newpass1 != "")
                {
                    //authenticate user
                    if (!DecryptPassword(user.email, oldpass, user.password))
                    {
                        return Error("Incorrect password");
                    }
                    //update password
                    if (newpass1 != newpass2) { return Error("Passwords do not match"); }
                    try
                    {
                        User.ValidatePassword(newpass1);
                    }
                    catch (Exception ex)
                    {
                        return Error(ex.Message);
                    }
                    Query.Users.UpdatePassword(userId, EncryptPassword(email, newpass1));
                }

                //update other info
                if(name != "" && name != user.name)
                {
                    Query.Users.UpdateName(userId, name);
                }
            }
            return Success();
        }
        #endregion
    }
}