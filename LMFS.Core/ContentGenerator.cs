using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LMFS.Core {
    public enum ContentType {
        Button_Folder, Button_File, Button_Action, Folder_Page, Content_Page, GeneircPage, NotFoundPage, ProcessingPage
    }
    public class ContentGenerator {
        string GenericPage = "<html>\r\n\t<head>\r\n\t\t<title>{0}</title>\r\n\t</head>\r\n\t<body style=\\\"font-family: Times New Roman\\\">\r\n\t\t{1}<hr/>\r\n\t\t<p>LMFS v:{LMFS:Version}<br/>Default Template </p>\r\n\t</body>\r\n<html>";
        string NotFoundPage = "<div><h1>404 Not Found</h1><p>Resource you request is not here.</p></div>";
        string ProcessingPage = "<div><h1>Processing</h1><p>Resource you request is still being processed.</p></div>";
        string ContentPageTemplate = "<html><head><title>{0}</title></head><body style=\"font-family: Times New Roman\">{1}<hr/><p>LMFS v:{LMFS:Version}<br/>Default Template </p></body><html>";
        string FolderPage = "<div><h2>{0}</h2><hr/><h1>Folders</h1>{1}<h1>Files</h1>{2}</div>";
        string button_folder = "<p><a href=\"{0}\">Folder:{1}</a></p>";
        string button_file = "<p><a href=\"{0}\">Files{1}</a></p>";
        string button_action = "<a href=\"{0}\">{1}</a>";
        public ContentGenerator() {

        }
        public void Export(string Directory) {
            {
                File.WriteAllText(Path.Combine(Directory, "content-page.tmpl"), ContentPageTemplate);
                File.WriteAllText(Path.Combine(Directory, "folder-page.tmpl"), FolderPage);
                File.WriteAllText(Path.Combine(Directory, "button-folder.tmpl"), button_folder);
                File.WriteAllText(Path.Combine(Directory, "button-file.tmpl"), button_file);
                File.WriteAllText(Path.Combine(Directory, "button-action.tmpl"), button_action);
                File.WriteAllText(Path.Combine(Directory, "generic-page.tmpl"), GenericPage);
                File.WriteAllText(Path.Combine(Directory, "not-found-page.tmpl"), NotFoundPage);
                File.WriteAllText(Path.Combine(Directory, "processing-page.tmpl"), ProcessingPage);
            }
        }
        readonly Version def_version = new Version(1, 0, 0, 0);
        public string ProcessEnvironment(string Content) {

            return Content
                .Replace("{LMFS:Version}", (typeof(ContentGenerator).Assembly.GetName().Version ?? def_version).ToString())
                .Replace("{OS:Name}", RuntimeInformation.RuntimeIdentifier);
        }
        public string Generate(ContentType contentType, params string[] args) {
            switch (contentType) {
                case ContentType.Button_Folder:
                    return string.Format(ProcessEnvironment(button_folder), args);
                case ContentType.Button_File:
                    return string.Format(ProcessEnvironment(button_file), args);
                case ContentType.Button_Action:
                    return string.Format(ProcessEnvironment(button_action), args);
                case ContentType.Folder_Page:
                    return string.Format(ProcessEnvironment(FolderPage), args);
                case ContentType.Content_Page:
                    return string.Format(ProcessEnvironment(ContentPageTemplate), args);
                case ContentType.GeneircPage:
                    return string.Format(ProcessEnvironment(GenericPage), args);
                case ContentType.NotFoundPage:
                    return string.Format(ProcessEnvironment(NotFoundPage), args);
                case ContentType.ProcessingPage:
                    return string.Format(ProcessEnvironment(ProcessingPage), args);
                default:
                    break;
            }
            return "";
        }
        public void LoadTemplates(DirectoryInfo directoryInfo) {
            var files = directoryInfo.EnumerateFiles();
            foreach (var item in files) {
                var name = item.Name.ToUpper();
                switch (name) {
                    case "CONTENT-PAGE.TMPL":
                        ContentPageTemplate = File.ReadAllText(item.FullName);
                        break;
                    case "FOLDER-PAGE.TMPL":
                        FolderPage = File.ReadAllText(item.FullName);
                        break;
                    case "BUTTON-FOLDER.TMPL":
                        button_folder = File.ReadAllText(item.FullName);
                        break;
                    case "BUTTON-FILE.TMPL":
                        button_file = File.ReadAllText(item.FullName);
                        break;
                    case "BUTTON-ACTION.TMPL":
                        button_action = File.ReadAllText(item.FullName);
                        break;
                    case "GENERIC-PAGE.TMPL":
                        GenericPage = File.ReadAllText(item.FullName);
                        break;
                    case "NOT-FOUND-PAGE.TMPL":
                        NotFoundPage = File.ReadAllText(item.FullName);
                        break;
                    case "PROCESSING-PAGE.TMPL":
                        ProcessingPage = File.ReadAllText(item.FullName);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
