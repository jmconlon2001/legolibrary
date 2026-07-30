using System.Net;
using System.Text;

namespace LegoTracker.Tests.TestSupport;

/// <summary>Routes requests by exact path+query to a canned JSON response, for testing HttpClient-based services without network access.</summary>
public class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, string> _responsesByPathAndQuery = new();

    public StubHttpMessageHandler RespondWith(string pathAndQuery, string json)
    {
        _responsesByPathAndQuery[pathAndQuery] = json;
        return this;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var key = request.RequestUri!.PathAndQuery;
        if (!_responsesByPathAndQuery.TryGetValue(key, out var json))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}
