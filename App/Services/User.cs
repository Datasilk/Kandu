using System;
using System.Text;

namespace Kandu.Services
{
    public class User : Service
    {
        #region "Authentication"
        public string Authenticate(string email, string password)
        {
            var encrypted = Query.Users.GetPassword(email);
            if (!DecryptPassword(email, password, encrypted)) { return Error(); }
            {
                //password verified by Bcrypt
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

            //load organizations tab
            tab.Clear();
            tab["title"] = "Organizations";
            tab["id"] = "orgs";
            tab["onclick"] = "S.user.details.tabs.select('orgs')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-orgs pad-top\"></div>");

            //load security tab
            tab.Clear();
            tab["title"] = "Security Groups";
            tab["id"] = "security";
            tab["onclick"] = "S.user.details.tabs.select('security')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-security pad-top\"></div>");

            //load email settings tab
            tab.Clear();
            tab["title"] = "Email Settings";
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
        #endregion
    }
}