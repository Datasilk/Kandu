using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kandu.Common.Platform
{
    public static class Boards
    {
        public static int Create(Request request, string name, string color, int teamId)
        {
            try
            {
                var id = Query.Boards.CreateBoard(new Query.Models.Board()
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

        public static void Update(Request request, int boardId, string name, string color, int teamId)
        {
            //check if user has access to board
            if (!Query.Boards.MemberExists(request.User.userId, boardId)) {
                throw new ServiceDeniedException();
            }

            //finally, update board
            try
            {
                Query.Boards.UpdateBoard(new Query.Models.Board()
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
            try
            {
                var board = Query.Boards.GetDetails(boardId);
                return JsonSerializer.Serialize(
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

        public static string RenderBoardsMenu(Request request)
        {
            var html = new StringBuilder();
            var htm = new StringBuilder();
            var section = new View("/Views/Boards/menu-section.html");
            var item = new View("/Views/Boards/menu-item.html");
            var boards = Query.Boards.GetList(request.User.userId);
            var favs = boards.Where((a) => { return a.favorite; });
            var teams = boards.OrderBy((a) => { return a.datecreated; }).Reverse().OrderBy((a) => { return a.ownerId == request.User.userId; });

            // Favorite Boards //////////////////////////////////////////
            if (favs.Count() > 0)
            {
                section["title"] = "Starred Boards";
                section["id"] = "favs";
                section["icon"] = "star-border-sm";
                htm = new StringBuilder();
                foreach (var fav in favs)
                {
                    item["id"] = fav.boardId.ToString();
                    item["url"] = "/board/" + fav.boardId + "/" + fav.name.Replace(" ", "-").ToLower();
                    item["color"] = "#" + fav.color;
                    item["title"] = fav.name;
                    item["owner"] = fav.ownerName;
                    item["star"] = fav.favorite ? "star" : "star-border";
                    htm.Append(item.Render());
                }
                section["items"] = htm.ToString();
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
                            section["items"] = htm.ToString();
                            html.Append(section.Render());
                        }
                        isnewTeam = true;
                        teamId = team.teamId;
                    }
                    if (isnewTeam == true)
                    {
                        section["title"] = team.teamName;
                        section["id"] = "team" + team.teamId.ToString();
                        section["icon"] = "user";
                    }

                    item["id"] = team.boardId.ToString();
                    item["url"] = "/board/" + team.boardId + "/" + team.name.Replace(" ", "-").ToLower();
                    item["color"] = "#" + team.color;
                    item["title"] = team.name;
                    item["owner"] = team.ownerName;
                    item["star"] = team.favorite ? "star" : "star-border";
                    htm.Append(item.Render());
                }
                section["items"] = htm.ToString();
                html.Append(section.Render());
            }

            // Team Boards (sort by user owned, then by date created) /////////
            return html.ToString();
        }

        public static void KeepBoardsMenuOpen(Request request, bool keepOpen)
        {
            Query.Users.KeepMenuOpen(request.User.userId, keepOpen);
            request.User.keepMenuOpen = keepOpen;
            request.User.Save(true);
        }

        public static void UseAllColorScheme(Request request, bool allColor)
        {
            Query.Users.AllColor(request.User.userId, allColor);
            request.User.allColor = allColor;
            request.User.Save(true);
        }
    }
}
