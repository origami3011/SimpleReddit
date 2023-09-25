using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReddit.Models
{
    public class RedditSetting
    {
        public string AppId { get; set; } 
        public string AppSecret { get; set; } 
        public string AppName { get; set; } 
        public string Redirect_uri { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
