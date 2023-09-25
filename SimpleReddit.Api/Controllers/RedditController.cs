using Microsoft.AspNetCore.Mvc;
using SimpleReddit.Api.Contracts;
using SimpleReddit.Models;

namespace SimpleReddit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedditController : ControllerBase
    {
        private readonly IRedditService _redditService;
        public RedditController(IRedditService redditService)
        {
            _redditService = redditService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _redditService.AuthenticateAsync(request));
        }

        [HttpGet("getsubreddit")]
        public async Task<ActionResult<SubRedditResponse>> GetSubRedditAsync(string subReddit = "all", string? after = "")
        {
            return Ok(await _redditService.GetSubRedditAsync(subReddit, after));
        }
    }
}
