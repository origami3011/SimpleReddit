using Reddit.Controllers;
using SimpleReddit.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReddit.Application.Contracts
{
    public interface IRedditService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);

        Task<AuthenticationResponse> RedirectUrlAsync(string error, string code, string state);

        Task<SubRedditResponse> GetSubRedditAsync(string searchString, string subReddit = "all");
    }
}
