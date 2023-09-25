using SimpleReddit.Models;

namespace SimpleReddit.Contracts
{
    public interface IAppService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);

        Task<SubRedditResponse> GetSubRedditAsync(string searchString = null);
    }
}
