namespace Kandu.Services
{
    public class User : Service
    {
        public User(Core KanduCore) : base(KanduCore)
        {
        }

        public string Authenticate(string email, string password)
        {
            if (S.User.LogIn(email, password) == true)
            {
                if(S.User.lastboardId == 0)
                {
                    return "success/boards";
                }
                return "success/board/" + S.User.lastboardId + "/" + S.User.lastboardName.Replace(" ","-").ToLower() ;
            }
            return "err";
        }

        public string SaveAdminPassword(string password)
        {
            if (S.Server.resetPass == true)
            {
                S.User.UpdateAdminPassword(password);
                return "success";
            }
            S.Response.StatusCode = 500;
            return "";
        }

        public string CreateAdminAccount(string name, string email, string password)
        {
            if (S.Server.hasAdmin == false && S.Server.environment == Server.enumEnvironment.development)
            {
                S.User.CreateAdminAccount(name, email, password);
                return "success";
            }
            S.Response.StatusCode = 500;
            return "";
        }
    }
}