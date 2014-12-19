using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GraderApi.Handlers
{
    public class PermissionsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var route = request.GetRouteData().Values["courseId"];
            var result =  request.CreateResponse(HttpStatusCode.OK, route.ToString());
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(result);
            return tsc.Task;
        }
    }
}