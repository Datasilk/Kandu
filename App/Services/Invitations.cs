namespace Kandu.Services
{
    public class Invitations : Service
    {
        public string Accept(string email, string publickey)
        {
            //validate invitation
            var invitation = Query.Invitations.Accept(email, publickey);
            if(invitation == null)
            {
                return Error("Invitation doesn't exist or is expired. Please request another invitation.");
            }
            return Success();
        }
    }
}
