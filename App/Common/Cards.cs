using System;
using System.Text;

namespace Kandu.Common
{
    public static class Cards
    {

        public static Query.Models.Card Create(int boardId, int listId, string name, string description = "", DateTime? dateDue = null, string colors = "", string type = "")
        {
            var card = new Query.Models.Card()
            {
                boardId = boardId,
                listId = listId,
                name = name,
                colors = colors,
                description = description,
                datedue = dateDue,
                datecreated = DateTime.Now,
                type = type
            };
            try
            {
                var id = Query.Cards.Create(card);
                card.cardId = id;
                return card;
            }
            catch (ServiceErrorException)
            {
                throw new ServiceErrorException("Error creating new card");
            }
        }

        public static string RenderComment(View view, int orgId, Query.Models.CardComment comment, bool isowner)
        {
            return RenderComment(view, comment.commentId, comment.userId, orgId, comment.name, comment.comment, comment.photo, comment.dateCreated, isowner);
        }

        public static string RenderComment(View view, int commentId, int userId, int orgId, string name, string comment, bool photo, DateTime datecreated, bool isowner)
        {
            view.Clear();
            view["comment-id"] = commentId.ToString();
            view["user-id"] = userId.ToString();
            view["org-id"] = orgId.ToString();
            view["name"] = name;
            view["date"] = datecreated.ToString("MM/dd/yyyy h:mm tt");
            view["photo"] = photo ? "/images/users/photos/" + userId + ".jpg" : "";
            if (photo) { view.Show("has-photo"); } else { view.Show("no-photo"); }
            if (isowner) { view.Show("is-owner"); }
            view["comment"] = comment;
            return view.Render();
        }
    }
}
