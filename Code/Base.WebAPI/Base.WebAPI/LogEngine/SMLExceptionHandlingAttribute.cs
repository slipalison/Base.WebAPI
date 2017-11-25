using Base.WebAPI.LogEngine;
using System;
using System.IO;
using System.Web;
using System.Web.Http.Filters;

namespace Base.WebAPI
{
    public sealed class SMLExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        private readonly string _logFolder;
        private ModelException _mExeption;
        public SMLExceptionHandlingAttribute() : base()
        {
            _logFolder = AppDomain.CurrentDomain.BaseDirectory + @"log" ;
        }

        public SMLExceptionHandlingAttribute(string logFolder) : base()
        {
            
            _logFolder = logFolder != null ? logFolder : AppDomain.CurrentDomain.BaseDirectory;
        }


        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {

            var bm = new BuildException(HttpContext.Current, actionExecutedContext.Exception, actionExecutedContext.Request.ToString());
            _mExeption = bm.ModelException();

            WriteLog.Write(_logFolder, _mExeption.ToString());

            base.OnException(actionExecutedContext);
        }

    }

}
