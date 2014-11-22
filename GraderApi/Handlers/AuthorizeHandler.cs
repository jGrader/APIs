using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraderDataAccessLayer.Repositories;


namespace GraderApi.Handlers
{
    public class AuthorizeHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            /*
            var sessionIdRepository = new SessionIdRepository();
            var query = request.RequestUri.ParseQueryString();
            int userId = int.TryParse(query["UserId"], out userId) ? userId : 0;

            var result = sessionIdRepository.IsAuthorized(userId);
            if (!result)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
            */
            return base.SendAsync(request, cancellationToken);
        }
    }
}