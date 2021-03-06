﻿using System;
using System.Collections.Generic;
using Kandu.Core;

namespace Kandu.Common.Card
{
    public static class Kanban
    {
        public static string RenderCard(IRequest request, Query.Models.Card card, string boardColor = "", string boardName = "")
        {
            var useLayout = false;
            View cardview;
            if(card.name.IndexOf("----") == 0)
            {
                //separator
                cardview = new View("/Views/Card/Kanban/Type/separator.html");
            }
            else if(card.name.IndexOf("# ") == 0)
            {
                //header
                cardview = new View("/Views/Card/Kanban/Type/header.html");
                cardview["name"] = card.name.TrimStart(new char[] { '#', ' ' });
            }
            else if(card.layout == Query.Models.Card.CardLayout.custom && card.type != "" && Core.Vendors.Cards.ContainsKey(card.type))
            {
                //custom card from vendor plugin
                var vendor = Core.Vendors.Cards[card.type];
                return vendor.Render(request);
            }
            else
            {
                //card
                cardview = new View("/Views/Card/Kanban/Type/card.html");
                useLayout = true;
            }
            
            if(useLayout == true)
            {
                //load card custom design
                var view = new View("/Views/Card/Kanban/Layout/" + card.layout.ToString() + ".html");

                if(card.name.IndexOf("[x]") == 0 || card.name.IndexOf("[X]") == 0)
                {
                    var checkmark = new View("/Views/Card/Kanban/Elements/checkmark.html");
                    view["name"] = checkmark.Render() + card.name.Substring(4);
                }
                else if (card.name.IndexOf("[!]") == 0 || card.name.IndexOf("[!]") == 0)
                {
                    var checkmark = new View("/Views/Card/Kanban/Elements/warning.html");
                    view["name"] = checkmark.Render() + card.name.Substring(4);
                }
                else
                {
                    view["name"] = card.name;
                }
                
                view["colors"] = "";

                if(boardColor != "")
                {
                    view.Show("has-board-color");
                    view["board-color"] = "#" + boardColor;
                    view["board-name"] = boardName;
                }

                //render custom design inside card container
                cardview["boardid"] = card.boardId.ToString();
                cardview["layout"] = view.Render();
            }

            //load card container
            cardview["id"] = card.cardId.ToString();

            //render card container
            return cardview.Render();
        }

        public static Tuple<Query.Models.Card, string> Details(int boardId, int cardId)
        {
            try
            {
                var card = Query.Cards.GetDetails(boardId, cardId);
                var view = new View("/Views/Card/Kanban/details.html");
                view["card-id"] = cardId.ToString();
                view["board-id"] = boardId.ToString();
                view["board-url"] = Boards.GetUrl(boardId, card.boardName);
                view["board-name"] = card.boardName;
                view["board-color"] = card.boardColor;
                view["title"] = card.name;
                view["list-name"] = card.listName;
                view["description"] = card.description;
                view["no-description"] = card.description.Length > 0 ? "hide" : "";
                view["has-description"] = card.description.Length <= 0 ? "hide" : "";
                view["archive-class"] = card.archived ? "hide" : "";
                view["restore-class"] = card.archived ? "" : "hide";
                view["delete-class"] = card.archived ? "" : "hide";
                return new Tuple<Query.Models.Card, string>(card, view.Render());
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error loading card details");
            }
        }

        public static List<string> RenderCardsForMember(IRequest request, int userId, int orgId = 0, int start = 1, int length = 20)
        {
            var html = new List<string>();
            var cards = Query.Cards.AssignedToMember(userId, orgId, start, length);
            var x = 0;
            foreach (var card in cards)
            {
                x++;
                if (x > length) break;
                html.Add(RenderCard(request, card, card.boardColor, card.boardName) + "\n");
            }
            return html;
        }
    }
}
