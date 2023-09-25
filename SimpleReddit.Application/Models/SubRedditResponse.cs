using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReddit.Application.Models
{
    public class SubRedditResponse : APIResponse
    {
        public SubRedditResponse(bool success, string message) : base(success, message)
        {
            PostDTOs = new List<PostDTO>();
        }

        public List<PostDTO> PostDTOs { get; set; }
    }
}
