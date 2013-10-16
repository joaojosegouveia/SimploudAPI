using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;

namespace Simploud.Api.OAuth
{
    public class OAuthSkydrive : IOAuth
    {
        private const string SkydriveAuthorizeBaseUri = "https://login.live.com/oauth20_authorize.srf";
        private const string SkydriveTokenBaseUri = "https://login.live.com/oauth20_token.srf";
        private string _consumerKey, _consumerSecret, _callbackUrl;

        public OAuthSkydrive(string consumerKey, string consumerSecret, string callbackUrl = null)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _callbackUrl = callbackUrl;
        }

        public OAuthToken GetRequestToken()
        {
            throw new NotImplementedException();
        }

        public Uri GetAuthorizeUri(OAuthToken requestToken)
        {
            var requestUri = String.Format("{0}?client_id={1}&scope=wl.signin%20wl.basic%20wl.skydrive_update&response_type=code&redirect_uri={2}",
                SkydriveAuthorizeBaseUri, _consumerKey, _callbackUrl);

            return new Uri(requestUri);
        }

        public OAuthToken GetAccessToken(OAuthToken requestToken, string verification = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(SkydriveTokenBaseUri);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";

            var req = string.Format("client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&grant_type=authorization_code", _consumerKey, _callbackUrl, _consumerSecret, requestToken.Secret);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(req);

            request.ContentLength = data.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }

            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var resp = reader.ReadToEnd();

            Simploud.Api.Models.AccessToken t = ParseJson<Simploud.Api.Models.AccessToken>(resp);

            return new OAuthToken("", t.access_token);
        }

        private static T ParseJson<T>(string json) where T : class, new()
        {
            var jobject = JObject.Parse(json);
            return JsonConvert.DeserializeObject<T>(jobject.ToString());
        }

        public Uri SignRequest(Uri uri, string consumerKey, string consumerSecret, OAuthToken token, string httpMethod, string callbackUrl = null, string verification = null)
        {
            throw new NotImplementedException();
        }

        public Uri SignRequest(Uri uri, string consumerKey, string consumerSecret, string callbackUrl = null, OAuthToken token = null, string verification = null)
        {
            throw new NotImplementedException();
        }
    }
}
