namespace Kandu.Services
{
    public class User : Service
    {
        public string Authenticate(string email, string password)
        {
            var encrypted = Query.Users.GetPassword(email);
            if (!DecryptPassword(email, password, encrypted)) { return Error(); }
            {
                //password verified by Bcrypt
                var user = Query.Users.AuthenticateUser(email, encrypted);
                if (user != null)
                {
                    User.LogIn(user.userId, user.email, user.name, user.datecreated, "", user.photo);
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
    }
}