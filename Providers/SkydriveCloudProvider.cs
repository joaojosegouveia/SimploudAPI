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

namespace Simploud.Api.Providers
{
    public class SkydriveCloudProvider : ICloudProvider
    {
        private const string SkydriveApiVersion = "v5.0";
        private const string SkydriveApi = "https://apis.live.net/" + SkydriveApiVersion + "/";

        private readonly OAuthToken _accessToken;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _root;
        private readonly string _root_folder_id;

        public SkydriveCloudProvider(string consumerKey, string consumerSecret, OAuthToken accessToken, string root)
        {
            _accessToken = accessToken;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _root = root;

            Uri uri = new Uri(new Uri(SkydriveApi), String.Format("me/skydrive?access_token={0}", _accessToken.Secret));
            var json = GetRequest(uri);
            var j = ParseJson<Simploud.Api.Models.File>(json);
            _root_folder_id = j.Id;
        }

        #region auxiliar functions

        private string GetRequest(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = WebRequestMethods.Http.Get;
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private string PostRequest(Uri uri, string request_body)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(request_body);

            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private string PutRequest(Uri uri, string request_body)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Put;

            request.ContentType = "application/json";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(request_body);

            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private string MoveRequest(Uri uri, string destination_id)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "MOVE";

            request.ContentType = "application/json";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes("\"destination\": \"" + destination_id + "\"");

            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private string DeleteRequest(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "DELETE";

            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private Simploud.Api.Models.File getProperties(string id)
        {
            Uri uri = new Uri(new Uri(SkydriveApi), String.Format("{0}?access_token={1}", id, _accessToken.Secret));

            var json = GetRequest(uri);
            return ParseJson<Simploud.Api.Models.File>(json);
        }

        private static T ParseJson<T>(string json) where T : class, new()
        {
            var jobject = JObject.Parse(json);
            return JsonConvert.DeserializeObject<T>(jobject.ToString());
        }

        private Quota getQuotaInfo()
        {
            Uri uri = new Uri(new Uri(SkydriveApi), "me/skydrive/quota?access_token=" + _accessToken.Secret);
            var json = GetRequest(uri);
            return ParseJson<Quota>(json);
        }

        #endregion

        public Account GetAccountInfo()
        {
            Uri uri = new Uri(new Uri(SkydriveApi), "me?access_token=" + _accessToken.Secret);

            var json = GetRequest(uri);
            Account account = ParseJson<Account>(json);
            account.DisplayName = account.Name;

            account.Quota = getQuotaInfo();

            return account;
        }

        public Simploud.Api.Models.File GetFiles(string path)
        {
            if (String.IsNullOrEmpty(path))
                path = "me/" + _root;
            Uri uri = new Uri(new Uri(SkydriveApi), String.Format("{0}/files?access_token={1}", path, _accessToken.Secret));

            var json = GetRequest(uri);
            return ParseJson<Simploud.Api.Models.File>(json);
        }

        public Simploud.Api.Models.FileSystemInfo CreateFolder(string path, string name)
        {
            if (path == "")
                path = _root_folder_id;
            Uri uri = new Uri(new Uri(SkydriveApi), String.Format("{0}?access_token={1}", path, _accessToken.Secret));
            var json = PostRequest(uri, "{\r   \"name\": \"" + name + "\", \r   \"description\": \"description here\"\r}");
            
            return ParseJson<Simploud.Api.Models.FileSystemInfo>(json);
        }

        public Simploud.Api.Models.FileSystemInfo Rename(string old_name, string new_name)
        {
            if (old_name == "")
                old_name = _root_folder_id;
            Uri uri = new Uri(new Uri(SkydriveApi), String.Format("{0}?access_token={1}", old_name, _accessToken.Secret));
            var json = PutRequest(uri, "{\r   \"name\": \"" + new_name + "\"\r}");

            return ParseJson<Simploud.Api.Models.FileSystemInfo>(json);
        }

        public Simploud.Api.Models.FileSystemInfo Move(string fromPath, string toPath)
        {
            Uri uri = new Uri(new Uri(SkydriveApi), String.Format("{0}?access_token={1}", fromPath, _accessToken.Secret));
            string json = MoveRequest(uri, toPath);

            return ParseJson<Simploud.Api.Models.FileSystemInfo>(json);
        }

        public Simploud.Api.Models.FileSystemInfo Delete(string path)
        {
            Uri uri = new Uri(new Uri(SkydriveApi), String.Format("{0}?access_token={1}", path, _accessToken.Secret));
            var json = DeleteRequest(uri);

            return null;
        }

        public byte[] DownloadFile(string path)
        {
            Simploud.Api.Models.File file = getProperties(path);
            byte[] response = new System.Net.WebClient().DownloadData(file.Source);

            return response;
        }

        public Simploud.Api.Models.FileSystemInfo UploadFile(string path, string filename, MemoryStream data)
        {
            if (path == "")
                path = _root_folder_id;

            Uri uri = new Uri(new Uri(SkydriveApi), String.Format("{0}/files/{1}?access_token={2}", path, filename, _accessToken.Secret));

            var request = (HttpWebRequest)WebRequest.Create(uri);
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
