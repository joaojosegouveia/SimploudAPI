using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Simploud.Api.OAuth
{
    public enum SignatureTypes
    {
        HMACSHA1,
        PLAINTEXT,
        RSASHA1
    }

    public interface IOAuthBase
    {
        string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType, out string normalizedUrl, out string normalizedRequestParameters, string callback, string verification);
        string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash);
        string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, out string normalizedUrl, out string normalizedRequestParameters, string callback, string verification);
        string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, out string normalizedUrl, out string normalizedRequestParameters, string callback, string verification);
        string UrlEncode(string value);
        string GenerateTimeStamp();
        string GenerateNonce();
    }
}
