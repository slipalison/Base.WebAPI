using System.Net.Http;
using System.Web.Http.Filters;

namespace Base.WebAPI.JsonCompression
{
    public sealed class DeflateCompressionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actContext)
        {
            try
            {
                var content = actContext.Response.Content;
                var bytes = content?.ReadAsByteArrayAsync().Result;
                var zlibbedContent = bytes == null ? new byte[0] :
                CompressionHelper.DeflateByte(bytes);
                actContext.Response.Content = new ByteArrayContent(zlibbedContent);
                actContext.Response.Content.Headers.Remove("Content-Type");
                actContext.Response.Content.Headers.Add("Content-encoding", "deflate");
                actContext.Response.Content.Headers.Add("Content-Type", "application/json");
                base.OnActionExecuted(actContext);
            }
            catch (System.Exception)
            {

                throw actContext.Exception;
            }
        }
    }
}
