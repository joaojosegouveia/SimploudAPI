using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simploud.Api.Models;
using Simploud.Api.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Simploud.Api.Providers
{
    public class CloudptCloudProvider : ICloudProvider
    {
        private const string CloudptApiVersion = "1";
        private const string CloudptBaseUri = "https://cloudpt.pt/";
        private const string CloudptAuthorizeBaseUri = "https://cloudpt.pt/";
        private const string CloudptApi = "https://publicapi.cloudpt.pt/" + CloudptApiVersion + "/";
        private const string CloudptApiContent = "https://api-content.cloudpt.pt/" + CloudptApiVersion + "/";

        private readonly OAuthToken _accessToken;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _root;

        public CloudptCloudProvider(string consumerKey, string consumerSecret, OAuthToken accessToken, string root)
        {
            _accessToken = accessToken;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _root = root;
        }

        #region auxiliar functions

        private string GetResponse(Uri uri)
        {
            var oauth = new OAuth.OAuthCloudpt(_consumerKey,_consumerSecret,null);

            var requestUri = oauth.SignRequest(uri, _consumerKey, _consumerSecret, _accessToken, "GET", null, null);
            HttpWebRequest request = null;

            request = (HttpWebRequest)WebRequest.Create(requestUri);

            request.Method = WebRequestMethods.Http.Get;
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private string PostRequest(Uri uri)
        {
            var oauth = new OAuth.OAuthCloudpt(_consumerKey, _consumerSecret, null);
            var requestUri = oauth.SignRequest(uri, _consumerKey, _consumerSecret, _accessToken, "POST", null, null);

            var req = requestUri.AbsoluteUri.Split('?');

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(req[1]);

            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private static T ParseJson<T>(string json) where T : class, new()
        {
            var jobject = JObject.Parse(json);
            return JsonConvert.DeserializeObject<T>(jobject.ToString());
        }

        private string UidFix(string response)
        {
            var regexPattern = "(\"uid\"): (?<uid>\\d+)";
            var replacePattern = "\"uid\": \"${uid}\"";

            response = Regex.Replace(response, regexPattern, replacePattern, RegexOptions.IgnoreCase);

            return response;
        }

        private static string UpperCaseUrlEncode(string s)
        {
            char[] temp = HttpUtility.UrlEncode(s).ToCharArray();
            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }

            var values = new Dictionary<string, string>()
            {
                { "+", "%20" },
                { "(", "%28" },
                { ")", "%29" }
            };

            var data = new StringBuilder(new string(temp));
            foreach (string character in values.Keys)
            {
                data.Replace(character, values[character]);
            }
            return data.ToString();
        }

        private string newNamePath(string old_name, string new_name)
        {
            string[] div = old_name.Split('/');
            int i;
            string new_path = "";

            for (i = 0; i < div.Length - 1; i++)
            {
                new_path += div[i] + "/";
            }

            return new_path + new_name;
        }

        #endregion

        public Account GetAccountInfo()
        {
            Uri uri = new Uri(new Uri(CloudptApi), "Account/Info");

            var json = GetResponse(uri);
            json = UidFix(json);
            return ParseJson<Account>(json);
        }

        public Simploud.Api.Models.File GetFiles(string path)
        {
            if (path != "" && path[0] == '/')
                path = path.Remove(0, 1);

            Uri uri = new Uri(new Uri(CloudptApi), String.Format("Metadata/{0}/{1}", _root, path));

            var json = GetResponse(uri);
            return ParseJson<Simploud.Api.Models.File>(json);
        }

        public Simploud.Api.Models.FileSystemInfo CreateFolder(string path, string name = null)
        {
            if (path == "" || path[0] != '/')
                path = "/" + path;

            if (path != "" && path[path.Length - 1] != '/')
                path = path + "/";

            Uri uri = new Uri(new Uri(CloudptApi),
                        String.Format("Fileops/CreateFolder?root={0}&path={1}", _root, UpperCaseUrlEncode(path + name)));
            var json = PostRequest(uri);

            return ParseJson<Simploud.Api.Models.FileSystemInfo>(json);
        }

        public Simploud.Api.Models.FileSystemInfo Rename(string old_name, string new_name)
        {
            if (old_name != "" && old_name[0] != '/')
                old_name = "/" + old_name;

            new_name = newNamePath(old_name, new_name);

            Uri uri = new Uri(new Uri(CloudptApi),
                        String.Format("Fileops/Move?root={0}&from_path={1}&to_path={2}",
                        _root, UpperCaseUrlEncode(old_name), UpperCaseUrlEncode(new_name)));
            var json = PostRequest(uri);

            return ParseJson<Simploud.Api.Models.FileSystemInfo>(json);
        }

        public Simploud.Api.Models.FileSystemInfo Move(string fromPath, string toPath)
        {
            Uri uri = new Uri(new Uri(CloudptApi),
                        String.Format("Fileops/Move?root={0}&from_path={1}&to_path={2}",
                        _root, UpperCaseUrlEncode(fromPath), UpperCaseUrlEncode(toPath)));
            var json = PostRequest(uri);

            return ParseJson<Simploud.Api.Models.FileSystemInfo>(json);
        }

        public Simploud.Api.Models.FileSystemInfo Delete(string path)
        {
            Uri uri = new Uri(new Uri(CloudptApi),
                        String.Format("Fileops/Delete?root={0}&path={1}",
                        _root, UpperCaseUrlEncode(path)));
            var json = PostRequest(uri);

            return ParseJson<Simploud.Api.Models.FileSystemInfo>(json);
        }

        public byte[] DownloadFile(string path)
        {
            if (path != "" && path[0] == '/')
                path = path.Remove(0, 1);

            Uri uri = new Uri(new Uri(CloudptApiContent),
                    String.Format("Files/{0}/{1}",
                    _root, UpperCaseUrlEncode(path)));

            var oauth = new OAuth.OAuthCloudpt(_consumerKey, _consumerSecret, null);
            var requestUri = oauth.SignRequest(uri, _consumerKey, _consumerSecret, null, _accessToken);

            var request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Http.Get;
            var response = request.GetResponse();

            var metadata = response.Headers["x-" + _root + "-metadata"];
            var file = ParseJson<Simploud.Api.Models.FileSystemInfo>(metadata);

            using (Stream responseStream = response.GetResponseStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                do
                {
                    bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                    memoryStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                return memoryStream.ToArray();
            }
        }

        public Simploud.Api.Models.FileSystemInfo UploadFile(string path, string filename, MemoryStream data)
        {
            if (path != "" && path[0] == '/')
                path = path.Remove(0, 1);

                Uri uri = new Uri(new Uri(CloudptApiContent),
                    String.Format("Files/{0}/{1}/{2}",
                    _root, UpperCaseUrlEncode(path),filename));

                var oauth = new OAuth.OAuthCloudpt(_consumerKey, _consumerSecret, null);
                var requestUri = oauth.SignRequest(uri, _consumerKey, _consumerSecret, _accessToken, "PUT");

                var request = (HttpWebRequest)WebRequest.Create(requestUri);
                request.Method = WebRequestMethods.Http.Put;
                request.KeepAlive = true;

                byte[] buffer = data.ToArray();

                request.ContentLength = buffer.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var json = reader.ReadToEnd();
                return ParseJson<Simploud.Api.Models.FileSystemInfo>(json);
        }
    }
}
