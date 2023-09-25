using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;
using SimpleReddit.Api.Contracts;
using SimpleReddit.Models;
using SimpleReddit.AuthTokenLib;
using SimpleReddit.AuthTokenLib.EventArgs;
using SimpleReddit.Cache.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SimpleReddit.Api.Controllers;
using System;
using Reddit.Inputs;
using System.Diagnostics.Metrics;
using uhttpsharp.Handlers;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net;
using uhttpsharp.Clients;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Reddit.Models.Internal;
using Microsoft.AspNetCore.Mvc;
using SimpleReddit.FileService.Contracts;
using SimpleReddit.FileService;
using Newtonsoft.Json;
using RestSharp;

namespace SimpleReddit.Api.Services
{
    public class RedditService : IRedditService
    {
        private const string BROWSER_PATH = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

        public RedditSetting _redditSetting { get; set; }
        private Reddit.RedditClient _redditClient;
        private readonly IDistributedCacheService _cacheService;
        private readonly ILogger<RedditService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public RedditService(IOptions<RedditSetting> settings, IDistributedCacheService cacheService, ILogger<RedditService> logService, IHttpClientFactory httpClientFactory, ITokenService tokenService)
        {
            _redditSetting = settings.Value;
            _cacheService = cacheService;
            _logger = logService;
            _tokenService = tokenService;

            IHttpClientFactory httpclientFactory = httpClientFactory;
            _httpClient = httpclientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://oauth.reddit.com");
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            if (request == null || !request.AppId.Equals(_redditSetting.AppId) || !request.AppSecret.Equals(_redditSetting.AppSecret) )
             {
                return new AuthenticationResponse()
                {
                    Message = "UnauthorizedResult",
                    Success = false
                };
            }
            var accessToken = await GetCurrentToken();
            _logger.LogInformation($"Current accesstoken {accessToken}");

            AuthenticationResponse response = new AuthenticationResponse()
            {
                Message = "Success",
                Success = true
            };

            if (accessToken == null)
            {
                try
                {
                    // Create a new instance of the auth token retrieval library.  --Kris
                    AuthTokenRetrieverLib authTokenRetrieverLib = new AuthTokenRetrieverLib(_redditSetting.AppId, 8080, "localhost", _redditSetting.Redirect_uri, _redditSetting.AppSecret);

                    authTokenRetrieverLib.AuthSuccess += C_AuthSuccess;

                    // Start the callback listener.  --Kris
                    authTokenRetrieverLib.AwaitCallback(false);

                    OpenBrowser(authTokenRetrieverLib.AuthURL());

                    authTokenRetrieverLib.StopListening();

                    response.AccessToken = authTokenRetrieverLib.AccessToken;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = "Error, please contact admin";
                    _logger.LogError($"AuthenticateAsync error {ex.Message}");                    
                }                
            }
            else
            {
                response.AccessToken = accessToken.AccessToken;
            }

            return response;
        }

        public static void OpenBrowser(string authUrl = "about:blank")
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(authUrl);
                    Process.Start(processStartInfo);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(BROWSER_PATH)
                    {
                        Arguments = authUrl
                    };
                    Process.Start(processStartInfo);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // For OSX run a separate command to open the web browser as found in https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
                Process.Start("open", authUrl);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Similar to OSX, Linux can (and usually does) use xdg for this task.
                Process.Start("xdg-open", authUrl);
            }
        }

        public async Task<SubRedditResponse> GetSubRedditAsync(string subReddit = "all", string? after = "")
        {
            SubRedditResponse response = new SubRedditResponse();
            var accessToken = await GetCurrentToken();

            if (accessToken == null)
                return response;
            try
            {
                _logger.LogInformation($"GetSubRedditAsync subReddit {subReddit} token {accessToken.AccessToken}");

                //response = await GetSub(subReddit, after);

                if (_redditClient == null)
                {
                    _redditClient = new RedditClient(_redditSetting.AppId, accessToken.RefreshToken, _redditSetting.AppSecret, accessToken.AccessToken);
                }

                List<Post> subs = _redditClient.Subreddit(subReddit).Posts.GetNew(new CategorizedSrListingInput(after: after, limit: 25));
                foreach (Post post in subs)
                {
                    PostDTO dig = new PostDTO()
                    {
                        Title = post.Title,
                        URL = "https://reddit.com" + post.Permalink,
                        Content = post.Listing.SelfText,
                        SubReddit = post.Subreddit,
                        PostedDate = post.Created,
                        ImageURL = post.Listing.URL
                    };
                    response.PostDTOs.Add(dig);
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return response;
        }
        public void C_AuthSuccess(object sender, AuthSuccessEventArgs e)
        {
            _cacheService.AddOrUpdateCacheAsync("cache_reddit_accesstoken", e);
            _tokenService.SaveTokenAsync(JsonConvert.SerializeObject(e));
            _logger.LogInformation($"Set accessToken {e.AccessToken} and refreshToken {e.RefreshToken}");            
        }

        public async Task<SubRedditResponse> GetSub(string subReddit = "all", string? after = "")
        {
            SubRedditResponse result = new SubRedditResponse();
            var accessToken = await _cacheService.GetCacheAsync<AuthSuccessEventArgs>($"cache_reddit_accesstoken").ConfigureAwait(false);

            if (accessToken == null) { return result; }

            using var productRequest = new HttpRequestMessage(HttpMethod.Get, $"/r/{subReddit}/new");
            productRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.AccessToken);
            var response = await _httpClient.SendAsync(productRequest).ConfigureAwait(false);

            if (response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var rateLimitRemainingHeader = response.Headers.GetValues("x-ratelimit-remaining").FirstOrDefault();
                var timeToResetRateLimitHeader = response.Headers.GetValues("x-ratelimit-reset").FirstOrDefault();

                string apiContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var list = await response.Content.ReadFromJsonAsync<List<Post>>().ConfigureAwait(false);
                if (list != null && list.Count > 0)
                {
                    foreach (Post post in list)
                    {
                        PostDTO dig = new PostDTO()
                        {
                            Title = post.Title,
                            URL = "https://reddit.com" + post.Permalink,
                            Content = post.Listing.SelfText,
                            SubReddit = post.Subreddit,
                            PostedDate = post.Created,
                            ImageURL = post.Listing.URL
                        };
                        result.PostDTOs.Add(dig);
                    }
                    result.Success = true;
                }
            }
            else
            {
                //  log for api error
                _logger.LogDebug(response.ToString());
            }
            return result;
        }

        public async Task<AuthSuccessEventArgs> GetCurrentToken()
        {
            var accessToken = new AuthSuccessEventArgs();
            accessToken = await _cacheService.GetCacheAsync<AuthSuccessEventArgs>($"cache_reddit_accesstoken").ConfigureAwait(false);
            if (accessToken == null)
            {                
                accessToken = JsonConvert.DeserializeObject<AuthSuccessEventArgs>(_tokenService.GetTokenAsync());
                _cacheService.AddOrUpdateCacheAsync("cache_reddit_accesstoken", accessToken);                
            }
            return accessToken;
        }
    }
}
