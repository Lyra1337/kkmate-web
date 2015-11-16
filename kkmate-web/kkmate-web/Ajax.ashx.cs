using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kkmate_web
{
    public class Ajax : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/javascript";
            context.Response.Write("Hello World");
        }
    }
}