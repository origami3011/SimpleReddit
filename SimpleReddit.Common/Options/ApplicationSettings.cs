using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReddit.Common.Options
{
    public class ApplicationSettings
    {
        public bool UseRedisCache { get; set; }
        public string RedditApiEndpoint { get; set; } 
    }
}
