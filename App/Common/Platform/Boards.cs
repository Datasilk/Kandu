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
            }catch (Exception ex)
            {
                throw new ServiceErrorException("Error creating new board");
            }   
        }

        public static void Update(Request request, int boardId, string name, string color, int orgId)
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
                    orgId = orgId
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

        public static string RenderList(Request request)
        {
            var createView = new View("/Views/Boards/create-board.html");
            var orgView = new View("/Views/Boards/org-head.html");
            var boards = Query.Boards.GetList(request.User.userId);
            var html = new StringBuilder();
            var item = new View("/Views/Boards/list-item.html");
            var orgId = 0;
            boards.ForEach((Query.Models.Board b) => {
                //check organization
                if (orgId != b.orgId)
                {
                    if (orgId > 0)
                    {
                        createView["onclick"] = "S.boards.add.show(null, null, '', " + orgId + ")";
                        html.Append(createView.Render());
                        html.Append("</div>");
                    } 
                    orgView.Clear();
                    orgView["name"] = b.orgName;
                    orgId = b.orgId;
                    html.Append(orgView.Render());
                    html.Append("<div class=\"org-boards\">");
                }
                item["favorite"] = b.favorite ? "1" : "";
                item["name"] = b.name;
                item["color"] = "#" + b.color;
                item["extra"] = b.favorite ? "fav" : "";
                item["id"] = b.boardId.ToString();
                item["orgId"] = orgId.ToString();
                item["type"] = b.type.ToString();
                item["url"] = Uri.EscapeUriString("/board/" + b.boardId + "/" + b.name.Replace(" ", "-").ToLower());
                html.Append(item.Render());
            });
            createView["onclick"] = "S.boards.add.show(null, null, '', " + orgId + ")";
            html.Append(createView.Render());
            html.Append("</div>");

            return html.ToString();
        }

        public static string RenderBoardMenu(Request request, int orgId = 0, bool listOnly = false, bool showSubTitle = true, int sort = 0, bool btnsInFront = false)
        {
            var html = new StringBuilder();
            var htm = new StringBuilder();
            var menu = new View("/Views/Board/menu.html");
            var item = new View("/Views/Board/menu-item.html");
            var boards = Query.Boards.GetList(request.User.userId, orgId, (Query.Boards.BoardsSort)sort);
            var favs = boards.Where((a) => { return a.favorite; });

            // Favorite Boards //////////////////////////////////////////
            if (favs.Count() > 0)
            {
                menu["title"] = "Starred Boards";
                menu["id"] = "favs";
                menu["icon"] = "star-border-sm";
                htm = new StringBuilder();
                foreach (var fav in favs)
                {
                    item["id"] = fav.boardId.ToString();
                    item["url"] = "/board/" + fav.boardId + "/" + fav.name.Replace(" ", "-").ToLower();
                    item["color"] = "#" + fav.color;
                    item["title"] = fav.name;
                    item["owner"] = fav.orgName;
                    item["star"] = fav.favorite ? "star" : "star-border";
                    if (showSubTitle) { item.Show("subtitle"); }
                    htm.Append(item.Render());
                }
                menu["items"] = htm.ToString();
            }

            // Boards (sorted by organization, favorite, alphabetical) //////////////////////////////////////////
            if (boards.Count() > 0)
            {
                var isnewOrg = true;
                htm = new StringBuilder();
                foreach (var board in boards)
                {
                    if (board.orgId != orgId)
                    {
                        if (orgId > 0)
                        {
                            menu["items"] = htm.ToString();
                            html.Append(menu.Render());
                        }
                        isnewOrg = true;
                        orgId = board.teamId;
                    }
                    if (isnewOrg == true)
                    {
                        menu["title"] = board.orgName;
                        menu["id"] = "org" + board.orgId.ToString();
                        menu["icon"] = "user";
                    }

                    item["id"] = board.boardId.ToString();
                    item["orgId"] = orgId.ToString();
                    item["url"] = "/board/" + board.boardId + "/" + board.name.Replace(" ", "-").ToLower();
                    item["color"] = "#" + board.color;
                    item["title"] = board.name;
                    item["owner"] = board.orgName;
                    item["star"] = board.favorite ? "star" : "star-border";
                    if (showSubTitle) { item.Show("subtitle"); }
                    htm.Append(item.Render());
                }
                menu["items"] = htm.ToString();
            }
            if(menu["items"] != "")
            {
                //add new board button
                if (request.User.CheckSecurity(orgId, Security.Keys.BoardCanCreate))
                {
                    var additem = new View("/Views/Boards/add-item.html");
                    menu["items"] = btnsInFront ? (additem.Render() + menu["items"]) : (menu["items"] + additem.Render());
                }

                //finally, render menu
                if (listOnly)
                {
                    return menu["items"];
                }
                html.Append(menu.Render());
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
