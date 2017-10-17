using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Kandu.Pipeline
{
    public class WebService
    {
        Core S;

        public WebService(Server server, HttpContext context, string[] paths, IFormCollection form = null)
        {
            //get parameters from request body, including page id
            var parms = new Dictionary<string, string>();
            object[] paramVals;
            var param = "";
            byte[] bytes = new byte[0];
            string data = "";
            int dataType = 0; //0 = ajax, 1 = HTML form post, 2 = multi-part form (with file uploads)

            //figure out what kind of data was sent with the request
            if (form == null)
            {
                //get POST data from request
                using (MemoryStream ms = new MemoryStream())
                {
                    context.Request.Body.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                data = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }else
            {
                //form files exist
                dataType = 2;
            }
            
            if (data.Length > 0)
            {
                if (data.IndexOf("Content-Disposition") > 0)
                {
                    //multi-part file upload
                    dataType = 2;
                }
                else if (data.IndexOf("{") >= 0 && data.IndexOf("}") > 0 && data.IndexOf(":") > 0)
                {
                    //get method parameters from POST S.ajax.post()
                    Dictionary<string, object> attr = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    foreach (KeyValuePair<string, object> item in attr)
                    {
                        parms.Add(item.Key.ToLower(), item.Value.ToString());
                    }
                }
                else if (data.IndexOf("=") >= 0)
                {
                    //HTML form POST data
                    dataType = 1;
                }
            }
            else
            {
                //get method parameters from query string
                foreach(var key in context.Request.Query.Keys)
                {
                    parms.Add(key.ToLower(), context.Request.Query[key].ToString());
                }
            }

            //start building Web API response (find method to execute & return results)
            S = new Core(server, context);

            //load service class from URL path
            string className = "Kandu.Services." + paths[1];
            string methodName = paths[2];
            if(paths.Length == 4) { className += "." + paths[2]; methodName = paths[3]; }
            var service = GetService(className);

            if (dataType == 1)
            {
                //parse HTML form POST data and send to new Service instance
                string[] items = S.Server.UrlDecode(data).Split('&');
                string[] item;
                for(var x = 0; x < items.Length; x++)
                {
                    item = items[x].Split('=');
                    service.Form.Add(item[0], item[1]);
                }
            }else if(dataType == 2)
            {
                //send multi-part file upload data to new Service instance
                service.Files = form.Files;
            }

            //execute method from new Service instance
            Type type = Type.GetType(className);
            MethodInfo method = type.GetMethod(methodName);

            //try to cast params to correct types
            ParameterInfo[] methodParams = method.GetParameters();

            paramVals = new object[methodParams.Length];
            for(var x = 0; x < methodParams.Length; x++)
            {
                //find correct key/value pair
                param = "";
                foreach(var item in parms)
                {
                    if(item.Key == methodParams[x].Name.ToLower())
                    {
                        param = item.Value;
                        break;
                    }
                }
                //cast params to correct (supported) types
                switch (methodParams[x].ParameterType.Name.ToLower())
                {
                    case "int32":
                        paramVals[x] = Int32.Parse(param);
                        break;

                    case "boolean":
                        paramVals[x] = param.ToLower() == "true" ? true : false;
                        break;

                    case "double":
                        paramVals[x] = double.Parse(param);
                        break;

                    case "datetime":
                        paramVals[x] = DateTime.Parse(param);
                        break;

                    default:
                        paramVals[x] = param;
                        break;
                }
            }

            object result = null;

            try
            {
                result = method.Invoke(service, paramVals);
            }catch(Exception ex)
            {
                throw ex.InnerException;
            }


            //finally, unload the Kandu Core:
            //close SQL connection, save User info, etc (before sending response)
            S.Unload();
            context.Response.ContentType = "text/json";
            if (result != null)
            {
                context.Response.WriteAsync((string)result);
            }else {
                context.Response.WriteAsync("{\"error\":\"no content returned\"}");
            }
        }

        private Service GetService(string className)
        {
            //hard-code all known services to increase server performance
            switch(className.ToLower()){

                default:
                    //last resort, find service class manually
                    Type type = Type.GetType(className);
                    return (Service)Activator.CreateInstance(type, new object[] { S });
            }
        }
    }
}
