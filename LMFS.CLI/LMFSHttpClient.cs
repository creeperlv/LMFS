using LMFS.Data;
using System;
using System.Net;

namespace LMFS.CLI {
    public class LMFSHttpClient {
        HttpClient httpClient;
        Uri uri;
        public LMFSHttpClient(Uri uri) {
            httpClient = new HttpClient();
            this.uri = uri;
            httpClient.DefaultRequestHeaders.UserAgent.Clear();

            httpClient.DefaultRequestHeaders.UserAgent.Add(System.Net.Http.Headers.ProductInfoHeaderValue.Parse("LMFS-Client/1"));
        }
        public List<string> GetList(string path,out bool Success) {
            var _uri = new Uri(uri, "get?path=" + Uri.EscapeDataString(path) + $"&type={DataType.Default}");

            Task<HttpResponseMessage> task = httpClient.GetAsync(_uri);
            task.Wait();
            HttpResponseMessage _a = task.Result;
            Task<string> _t = _a.Content.ReadAsStringAsync();
            _t.Wait();
            string s = _t.Result;
            var l = s.Split('\r', '\n').ToList();

            l.RemoveAll((a) => a == "");
            Success = (_a.StatusCode == HttpStatusCode.OK);
            return l;
        }
        public string Get(string path, string output) {
            var _uri = new Uri(uri, "get?path=" + Uri.EscapeDataString(path) + $"&type={DataType.Default}&confirm={true}");
            Task<HttpResponseMessage> task = httpClient.GetAsync(_uri);
            task.Wait();
            HttpResponseMessage _a = task.Result;
            var target = new FileInfo(output);
            if (!target.Directory.Exists) {
                target.Directory.Create();
            }
            if (target.Exists) {
                target.Delete();
            }
            if (_a.StatusCode == System.Net.HttpStatusCode.OK)
                using (var f = target.Open(FileMode.OpenOrCreate)) {
                    var s = _a.Content.ReadAsStream();
                    s.CopyTo(f);
                    s.Dispose();
                    _a.Dispose();
                    return "OK";
                }
            else {

                Task<string> _t = _a.Content.ReadAsStringAsync();
                _t.Wait();
                string str = _t.Result;
                _a.Dispose();
                return str;
            }
        }
        public string Remove(string path) {
            var _uri = new Uri(uri, "rm?path=" + Uri.EscapeDataString(path) + $"&confirm={true}");
            var tp = httpClient.DeleteAsync(_uri);
            tp.Wait();
            Task<string> _t = tp.Result.Content.ReadAsStringAsync();
            _t.Wait();
            string s = _t.Result;
            return s;
        }
        public string Push(string path, string source) {
            var f = new FileInfo(source);
            HttpQueries httpQueries = new HttpQueries();
            httpQueries.Set("path", path);
            httpQueries.Set("name", f.Name);
            httpQueries.Set("confirm", $"{true}");
            var _uri = new Uri(uri, "push?"+httpQueries.ToString());
            using (var s = f.OpenRead()) {
                var sc = new StreamContent(s);
                var tp = httpClient.PutAsync(_uri, sc);
                tp.Wait();
                Task<string> _t = tp.Result.Content.ReadAsStringAsync();
                _t.Wait();
                string str = _t.Result;
                return str;
            }

        }

        internal bool Mkdir(string path) {
            var _uri = new Uri(uri, "mkdir?path=" + Uri.EscapeDataString(path) + $"&confirm={true}");
            var tp = httpClient.DeleteAsync(_uri);
            tp.Wait();
            Task<string> _t = tp.Result.Content.ReadAsStringAsync();
            _t.Wait();
            string s = _t.Result;
            return s.StartsWith("DONE");
        }
    }
}