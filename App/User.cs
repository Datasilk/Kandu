using System;
using System.Collections.Generic;

namespace Kandu
{
    public class User
    {
        private Core S;
        private bool _loaded = false;

        [Serializable]
        public struct structSettings
        {
            public bool keepMenuOpen;
            public bool allColor;
        }

        protected List<int> boards;
        public structSettings Settings;

        public User(Core DatasilkCore)
        {
            S = DatasilkCore;
        }

        public void Start()
        {
            if (_loaded == false)
            {
                _loaded = true;
                if(S.User.userId > 0)
                {
                    if (S.User.Data.ContainsKey("boards"))
                    {
                        boards = (List<int>)S.Util.Serializer.ReadObject(S.User.Data["boards"], typeof(List<int>));
                        S.User.Data["boards"] = S.Util.Serializer.WriteObjectToString(boards);
                        if (boards == null) { boards = new List<int>(); }
                    }
                    else
                    {
                        UpdateSecurity();
                    }
                    if (S.User.Data.ContainsKey("settings"))
                    {
                        //load user settings from cache
                        Settings = (structSettings)S.Util.Serializer.ReadObject(S.User.Data["settings"], typeof(structSettings));
                    }
                    else
                    {
                        //load user settings from database
                        var query = new Query.Users(S.Server.sqlConnectionString);
                        var user = query.GetInfo(S.User.userId);
                        Settings.keepMenuOpen = user.keepmenu;
                        Settings.allColor = user.allcolor;
                        S.User.Data["settings"] = S.Util.Serializer.WriteObjectToString(Settings);
                        S.User.saveSession = true;
                    }
                }
            }
        }

        public bool CheckSecurity(int boardId)
        {
            if(S.User.userId <= 0) { return false; }
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

        public void Save()
        {
            S.User.saveSession = true;
        }

        public void SaveSettings()
        {
            S.User.Data["settings"] = S.Util.Serializer.WriteObjectToString(Settings);
            S.User.saveSession = true;
        }
    }
}
