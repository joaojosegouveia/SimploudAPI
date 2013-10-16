using System;
using System.IO;
using System.Net;

namespace Simploud.Api.OAuth
{
    public class OAuthDropbox : IOAuth
    {
        private readonly IOAuthBase _oAuthBase;
        private const string DropboxApiVersion = "1";
        private const string DropboxBaseUri = "https://api.dropbox.com/" + DropboxApiVersion + "/";
        private const string DropboxAuthorizeBaseUri = "https://www.dropbox.com/" + DropboxApiVersion + "/";
        private string _consumerKey, _consumerSecret, _callbackUrl;

        public OAuthDropbox(string consumerKey, string consumerSecret, string callbackUrl = null)
        {
            _oAuthBase = new OAuthBaseDropbox();

            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _callbackUrl = callbackUrl;
        }

        public OAuthToken GetRequestToken()
        {
            var uri = new Uri(new Uri(DropboxBaseUri), "oauth/request_token");

            if(_callbackUrl != null)
                _callbackUrl = _oAuthBase.UrlEncode(_callbackUrl);

            uri = SignRequest(uri, _consumerKey, _consumerSecret, _callbackUrl);

            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;

            var response = request.GetResponse();

            var queryString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var parts = queryString.Split('&');
            var token = parts[1].Substring(parts[1].IndexOf('=') + 1);
            var secret = parts[0].Substring(parts[0].IndexOf('=') + 1);

            return new OAuthToken(token, secret);
        }

        public Uri GetAuthorizeUri(OAuthToken requestToken)
        {
            string query_string = null;
            query_string = String.Format("oauth_token={0}", requestToken.Token);

            if (_callbackUrl != null)
                query_string = String.Format("{0}&&oauth_callback={1}", query_string, _callbackUrl);
        
            var authorizeUri = String.Format("{0}{1}?{2}", new Uri(DropboxAuthorizeBaseUri), "oauth/authorize", query_string);
            return new Uri(authorizeUri);
        }

        public OAuthToken GetAccessToken(OAuthToken requestToken, string verification = null)
        {
            var uri = new Uri(new Uri(DropboxBaseUri), "oauth/access_token");

            uri = SignRequest(uri, _consumerKey, _consumerSecret, null, requestToken, verification);

            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;

            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var accessToken = reader.ReadToEnd();

            var parts = accessToken.Split('&');
            var token = parts[1].Substring(parts[1].IndexOf('=') + 1);
            var secret = parts[0].Substring(parts[0].IndexOf('=') + 1);

            return new OAuthToken(token, secret);
        }

        public Uri SignRequest(Uri uri, string consumerKey, string consumerSecret, OAuthToken token, string httpMethod, string callbackUrl = null, string verification = null)
        {
            var nonce = _oAuthBase.GenerateNonce();
            var timestamp = _oAuthBase.GenerateTimeStamp();
            string parameters;
            string normalizedUrl;

            string callback = callbackUrl == null ? String.Empty : callbackUrl;
            string requestToken = token == null ? String.Empty : token.Token;
            string tokenSecret = token == null ? String.Empty : token.Secret;
            string verify = verification == null ? String.Empty : verification;

            var signature = _oAuthBase.GenerateSignature(
                uri, consumerKey, consumerSecret,
                requestToken, tokenSecret, httpMethod, timestamp,
                nonce, SignatureTypes.HMACSHA1,
                out normalizedUrl, out parameters, callback, verification);

            var requestUri = String.Format("{0}?{1}&oauth_signature={2}", 
                normalizedUrl, parameters, _oAuthBase.UrlEncode(signature));
           
            return new Uri(requestUri);
        }

        public Uri SignRequest(Uri uri, string consumerKey, string consumerSecret, string callbackUrl = null, OAuthToken token = null, string verification = null)
        {
            return SignRequest(uri, consumerKey, consumerSecret, token, "GET", callbackUrl, verification);
        }
    }
}