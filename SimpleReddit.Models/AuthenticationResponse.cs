using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleReddit.Models
{
    public class AuthenticationResponse : APIResponse
    {
        [JsonPropertyName("AccessToken")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
