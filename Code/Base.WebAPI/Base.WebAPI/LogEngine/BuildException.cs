using System;
using System.Linq;
using System.Web;

namespace Base.WebAPI.LogEngine
{
    public class BuildException
    {
        private HttpContext _context;
        private System.Exception _err;
        private ModelException _mExeption;

        public BuildException(HttpContext context, System.Exception err, string request)
        {
            _context = context;
            _err = err;
            _mExeption = BuildExeption(err, request);
            if (_context.User.Identity.IsAuthenticated)
                _mExeption.Identity = _context.User.Identity;
        }

        public ModelException ModelException() => _mExeption;

        private static ModelException BuildExeption(System.Exception err, string request) =>
            new ModelException
            {
                msg = err.FromHierarchy(ex => ex.InnerException).Select(ex => ex.Message),
                request = request,
                source = err.Source,
                stackTrace = err.StackTrace,
                targetSite = err.TargetSite?.DeclaringType.ToString(),
            };
        public override string ToString() => this.ModelException().ToString();
        
    }


    
}
