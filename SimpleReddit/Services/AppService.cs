using Microsoft.Extensions.Options;
using SimpleReddit.Common.Options;
using SimpleReddit.Contracts;
using SimpleReddit.Models;
using System.Net.Http;

namespace SimpleReddit.Services
{
    public class AppService : IAppService
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly HttpClient _httpClient;

        public AppService(IHttpClientFactory httpClientFactory, IOptions<ApplicationSettings> applicationSettings)
        {
            IHttpClientFactory httpclientFactory = httpClientFactory;
            this._httpClient = httpclientFactory.CreateClient();
            this._applicationSettings = applicationSettings.Value;
        }

        public Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<SubRedditResponse> GetSubRedditAsync(string searchString = null)
        {
            var result = new SubRedditResponse();
            using var productRequest = new HttpRequestMessage(HttpMethod.Get, $"{_applicationSettings.RedditApiEndpoint}getsubreddit?{searchString}");
            var response = await _httpClient.SendAsync(productRequest).ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                result = await response.Content.ReadFromJsonAsync<SubRedditResponse>().ConfigureAwait(false);
            }

            return result;
        }
    }
}
