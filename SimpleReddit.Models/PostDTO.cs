using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleReddit.Models
{
    public class PostDTO 
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("URL")]
        public string URL { get; set; } = string.Empty;
        [JsonPropertyName("ImageURL")]
        public string ImageURL { get; set; } = string.Empty;
        [JsonPropertyName("Content")]
        public string Content { get; set; } = string.Empty;
        [JsonPropertyName("SubReddit")]
        public string SubReddit { get; set; } = string.Empty;
        [JsonPropertyName("PostedDate")]
        public DateTime PostedDate { get; set; } = DateTime.Now;
    }
}
