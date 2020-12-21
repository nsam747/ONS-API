using System.Net.Http;
using System.Threading.Tasks;

namespace Acre.Backend.Ons.Abstractions
{
    // Wrapper around the HttpClient to allow for the interface it implements to be overridded during testing with FakeItEasy
    public class HttpClientHandler : IHttpClient
    {
        private readonly HttpClient _client;
        public HttpClientHandler(HttpClient client) {
            _client = client;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await _client.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _client.SendAsync(request);
        }
    }
}