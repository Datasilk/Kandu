using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kandu.Services
{
    public class Boards : Service
    {
        public Boards(Core KanduCore) : base(KanduCore)
        {
        }

        public string Create(string name, string color, int teamId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var query = new Query.Boards(S.Server.sqlConnectionString);
            query.CreateBoard(new Query.Models.Board()
            {
                name = name,
                security = 1,
                color = color,
                teamId = teamId
            }, S.User.userId);
            return Success();
        }

        public string Details(int boardId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var query = new Query.Boards(S.Server.sqlConnectionString);
            var board = query.GetBoardDetails(boardId);
            return S.Util.Serializer.WriteObjectToString( 
                new Dictionary<string,Dictionary<string,string>>(){
                    {
                        "board", new Dictionary<string,string>(){
                            { "name", board.name },
                            { "color", "#" + board.color },
                            {"teamId", board.teamId.ToString() }
                        }
                    }
                }
            );
        }

        public string Update(int boardId, string name, string color, int teamId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var query = new Query.Boards(S.Server.sqlConnectionString);

            //check if user has access to board
            if (!query.MemberExists(S.User.userId, boardId)) { return AccessDenied(); }
            
            //finally, update board
            query.UpdateBoard(new Query.Models.Board()
            {
                name = name,
                boardId = boardId,
                color = color,
                teamId = teamId
            });
            return Success();
        }


        public string BoardsMenu()
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var html = new StringBuilder();
            var htm = new StringBuilder();
            var section = new Scaffold("/Services/Boards/menu-section.html", S.Server.Scaffold);
            var item = new Scaffold("/Services/Boards/menu-item.html", S.Server.Scaffold);
            var query = new Query.Boards(S.Server.sqlConnectionString);
            var boards = query.GetList(S.User.userId);
            var favs = boards.Where((a) => { return a.favorite; });
            var teams = boards.OrderBy((a) => { return a.datecreated; }).Reverse().OrderBy((a) => { return a.ownerId == S.User.userId; });

            // Favorite Boards //////////////////////////////////////////
            if (favs.Count() > 0)
            {
                section.Data["title"] = "Starred Boards";
                section.Data["id"] = "favs";
                section.Data["icon"] = "star-border-sm";
                htm = new StringBuilder();
                foreach (var fav in favs)
                {
                    item.Data["id"] = fav.boardId.ToString();
                    item.Data["url"] = "/board/" + fav.boardId + "/" + fav.name.Replace(" ", "-").ToLower();
                    item.Data["color"] = "#" + fav.color;
                    item.Data["title"] = fav.name;
                    item.Data["owner"] = fav.ownerName;
                    item.Data["star"] = fav.favorite ? "star" : "star-border";
                    htm.Append(item.Render());
                }
                section.Data["items"] = htm.ToString();
                html.Append(section.Render());
            }

            // Team Boards //////////////////////////////////////////
            if (teams.Count() > 0)
            {
                var teamId = 0;
                var isnewTeam = true;
                htm = new StringBuilder();
                foreach (var team in teams)
                {
                    if(team.teamId != teamId)
                    {
                        if (teamId > 0)
                        {
                            section.Data["items"] = htm.ToString();
                            html.Append(section.Render());
                        }
                        isnewTeam = true;
                        teamId = team.teamId;
                    }
                    if(isnewTeam == true)
                    {
                        section.Data["title"] = team.teamName;
                        section.Data["id"] = "team" + team.teamId.ToString();
                        section.Data["icon"] = "user";
                    }

                    item.Data["id"] = team.boardId.ToString();
                    item.Data["url"] = "/board/" + team.boardId + "/" + team.name.Replace(" ", "-").ToLower();
                    item.Data["color"] = "#" + team.color;
                    item.Data["title"] = team.name;
                    item.Data["owner"] = team.ownerName;
                    item.Data["star"] = team.favorite ? "star" : "star-border";
                    htm.Append(item.Render());
                }
                section.Data["items"] = htm.ToString();
                html.Append(section.Render());
            }

            // Team Boards (sort by user owned, then by date created) /////////
            return html.ToString();
        }

        public string KeepMenuOpen(bool keepOpen)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var query = new Query.Users(S.Server.sqlConnectionString);
            query.KeepMenuOpen(S.User.userId, keepOpen);
            UserInfo.Settings.keepMenuOpen = keepOpen;
            UserInfo.SaveSettings();
            return "";
        }

        public string AllColor(bool allColor)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var query = new Query.Users(S.Server.sqlConnectionString);
            query.AllColor(S.User.userId, allColor);
            UserInfo.Settings.allColor = allColor;
            UserInfo.SaveSettings();
            return "";
        }

    }
}
