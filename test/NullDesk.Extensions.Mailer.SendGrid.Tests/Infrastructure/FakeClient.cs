using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SendGrid;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{
    public class FakeClient : Client
    {
        public Response FakeTestResponse { get; set; }
        public FakeClient
        (
            string apiKey,
            string host = null,
            Dictionary<string, string> requestHeaders = null,
            string version = "v3",
            string urlPath = null
        ) :
        base
        (
            apiKey,
            host,
            requestHeaders,
            version,
            urlPath
        )
        { }

        public override Task<Response> MakeRequest(HttpClient client, HttpRequestMessage request)
        {
            return Task.FromResult(FakeTestResponse ?? new Response(HttpStatusCode.Accepted, null, null));
        }
    }
}
