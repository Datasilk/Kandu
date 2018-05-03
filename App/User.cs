using System.Collections.Generic;

namespace Datasilk
{
    public partial class User
    {

        public bool keepMenuOpen;
        public bool allColor;
        public List<int> boards = new List<int>();

        private Server Server { get; } = Server.Instance;

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
