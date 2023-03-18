using LibCLCC.NET.Collections;
using System.Text;

namespace LMFS.Data {
    public enum DataType {
        Default, Zip, Hash
    }
    public class HttpQueries {
        Dictionary<string, string> data = new Dictionary<string, string>();
        public string? Get(string key) {
            var uk = key.ToLower();
            if (data.ContainsKey(uk)) {
                return data[uk];
            }
            return null;
        }
        public string? Get(string key, string fallback) {
            var uk = key.ToLower();
            if (data.ContainsKey(uk)) {
                return data[uk];
            }
            return fallback;
        }
        public void Set(string key, string value) {
            var uk = key.ToLower();
            if (data.ContainsKey(uk)) {
                data[uk] = value;
            }
            else data.Add(uk, value);
        }
        public override string ToString() {
            List<string> content = new List<string>();
            foreach (var item in data) {
                content.Add(Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value));
            }
            return string.Join('&', content);
        }
        public static HttpQueries FromString(string s) {
            HttpQueries queries = new HttpQueries();
            var uds = Uri.UnescapeDataString(s).Split('&');
            foreach (var kv in uds) {
                var kvp = kv.Split('=');
                queries.data.Add(kvp[0].ToLower(), kvp[1]);
            }
            return queries;
        }
    }
}