using System;
using System.Collections.Generic;

namespace Kandu.Models.Trello
{
    #region "Board"
    public class Board
    {
        public string id;
        public string name;
        public string desc;
        public string descData;
        public bool closed;
        public string idOrganization;
        public bool invited;
        public Limits limits;
        public bool pinned;
        public bool starred;
        public string url;
        public BoardPreferences prefs;
        public string shortLink;
        public bool subscribed;
        public Dictionary<string, string> labelNames;
        public DateTime dateLastActivity;
        public DateTime dateLasView;
        public string shortUrl;
        public List<Action> actions;
        public List<Card> cards;
        public List<Label> labels;
        public List<BoardList> lists;
        public List<Member> members;
        public List<Checklist> checklists;
        public List<Membership> memberships;

    }
    #endregion

    #region "Board Limits"
    public class Limit
    {
        public string status;
        public double disableAt;
        public double warnAt;
    }

    public class Limits
    {
        public LimitsAttachments attachments;
        public LimitsBoards boards;
        public LimitsCards cards;
        public LimitsChecklists checklists;
        public LimitsLabels labels;
        public LimitsLists lists;
    }

    public class LimitsAttachments
    {
        public Limit perBoard;
        public Limit perCard;
    }

    public class LimitsBoards
    {
        public Limit totalMembersPerBoard;
    }

    public class LimitsCards
    {
        public Limit openPerBoard;
        public Limit totalPerBoard;
    }

    public class LimitsChecklists
    {
        public Limit perBoard;
    }

    public class LimitsLabels
    {
        public Limit perBoard;
    }
    public class LimitsLists
    {
        public Limit openPerBoard;
        public Limit totalPerBoard;
    }
    #endregion

    #region "Board Preferences"
    public class BoardPreferences
    {
        public string permissionLevel;
        public string voting;
        public string comments;
        public string invitations;
        public bool selfJoin;
        public bool cardCovers;
        public string cardAging;
        public bool calendarFeedEnabled;
        public string background;
        public string backgroundImage;
        public List<BackgroundImageScaled> backgroundImageScaled;
        public bool backgroundTile;
        public string backgroundBrightness;
        public string backgroundColor;
        public string backgroundBottomColor;
        public string backgroundTopColor;
        public bool canBePublic;
        public bool canBeOrg;
        public bool canBePrivate;
        public bool canInvite;
    }

    public class BackgroundImageScaled
    {
        public int width;
        public int height;
        public string url;
    }
    #endregion

    #region "Board Actions"
    public class Action
    {
        public string id;
        public string idMemberCreator;
        public ActionData data;
        public string type;
        public DateTime date;

    }

    public class ActionData
    {
        public ActionCheckItem checkItem;
        public ActionChecklist checklist;
        public ActionCard card;
        public ActionBoard board;
        public ActionList list;
        public ActionDataOld old;
    }

    public class ActionCheckItem
    {
        public string textData;
        public string state;
        public string name;
        public string id;
    }

    public class ActionChecklist
    {
        public string name;
        public string id;
    }

    public class ActionCard
    {
        public string shortLink;
        public string idShort;
        public string name;
        public string id;
    }

    public class ActionBoard
    {
        public string shortLink;
        public string name;
        public string id;
    }

    public class ActionList
    {
        public string name;
        public string id;
        public double pos;
    }

    public class ActionDataOld
    {
        public double pos;
        public string name;
    }


    #endregion

    #region "Board Cards"
    public class Card
    {
        public string id;
        public bool closed;
        public DateTime dateLastActivity;
        public string desc;
        public string idBoard;
        public string idList;
        public string idShort;
        public string idAttachmentCover;
        public Limits limit;
        public bool manualCoverAttachment;
        public string name;
        public double pos;
        public string shortLink;
        public CardBadges badges;
        public bool dueComplete;
        public DateTime? due;
        public string email;
        public string[] idChecklists;
        public string[] idMembers;
        public List<CardLabel> labels;
        public string shortUrl;
        public bool subscribed;
        public string url;
        public List<CardAttachment> attachments;
    }

    public class CardBadges
    {
        public int votes;
        public AttachmentsByType attachmentsByType;
        public bool viewingMemberVoted;
        public bool subscribed;
        public int checkItems;
        public int checkItemsChecked;
        public int comments;
        public int attachments;
        public bool description;
        public DateTime? due;
        public bool dueComplete;
    }

    public class AttachmentsByType
    {
        public struct TrelloType
        {
            public int board;
            public int card;
        }

        public TrelloType trello;
    }

    public class CardLabel
    {
        public string id;
        public string idBoard;
        public string name;
        public string color;
        public int uses;
    }

    public class CardAttachment
    {
        public double bytes;
        public DateTime date;
        public string edgeColor;
        public string idMember;
        public bool isUpload;
        public string mimeType;
        public string name;
        public List<CardAttachmentPreview> previews;
        public string url;
        public double pos;
        public string id;
    }

    public class CardAttachmentPreview
    {
        public double bytes;
        public string url;
        public int height;
        public int width;
        public string _id;
        public bool scaled;
    }
    #endregion

    #region "Board Labels"
    public class Label
    {
        public string id;
        public string idBoard;
        public string name;
        public string color;
        public string uses;
    }
    #endregion

    #region "Board Lists"
    public class BoardList
    {
        public string id;
        public string name;
        public bool closed;
        public string idBoard;
        public double pos;
        public bool subscribed;
        public Limits limits;
    }
    #endregion

    #region "Board Members"
    public class Member
    {
        public string id;
        public string avatarHash;
        public string bio;
        public bool confirmed;
        public string fullName;
        public string initials;
        public string memberType;
        public string status;
        public string url;
        public string username;
    }
    #endregion

    #region "Board Checklists"
    public class Checklist
    {
        public string id;
        public string name;
        public string idBoard;
        public string idCard;
        public double pos;
        public Limits limits;
        public List<ChecklistItems> checkItems;
    }

    public class ChecklistItems
    {
        public string state;
        public string idChecklist;
        public string id;
        public string name;
        public double pos;
    }
    #endregion

    #region "Board Memberships"
    public class Membership
    {
        public string id;
        public string idMember;
        public string memberType;
        public bool unconfirmed;
        public bool deactivated;
    }
    #endregion
}