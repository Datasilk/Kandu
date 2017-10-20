namespace Kandu.Services
{
    public class Boards : Service
    {
        public Boards(Core KanduCore) : base(KanduCore)
        {
        }

        public string Create(string name, string color, int teamId)
        {
            if (S.User.userId == 0) { return AccessDenied(); } //check security
            var query = new Query.Boards(S.Server.sqlConnection);
            query.CreateBoard(new Query.Models.Board()
            {
                name = name,
                security = 1,
                color = color,
                teamId = teamId
            });
            return Success();
        }
    }
}
