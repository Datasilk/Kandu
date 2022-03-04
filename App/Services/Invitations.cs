using System;

namespace Kandu.Services
{
    public class Invitations : Service
    {
        public string Accept(string email, string publickey)
        {
            //validate invitation
            try
            {
                var invitation = Query.Invitations.Accept(email, publickey);
                if (invitation == null)
                {
                    return Error("Invitation doesn't exist or is expired. Please request another invitation.");
                }
            }
            catch(Exception ex)
            {
                Query.Logs.LogError(User.UserId, "Invitations/Accept", "", ex.Message, ex.StackTrace);
                return Error("Error occurred when trying to accept an invitation");
            }
            return Success();
        }
    }
}
