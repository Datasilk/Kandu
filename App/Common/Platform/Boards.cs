using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kandu.Common.Platform
{
    public static class Boards
    {
        public static int Create(Request request, string name, string color, int orgId)
        {
            try
            {
                var id = Query.Boards.CreateBoard(new Query.Models.Board()
                {
                    name = name,
                    color = color,
                    orgId = orgId
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

        public static string RenderBoardMenu(Request request)
        {
            var html = new StringBuilder();
            var htm = new StringBuilder();
            var section = new View("/Views/Board/menu.html");
            var item = new View("/Views/Board/menu-item.html");
            var boards = Query.Boards.GetList(request.User.userId);
            var favs = boards.Where((a) => { return a.favorite; });

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
                    item["owner"] = fav.orgName;
                    item["star"] = fav.favorite ? "star" : "star-border";
                    htm.Append(item.Render());
                }
                section["items"] = htm.ToString();
                html.Append(section.Render());
            }

            // Boards (sorted by organization, favorite, alphabetical) //////////////////////////////////////////
            if (boards.Count() > 0)
            {
                var orgId = 0;
                var isnewOrg = true;
                htm = new StringBuilder();
                foreach (var board in boards)
                {
                    if (board.orgId != orgId)
                    {
                        if (orgId > 0)
                        {
                            section["items"] = htm.ToString();
                            html.Append(section.Render());
                        }
                        isnewOrg = true;
                        orgId = board.teamId;
                    }
                    if (isnewOrg == true)
                    {
                        section["title"] = board.orgName;
                        section["id"] = "org" + board.orgId.ToString();
                        section["icon"] = "user";
                    }

                    item["id"] = board.boardId.ToString();
                    item["url"] = "/board/" + board.boardId + "/" + board.name.Replace(" ", "-").ToLower();
                    item["color"] = "#" + board.color;
                    item["title"] = board.name;
                    item["owner"] = board.orgName;
                    item["star"] = board.favorite ? "star" : "star-border";
                    htm.Append(item.Render());
                }
                section["items"] = htm.ToString();
                html.Append(section.Render());
            }
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
