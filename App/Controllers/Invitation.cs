namespace Kandu.Controllers
{
    public class Invitation: Controller
    {
        public override string Render(string body = "")
        {
            AddScript("/js/views/invitations/invitation.js");
            var view = new View("/Views/Invitations/accept-form.html");
            return base.Render(view.Render());
        }
    }
}
