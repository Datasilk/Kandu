using System;
using System.IO;
using System.Linq;

namespace Kandu.Pages.Imports
{
    public class Trello : Page
    {
        public Trello(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security

            if (S.Request.QueryString.Value.Contains("?upload"))
            {
                //uploaded json file
                if (Files != null)
                {
                    if (Files.Count > 0)
                    {
                        Models.Trello.Board board = null;
                        using (var stream = Files[0].OpenReadStream())
                        {
                            var txt = (new StreamReader(stream)).ReadToEnd();
                            board = (Models.Trello.Board)S.Util.Serializer.ReadObject(txt, typeof(Models.Trello.Board));
                        }

                        if (board != null)
                        {
                            //show success page in iframe
                            var scaffold = new Scaffold("/Pages/Import/Trello/success.html", S.Server.Scaffold);
                            scaffold.Data["name"] = board.name;

                            //import board
                            var boards = new Query.Boards(S.Server.sqlConnectionString);
                            var lists = new Query.Lists(S.Server.sqlConnectionString);
                            var cards = new Query.Cards(S.Server.sqlConnectionString);
                            var merge = S.Request.QueryString.Value.Contains("merge");
                            var boardType = S.Request.Query.ContainsKey("type") ? int.Parse(S.Request.Query["type"]) : 0;
                            var sort = 0;
                            var sortCard = 0;
                            var bgColor = board.prefs.backgroundColor != null ? board.prefs.backgroundColor : board.prefs.backgroundBottomColor;
                            if(bgColor == null)
                            {
                                bgColor = board.prefs.backgroundTopColor;
                            }
                            var boardId = boards.Import(new Query.Models.Board()
                            {
                                name = board.name,
                                archived = board.closed,
                                color = bgColor,
                                datecreated = board.actions.Last().date,
                                lastmodified = board.dateLastActivity,
                                ownerId = S.User.userId,
                                favorite = board.pinned,
                                security = (short)(board.prefs.permissionLevel == "private" ? 1 : 0),
                                type = (Query.Models.Board.BoardType)boardType

                            }, S.User.userId, merge);

                            if(!S.Util.IsEmpty(boardId))
                            {
                                //import each list
                                sort = 0;
                                board.lists.ForEach((list) => {
                                    if(list.closed == false)
                                    {
                                        var listId = lists.Import(new Query.Models.List()
                                        {
                                            boardId = boardId,
                                            name = list.name,
                                            sort = sort
                                        }, merge);

                                        //import cards for each list
                                        sortCard = 0;
                                        board.cards.FindAll((c) => c.idList == list.id).ForEach((card) =>
                                        {
                                            if(card.closed == false)
                                            {
                                                var cardDate = board.actions.FindLast((a) => a.data != null ? (a.data.card != null ? (a.data.card.id != null ? a.data.card.id == card.id : false) : false) : false);
                                                cards.Import(new Query.Models.Card()
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
                                                    type = 0
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
                }
                //show upload form in iframe
                return S.Server.LoadFileFromCache("/Pages/Import/Trello/trello.html");
            }
            else
            {
                //show upload form in iframe
                return S.Server.LoadFileFromCache("/Pages/Import/Trello/trello.html");
            }
        }
    }
}
