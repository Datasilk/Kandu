using System;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Kandu
{
    public class User
    {
        protected HttpContext Context;

        //fields saved into user session
        public int userId { get; set; } = 0;
        public string visitorId { get; set; } = "";
        public string email { get; set; } = "";
        public string name { get; set; } = "";
        public string displayName { get; set; } = "";
        public bool photo { get; set; } = false;
        public DateTime datecreated { get; set; }
        public Dictionary<int, Dictionary<string, bool>> Security { get; set; } = new Dictionary<int, Dictionary<string, bool>>();
        public bool resetPass { get; set; } = false;
        public bool keepMenuOpen { get; set; }
        public bool allColor { get; set; }
        public List<int> boards { get; set; } = new List<int>();

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
                user = new User().SetContext(context);
            }
            user.Init(context);
            return user;
        }

        public User SetContext(HttpContext context)
        {
            Context = context;
            return this;
        }

        public virtual void Init(HttpContext context)
        {
            //generate visitor id
            Context = context;
            if (visitorId == "" || visitorId == null)
            {
                visitorId = NewId();
                changed = true;
            }
            
            //check for persistant cookie
            if (userId <= 0 && context.Request.Cookies.ContainsKey("authId"))
            {
                var user = Query.Users.AuthenticateUser(context.Request.Cookies["authId"]);
                if (user != null)
                {
                    //persistant cookie was valid, log in
                    LogIn(user.userId, user.email, user.name, user.datecreated, "", user.photo);
                }
            }
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

        public void LogIn(int userId, string email, string name, DateTime datecreated, string displayName = "", bool photo = false)
        {
            this.userId = userId;
            this.email = email;
            this.photo = photo;
            this.name = name;
            this.displayName = displayName;
            this.datecreated = datecreated;

            //TODO: load security keys for user

            //load Kandu-specific properties for user from database
            var user = Query.Users.GetInfo(userId);
            keepMenuOpen = user.keepmenu;
            allColor = user.allcolor;

            boards = Query.Boards.GetBoardsForMember(userId);
            if (boards == null) { boards = new List<int>(); }

            //create persistant cookie
            var auth = Query.Users.CreateAuthToken(userId);
            var options = new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1)
            };

            Context.Response.Cookies.Append("authId", auth, options);

            changed = true;
        }

        public void LogOut()
        {
            userId = 0;
            email = "";
            name = "";
            photo = false;
            changed = true;
            Context.Response.Cookies.Delete("authId");
            Save();
        }

        public bool CheckSecurity(int boardId)
        {
            //check Kandu-specific security settings
            if (userId <= 0) { return false; }
            if (boards.Contains(boardId))
            {
                return true;
            }
            return false;
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