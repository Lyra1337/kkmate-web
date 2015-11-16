using kkmate_web.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace kkmate_web.Controllers
{
    public class ApiController
    {
        private static readonly ApiController instance = new ApiController();
        public static ApiController Instance => ApiController.instance;

        private Dictionary<string, Type> AvailableTypes { get; }

        public ApiController()
        {
            this.AvailableTypes = this.GetAvailableTypes();
        }

        public void HandleRequest(HttpContext context)
        {
            var url = context.Request.Url.LocalPath.ToLower();

            if (url.StartsWith("/api.json") == true)
            {
                string typeString = context.Request.QueryString["type"];
                string actionString = context.Request.QueryString["action"];
                string parameters = context.Request.QueryString["params"];

                string response = null;

                if (String.IsNullOrEmpty(typeString) == false && String.IsNullOrEmpty(actionString) == false)
                {
                    Type type = this.AvailableTypes[typeString];

                    if (type != null)
                    {
                        var method = type.GetMethod(actionString);

                        if (method != null)
                        {
                            object[] passingParameters = this.GetParametersForMethod(method, parameters);

                            var result = method.Invoke(Activator.CreateInstance(type), passingParameters);
                            response = JsonConvert.SerializeObject(result);
                        }
                        else
                        {
                            response = "Method not found";
                            context.Response.StatusCode = 404;
                        }
                    }
                    else
                    {
                        response = "Type not found";
                        context.Response.StatusCode = 404;
                    }
                }
                else
                {
                    response = "You have to specify 'type' and 'action'";
                    context.Response.StatusCode = 404;
                }

                this.WriteStream(context.Response.OutputStream, response);
            }
        }

        private object[] GetParametersForMethod(MethodInfo method, string parameters)
        {
            List<object> result = new List<object>();
            ParameterInfo[] methodParams = method.GetParameters();

            if (methodParams.Length == 0)
            {
                return null;
            }

            if (parameters.Length == 0)
            {
                return null;
            }

            string[] splittedParameters = parameters.Split(',');

            if (methodParams.Length != parameters.Length)
            {
                throw new ArgumentException("parameter count does not match");
            }

            for (int i = 0; i < methodParams.Length; i++)
            {
                ParameterInfo param = methodParams[i];
                string value = splittedParameters[i];

                if (param.ParameterType == typeof(int))
                {
                    result.Add(Int32.Parse(value));
                }
                else if (param.ParameterType == typeof(bool))
                {
                    result.Add(Boolean.Parse(value));
                }
                else
                {
                    result.Add(value);
                }
            }

            return result.ToArray();
        }

        private void WriteStream(Stream stream, string text)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(text);
            }
        }

        private Dictionary<string, Type> GetAvailableTypes()
        {
            var attributeType = typeof(ApiAccessibleAttribute);

            return Assembly.GetAssembly(attributeType).GetTypes()
                .Where(x => x.GetCustomAttributes(attributeType, false).Any())
                .ToDictionary(x => x.Name, x => x);
        }
    }
}