using System;
using System.Web;

namespace Base.WebAPI.LogEngine
{
    public class SmlException : Exception
    {
        public SmlException(string message): base(message)
        {
            var bE = new BuildException(HttpContext.Current, new Exception(message), HttpContext.Current.Request.ToString());
        }

        public SmlException(string message, Exception inner)
        : base(message, inner)
        {
            var bE = new BuildException(HttpContext.Current, inner, HttpContext.Current.Request.ToString());
        }

        public void Log() {

        }
    }
}
