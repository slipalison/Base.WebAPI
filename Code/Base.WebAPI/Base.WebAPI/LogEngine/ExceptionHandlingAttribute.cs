using Base.WebAPI.LogEngine;
using System;
using System.IO;
using System.Web;
using System.Web.Http.Filters;

namespace Base.WebAPI
{
    public sealed class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        private readonly string _logFolder;
        private ModelException _mExeption;


        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {

            var bm = new BuildException(HttpContext.Current, actionExecutedContext.Exception, actionExecutedContext.Request.ToString());
            _mExeption = bm.ModelException();

            WriteLog.Write(_logFolder, _mExeption.ToString());

            base.OnException(actionExecutedContext);
        }
        public ExceptionHandlingAttribute() : base()
        {
            _logFolder = AppDomain.CurrentDomain.BaseDirectory + @"log";
        }

        public ExceptionHandlingAttribute(string logFolder) : base()
        {

            _logFolder = logFolder != null ? logFolder : AppDomain.CurrentDomain.BaseDirectory;
        }

    }

}
