using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace BMW.Frameworks.HtmlHelpers
{
    public class eWebRequest
    {
        const string sUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        const string sContentType = "application/x-www-form-urlencoded";
        const string sRequestEncoding = "utf-8";
        const string sResponseEncoding = "utf-8";

        /// <summary>
        /// Post data��url
        /// </summary>
        /// <param name="data">Ҫpost������</param>
        /// <param name="url">Ŀ��url</param>
        /// <returns>��������Ӧ</returns>
        public static string PostDataToUrl(string data, string url)
        {
            ServicePointManager.Expect100Continue = false;
            Encoding encoding = Encoding.GetEncoding(sRequestEncoding);
            byte[] bytesToPost = encoding.GetBytes(data);
            return PostDataToUrl(bytesToPost, url);
        }

        /// <summary>
        /// Post data��url
        /// </summary>
        /// <param name="data">Ҫpost������</param>
        /// <param name="url">Ŀ��url</param>
        /// <returns>��������Ӧ</returns>
        static string PostDataToUrl(byte[] data, string url)
        {
            #region ����httpWebRequest����
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                throw new ApplicationException(
                    string.Format("Invalid url string: {0}", url)
                    );
            }
            #endregion

            #region ���httpWebRequest�Ļ�����Ϣ
            httpRequest.UserAgent = sUserAgent;
            httpRequest.ContentType = sContentType;
            httpRequest.Method = "POST";
            #endregion

            #region ���Ҫpost������
            httpRequest.ContentLength = data.Length;
            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            #endregion

            #region ����post���󵽷���������ȡ������������Ϣ
            Stream responseStream;
            try
            {
                responseStream = httpRequest.GetResponse().GetResponseStream();
            }
            catch (Exception e)
            {
                // log error
                Console.WriteLine(
                    string.Format("POST���������쳣��{0}", e.Message)
                    );
                throw e;
            }
            #endregion

            #region ��ȡ������������Ϣ
            string stringResponse = string.Empty;
            using (StreamReader responseReader =
                new StreamReader(responseStream, Encoding.GetEncoding(sResponseEncoding)))
            {
                stringResponse = responseReader.ReadToEnd();
            }
            responseStream.Close();
            #endregion
            return stringResponse;
        }
    }
}
