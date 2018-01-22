using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
}
