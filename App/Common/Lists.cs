using System;

namespace Kandu.Common
{
    public static class Lists
    {
        public static int Create(int boardId, string name, int sort = 0)
        {
            try
            {
                return Query.Lists.CreateList(
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
            try
            {
                Query.Lists.Archive(listId);
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error archiving list");
            }
        }
    }
}
