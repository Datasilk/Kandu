namespace Kandu.Core
{
    public interface IController: Datasilk.Core.Web.IController
    {
        string Title { get; set; }
        string Description { get; set; }
        string Theme { get; set; }
        string Favicon { get; set; }
        bool ContainsResource(string url);
        void LoadHeader(ref View view, Controller.HasMenu hasMenu = Controller.HasMenu.None);
    }
}