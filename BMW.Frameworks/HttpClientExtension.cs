using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace BMW.Frameworks
{
    public static class HttpClientExtension
    {
        public static HttpResponseMessage PostJson(this HttpClient httpClient, string requestUri, object data)
        {
            var httpContent = new ObjectContent(data.GetType(), data, new JsonMediaTypeFormatter());
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(requestUri, httpContent).Result;
        }

        public static HttpResponseMessage PostJson(this HttpClient httpClient, string requestUri)
        {
            return httpClient.PostAsync(requestUri, null).Result;
        }

        public static HttpResponseMessage PutJson(this HttpClient httpClient, string requestUri, object data)
        {
            var httpContent = new ObjectContent(data.GetType(), data, new JsonMediaTypeFormatter());
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PutAsync(requestUri, httpContent).Result;
        }

        public static HttpResponseMessage Get(this HttpClient httpClient, string requestUri)
        {
            return httpClient.GetAsync(requestUri).Result;
        }

        public static HttpResponseMessage Delete(this HttpClient httpClient, string uri)
        {
            return httpClient.DeleteAsync(uri).Result;
        }

        public static HttpResponseMessage PostFileRaw(
            this HttpClient httpClient,
            string requestUri,
            params FileItem[] fileItemCollection)
        {
            var multipartContent = new MultipartContent(
                "form-data",
                "----WebKitFormBoundaryZbpTnfAXTcLq2sE9");

            fileItemCollection.ToList().ForEach(fileItem =>
            {
                var streamContent = new StreamContent(fileItem.Stream);
                streamContent.Headers.Add("Content-Disposition", "form-data; name=file; " + string.Format("filename=\"{0}\"", fileItem.FileName));
                multipartContent.Add(streamContent);
            });

            return httpClient.PostAsync(requestUri, multipartContent).Result;
        }

        public static T GetDto<T>(this HttpResponseMessage message)
        {
            string result = message.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(result);
        }
    }

    public class FileItem
    {
        public FileItem(string content, string fileName)
            : this(CreateFileStream(content), fileName)
        {
        }

        public FileItem(Stream fileStream, string fileName)
        {
            this.Stream = fileStream;
            this.FileName = fileName;
        }

        public Stream Stream { get; }

        public string FileName { get; }

        static MemoryStream CreateFileStream(string fileContent)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        }
    }
}