using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simploud.Api.OAuth
{
    public interface IOAuth
    {
        OAuthToken GetRequestToken();
        Uri GetAuthorizeUri(OAuthToken requestToken);
        OAuthToken GetAccessToken(OAuthToken requestToken, string verification = null);
        Uri SignRequest(Uri uri, string consumerKey, string consumerSecret, OAuthToken token, string httpMethod, string callbackUrl = null, string verification = null);
        Uri SignRequest(Uri uri, string consumerKey, string consumerSecret, string callbackUrl = null, OAuthToken token = null, string verification = null);
    }
}
