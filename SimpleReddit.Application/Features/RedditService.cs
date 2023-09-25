
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;
using SimpleReddit.Application.Contracts;
using SimpleReddit.Application.Models;
using SimpleReddit.Application.Utilities;
using SimpleReddit.AuthTokenLib;
using SimpleReddit.AuthTokenLib.EventArgs;

namespace SimpleReddit.Application.Features
{
    public class RedditService : IRedditService
    {
        private const string BROWSER_PATH = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

        public RedditSetting _redditSetting { get; set; }
        private RedditClient _redditClient;
        private readonly IDistributedCacheService _cacheService;

        private string accessToken;
        private string refreshToken;

        public RedditService(IOptions<RedditSetting> settings, IDistributedCacheService cacheService)
        {
            _redditSetting = settings.Value;
            _cacheService = cacheService;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            var accessToken = await _cacheService.GetCacheAsync<AuthSuccessEventArgs>($"cache_reddit_accesstoken").ConfigureAwait(false);

            AuthenticationResponse response = new AuthenticationResponse();
            if (accessToken == null)
            {
                // Create a new instance of the auth token retrieval library.  --Kris
                AuthTokenRetrieverLib authTokenRetrieverLib = new AuthTokenRetrieverLib(_redditSetting.AppId, 8080, "localhost", _redditSetting.Redirect_uri, _redditSetting.AppSecret);

                authTokenRetrieverLib.AuthSuccess += C_AuthSuccess;

                // Start the callback listener.  --Kris
                authTokenRetrieverLib.AwaitCallback(true);

                OpenBrowser(authTokenRetrieverLib.AuthURL());

                authTokenRetrieverLib.StopListening();

                response = new AuthenticationResponse
                {
                    AppId = request.AppId,
                    AppSecret = request.AppSecret,
                    AccessToken = authTokenRetrieverLib.AccessToken,
                    RefreshToken = authTokenRetrieverLib.RefreshToken,
                };
            }
            else
            {
                response = new AuthenticationResponse()
                {
                    AppId = request.AppId,
                    AppSecret = request.AppSecret,
                    AccessToken = accessToken.AccessToken,
                    RefreshToken = accessToken.RefreshToken,
                };
            }

            //_redditClient = new RedditClient(_redditSetting.AppId, _redditSetting.RefreshToken, _redditSetting.AppSecret, _redditSetting.AccessToken);

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

        public Task<AuthenticationResponse> RedirectUrlAsync(string error, string code, string state)
        {
            throw new NotImplementedException();
        }

        public async Task<SubRedditResponse> GetSubRedditAsync(string searchString, string subReddit = "all")
        {
            RedditClient reddit = new RedditClient(_redditSetting.AppId, refreshToken, _redditSetting.AppSecret, accessToken);

            SubRedditResponse response = new SubRedditResponse(false, string.Empty);

            if (string.IsNullOrWhiteSpace(subReddit))
            {
                subReddit = "all";
            }
            List<Post> subs = reddit.Subreddit(subReddit).Search(new SearchGetSearchInput(searchString));
            foreach (Post post in subs)
            {
                PostDTO dig = new(post);
                dig.Title = post.Title;

                response.PostDTOs.Add(dig);
            }

            response.Success = true;

            return response;
        }
        public void C_AuthSuccess(object sender, AuthSuccessEventArgs e)
        {
            Console.Clear();

            Console.WriteLine("Token retrieval successful!");

            Console.WriteLine();

            Console.WriteLine("Access Token: " + e.AccessToken);
            Console.WriteLine("Refresh Token: " + e.RefreshToken);

            _cacheService.AddOrUpdateCacheAsync("cache_reddit_accesstoken", e);

            //_cacheService.AddOrUpdateCacheAsync<IEnumerable<AuthSuccessEventArgs>>()$"cache_reddit_accesstoken", e);

            Console.WriteLine();

            Console.WriteLine("Press any key to exit....");
        }
    }
}
