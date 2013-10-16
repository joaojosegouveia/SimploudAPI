﻿namespace Simploud.Api.OAuth
{
    public class OAuthToken
    {
        public OAuthToken(string token, string secret)
        {
            Token = token;
            Secret = secret;
        }

        public string Token { get; private set; }

        public string Secret { get; private set; }
    }
}
