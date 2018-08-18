using System;

namespace Kandu.Common.Platform
{
    public static class Lists
    {
        public static int Create(int boardId, string name, int sort = 0)
        {
            var query = new Query.Lists();
            try
            {
                return query.CreateList(
                    new Query.Models.List()
                    {
                        boardId = boardId,
                        name = name,
                        sort = sort
                    }
                );
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error creating list");
            }
        }

        public static void Archive(int listId)
        {
            var query = new Query.Lists();
            try
            {
                query.Archive(listId);
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error archiving list");
            }
        }
    }
}
