using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleReddit.Models
{
    public class SubRedditResponse : APIResponse
    {
        [JsonPropertyName("postDTOs")]
        public List<PostDTO> PostDTOs { get; set; } = new List<PostDTO>();
    }
}
