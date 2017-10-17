using System;
using Newtonsoft.Json;

namespace Kandu
{

    public class User
    { 

        [JsonIgnore]
        public Core S;
        [JsonIgnore]
        private string salt = "";
        
        public int userId = 0;
        public string visitorId = "";
        public string email = "";
        public string name = "";
        public bool photo = false;
        public bool isBot = false;
        public bool useAjax = true;
        public bool isMobile = false;
        public bool isTablet = false;
        public DateTime datecreated;
        public int lastboardId = 0;
        public string lastboardName = "";

        [JsonIgnore]
        public bool saveSession = false;

        public void Init(Core KanduCore)
        {
            S = KanduCore;

            //generate visitor id
            if (visitorId == "" || visitorId == null) { visitorId = S.Util.Str.CreateID(); saveSession = true; }
        }

        public virtual void Load()
        { 
        }

        /// <summary>
        /// Authenticate user credentials and log into user account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool LogIn(string email, string password)
        {
            Load();
            //var sqlUser = new SqlQueries.User(S);
            var query = new Query.Users(S.SqlConnectionString);
            var encrypted = query.GetPassword(email);
            if(!DecryptPassword(email, password, encrypted)) { return false; }
            {
                //password verified by Bcrypt
                var user = query.AuthenticateUser(email, encrypted);
                if (user != null)
                {
                    userId = user.userId;
                    this.email = email;
                    photo = user.photo;
                    name = user.name;
                    datecreated = user.datecreated;
                    lastboardId = user.lastboard;
                    lastboardName = user.lastboardName != null ? user.lastboardName : "";
                    saveSession = true;
                    return true;
                }
            }
            
            return false;
        }

        public void LogOut()
        {
            Load();
            saveSession = true;
            S.Session.Remove("user");
        }

        public string EncryptPassword(string email, string password)
        {
            var bCrypt = new BCrypt.Net.BCrypt();
            return BCrypt.Net.BCrypt.HashPassword(email + salt + password, S.Server.bcrypt_workfactor);

        }

        public bool DecryptPassword(string email, string password, string encrypted)
        {
            return BCrypt.Net.BCrypt.Verify(email + salt + password, encrypted);
        }



        public bool UpdateAdminPassword(string password)
        {
            Load();
            var update = false; //security check
            var emailAddr = email;
            var queryUser = new Query.Users(S.SqlConnectionString);
            var adminId = 1;
            if (S.Server.resetPass == true)
            {
                //securely change admin password
                //get admin email address from database
                emailAddr = queryUser.GetEmail(adminId);
                if (emailAddr != "" && emailAddr != null) { update = true; }
            }
            if(update == true)
            {
                queryUser.UpdatePassword(adminId, EncryptPassword(emailAddr, password));
                S.Server.resetPass = false;
            }
            return false;
        }

        public void CreateAdminAccount(string name, string email, string password)
        {
            Load();
            var queryUser = new Query.Users(S.SqlConnectionString);
            queryUser.CreateUser(new Query.Models.User()
            {
                name=name,
                email=email,
                password = EncryptPassword(email, password)
            });
            S.Server.hasAdmin = true;
            S.Server.resetPass = false;
        }
    }
}
