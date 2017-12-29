namespace Kandu.Services
{
    public class User : Service
    {
        public User(Core LegendaryCore) : base(LegendaryCore)
        {
        }

        public string Authenticate(string email, string password)
        {

            //var sqlUser = new SqlQueries.User(S);
            var query = new Query.Users(S.Server.sqlConnectionString);
            var encrypted = query.GetPassword(email);
            if (!DecryptPassword(email, password, encrypted)) { return Error(); }
            {
                //password verified by Bcrypt
                var user = query.AuthenticateUser(email, encrypted);
                if (user != null)
                {
                    S.User.userId = user.userId;
                    S.User.email = email;
                    S.User.photo = user.photo;
                    S.User.name = user.name;
                    S.User.datecreated = user.datecreated;
                    S.User.saveSession = true;

                    if (user.lastboard == 0)
                    {
                        return "success|boards";
                    }
                    return "success|board/" + user.lastboard + "/" + user.lastboardName.Replace(" ", "-").ToLower();
                }
            }
            return Error();
        }

        public string SaveAdminPassword(string password)
        {
            if (S.Server.resetPass == true)
            {
                var update = false; //security check
                var emailAddr = "";
                var queryUser = new Query.Users(S.Server.sqlConnectionString);
                var adminId = 1;
                if (S.Server.resetPass == true)
                {
                    //securely change admin password
                    //get admin email address from database
                    emailAddr = queryUser.GetEmail(adminId);
                    if (emailAddr != "" && emailAddr != null) { update = true; }
                }
                if (update == true)
                {
                    queryUser.UpdatePassword(adminId, EncryptPassword(emailAddr, password));
                    S.Server.resetPass = false;
                }
                return Success();
            }
            S.Response.StatusCode = 500;
            return "";
        }

        public string CreateAdminAccount(string name, string email, string password)
        {
            if (S.Server.hasAdmin == false && S.Server.environment == Server.enumEnvironment.development)
            {
                var queryUser = new Query.Users(S.Server.sqlConnectionString);
                queryUser.CreateUser(new Query.Models.User()
                {
                    name = name,
                    email = email,
                    password = EncryptPassword(email, password)
                });
                S.Server.hasAdmin = true;
                S.Server.resetPass = false;
                return "success";
            }
            S.Response.StatusCode = 500;
            return "";
        }

        public void LogOut()
        {
            S.User.LogOut();
        }

        public string EncryptPassword(string email, string password)
        {
            var bCrypt = new BCrypt.Net.BCrypt();
            return BCrypt.Net.BCrypt.HashPassword(email + S.Server.salt + password, S.Server.bcrypt_workfactor);

        }

        public bool DecryptPassword(string email, string password, string encrypted)
        {
            return BCrypt.Net.BCrypt.Verify(email + S.Server.salt + password, encrypted);
        }
    }
}