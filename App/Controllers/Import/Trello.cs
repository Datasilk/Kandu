using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Utility.Serialization;

namespace Kandu.Pages.Imports
{
    public class Trello : Page
    {
        public Trello(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security

            if (context.Request.QueryString.Value.Contains("?upload"))
            {
                //uploaded json file
                var files = context.Request.Form.Files;
                if (files.Count > 0)
                {
                    Models.Trello.Board board = null;
                    try
                    {
                        var ms = new MemoryStream();
                        files[0].CopyTo(ms);
                        ms.Position = 0;
                        var sr = new StreamReader(ms);
                        var txt = sr.ReadToEnd();

                        board = (Models.Trello.Board)Serializer.ReadObject(txt.ToString(), typeof(Models.Trello.Board));
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }

                    if (board != null)
                    {
                        //show success page in iframe
                        var scaffold = new Scaffold("/Views/Import/Trello/success.html", Server.Scaffold);
                        scaffold.Data["name"] = board.name;

                        //import board
                        var merge = context.Request.QueryString.Value.Contains("merge");
                        var boardType = context.Request.Query.ContainsKey("type") ? int.Parse(context.Request.Query["type"]) : 0;
                        var sort = 0;
                        var sortCard = 0;
                        var bgColor = board.prefs.backgroundColor != null ? board.prefs.backgroundColor : board.prefs.backgroundBottomColor;
                        if (bgColor == null)
                        {
                            bgColor = board.prefs.backgroundTopColor;
                        }
                        var boardId = Query.Boards.Import(new Query.Models.Board()
                        {
                            name = board.name,
                            archived = board.closed,
                            color = bgColor,
                            datecreated = board.actions.Last().date,
                            lastmodified = board.dateLastActivity,
                            ownerId = User.userId,
                            favorite = board.pinned,
                            security = (short)(board.prefs.permissionLevel == "private" ? 1 : 0),
                            type = (Query.Models.Board.BoardType)boardType

                        }, User.userId, merge);

                        if (!Utility.Objects.IsEmpty(boardId))
                        {
                            //import each list
                            sort = 0;
                            board.lists.ForEach((list) =>
                            {
                                if (list.closed == false)
                                {
                                    var listId = Query.Lists.Import(new Query.Models.List()
                                    {
                                        boardId = boardId,
                                        name = list.name,
                                        sort = sort
                                    }, merge);

                                    //import cards for each list
                                    sortCard = 0;
                                    board.cards.FindAll((c) => c.idList == list.id).ForEach((card) =>
                                    {
                                        if (card.closed == false)
                                        {
                                            var cardDate = board.actions.FindLast((a) => a.data != null ? (a.data.card != null ? (a.data.card.id != null ? a.data.card.id == card.id : false) : false) : false);
                                            Query.Cards.Import(new Query.Models.Card()
                                            {
                                                boardId = boardId,
                                                archived = card.closed,
                                                colors = string.Join(",", card.labels.Select((a) => a.color).ToArray()),
                                                datecreated = cardDate != null ? cardDate.date : DateTime.Now,
                                                datedue = card.due,
                                                description = card.desc,
                                                listId = listId,
                                                name = card.name,
                                                sort = sortCard,
                                                layout = 0
                                            }, merge);

                                                //import checklists for each card
                                                sortCard++;
                                        }
                                    });
                                    sort++;
                                }
                            });
                        }

                        return scaffold.Render();
                    }
                    else
                    {
                        return "Could not parse json file correctly";
                    }

                }

                //show upload form in iframe
                return Server.LoadFileFromCache("/Views/Import/Trello/trello.html");
            }
            else
            {
                //show upload form in iframe
                return Server.LoadFileFromCache("/Views/Import/Trello/trello.html");
            }
        }
    }
}
