using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReddit.Application.Models
{
    public class AuthenticationRequest
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty; 
        public string AccessToken { get; set; } = string.Empty;
        public string Redirect_uri { get; set; } = string.Empty;
    }
}
