using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Kandu.Core;

namespace Kandu
{

    public class User: IUser
    {
        protected HttpContext Context;

        //fields saved into user session
        public int UserId { get; set; } = 0;
        public string VisitorId { get; set; } = "";
        public int OrgId { get; set; }
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public bool Photo { get; set; } = false;
        public DateTime DateCreated { get; set; }
        public Dictionary<int, List<SecurityKey>> Keys { get; set; } = new Dictionary<int, List<SecurityKey>>();
        public bool ResetPass { get; set; } = false;
        public bool KeepMenuOpen { get; set; }
        public bool AllColor { get; set; }
        public List<int> Boards { get; set; } = new List<int>();

        //private fields
        protected bool changed = false;

        //get User object from session
        public static User Get(HttpContext context)
        {
            User user;
            if (context.Session.Get("user") != null)
            {
                user = JsonSerializer.Deserialize<User>(GetString(context.Session.Get("user")));
            }
            else
            {
                user = (User)new User().SetContext(context);
            }
            user.Init(context);
            return user;
        }

        public IUser SetContext(HttpContext context)
        {
            Context = context;
            return this;
        }

        public virtual void Init(HttpContext context)
        {
            //generate visitor id
            Context = context;
            if (VisitorId == "" || VisitorId == null)
            {
                VisitorId = NewId();
                changed = true;
            }
            
            //check for persistant cookie
            if (UserId <= 0 && context.Request.Cookies.ContainsKey("authId"))
            {
                var user = Query.Users.AuthenticateUser(context.Request.Cookies["authId"]);
                if (user != null)
                {
                    //persistant cookie was valid, log in
                    LogIn(user.userId, user.orgId, user.email, user.name, user.datecreated, "", user.photo);
                }
            }
        }

        public void LogIn(int userId, int orgId, string email, string name, DateTime datecreated, string displayName = "", bool photo = false)
        {
            this.UserId = userId;
            this.Email = email;
            this.Photo = photo;
            this.Name = name;
            this.DisplayName = displayName;
            this.DateCreated = datecreated;

            //load security keys for user
            var keys = Query.Security.AllKeysForUser(userId);
            foreach(var key in keys)
            {
                if (Keys.ContainsKey(key.orgId))
                {
                    Keys[key.orgId].Add(new SecurityKey
                    {
                        Key = key.key,
                        Enabled = key.enabled,
                        Scope = Enum.Parse<Models.Scope>(key.scope.ToString()),
                        ScopeId = key.scopeId
                    });
                }
                else
                {
                    Keys.Add(key.orgId, new List<SecurityKey>() {new SecurityKey
                    {
                        Key = key.key,
                        Enabled = key.enabled,
                        Scope = Enum.Parse<Models.Scope>(key.scope.ToString()),
                        ScopeId = key.scopeId
                    }});
                }
            }

            //load Kandu-specific properties for user from database
            var user = Query.Users.GetInfo(userId);
            KeepMenuOpen = user.keepmenu;
            AllColor = user.allcolor;

            Boards = Query.Boards.GetBoardsForMember(userId);
            if (Boards == null) { Boards = new List<int>(); }

            //create persistant cookie
            var auth = Query.Users.CreateAuthToken(userId);
            var options = new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1)
            };

            Context.Response.Cookies.Append("authId", auth, options);

            changed = true;
            Save();
        }

        public void LogOut()
        {
            UserId = 0;
            Email = "";
            Name = "";
            Photo = false;
            changed = true;
            Context.Response.Cookies.Delete("authId");
            Save();
        }

        public void Save(bool changed = false)
        {
            if (this.changed == true && changed == false)
            {
                Context.Session.Set("user", GetBytes(JsonSerializer.Serialize<User>(this)));
                this.changed = false;
            }
            if (changed == true)
            {
                this.changed = true;
            }
        }

        public bool CheckSecurity(int boardId)
        {
            //check Kandu-specific security settings
            if (UserId <= 0) { return false; }
            if (Boards.Contains(boardId))
            {
                return true;
            }
            return false;
        }

        public bool CheckSecurity(int orgId, string key, Models.Scope scope = Models.Scope.All, int scopeId = 0)
        {
            if (Keys.ContainsKey(orgId))
            {
                if(Keys[orgId].Any(a => a.Key == "Owner" && a.Enabled == true)) 
                { 
                    //owner of organization
                    return true; 
                }
                if (Keys[orgId].Any(a => a.Key == key))
                {
                    var orgkeys = Keys[orgId];
                    if (scope != Models.Scope.All)
                    {
                        //specific scope
                        return orgkeys.Any(a => a.Key == key && a.Enabled == true && (a.Scope == Models.Scope.All || (a.Scope == scope && a.ScopeId == scopeId)));
                    }
                    else
                    {
                        //all scopes
                        return orgkeys.Any(a => a.Key == key && a.Enabled == true);
                    }
                }
            }
            return false;
        }

        public bool IsInOrganization(int orgId)
        {
            return Keys.ContainsKey(orgId);
        }

        public void ValidatePassword(string password)
        {
            //set default settings
            var minChars = 6;
            var maxChars = 24;
            var minNumbers = 1;
            var minUppercase = 1;
            var minSpecialChars = 0;
            var noSpaces = false;

            //validate password
            if (password.Length < minChars)
            {
                throw new Exception("Password must be at least " + minChars + " characters in length");
            }
            if (password.Length > maxChars)
            {
                throw new Exception("Password must be less than " + (maxChars + 1) + " characters in length");
            }
            char lastchar = char.MaxValue;
            var numbers = 0;
            var uppercase = 0;
            var special = 0;
            var spaces = 0;
            var consecutives = 0;
            var maxconsecutives = 0;
            for (var x = 0; x < password.Length; x++)
            {
                var p = password[x];
                if (lastchar == p)
                {
                    consecutives++;
                    if (consecutives > maxconsecutives)
                    {
                        maxconsecutives = consecutives;
                    }
                }
                else if (consecutives > 0)
                {
                    consecutives = 0;
                }
                if (char.IsNumber(p)) { numbers++; }
                else if (char.IsUpper(p)) { uppercase++; }
                else if (p == ' ') { spaces++; }
                else if (char.IsLetter(p)) { }
                else { special++; }
            }

            if (numbers < minNumbers)
            {
                throw new Exception("Password must contain at least " + minNumbers + " number" + (minNumbers > 1 ? "s" : ""));
            }
            if (numbers < minUppercase)
            {
                throw new Exception("Password must contain at least " + minUppercase + " uppercase letter" + (minUppercase > 1 ? "s" : ""));
            }
            if (numbers < minSpecialChars)
            {
                throw new Exception("Password must contain at least " + minSpecialChars + " special character" + (minSpecialChars > 1 ? "s" : ""));
            }
            if (noSpaces && spaces > 0)
            {
                throw new Exception("Password cannot contain spaces");
            }
        }

        #region "Helpers"

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return string.Join("", chars);
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string NewId(int length = 3)
        {
            string result = "";
            for (var x = 0; x <= length - 1; x++)
            {
                int type = new Random().Next(1, 3);
                int num;
                switch (type)
                {
                    case 1: //a-z
                        num = new Random().Next(0, 26);
                        result += (char)('a' + num);
                        break;

                    case 2: //A-Z
                        num = new Random().Next(0, 26);
                        result += (char)('A' + num);
                        break;

                    case 3: //0-9
                        num = new Random().Next(0, 9);
                        result += (char)('1' + num);
                        break;

                }

            }
            return result;
        }

        #endregion
    }
}