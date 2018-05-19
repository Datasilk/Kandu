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
                var query = new Kandu.Query.Users();
                var user = query.AuthenticateUser(context.Request.Cookies["authId"]);
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
            var query = new Kandu.Query.Users();
            var user = query.GetInfo(userId);
            keepMenuOpen = user.keepmenu;
            allColor = user.allcolor;

            var query2 = new Kandu.Query.Boards();
            boards = query2.GetBoardsForMember(userId);
            if (boards == null) { boards = new List<int>(); }

            //create persistant cookie
            var query3 = new Kandu.Query.Users();
            var auth = query3.CreateAuthToken(userId);
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
