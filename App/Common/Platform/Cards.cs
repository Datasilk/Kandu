using System;

namespace Kandu.Common.Platform
{
    public static class Cards
    {

        public static Query.Models.Card Create(int boardId, int listId, string name, string description = "", DateTime? dateDue = null, string colors = "")
        {
            var query = new Query.Cards();
            var card = new Query.Models.Card()
            {
                boardId = boardId,
                listId = listId,
                name = name,
                colors = colors,
                description = description,
                datedue = dateDue,
                datecreated = DateTime.Now
            };
            try
            {
                var id = query.Create(card);
                card.cardId = id;
                return card;
            }
            catch (ServiceErrorException)
            {
                throw new ServiceErrorException("Error creating new card");
            }
        }
    }
}
