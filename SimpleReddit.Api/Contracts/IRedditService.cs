using SimpleReddit.Models;

namespace SimpleReddit.Api.Contracts
{
    public interface IRedditService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);

        Task<SubRedditResponse> GetSubRedditAsync(string subReddit = "all", string? after = "");
    }
}
