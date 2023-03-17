namespace LMFS.Core {
    public class MIMEMap{
        public Dictionary<string,string> mimes = new Dictionary<string,string>();
        public string Match(string extension) {
            if(mimes.TryGetValue(extension.ToLower(), out var result))return result;
            return "application/octet-stream";
        }
        public static MIMEMap GenerateMIMEMap() {
            MIMEMap map= new MIMEMap();
            map.mimes.Add(".htm", System.Net.Mime.MediaTypeNames.Text.Html);
            map.mimes.Add(".html", System.Net.Mime.MediaTypeNames.Text.Html);
            map.mimes.Add(".txt", System.Net.Mime.MediaTypeNames.Text.Plain);
            map.mimes.Add(".cs", System.Net.Mime.MediaTypeNames.Text.Plain);
            map.mimes.Add(".c", System.Net.Mime.MediaTypeNames.Text.Plain);
            map.mimes.Add(".cpp", System.Net.Mime.MediaTypeNames.Text.Plain);
            map.mimes.Add(".h", System.Net.Mime.MediaTypeNames.Text.Plain);
            map.mimes.Add(".cxx", System.Net.Mime.MediaTypeNames.Text.Plain);
            map.mimes.Add(".jpg", System.Net.Mime.MediaTypeNames.Image.Jpeg);
            map.mimes.Add(".jpeg", System.Net.Mime.MediaTypeNames.Image.Jpeg);
            map.mimes.Add(".zip", System.Net.Mime.MediaTypeNames.Application.Zip);
            map.mimes.Add(".png", "image/png");
            map.mimes.Add(".js", "text/javascript");
            return map;
        }
    }
}