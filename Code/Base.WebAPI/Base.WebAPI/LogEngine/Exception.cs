using System;
using System.Web;

namespace Base.WebAPI.LogEngine
{
    public class Exception : System.Exception
    {
        public Exception(string message): base(message)
        {
            var bE = new BuildException(HttpContext.Current, new System.Exception(message), HttpContext.Current.Request.ToString());
        }

        public Exception(string message, System.Exception inner)
        : base(message, inner)
        {
            var bE = new BuildException(HttpContext.Current, inner, HttpContext.Current.Request.ToString());
        }

        public void Log() {

        }
    }
}
