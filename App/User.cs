using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Kandu
{

    public class User
    { 

        [JsonIgnore]
        public Core S;
        
        public int userId = 0;
        public string visitorId = "";
        public string email = "";
        public bool active = false;
        public bool photo = false;
        public string name = "";
        public bool isBot = false;
        public bool useAjax = true;
        public bool isMobile = false;
        public bool isTablet = false;

        [JsonIgnore]
        public bool saveSession = false;

        public void Init(Core WebsilkCore)
        {
            S = WebsilkCore;

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
        public bool LogIn(string email, string password, int websiteId, int ownerId)
        {
            Load();
            //var sqlUser = new SqlQueries.User(S);
            var query = new Query.Users(S.SqlConnectionString);
            var dbpass = query.GetPassword(email);
            if(dbpass == "") { return false; }
            if(BCrypt.Net.BCrypt.Verify(password, dbpass))
            {
                //password verified by Bcrypt
                var user = query.AuthenticateUser(email, dbpass);
                if (user != null)
                {
                    userId = user.userId;
                    this.email = email;
                    photo = user.photo;
                    name = user.name;
                    active = user.active;
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
                var bCrypt = new BCrypt.Net.BCrypt();
                var encrypted = BCrypt.Net.BCrypt.HashPassword(password, S.Server.bcrypt_workfactor);
                queryUser.UpdatePassword(adminId, encrypted);
                S.Server.resetPass = false;
            }
            return false;
        }

        #region "security"
        public structSecurityWebsite GetSecurityForWebsite(int userId, int websiteId, int ownerId)
        {
            Load();
            var query = new Query.Security(S.SqlConnectionString);
            var security = new structSecurityWebsite();
            var items = new Dictionary<string, bool[]>();
            security.websiteId = websiteId;
            security.ownerId = ownerId;
            var sec = query.GetSecurity(websiteId, userId);
            if(sec != null)
            {
                foreach(var item in sec)
                {
                    var data = item.security;
                    var d = new string[] { };
                    var b = new List<bool>();
                    if(data != "")
                    {
                        d = data.Split(',');
                        foreach(var v in d)
                        {
                            if(v == "1") { b.Add(true); }else { b.Add(false); }
                        }
                    }
                    items.Add(item.feature, b.ToArray());
                }
            }
            security.security = items;
            return security;
        }

        public bool checkSecurity(int websiteId, string feature, enumSecurity securityIndex)
        {
            Load();
            var i = security.FindIndex(a => a.websiteId == websiteId);
            if(i >= 0)
            {
                var website = security[i];
                if(website.ownerId == userId) { return true; } //website owner
                if (website.security.ContainsKey(feature))
                {
                    var data = website.security[feature];
                    if(data != null)
                    {
                        if(data.Length >= (int)securityIndex + 1)
                        {
                            return data[(int)securityIndex];
                        }
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
