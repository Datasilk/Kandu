using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Serialization;

namespace Kandu.Common.Platform
{
    public static class Boards
    {
        public static int Create(Datasilk.Request request, string name, string color, int teamId)
        {
            Server Server = Server.Instance;
            var query = new Query.Boards();
            try
            {
                var id = query.CreateBoard(new Query.Models.Board()
                {
                    name = name,
                    security = 1,
                    color = color,
                    teamId = teamId
                }, request.User.userId);

                //add board Id to user's permissions for boards
                request.User.boards.Add(id);
                request.User.Save(true);

                return id;
            }catch (Exception)
            {
                throw new ServiceErrorException("Error creating new board");
            }   
        }

        public static void Update(Datasilk.Request request, int boardId, string name, string color, int teamId)
        {
            Server Server = Server.Instance;
            var query = new Query.Boards();

            //check if user has access to board
            if (!query.MemberExists(request.User.userId, boardId)) {
                throw new ServiceDeniedException();
            }

            //finally, update board
            try
            {
                query.UpdateBoard(new Query.Models.Board()
                {
                    name = name,
                    boardId = boardId,
                    color = color,
                    teamId = teamId
                });
            }
            catch(Exception)
            {
                throw new ServiceErrorException("Error updating existing board");
            }
        }

        public static string Details(int boardId)
        {
            Server Server = Server.Instance;
            var query = new Query.Boards();
            try
            {
                var board = query.GetDetails(boardId);
                return Serializer.WriteObjectToString(
                    new Dictionary<string, Dictionary<string, string>>(){
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
            catch (Exception)
            {
                throw new ServiceErrorException("Error displaying board details");
            }
            
        }

        public static string RenderBoardsMenu(Datasilk.Request request)
        {
            Server Server = Server.Instance;
            var html = new StringBuilder();
            var htm = new StringBuilder();
            var section = new Scaffold("/Views/Boards/menu-section.html", Server.Scaffold);
            var item = new Scaffold("/Views/Boards/menu-item.html", Server.Scaffold);
            var query = new Query.Boards();
            var boards = query.GetList(request.User.userId);
            var favs = boards.Where((a) => { return a.favorite; });
            var teams = boards.OrderBy((a) => { return a.datecreated; }).Reverse().OrderBy((a) => { return a.ownerId == request.User.userId; });

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
                    if (team.teamId != teamId)
                    {
                        if (teamId > 0)
                        {
                            section.Data["items"] = htm.ToString();
                            html.Append(section.Render());
                        }
                        isnewTeam = true;
                        teamId = team.teamId;
                    }
                    if (isnewTeam == true)
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

        public static void KeepBoardsMenuOpen(Datasilk.Request request, bool keepOpen)
        {
            Server Server = Server.Instance;
            var query = new Query.Users();
            query.KeepMenuOpen(request.User.userId, keepOpen);
            request.User.keepMenuOpen = keepOpen;
            request.User.Save(true);
        }

        public static void UseAllColorScheme(Datasilk.Request request, bool allColor)
        {
            Server Server = Server.Instance;
            var query = new Query.Users();
            query.AllColor(request.User.userId, allColor);
            request.User.allColor = allColor;
            request.User.Save(true);
        }
    }
}
