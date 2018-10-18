using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Datasilk
{
    public partial class User
    {

        public bool keepMenuOpen;
        public bool allColor;
        public List<int> boards = new List<int>();

        private Server Server { get; } = Server.Instance;

        partial void VendorInit()
        {
            //check for persistant cookie
            if (userId <= 0 && context.Request.Cookies.ContainsKey("authId"))
            {
                var user = Query.Users.AuthenticateUser(context.Request.Cookies["authId"]);
                if (user != null)
                {
                    //persistant cookie was valid, log in
                    LogIn(user.userId, user.email, user.name, user.datecreated, "", 1, user.photo);
                }
            }
        }

        partial void VendorLogIn()
        {
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

            context.Response.Cookies.Append("authId", auth, options);
        }

        public bool CheckSecurity(int boardId)
        {
            //check Kandu-specific security settings
            if(userId <= 0) { return false; }
            if (boards.Contains(boardId)){
                return true;
            }
            return false;
        }
    }
}
