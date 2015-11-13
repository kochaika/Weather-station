using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net;
using System.IO;

// More information about OAuth v1.0: http://oauth.net/core/1.0/
// More information about TwitterAPI: https://dev.twitter.com/rest/public/search

/*
 * Updates the current status, also known as Tweeting
 * Twitter account for updating statuses: https://twitter.com/pro100kot14
 */
namespace Twitter
{
    class TwitterStatusUpdater
    {
        public TwitterStatusUpdater()
        {

        }

        public void Update(string tweet)
        {
            if (tweet.Length > 140)
            {
                throw new ApplicationException("Too long argument ("+ tweet.Length +" characters). Tweet size should not be more than 140 characters");
            }
            // request details
            var oauth_nonce = Convert.ToBase64String(
                new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow
                - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // OAuth Base String
        string baseString = "oauth_consumer_key=" + oauth_consumer_key +
                             "&oauth_nonce=" + oauth_nonce +
                             "&oauth_signature_method=" + oauth_signature_method +
                             "&oauth_timestamp=" + oauth_timestamp +
                             "&oauth_token=" + oauth_token +
                             "&oauth_version=" + oauth_version +
                             "&status=" + Uri.EscapeDataString(tweet);

            baseString = string.Concat("POST&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));
            
            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                    "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // Auth header
            string authHeader = "OAuth " + 
                                "oauth_nonce=\"" + Uri.EscapeDataString(oauth_nonce) + "\", " +
                                "oauth_signature_method=\"" + Uri.EscapeDataString(oauth_signature_method) + "\", " +
                                "oauth_timestamp=\"" + Uri.EscapeDataString(oauth_timestamp) + "\", " +
                                "oauth_consumer_key=\"" + Uri.EscapeDataString(oauth_consumer_key) + "\", " +
                                "oauth_token=\"" + Uri.EscapeDataString(oauth_token) + "\", " +
                                "oauth_signature=\"" + Uri.EscapeDataString(oauth_signature) + "\", " +
                                "oauth_version=\"" + Uri.EscapeDataString(oauth_version) + "\", ";

            // Request
            ServicePointManager.Expect100Continue = false;
            var postBody = "status=" + Uri.EscapeDataString(tweet);
            var request_url = resource_url + "?" + postBody;
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(request_url);
                request.Headers.Add("Authorization", authHeader);
                request.Method = "POST";
                WebResponse response = request.GetResponse();
                responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (System.Net.WebException e)
            {
                throw new ApplicationException("Twitter server response not OK or other WebExeption:\n"+e.ToString());
            }
            catch(System.Exception e)
            {
                throw new ApplicationException("Error while connecting to Twitter server:\n" + e.ToString());
            }
        }

        public string GetLastResponse()
        {
            return responseData;
        }

            // OAuth keys
        private const string oauth_token = "4142570999-UddnmbFfu3yLJLzZNv542dfYirLs5OlJELA5rVJ";
        private const string oauth_token_secret = "70I0x4NlV87CmwWSDqlQHWrXuFCYUHt046nvzD5kIDZT3";
        private const string oauth_consumer_key = "5CEgo3UiwooHV985j5SM4pfgl";
        private const string oauth_consumer_secret = "tBSNilkfhWVYljYCW4qwwPgxXO7hhf07yGtElC8ED7A3Zwos26";
            // OAuth details
        private const string oauth_version = "1.0";
        private const string oauth_signature_method = "HMAC-SHA1";
            //Twitter API Update
            //Read more: https://dev.twitter.com/rest/reference/post/statuses/update
        private const string resource_url = "https://api.twitter.com/1.1/statuses/update.json";
            //Last received response
        private string responseData;
    }
}
