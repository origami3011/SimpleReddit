using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleReddit.Models
{
    public class APIResponse
    {
        [JsonPropertyName("Success")]
        public bool Success { get; set; } = false;
        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;
    }
}
