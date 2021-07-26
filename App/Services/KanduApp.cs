using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace Kandu.Services
{
    public class KanduApp : Service
    {
        public string Details()
        {
            if(!User.IsAppOwner()) { return AccessDenied(); }
            var tabHtml = new StringBuilder();
            var contentHtml = new StringBuilder();
            var view = new View("/Views/App/details.html");
            var tab = new View("/Views/Shared/tab.html");

            //load application tab
            tab["title"] = "Settings";
            tab["id"] = "settings";
            tab["onclick"] = "S.kandu.details.tabs.select('settings')";
            tab.Show("selected");
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-settings\">" + RefreshSettings() + "</div>");

            //load email client & actions tab
            tab.Clear();
            tab["title"] = "Email";
            tab["id"] = "email";
            tab["onclick"] = "S.kandu.details.tabs.select('email')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-email\"></div>");

            //load plugins tab
            tab.Clear();
            tab["title"] = "Plugins";
            tab["id"] = "plugins";
            tab["onclick"] = "S.kandu.details.tabs.select('plugins')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-plugins\"></div>");

            view["tabs"] = tabHtml.ToString();
            view["content"] = contentHtml.ToString();
            return view.Render();
        }

        public string RefreshSettings()
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }
            var view = new View("/Views/App/settings.html");

            return view.Render();
        }

        #region "Email Clients & Actions"
        public string RefreshEmail()
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }
            var view = new View("/Views/App/email.html");
            var emailClients = Common.Email.VendorClients;
            var clients = Query.EmailClients.GetList();
            var html = new StringBuilder();

            if (Common.Email.VendorClients.Count > 0)
            {
                //load email client saved list
                if(clients.Count > 0)
                {
                    var viewItem = new View("/Views/Email/list-item.html");
                    foreach (var client in clients)
                    {
                        var vendor = emailClients.Where(a => a.Key == client.key).FirstOrDefault();
                        if (vendor != null)
                        {
                            viewItem.Clear();
                            viewItem.Bind(client);
                            viewItem["name"] = vendor.Name;
                            viewItem["onclick"] = "S.kandu.email.add.show(" + client.clientId + ")";
                            html.Append(viewItem.Render());
                        }
                    }
                    view["clients"] = html.ToString();
                    html.Clear();
                }
                else
                {
                    //no email clients exist
                    var viewAlt = new View("/Views/Email/no-clients.html");
                    view["clients"] = viewAlt.Render();
                }
            }

            //render email actions
            var viewEmailAction = new View("/Views/Email/action.html");
            var emailActions = Common.Email.VendorActions;
            var allActions = Query.EmailActions.GetList();

            foreach (var action in emailActions)
            {
                var config = allActions.Where(a => a.action == action.Key).FirstOrDefault();
                viewEmailAction.Bind(new
                {
                    action = new
                    {
                        key = action.Key,
                        name = action.Name,
                        fromname = config?.fromName ?? "",
                        fromaddress = config?.fromAddress ?? "",
                        templatefile = action.TemplateFile,
                        subject = config?.subject ?? "",
                        options = string.Join("",
                            clients.Select(a => "<option value=\"" + a.clientId + "\"" +
                            (config?.clientId == a.clientId ? " selected=\"selected\"" : "") +
                            ">" + emailClients.Where(b => b.Key == a.key).FirstOrDefault()?.Name + ": " + a.label + "</option>"
                            )),
                        onclick = "S.kandu.email.action.details(" + (config?.clientId ?? "") + ")"
                    }
                });
                if (action.UserDefinedSubject) { viewEmailAction.Show("user-subject"); }
                if (action.TemplateFile == "") { viewEmailAction.Show("any-file"); }
                else { viewEmailAction.Show("template-file"); }
                html.Append(viewEmailAction.Render());
                viewEmailAction.Clear();
            }
            view["actions"] = html.ToString();

            return view.Render();
        }

        public string RenderEmailClientForm(string key = "", string id = "")
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }

            //generate email client form
            var view = new View("/Views/Email/client-form.html");
            var emailClients = Common.Email.VendorClients;
            var clientoptions = new StringBuilder();

            //check for vendor email clients
            if (emailClients.Count == 0)
            {
                return Error("No email clients exist");
            }

            //load email client form fields
            Query.Models.EmailClient config = null;
            if (!string.IsNullOrEmpty(id))
            {
                config = Query.EmailClients.GetConfig(id);
                if (config == null) { return Error("Could not find configuration for email client ID " + id); }
                var client = Common.Email.VendorClients.Where(a => a.Key == config.key).FirstOrDefault();
                if (client == null) { return Error("Could not find vendor for email client key \"" + key + "\""); }
                view["clientId"] = client.Key;
                view["client-label"] = client.Name;
                view["label"] = config.label;
                view["parameters"] = LoadEmailClientParameters(client, config);
                view.Show("edit");
            }
            else if (!string.IsNullOrEmpty(key))
            {
                view["parameters"] = LoadEmailClientParameters(emailClients.Where(a => a.Key == key).FirstOrDefault());
                view.Show("is-new");
            }
            else
            {
                view["parameters"] = LoadEmailClientParameters(emailClients.FirstOrDefault());
                view.Show("is-new");
            }

            //add list of available email clients
            if (string.IsNullOrEmpty(id))
            {
                foreach (var item in emailClients)
                {
                    clientoptions.Append("<option value=\"" + item.Key + "\"" +
                        (
                            string.IsNullOrEmpty(key) ?
                            (item == emailClients[0] ? " selected" : "") :
                            (item.Key == key ? " selected" : "")
                        ) +
                        ">" + item.Name + "</option>");
                }
                view["client-options"] = clientoptions.ToString();
            }
            
            return view.Render();
        }

        private string LoadEmailClientParameters(Vendor.IVendorEmailClient client, Query.Models.EmailClient savedClient = null)
        {
            var html = new StringBuilder("<div class=\"row\">");
            foreach (var param in client.Parameters)
            {
                html.Append("<div class=\"col six\">");
                var value = (savedClient != null && savedClient.config.ContainsKey(param.Key) ? savedClient.config[param.Key] : param.Value.DefaultValue).Replace("\"", "&quot;");
                var idattr = " id=\"" + client.Key + "_" + param.Key + "\"";
                switch (param.Value.DataType)
                {
                    case Vendor.EmailClientDataType.Boolean:
                        break;
                    default:
                        html.Append("<div class=\"row field\">" + param.Value.Name + "</div>");
                        break;
                }
                switch (param.Value.DataType)
                {
                    case Vendor.EmailClientDataType.Text:
                        html.Append("<div class=\"row input\"><input type=\"text\"" + idattr + " value=\"" + value + "\"/></div>");
                        break;
                    case Vendor.EmailClientDataType.UserOrEmail:
                        html.Append("<div class=\"row input\"><input type=\"text\"" + idattr + " value=\"" + value + "\" autocomplete=\"new-email\"/></div>");
                        break;
                    case Vendor.EmailClientDataType.Password:
                        html.Append("<div class=\"row input\"><input type=\"password\"" + idattr + " value=\"" + (value != "" ? "********" : "") + "\" autocomplete=\"new-password\"/></div>");
                        break;
                    case Vendor.EmailClientDataType.Number:
                        html.Append("<div class=\"row input\"><input type=\"number\"" + idattr + " value=\"" + value + "\"/></div>");
                        break;
                    case Vendor.EmailClientDataType.List:
                        html.Append("<div class=\"row input\"><select" + idattr + ">" +
                            string.Join("", param.Value.ListOptions?.Select(a => "<option value=\"" + a + "\">" + a + "</option>") ?? new string[] { "" }) +
                            "</select></div>");
                        break;
                    case Vendor.EmailClientDataType.Boolean:
                        html.Append("<div class=\"row input\"><input type=\"checkbox\"" + idattr + (value == "1" || value.ToLower() == "true" ? " checked=\"checked\"" : "") + " />" +
                            "<label for=\"" + client.Key + "_" + param.Key + "\">" + param.Value.Name + "</label>" +
                            "</div>");
                        break;
                }
                html.Append("</div>");
            }
            return html.ToString() + "</div>";
        }

        public string CreateEmailClient(string key, string label, Dictionary<string, string> parameters)
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }
            var client = Common.Email.VendorClients.Where(a => a.Key == key).FirstOrDefault();
            if(client == null)
            {
                return Error("Could not find email client " + key);
            }

            //validate parameters
            try
            {
                ValidateEmailClientParameters(parameters, client);
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }
            //save to the database
            Query.EmailClients.Save("", key, label, parameters);

            return Success();
        }

        public string UpdateEmailClient(string clientId, string key, string label, Dictionary<string, string> parameters)
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }
            var client = Common.Email.VendorClients.Where(a => a.Key == key).FirstOrDefault();
            if (client == null)
            {
                return Error("Could not find email client " + key);
            }

            //validate parameters
            try
            {
                ValidateEmailClientParameters(parameters, client);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
            //save to the database
            Query.EmailClients.Save(clientId, key, label, parameters);

            return Success();
        }

        private void ValidateEmailClientParameters(Dictionary<string, string> parameters, Vendor.IVendorEmailClient client)
        {
            foreach (var item in parameters)
            {
                if (!client.Parameters.ContainsKey(item.Key))
                {
                    throw new Exception("Could not find parameter " + item.Key);
                }
                var param = client.Parameters[item.Key];
                if (param.Required == true && string.IsNullOrEmpty(item.Value))
                {
                    throw new Exception(param.Name + " is required");
                }
                if (param.DataType == Vendor.EmailClientDataType.Boolean)
                {
                    if (item.Value != "True" && item.Value != "False")
                    {
                        throw new Exception(param.Name + " is not valid");
                    }
                }
            }
        }
        #endregion

        #region "Plugins"
        public string RefreshPlugins()
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }
            var item = new View("/Views/App/plugin.html");
            var html = new StringBuilder();
            foreach(var vendor in Core.Vendors.Details)
            {
                item.Clear();
                item["name"] = vendor.Name;
                html.Append(item.Render());
            }
            if(Core.Vendors.Details.Count == 0)
            {
                html.Append(Cache.LoadFile("/Views/App/no-plugins.html"));
            }
            return html.ToString();
        }
        #endregion  
    }
}
