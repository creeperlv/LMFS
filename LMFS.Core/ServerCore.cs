using LMFS.Data;
using LMFS.Data.Utilities;
using LMFS.Exchange.Core;
using LMFS.Server.Core.Auth;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace LMFS.Core
{
    public class ServerCore
    {
        HttpListener HttpListener;
        ServerConfiguration configuration;
        ContentGenerator contGen = null;
        MIMEMap mime_map;
        UserBase UserBase = null;
        AuthBase AuthBase = null;
        public void LoadTemplate()
        {
            if (contGen != null)
                contGen.LoadTemplates(new DirectoryInfo(configuration.Template));
        }

        public ServerCore(ServerConfiguration configuration)
        {
            HttpListener = new HttpListener();
            this.configuration = configuration;
            contGen = new ContentGenerator();
            if (configuration.MimeMap != null)
            {
                mime_map = configuration.MimeMap;
            }
            else
            {
                mime_map = MIMEMap.GenerateMIMEMap();
            }
            if (configuration.Template != null)
            {
                contGen.LoadTemplates(new DirectoryInfo(configuration.Template));
            }
            foreach (var item in configuration.ListeningUrl)
            {
                HttpListener.Prefixes.Add(item);

            }
        }
        public void Start()
        {
            HttpListener.Start();
        }
        public void Listen()
        {
            while (true)
            {
                var contenxt = HttpListener.GetContext();
                Task.Run(() => Process(contenxt));
            }
        }
        void Process(HttpListenerContext context)
        {
            var path = context.Request.Url.LocalPath;
            //foreach (var item in context.Request.Headers.GetValues("User-Agent")) {
            //    //Console.WriteLine(item);
            //}
            Trace.WriteLine("Query:" + path);
            if (path.StartsWith("/browse"))
            {
                Browse(context);
            }
            else if (path.StartsWith("/dump"))
            {
                Dump(context);
            }
            else if (path.StartsWith("/auth"))
            {
                _Auth(context);
            }
            else if (path.StartsWith("/temp"))
            {
                Temp(context);
            }
            else if (path.StartsWith("/get"))
            {
                Get(context);
            }
            else if (path.StartsWith("/set"))
            {
                Set(context);
            }
            else if (path.StartsWith("/push"))
            {
                Push(context);
            }
            else if (path.StartsWith("/rm"))
            {
                Remove(context);
            }
            else if (path.StartsWith("/mkdir"))
            {
                Mkdir(context);
            }
        }
        private void _Auth(HttpListenerContext context)
        {
            var __q = context.Request.Url.Query;
            if (__q != null)
            {
                HttpQueries httpQueries = HttpQueries.FromString(__q);
                var un = httpQueries.Get("username");
                var hash = httpQueries.Get("hash");
                var salt = httpQueries.Get("rand");

            }
            else
            {
                Response(context.Response, "WRONG_QUERY", HttpStatusCode.BadRequest);
                return;
            }
        }
        private void Push(HttpListenerContext context)
        {
            var __q = context.Request.Url.Query;
            if (__q == null)
            {
                Response(context.Response, "WRONG_QUERY", HttpStatusCode.BadRequest);
                return;
            }
            if (Auth(context))
            {
                __q = __q.Substring(1);
                HttpQueries httpQueries = HttpQueries.FromString(__q);
                Console.WriteLine(httpQueries.ToString());
                var path = httpQueries.Get("path");
                var name = httpQueries.Get("name");
                var confirm = httpQueries.Get("confirm", "false");
                bool.TryParse(confirm, out bool WillConfirm);
                var content_type = httpQueries.Get("type", "default");
                Console.WriteLine($"Will: Rec to :{path}, name is={name}");
                if (GetPath(path, out var rel, out var com, out var mpt))
                {
                    DataType dataType = DataType.Default;
                    if (!Enum.TryParse<DataType>(content_type, out dataType))
                        dataType = DataType.Default;
                    if (com.EndsWith("/") || com.EndsWith("\\"))
                    {
                        //Directory.
                        if (name == null)
                        {
                            if (dataType == DataType.Default)
                            {
                                Response(context.Response, "MISSING_NAME", HttpStatusCode.BadRequest);
                                return;
                            }
                            else if (dataType == DataType.Zip)
                            {
                                var final = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                                using (var fs = File.Create(final))
                                {
                                    context.Request.InputStream.CopyTo(fs);
                                }
                                ZipFile.ExtractToDirectory(final, com);
                                File.Delete(final);
                                if (WillConfirm)
                                {
                                    Response(context.Response, "DONE", HttpStatusCode.OK);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            com = Path.Combine(com, name);
                            if (dataType == DataType.Default)
                            {

                                if (File.Exists(com)) File.Delete(com);
                                using (var fs = File.Create(com))
                                {
                                    context.Request.InputStream.CopyTo(fs);
                                }
                                if (WillConfirm)
                                {
                                    Response(context.Response, "DONE", HttpStatusCode.OK);
                                    return;
                                }

                            }
                            else if (dataType == DataType.Zip)
                            {
                                var final = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                                using (var fs = File.Create(final))
                                {
                                    context.Request.InputStream.CopyTo(fs);
                                }
                                if (!Directory.Exists(com)) Directory.CreateDirectory(com);
                                ZipFile.ExtractToDirectory(final, com);
                                File.Delete(final);
                                if (WillConfirm)
                                {
                                    Response(context.Response, "DONE", HttpStatusCode.OK);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (name == null)
                        {
                            Console.WriteLine("Receiving Data");
                            if (File.Exists(com)) File.Delete(com);
                            using (var fs = File.Create(com))
                            {
                                context.Request.InputStream.CopyTo(fs);
                            }
                            if (WillConfirm)
                            {
                                Response(context.Response, "DONE", HttpStatusCode.OK);
                                return;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Receiving Named Data");
                            var final = Path.Combine(com, name);
                            Console.WriteLine("FINAL:" + final);
                            if (File.Exists(final))
                                File.Delete(final);
                            using (var fs = File.Create(final))
                            {
                                context.Request.InputStream.CopyTo(fs);
                                Console.WriteLine("Done...");
                            }
                            if (WillConfirm)
                                Console.WriteLine("Will Confirm");
                            if (WillConfirm)
                            {
                                Response(context.Response, "DONE", HttpStatusCode.OK);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    Response(context.Response, "MISMATCH_MAPPING", HttpStatusCode.NotFound);
                    return;
                }
            }
            else
            {
                Response(context.Response, "AUTH_FAIL", HttpStatusCode.Unauthorized);
                return;
            }
            context.Response.Close();
        }
        private void Mkdir(HttpListenerContext context)
        {
            var __q = context.Request.Url.Query;
            if (__q == null)
            {
                Response(context.Response, "WRONG_QUERY", HttpStatusCode.BadRequest);
                return;
            }
            if (Auth(context))
            {
                __q = __q.Substring(1);
                HttpQueries httpQueries = HttpQueries.FromString(__q);
                var path = httpQueries.Get("path");
                var confirm = httpQueries.Get("confirm", "false");
                bool.TryParse(confirm, out bool WillConfirm);
                if (GetPath(path, out var rel, out var com, out var mpt))
                {
                    if (Directory.Exists(com))
                    {
                        Response(context.Response, "ALREADY_EXISTS", HttpStatusCode.OK);
                    }
                    else
                    {
                        try
                        {
                            Directory.CreateDirectory(com);
                            if (WillConfirm)
                                Response(context.Response, $"DONE", HttpStatusCode.OK);
                        }
                        catch (Exception e)
                        {
                            Response(context.Response, $"CREATION_FAILED\n{e.Message}", HttpStatusCode.Conflict);

                        }
                    }
                }
                else
                {
                    Response(context.Response, "MISMATCH_MAPPING", HttpStatusCode.NotFound);
                    return;
                }
            }
            else
            {
                Response(context.Response, "AUTH_FAIL", HttpStatusCode.Unauthorized);
                return;
            }
            context.Response.Close();
        }
        private void Response(HttpListenerResponse response, string msg, HttpStatusCode code)
        {
            response.StatusCode = (int)code;
            using (StreamWriter SW = new StreamWriter(response.OutputStream))
            {
                SW.Write(msg);
            }
            response.Close();
        }
        public bool Auth(HttpListenerContext context)
        {
            if (configuration.UserAuth == false) return true;
            var auth = context.Request.Cookies["auth"];

            return true;
        }
        private void Remove(HttpListenerContext context)
        {
            var __q = context.Request.Url.Query;
            if (__q == null)
            {
                Response(context.Response, "WRONG_QUERY", HttpStatusCode.BadRequest);
                return;
            }
            if (Auth(context))
            {
                __q = __q.Substring(1);
                HttpQueries httpQueries = HttpQueries.FromString(__q);
                var path = httpQueries.Get("path");
                var confirm = httpQueries.Get("confirm", "false");
                if (path == null)
                {
                    Response(context.Response, "WRONG_QUERY", HttpStatusCode.BadRequest);
                    return;
                }
                bool.TryParse(confirm, out bool WillConfirm);
                if (GetPath(path, out var rel, out var com, out var mpt))
                {
                    if (Directory.Exists(com))
                    {
                        Directory.Delete(com, true);
                    }
                    else if (File.Exists(com))
                    {
                        File.Delete(com);
                    }
                    else
                    {
                        Response(context.Response, "TARGET_NOT_FOUND", HttpStatusCode.NotFound);
                        return;
                    }
                }
                else
                {
                    Response(context.Response, "MISMATCH_MAPPING", HttpStatusCode.NotFound);
                    return;
                }
                if (WillConfirm)
                {
                    Response(context.Response, "DONE", HttpStatusCode.OK);
                }
            }
            else
            {
                Response(context.Response, "AUTH_FAIL", HttpStatusCode.Unauthorized);
            }
        }

        private bool GetPath(string query_path, out string relative, out string combined, out string mapped_target)
        {
            foreach (var item in configuration.PathMap)
            {
                if (query_path.ToUpper().StartsWith(item.Key.ToUpper()))
                {
                    relative = query_path.Substring(item.Key.Length);
                    relative = Uri.UnescapeDataString(relative);
                    combined = Path.Combine(item.Value, relative);
                    mapped_target = item.Value;
                    return true;
                }
            }
            relative = null;
            combined = null;
            mapped_target = null;
            return false;
        }
        private void Set(HttpListenerContext context)
        {
            var query = context.Request.Url.Query;
            if (query == null || query == "")
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Close();
            }
            else
            {
                query = query.Substring(1);
                HttpQueries httpQueries = HttpQueries.FromString(query);
                var queries = query.Split('&');
                var _path = httpQueries.Get("path");
                DateTime? date = null;
                {
                    var writeTime = httpQueries.Get("writetime");
                    if (writeTime != null)
                    {
                        try
                        {
                            date = new DateTime(long.Parse(writeTime));
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                if (GetPath(_path, out var relative, out var combined, out var root))
                {
                    if (Directory.Exists(combined))
                    {
                        if (date != null)
                        {
                            Directory.SetLastWriteTime(combined, date.Value);
                        }
                    }
                    else if (File.Exists(combined))
                    {
                        File.SetLastWriteTime(combined, date.Value);
                    }
                    else
                    {
                        Response(context.Response, "RESOURCE_NOT_FOUND", HttpStatusCode.NotFound);
                        return;
                    }
                }
                else
                {
                    Response(context.Response, "MISMATCH_MAPPING", HttpStatusCode.NotFound);
                    return;
                }
            }
        }
        private void Get(HttpListenerContext context)
        {
            var query = context.Request.Url.Query;
            if (query == null || query == "")
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Close();
            }
            else
            {
                query = query.Substring(1);
                HttpQueries httpQueries = HttpQueries.FromString(query);
                var queries = query.Split('&');
                var _path = httpQueries.Get("path");
                DataType responseType = DataType.Default;
                {
                    var rt = httpQueries.Get("type");
                    if (rt != null)
                    {

                        responseType = Enum.Parse<DataType>(rt);
                    }
                }
                if (GetPath(_path, out var relative, out var combined, out var root))
                {
                    if (Directory.Exists(combined))
                    {

                        switch (responseType)
                        {
                            case DataType.Default:
                                {
                                    //Send back list.
                                    DirectoryInfo directoryInfo = new DirectoryInfo(combined);
                                    StringBuilder sb = new StringBuilder();
                                    foreach (var item in directoryInfo.EnumerateDirectories())
                                    {
                                        sb.Append("DIR:");
                                        sb.Append(item.Name);
                                        sb.Append('\n');
                                    }
                                    foreach (var item in directoryInfo.EnumerateFiles())
                                    {
                                        sb.Append("FLE:");
                                        sb.Append(item.Name);
                                        sb.Append('\n');
                                    }
                                    Response(context.Response, sb.ToString(), HttpStatusCode.OK);
                                    //context.Response.Close();
                                }
                                break;
                            case DataType.Zip:
                                break;
                            case DataType.WriteTime:
                                {
                                    Response(context.Response, Directory.GetLastWriteTimeUtc(combined).ToBinary().ToString(), HttpStatusCode.NotFound);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (File.Exists(combined))
                    {
                        if (responseType == DataType.Default)
                        {
                            using (var fs = File.OpenRead(combined))
                            {
                                fs.CopyTo(context.Response.OutputStream);
                            }
                            context.Response.Close();
                        }
                        else if (responseType == DataType.Hash)
                        {
                            Response(context.Response, File.ReadAllBytes(combined).HashString(), HttpStatusCode.NotFound);

                        }
                        else if (responseType == DataType.WriteTime)
                        {
                            Response(context.Response, File.GetLastWriteTimeUtc(combined).ToBinary().ToString(), HttpStatusCode.NotFound);

                        }
                    }
                    else
                    {
                        Response(context.Response, "RESOURCE_NOT_FOUND", HttpStatusCode.NotFound);
                        return;
                    }
                }
                else
                {
                    Response(context.Response, "MISMATCH_MAPPING", HttpStatusCode.NotFound);
                    return;
                }
            }
        }

        private void Temp(HttpListenerContext context)
        {

            var query = context.Request.Url.Query;
            if (query == null || query == "")
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Close();
            }
            else
            {
                query = query.Substring(1);
                var queries = query.Split('&');
                var p = "";
                foreach (var item in queries)
                {
                    var kv = item.Split('=');
                    switch (kv[0])
                    {
                        case "file":
                            {
                                p = kv[1];
                            }
                            break;
                        default:
                            break;
                    }
                }
                p = Uri.UnescapeDataString(p);
                Trace.WriteLine("Getting Temp:" + p);
                if (CompressionManager.Query(p, out var ready))
                {
                    if (ready)
                    {
                        context.Response.AddHeader("Content-Disposition", "attachment; filename=" + Uri.EscapeDataString(p));
                        var abs = Path.Combine(Path.GetTempPath(), p);
                        FileInfo fi = new FileInfo(abs);
                        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Zip;
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        {
                            using (var s = fi.OpenRead())
                            {
                                byte[] buf = new byte[1024 * 128];
                                Span<byte> buffer = new Span<byte>(buf);
                                while (s.Read(buffer) != 0)
                                {
                                    context.Response.OutputStream.Write(buffer);
                                }
                            }
                            context.Response.Close();
                        }
                    }
                    else
                    {
                        var t = contGen.Generate(ContentType.GeneircPage, "Processing", contGen.Generate(ContentType.ProcessingPage, ""), "");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = "text/html";
                        using (StreamWriter SW = new(context.Response.OutputStream))
                        {
                            SW.Write(t);
                        }
                        context.Response.Close();
                    }
                }
                else
                {
                    ReturnNotFound(context, p);
                }
            }
        }

        private void ReturnNotFound(HttpListenerContext context, string p)
        {
            Trace.WriteLine("resource not exist:" + p);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            var t = contGen.Generate(ContentType.GeneircPage, "Not Found", contGen.Generate(ContentType.NotFoundPage));
            context.Response.ContentType = "text/html";
            using (StreamWriter SW = new(context.Response.OutputStream))
            {
                SW.Write(t);
            }
            context.Response.Close();
        }

        private void Dump(HttpListenerContext context)
        {
            var query = context.Request.Url.Query;
            if (query == null || query == "")
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Close();
            }
            else
            {
                query = query.Substring(1);
                var hquery = HttpQueries.FromString(query);
                var p = hquery.Get("path");
                if (GetPath(p, out _, out var abs, out var root))
                {
                    if (Directory.Exists(abs))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Redirect;
                        var file = CompressionManager.QueryComopress(abs);
                        context.Response.Redirect("temp?file=" + Uri.EscapeDataString(file));
                        context.Response.Close();
                        return;

                    }
                    else
                    {

                        ReturnNotFound(context, p);
                    }
                }
                ReturnNotFound(context, p);
            }
        }
        private void Browse(HttpListenerContext context)
        {
            var query = context.Request.Url.Query;
            if (query == null || query == "")
            {
                var k = configuration.PathMap.Keys;
                string folderContent = "";
                foreach (var folder in k)
                {
                    folderContent += contGen.Generate(ContentType.Button_Folder, $"./browse?path={folder}", folder);
                }

                var folderP = contGen.Generate(ContentType.Folder_Page, "/", folderContent, "");
                var contentP = contGen.Generate(ContentType.Content_Page, $"Browse:/", folderP);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "text/html";
                using (StreamWriter SW = new(context.Response.OutputStream))
                {
                    SW.Write(contentP);
                }
                context.Response.Close();
            }
            else
            {
                query = query.Substring(1);

                var hquery = HttpQueries.FromString(query);
                var p = hquery.Get("path");

                Trace.WriteLine("Browse:" + p);
                if (GetPath(p, out var relative, out var abs, out string mapped_target))
                {
                    if (relative.Contains("/../") || relative.StartsWith("../") || relative.EndsWith("/.."))
                    {
                        ReturnNotFound(context, p);
                        return;
                    }
                    if (Directory.Exists(abs))
                    {

                        if (!p.EndsWith("/") && !p.EndsWith("\\"))
                        {
                            context.Response.Redirect($"./browse?path={p}/");
                            context.Response.Close();
                            return;
                        }
                        DirectoryInfo directoryInfo = new DirectoryInfo(abs);
                        {
                            var fsi1 = new DirectoryInfo(mapped_target);
                            if (!directoryInfo.FullName.StartsWith(fsi1.FullName))
                            {
                                ReturnNotFound(context, p);
                                return;
                            }
                        }
                        string folderContent = "";
                        if (relative.Length > 0)
                        {
                            var _p = p.Substring(0, p.LastIndexOf('/'));
                            //Console.WriteLine(_p);
                            _p = _p.Substring(0, _p.LastIndexOf("/") + 1);
                            //Console.WriteLine(_p);

                            folderContent +=
                                    contGen.Generate(ContentType.Button_Folder, $"./browse?path={Uri.EscapeDataString(_p)}", "..");

                        }
                        foreach (var folder in directoryInfo.EnumerateDirectories())
                        {
                            HttpQueries httpQueries = new HttpQueries();
                            httpQueries.Set("path", (Path.Combine(p, folder.Name)) + "/");
                            folderContent += contGen.Generate(ContentType.Button_Folder, $"./browse?{httpQueries}", folder.Name);
                        }
                        string filedContent = "";
                        foreach (var file in directoryInfo.EnumerateFiles())
                        {
                            HttpQueries httpQueries = new HttpQueries();
                            httpQueries.Set("path", (Path.Combine(p, file.Name)));
                            filedContent += contGen.Generate(ContentType.Button_File, $"./browse?{httpQueries}", file.Name);
                        }
                        var folderP = contGen.Generate(ContentType.Folder_Page, p, folderContent, filedContent);
                        var contentP = contGen.Generate(ContentType.Content_Page, $"Browse:{p}", folderP);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = "text/html";
                        using (StreamWriter SW = new(context.Response.OutputStream))
                        {
                            SW.Write(contentP);
                        }
                        context.Response.Close();
                    }
                    else if (File.Exists(abs))
                    {
                        //System.Net.Mime.MediaTypeNames.Image.
                        FileInfo fi = new FileInfo(abs);
                        var type = mime_map.Match(fi.Extension);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = type;
                        if (type.StartsWith("text/"))
                        {
                            using (StreamWriter SW = new(context.Response.OutputStream))
                            {
                                SW.Write(File.ReadAllText(fi.FullName));
                            }
                        }
                        else
                        {
                            using (var s = fi.OpenRead())
                            {
                                byte[] buf = new byte[1024 * 128];
                                Span<byte> buffer = new Span<byte>(buf);
                                while (s.Read(buffer) != 0)
                                {
                                    context.Response.OutputStream.Write(buffer);
                                }
                            }
                            context.Response.Close();
                        }
                    }
                }
                else
                {
                    ReturnNotFound(context, p);
                }
            }
        }
    }
}