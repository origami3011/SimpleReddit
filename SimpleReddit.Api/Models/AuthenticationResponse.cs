using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReddit.Api.Models
{
    public class AuthenticationResponse : APIResponse
    {
        public AuthenticationResponse(bool success, string message) : base(success, message)
        {
        }

        public string AccessToken { get; set; } = string.Empty;
        public int ExpireIn { get; set; }
    }
}
