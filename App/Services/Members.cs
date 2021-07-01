using System.Text;
using Utility.Strings;

namespace Kandu.Services
{
    public class Members : Service
    {

        public string RefreshList(int orgId, int page = 1, int length = 10, string search = "", bool canUseEmail = false, string buttonLabel = "Search", int? excludeTeamId = null, string onclick = "", string emailOnClick = "")
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            if (page <= 0) { page = 1; }
            if(buttonLabel == "") { buttonLabel = "Search"; }
            var viewSearch = new View("/Views/Members/search.html");
            var resultsInfo = new View("/Views/Members/results-info.html");
            var useEmail = new View("/Views/Members/use-email.html");
            var isEmail = search.IsEmail();
            if (isEmail)
            {
                useEmail["email"] = search;
                useEmail["onclick"] = (emailOnClick != "" ? emailOnClick : "S.members.add.selectEmail") + "(event, '" + search + "')";
            }
            viewSearch["search"] = search;
            viewSearch["page"] = page.ToString();
            viewSearch["button-label"] = buttonLabel;
            var pagelist = new StringBuilder();
            var count = length > 0 ? Query.Organizations.GetMembersCount(orgId, page, length, search) : 0;
            var memberslist = "<div class=\"grid-items\">" + (
                length > 0 ? Common.Platform.Members.RenderList(this, orgId, page, length, search) : 
                (canUseEmail == true ? useEmail.Render() : "")
                ) + "</div>";
            var paging = "";
            resultsInfo["count"] = count.ToString();
            if(count > 0)
            {
                resultsInfo.Show("has-results");
            }else if(count <= 0)
            {
                resultsInfo.Show("no-results");
            }
            if(count > 1)
            {
                resultsInfo.Show("plural");
            }

            if (length > 0 && count > page * length)
            {
                var pagingnum = new View("/Views/Shared/paging-number.html");
                var pagingbacknext = new View("/Views/Shared/paging-backnext.html");
                for (var x = 1; x <= count; x++)
                {
                    pagingnum.Clear();
                    pagingnum["number"] = x.ToString();
                    pagelist.Append(pagingnum.Render());
                }
                pagingbacknext["direction"] = "back";
                if(page == 1)
                {
                    pagingbacknext.Show("disabled");
                }
                pagingbacknext.Show("back");
                paging = pagingbacknext.Render();
                pagingbacknext.Clear();
                pagingbacknext["direction"] = "next";
                pagingbacknext.Show("next");
                if(count <= page * length)
                {
                    pagingbacknext.Show("disabled");
                }
                paging = "<div class=\"row paging\">" + paging + pagelist.ToString() + pagingbacknext.Render() + "</div>";

                //render members list
                viewSearch["members-list"] = resultsInfo.Render() + memberslist + paging;
            }
            else if(length > 0)
            {
                //render members list
                viewSearch["members-list"] = resultsInfo.Render() + memberslist;
            }
            else
            {
                //render new search message
                var newsearch = new View("/Views/Members/new-search.html");
                newsearch["orgId"] = orgId.ToString();
                viewSearch["members-list"] = newsearch.Render();
            }
            return length == 0 ? viewSearch.Render() : resultsInfo.Render() + memberslist + paging;
        }
    }
}
