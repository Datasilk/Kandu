using System;
using System.Collections.Generic;

namespace Kandu
{
    public class User
    {
        private Core S;
        private bool _loaded = false;

        protected List<int> boards;

        public User(Core DatasilkCore)
        {
            S = DatasilkCore;
        }

        private void Start()
        {
            if (_loaded == false)
            {
                _loaded = true;
                if (S.User.Data.ContainsKey("boards"))
                {
                    boards = (List<int>)S.Util.Serializer.ReadObject(S.User.Data["boards"], typeof(List<int>));
                    if (boards == null) { boards = new List<int>(); }
                }
                else
                {
                    UpdateSecurity();
                }
            }
        }

        public bool CheckSecurity(int boardId)
        {
            Start();
            if (boards.Contains(boardId)){
                return true;
            }
            return false;
        }

        public void UpdateSecurity()
        {
            //reloads all security credentials for the user directly from the database
            var query = new Query.Boards(S.Server.sqlConnectionString);
            boards = query.GetBoardsForMember(S.User.userId);
            if(boards == null) { boards = new List<int>(); }
            Save();
        }

        private void Save()
        {
            S.User.Data["boards"] = S.Util.Serializer.WriteObjectToString(boards);
        }
    }
}
