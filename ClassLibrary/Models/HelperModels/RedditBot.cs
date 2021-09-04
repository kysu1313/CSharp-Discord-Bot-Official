using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class RedditBot
    {
        public string _id { get; set; }
        public string _username { get; set; }
        public string _password { get; set; }
        public string _token { get; set; }
        public string _endpoint { get; set; }

        public RedditBot(string id, string username, string password, string token, string endpoint)
        {
            _id = id;
            _username = username;
            _password = password;
            _token = token;
            _endpoint = endpoint;
        }
    }
}
