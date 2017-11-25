using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Base.WebAPI
{
    public class BaseApiController : ApiController
    {
        public BaseApiController() : base()
        {
        }

        protected virtual Task<HttpResponseMessage> TaskHttpResponseMessage(HttpStatusCode statusCode, object result)
        {
            var res = Request.CreateResponse(statusCode, result);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(res);
            return tsc.Task;
        }

    }
}
