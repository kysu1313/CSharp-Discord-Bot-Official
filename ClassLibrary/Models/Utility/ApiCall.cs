using System.IO;
using System.Net;
using System.Text;

namespace ClassLibrary.Models.Utility
{
    public static class ApiCall  
    {  
        public static string GetApi(string ApiUrl)  
        {  
  
            var responseString = "";  
            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);  
            request.Method = "GET";  
            request.ContentType = "application/json";  
  
            using (var response1 = request.GetResponse())  
            {  
                using (var reader = new StreamReader(response1.GetResponseStream()))  
                {  
                    responseString = reader.ReadToEnd();  
                }  
            }  
            return responseString;  
  
        }  
  
  
  
        public static string PostApi(string ApiUrl, string postData = "")  
        {  
  
            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);  
            var data = Encoding.ASCII.GetBytes(postData);  
            request.Method = "POST";  
            request.ContentType = "application/x-www-form-urlencoded";  
            request.ContentLength = data.Length;  
            using (var stream = request.GetRequestStream())  
            {  
                stream.Write(data, 0, data.Length);  
            }  
            var response = (HttpWebResponse)request.GetResponse();  
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();  
            return responseString;  
        }  
  
  
  
     
  
  
    }  
}