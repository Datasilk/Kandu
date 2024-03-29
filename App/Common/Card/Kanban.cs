﻿using System;
using System.Collections.Generic;
using System.Text;
using Kandu.Core;
using Utility.Strings;

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
            else if(card.layout == Query.Models.CardLayout.custom && card.type != "" && Core.Vendors.Cards.ContainsKey(card.type))
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

        public static Tuple<Query.Models.Card, string> RenderDetails(IRequest request, int boardId, int cardId, int userId)
        {
            try
            {
                var card = Query.Cards.GetDetails(cardId, userId);
                var view = new View("/Views/Card/Kanban/details.html");
                view["card-id"] = cardId.ToString();
                view["org-id"] = card.orgId.ToString();
                view["board-id"] = boardId.ToString();
                view["board-url"] = Boards.GetUrl(boardId, card.boardName);
                view["board-name"] = card.boardName;
                view["board-color"] = card.boardColor;
                view["title"] = card.name;
                view["list-name"] = card.listName;

                //protected features
                if(request.User.CheckSecurity(card.orgId, new string[] { Security.Keys.CardFullAccess.ToString(), Security.Keys.CardCanUpdate.ToString() }, Models.Scope.Card, cardId)
                    || request.User.CheckSecurity(card.orgId, new string[] { Security.Keys.BoardsFullAccess.ToString(), Security.Keys.BoardCanUpdate.ToString() }, Models.Scope.Board, boardId))
                {
                    view.Show("can-move");
                    view.Show("can-archive");
                    view.Show("can-restore");
                    view.Show("can-delete");
                    if(card.checklist.Count == 0)
                    {
                        view.Show("can-create-checklist");
                    }
                    if (card.attachments.Count == 0)
                    {
                        view.Show("can-upload-files");
                    }
                    view["archive-class"] = card.archived ? "hide" : "";
                    view["restore-class"] = card.archived ? "" : "hide";
                    view["delete-class"] = card.archived ? "" : "hide";
                }

                //description
                var viewDescription = new View("/Views/Card/Kanban/Details/description.html");
                var viewDescriptionMenu = new View("/Views/Card/Kanban/Details/description-menu.html");
                viewDescription["description"] = card.description;
                viewDescription["no-description"] = card.description.Length > 0 ? "hide" : "";
                viewDescription["has-description"] = card.description.Length <= 0 ? "hide" : "";
                view["description"] = Accordion.Render("Description", viewDescription.Render(), "card-description", "", viewDescriptionMenu.Render(), true);

                //assigned to
                if (card.userIdAssigned > 0)
                {
                    var viewAssignedTo = new View("/Views/Card/Kanban/Details/assigned-to.html");
                    viewAssignedTo["assigned-userid"] = card.userIdAssigned.ToString();
                    viewAssignedTo["assigned-name"] = card.assignedName;
                    viewAssignedTo["org-id"] = card.orgId.ToString();
                    view["assigned-to"] = viewAssignedTo.Render();
                    view.Show("assigned");
                }
                else
                {
                    view.Show("not-assigned");
                }

                //due date
                if (card.datedue.HasValue)
                {
                    view["duedate"] = "Due " + card.datedue.Value.ToString("MM/dd/yyyy");
                    view.Show("has-duedate");
                }
                else
                {
                    view.Show("no-duedate");
                }

                //checklist
                if(card.checklist.Count > 0)
                {
                    view["checklist"] = RenderChecklist(card);
                }

                //attachments
                if (card.attachments.Count > 0)
                {
                    foreach (var attachment in card.attachments)
                    {
                        attachment.path = "/Attachment?c=" + cardId + "&f=" + attachment.filename;
                    }
                    view["attachments"] = RenderAttachments(card);
                }


                //comments
                var html = new StringBuilder();
                var viewComments = new View("/Views/Card/Kanban/Details/comments.html");
                var viewComment = new View("/Views/Card/Kanban/Details/comment.html");
                var viewCommentMenu = new View("/Views/Card/Kanban/Details/comment-menu.html");
                if(card.comments.Count > 0)
                {
                    foreach (var comment in card.comments)
                    {
                        html.Append(Cards.RenderComment(viewComment, card.orgId, comment, comment.userId == userId));
                    }
                    viewComments["content"] = html.ToString();
                }
                else
                {
                    //no comments
                    viewComments["content"] = Cache.LoadFile("/Views/Card/Kanban/Details/no-comments.html");
                }
                
                view["comments"] = Accordion.Render("Comments", viewComments.Render(), "card-comments", "icon-chat-single-left", viewCommentMenu.Render(), true);

                return new Tuple<Query.Models.Card, string>(card, view.Render());
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error loading card details");
            }
        }

        public static string RenderChecklist(Query.Models.CardDetails card)
        {
            var html = new StringBuilder();
            string body;
            if (card.checklist.Count > 0)
            {
                var viewItem = new View("/Views/Card/Kanban/Details/checklist-item.html");
                foreach (var item in card.checklist)
                {
                    html.Append(RenderChecklistItem(item, viewItem));
                }
                body = html.ToString();
            }
            else
            {
                body = Cache.LoadFile("/Views/Card/Kanban/Details/no-checklistitems.html");
            }
            var viewChecklistMenu = new View("/Views/Card/Kanban/Details/checklist-menu.html");
            return Accordion.Render("Checklist", body, "card-checklist", "icon-check", viewChecklistMenu.Render(), true);
        }

        public static string RenderChecklistItem(Query.Models.CardChecklistItem item, View view = null)
        {
            if(view == null)
            {
                view = new View("/Views/Card/Kanban/Details/checklist-item.html");
            }
            else
            {
                view.Clear();
            }
            view["id"] = item.itemId.ToString();
            view["text"] = Utility.Strings.Web.HtmlEncode(item.label);
            if (item.label.StartsWith("#"))
            {
                //show header label
                view.Show("header-item");
                view["text"] = item.label.Substring(1).Trim();
            }
            else
            {
                //show checkbox & label
                view.Show("checklist-item");
                if (item.isChecked == true)
                {
                    view.Show("checked");
                }
            }
            return view.Render();
        }

        public enum AttachmentsLayout
        {
            List = 0,
            Gallery = 1
        }

        public static string RenderAttachments(int cardId, int userId)
        {
            var card = Query.Cards.GetDetails(cardId, userId);
            foreach(var attachment in card.attachments)
            {
                attachment.path = "/Attachment?c=" + cardId + "&f=" + attachment.filename;
            }
            return RenderAttachments(card);
        }

        public static string RenderAttachments(Query.Models.CardDetails card, AttachmentsLayout layout = AttachmentsLayout.Gallery)
        {
            var html = new StringBuilder();
            string body;
            if (card.attachments.Count > 0)
            {
                var file = "list-item.html";
                switch (layout)
                {
                    case AttachmentsLayout.Gallery:
                        file = "gallery-item.html";
                        break;
                }
                var viewItem = new View("/Views/Card/Kanban/Details/Attachment/" + file);
                foreach (var item in card.attachments)
                {
                    html.Append(RenderAttachment(item, viewItem));
                }
                body = html.ToString();
            }
            else
            {
                body = Cache.LoadFile("/Views/Card/Kanban/Details/no-attachments.html");
            }
            var viewAttachmentsMenu = new View("/Views/Card/Kanban/Details/attachments-menu.html");
            return Accordion.Render("Attachments", body, "card-attachments", "icon-file", viewAttachmentsMenu.Render(), true);
        }

        public static string RenderAttachment(Query.Models.CardAttachment item, View view)
        {
            view.Clear();
            view["id"] = item.attachmentId.ToString();
            var ext = item.filename.GetFileExtension();
            var filetype = Files.GetFileType(item.filename);
            switch (filetype)
            {
                case Files.FileType.Image:
                    view.Show("use-img");
                    view["img-src"] = item.path + "&s=thumb";
                    break;
                default:
                    view.Show("use-icon");
                    switch (filetype)
                    {
                        case Files.FileType.Document:
                            view["icon"] = "icon-file-doc";
                            if(ext == "pdf") { view["icon"] = "icon-file-pdf"; }
                            break;
                        case Files.FileType.Compressed:
                            view["icon"] = "icon-file-zip";
                            break;
                        case Files.FileType.Video:
                            view["icon"] = "icon-file-video";
                            break;
                        case Files.FileType.Audio:
                            view["icon"] = "icon-file-video";
                            break;
                        default:
                            view["icon"] = "icon-file";
                            break;
                    }
                    break;
            }

            view["filename"] = item.filename;
            view["url"] = item.path;


            return view.Render();
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
