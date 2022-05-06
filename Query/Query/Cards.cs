using System;
using System.Collections.Generic;
using System.Linq;

namespace Query
{
    public static class Cards
    {
        public static int Create(Models.Card card)
        {
            return Sql.ExecuteScalar<int>(
                "Card_Create",
                new
                {
                    card.listId,
                    card.boardId,
                    card.layout,
                    card.colors,
                    card.name,
                    card.type,
                    datedue = card.datedue == null ? DateTime.Now.AddYears(-100) : card.datedue,
                    card.description,
                    card.json
                }
            );
        }

        public static int Import(Models.Card card, bool merge = false)
        {
            return Sql.ExecuteScalar<int>(
                "Card_Import",
                new
                {
                    card.listId,
                    card.boardId,
                    card.layout,
                    card.colors,
                    card.name,
                    card.type,
                    datedue = card.datedue == null ? DateTime.Now.AddYears(-100) : card.datedue,
                    card.description,
                    card.json,
                    merge
                }
            );
        }

        public static void Archive(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Archive",new { boardId, cardId });
        }

        public static Models.CardDetails GetDetails(int cardId, int userId)
        {
            using (var conn = new Connection("Card_GetDetails", new { cardId, userId }))
            {
                var reader = conn.PopulateMultiple();
                var details = reader.ReadFirst<Models.CardDetails>();
                if(details != null)
                {
                    details.labels = reader.Read<Models.Label>().ToList();
                    details.checklist = reader.Read<Models.CardChecklistItem>().ToList();
                    details.attachments = reader.Read<Models.CardAttachment>().ToList();
                    details.comments = reader.Read<Models.CardComment>().ToList();
                }
                return details;
            }
        }

        public static Models.CardBoard GetBoard(int cardId)
        {
            return Sql.Populate<Models.CardBoard>("Card_GetBoard", new { cardId }).FirstOrDefault();
        }

        public static void Restore(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Restore",new { boardId, cardId });
        }

        public static void Delete(int boardId, int cardId)
        {
            Sql.ExecuteNonQuery("Card_Delete",new { boardId, cardId });
        }

        public static Models.Card GetInfo(int cardId)
        {
            return Sql.Populate<Models.Card>("Card_GetInfo", new { cardId }).FirstOrDefault();
        }

        public static List<Models.Card> GetList(int boardId, int listId = 0, int start = 1, int length = 2000, bool archivedOnly = false)
        {
            return Sql.Populate<Models.Card>("Cards_GetList",new { boardId, listId, start, length, archivedOnly });
        }

        public static List<Models.CardDetails> AssignedToMember(int userId, int orgId = 0, int start = 1, int length = 20, bool archivedOnly = false)
        {
            return Sql.Populate<Models.CardDetails>(
                "Cards_AssignedToMember",
                new { userId, orgId, start, length, archivedOnly }
            );
        }

        public static List<Models.CardMember> Members(int cardId, string search = "")
        {
            return Sql.Populate<Models.CardMember>("Card_GetMembers", new { cardId, search }).Distinct().ToList();
        }

        public static void Move(int boardId, int listId, int cardId, int[] cardIds)
        {
            Sql.ExecuteNonQuery("Card_Move",new { boardId, listId, cardId, ids = string.Join(",", cardIds) });
        }

        public static void UpdateName(int boardId, int cardId, string name)
        {
            Sql.ExecuteNonQuery("Card_UpdateName",new { boardId, cardId, name });
        }

        public static void UpdateDescription(int boardId, int cardId, string description)
        {
            Sql.ExecuteNonQuery("Card_UpdateDescription",new { boardId, cardId, description });
        }

        public static void UpdateJson(int boardId, int cardId, string json)
        {
            Sql.ExecuteNonQuery("Card_UpdateJson",new { boardId, cardId, json });
        }

        public static void UpdateAssignedTo(int cardId, int userId, int userIdAssigned)
        {
            Sql.ExecuteNonQuery("Card_UpdateAssignedTo",new { cardId, userId, userIdAssigned });
        }

        public static void UpdateDueDate(int cardId, int userId, DateTime? duedate)
        {
            Sql.ExecuteNonQuery("Card_UpdateDueDate",new { cardId, userId, duedate });
        }

        public static int AddComment(int cardId, int userId, string comment)
        {
            return Sql.ExecuteScalar<int>("Card_Comment_Add", new { cardId, userId, comment });
        }

        public static void UpdateComment(int commentId, int cardId, int userId, string comment)
        {
            Sql.ExecuteNonQuery("Card_Comment_Update", new { commentId, cardId, userId, comment });
        }

        public static void RemoveComment(int commentId, int cardId, int userId)
        {
            Sql.ExecuteNonQuery("Card_Comment_Remove", new { commentId, cardId, userId });
        }

        public static Models.CardComment GetComment(int cardId, int commentId, int userId)
        {
            return Sql.Populate<Models.CardComment>("Card_Comment_Get", new { cardId, commentId, userId }).FirstOrDefault();
        }

        public static void FlagComment(int commentId, int cardId, int userId)
        {
            Sql.ExecuteNonQuery("Card_Comment_Flag", new { commentId, cardId, userId });
        }

        public static Models.CardChecklistItem GetChecklistItem(int cardId, int itemId)
        {
            return Sql.ExecuteScalar<Models.CardChecklistItem>("Card_Checklist_GetItem", new { cardId, itemId });
        }

        public static Models.CardChecklistItem AddChecklistItem(int cardId, int userId, string label, bool ischecked, int sort = -1)
        {
            return Sql.Populate<Models.CardChecklistItem>("Card_Checklist_AddItem", new { cardId, userId, label, ischecked, sort }).FirstOrDefault();
        }

        public static void UpdateChecklistItem(int itemId, int cardId, int userId, int sort, string label, bool ischecked = false)
        {
            Sql.ExecuteNonQuery("Card_Checklist_UpdateItem", new { itemId, userId, cardId, sort, label, ischecked });
        }

        public static void UpdateChecklistItemLabel(int itemId, int cardId, int userId, string label)
        {
            Sql.ExecuteNonQuery("Card_Checklist_UpdateItemLabel", new { itemId, userId, cardId, label});
        }

        public static void UpdateChecklistItemChecked(int itemId, int cardId, int userId, bool ischecked)
        {
            Sql.ExecuteNonQuery("Card_Checklist_UpdateItemChecked", new { itemId, userId, cardId, ischecked });
        }

        public static void RemoveChecklistItem(int itemId, int cardId, int userId)
        {
            Sql.ExecuteNonQuery("Card_Checklist_RemoveItem", new { itemId, cardId, userId });
        }

        public static void SortChecklist(int cardId, int userId, int[] ids)
        {
            Sql.ExecuteNonQuery("Card_Checklist_SortItems", new { userId, cardId, ids = string.Join(',', ids) });
        }

        public static void AddAttachments(int cardId, int userId, string[] filenames)
        {
            var list = new Models.Xml.Attachments() 
            { 
                file = filenames.Select(a => new Models.Xml.Attachments.Filename() { filename = a }).ToArray() 
            };
            Sql.ExecuteNonQuery("Card_Attachments_Add", new { userId, cardId, files = Common.Serializer.ToXmlDocument(list).OuterXml.Replace("encoding=\"utf-8\"", "") });
        }

        public static void RemoveAttachment(int cardId, int userId, int attachmentId)
        {
            Sql.ExecuteNonQuery("Card_Attachment_Remove", new { userId, cardId, attachmentId });
        }

        public static Models.CardAttachment GetAttachment(int attachmentId)
        {
            return Sql.Populate<Models.CardAttachment>("Card_Attachment_GetInfo", new { attachmentId }).FirstOrDefault();
        }
    }
}
