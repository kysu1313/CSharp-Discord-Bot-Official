using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.HelperModels
{
    public class RedditResponse
    {

        public string subreddit { get; set; }
        public List<Post> posts { get; set; }
        public bool? isNsfw { get; set; }

    }
}
