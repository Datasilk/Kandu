using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Kandu.Vendor;


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

        #region "Email Clients"
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
                        //key = action.Key,
                        name = action.Name,
                        //fromname = config?.fromName ?? "",
                        //fromaddress = config?.fromAddress ?? "",
                        //templatefile = action.TemplateFile,
                        //subject = config?.subject ?? action.SubjectDefault,
                        //options = string.Join("",
                        //    clients.Select(a => "<option value=\"" + a.clientId + "\"" +
                        //    (config?.clientId == a.clientId ? " selected=\"selected\"" : "") +
                        //    ">" + emailClients.Where(b => b.Key == a.key).FirstOrDefault()?.Name + ": " + a.label + "</option>"
                        //    )),
                        onclick = "S.kandu.email.action.details('" + action.Key + "')"
                    }
                });
                if (action.UserDefinedSubject) { viewEmailAction.Show("user-subject"); }
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
                view["clientId"] = config.clientId.ToString();
                view["key"] = client.Key;
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

        private string LoadEmailClientParameters(IVendorEmailClient client, Query.Models.EmailClient savedClient = null)
        {
            var html = new StringBuilder("<div class=\"row\">");
            foreach (var param in client.Parameters)
            {
                html.Append("<div class=\"col six\">");
                var value = (savedClient != null && savedClient.config.ContainsKey(param.Key) ? savedClient.config[param.Key] : param.Value.DefaultValue).Replace("\"", "&quot;");
                var idattr = " id=\"" + client.Key + "_" + param.Key + "\"";
                var placeholder = !string.IsNullOrEmpty(param.Value.Placeholder) ? " placeholder=\"" + param.Value.Placeholder + "\"" : "";
                switch (param.Value.DataType)
                {
                    case EmailClientDataType.Boolean:
                        break;
                    default:
                        html.Append("<div class=\"row field\">" + param.Value.Name + "</div>");
                        break;
                }
                switch (param.Value.DataType)
                {
                    case EmailClientDataType.Text:
                        html.Append("<div class=\"row input\"><input type=\"text\"" + idattr + " value=\"" + value + "\"" + placeholder + "/></div>");
                        break;
                    case EmailClientDataType.UserOrEmail:
                        html.Append("<div class=\"row input\"><input type=\"text\"" + idattr + " value=\"" + value + "\"" + placeholder + " autocomplete=\"new-email\"/></div>");
                        break;
                    case EmailClientDataType.Password:
                        html.Append("<div class=\"row input\"><input type=\"password\"" + idattr + " value=\"" + (value != "" ? "********" : "") + "\" autocomplete=\"new-password\"/></div>");
                        break;
                    case EmailClientDataType.Number:
                        html.Append("<div class=\"row input\"><input type=\"number\"" + idattr + " value=\"" + value + "\"" + placeholder + "/></div>");
                        break;
                    case EmailClientDataType.List:
                        html.Append("<div class=\"row input\"><select" + idattr + ">" +
                            string.Join("", param.Value.ListOptions?.Select(a => "<option value=\"" + a + 
                                (param.Value.DefaultValue == a ? " selected" : "") + 
                                "\">" + a + "</option>") ?? new string[] { "" }) + "</select></div>");
                        break;
                    case EmailClientDataType.Boolean:
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
                ValidateEmailClientParameters(parameters, client, Query.EmailClients.GetConfig(clientId));
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
            //save to the database
            Query.EmailClients.Save(clientId, key, label, parameters);

            //update cached objects

            var vendorClient = Core.Vendors.EmailClients.Where(a => a.Key == key).FirstOrDefault();
            Common.Email.ClearVendorClients();
            Common.Email.ClearActions();
            vendorClient.Value.Parameters = Common.Email.VendorClients.Where(a => a.Key == key).FirstOrDefault()?.Parameters ?? vendorClient.Value.Parameters;
            return Success();
        }

        private void ValidateEmailClientParameters(Dictionary<string, string> parameters, IVendorEmailClient client, Query.Models.EmailClient emailClient = null)
        {
            var changes = new List<KeyValuePair<string, string>>();
            foreach (var item in parameters)
            {
                if (!client.Parameters.ContainsKey(item.Key))
                {
                    throw new Exception("Could not find parameter " + item.Key);
                }
                var param = client.Parameters[item.Key];
                if (param.Required == true && string.IsNullOrEmpty(item.Value) && param.DataType != EmailClientDataType.Password)
                {
                    throw new Exception(param.Name + " is required");
                }
                if(param.DataType == EmailClientDataType.Password)
                {
                    //find password placeholder
                    if(item.Value.Replace("*", "") == "")
                    {
                        if(emailClient != null && emailClient.config.ContainsKey(item.Key))
                        {
                            //no password change
                            changes.Add( new KeyValuePair<string, string>(item.Key, emailClient.config[item.Key]));
                        }
                        else
                        {
                            //no password
                            changes.Add(new KeyValuePair<string, string>(item.Key, ""));
                        }
                        
                    }
                }

                //check boolean parameters for valid values
                if (param.DataType == EmailClientDataType.Boolean)
                {
                    if (item.Value != "True" && item.Value != "False")
                    {
                        throw new Exception(param.Name + " is not valid");
                    }
                }

                //make chamges to parameters list
                foreach (var change in changes)
                {
                    parameters[change.Key] = change.Value;
                }
            }
        }

        public string RemoveEmailClient(string clientId)
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }
            try
            {
                Query.EmailClients.Remove(clientId);
            }catch(Exception)
            {
                return Error("An error occurred when trying to remove the email client, ID " + clientId);
            }
            return Success();
        }
        #endregion

        #region "Email Actions"

        public string RenderEmailActionForm(string key = "")
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }

            //generate email action form
            var view = new View("/Views/Email/action-form.html");
            var action = Common.Email.VendorActions.Where(a => a.Key == key).FirstOrDefault();
            if (action == null)
            {
                return Error("Could not find email action \"" + key + "\"");
            }
            var config = Query.EmailActions.GetInfo(key);
            if(config == null)
            {
                //load default email action template
                var template = Cache.LoadFile(action.TemplateFile);
                config = new Query.Models.EmailClientAction()
                {
                    clientId = 0,
                    subject = action.DefaultSubject,
                    bodyText = "",
                    bodyHtml = template
                };
            }
            if(config.subject == "")
            {
                config.subject = action.DefaultSubject;
            }

            var emailClients = Common.Email.VendorClients;
            var clients = Query.EmailClients.GetList();
            var clientoptions = new StringBuilder();
            foreach (var item in clients)
            {
                var vendorClient = emailClients.Where(a => a.Key == item.key).FirstOrDefault();
                clientoptions.Append("<option value=\"" + item.clientId + "\"" +
                    (
                        string.IsNullOrEmpty(key) ?
                        (item == emailClients[0] ? " selected" : "") :
                        (item.clientId == (config != null ? config.clientId : 0) ? " selected" : "")
                    ) +
                    ">" + (vendorClient?.Name ?? "[Unknown client]") + " - " + item.label + "</option>");
            }
            view["client-options"] = clientoptions.ToString();
            if (config != null) { view.Bind(config); }
            if(action.UserDefinedSubject == true)
            {
                view.Show("user-subject");
            }
            if (action.UserDefinedBody == true)
            {
                view.Show("user-body");
            }
            return view.Render();
        }

        public string UpdateEmailAction(string key, int clientId, string fromName, string fromAddress, string subject, string bodyText, string bodyHtml)
        {
            if (!User.IsAppOwner()) { return AccessDenied(); }
            try
            {
                Query.EmailActions.Save(key, clientId, subject, fromName, fromAddress, bodyText, bodyHtml);
            }
            catch (Exception ex)
            {
                return Error("Error saving email action");
            }
            return Success();
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
