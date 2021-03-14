using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using BMW.Frameworks.WebRequest.Exceptions;

namespace BMW.Frameworks.WebRequest
{
    public class MultipartRequestDispatcher : RequestDispatcher
    {
        #region ctor
        public MultipartRequestDispatcher(HttpRequest request, String targetUrl)
            : base(request, targetUrl)
        {
        }

        public MultipartRequestDispatcher(HttpRequest request, String targetUrl, String targetEncoding, String returnEncoding,
            CookieContainer cookie, Int32 maxRetryTimes, Int32 retryWaitTime)
            : base(request, targetUrl, targetEncoding, returnEncoding, cookie, maxRetryTimes, retryWaitTime)
        {
        }

        public MultipartRequestDispatcher(IList<AbstractPostData> parameters, String targetUrl)
            : base(parameters, targetUrl)
        {
        }

        public MultipartRequestDispatcher(IList<AbstractPostData> parameters, String targetUrl, String targetEncoding, String returnEncoding,
            CookieContainer cookie, Int32 maxRetryTimes, Int32 retryWaitTime) 
            : base(parameters, targetUrl, targetEncoding, returnEncoding, cookie, maxRetryTimes, retryWaitTime)
        {
        }
        #endregion

        #region usefull code
        private long GetContentLength(String boundary, IList<AbstractPostData> parameters, String targetCharset)
        {
			if (string.IsNullOrEmpty(boundary) || parameters == null || parameters.Count == 0)
			{
				return 0;
			}

			long length = 0;
			String contentDisposition = "--" + boundary + "\r\n";
			byte[] byteStreamEnd = Encoding.GetEncoding(targetCharset).GetBytes("\r\n--" + boundary + "--\r\n");

			Int32 i = 0;
			foreach (AbstractPostData param in parameters)
			{
				String tempContentDisposition = contentDisposition + param.ContentDisposition + "\r\n\r\n";
				param.TargetCharset = targetCharset;
				length += Encoding.GetEncoding(targetCharset).GetByteCount(tempContentDisposition);
				length += param.BinaryValue.Length;

				if (i != parameters.Count - 1)
				{
					length += Encoding.GetEncoding(targetCharset).GetByteCount("\r\n");
				}

				i++;
			}

			length += byteStreamEnd.Length;

			return length;
		}
        #endregion

        #region overrides
        protected override HttpWebRequest GetHttpWebRequest(string uri, CookieContainer cookieContainer, IList<AbstractPostData> parameters, string targetEncoding)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new BaseAppException("目标网址不能为空");
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "POST";
            request.KeepAlive = true;
            request.Timeout = DEFAULT_REQUEST_TIMEOUT_TIME;
            request.ReadWriteTimeout = DEFAULT_REQUEST_TIMEOUT_TIME;

            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; InfoPath.1)";

            String boundary = "---------------------" + DateTime.Now.Ticks.ToString("x");
            if (request.CookieContainer == null)
            {
                request.CookieContainer = cookieContainer;
            }

            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.ContentLength = GetContentLength(boundary, parameters, targetEncoding);

            if (parameters != null && parameters.Count != 0)
            {
                Stream requireStream = request.GetRequestStream();
                String contentDisposition = "--" + boundary + "\r\n";
                byte[] byteContentDisposition = null;
				byte[] byteStreamEnd = Encoding.GetEncoding(targetEncoding).GetBytes("\r\n--" + boundary + "--\r\n");
				byte[] enterBytes = Encoding.GetEncoding(targetEncoding).GetBytes("\r\n");

                AbstractPostData param = null;
                try
                {
                    for (Int32 i = 0; i < parameters.Count; i++)
                    {
                        param = parameters[i];
                        String tempContentDisposition = contentDisposition + param.ContentDisposition + "\r\n\r\n";
						byteContentDisposition = Encoding.GetEncoding(targetEncoding).GetBytes(tempContentDisposition);
                        requireStream.Write(byteContentDisposition, 0, byteContentDisposition.Length);
                        requireStream.Write(param.BinaryValue, 0, param.BinaryValue.Length);

                        if (i != parameters.Count - 1)
                        {
                            requireStream.Write(enterBytes, 0, enterBytes.Length);
                        }
                    }

                    requireStream.Write(byteStreamEnd, 0, byteStreamEnd.Length);
                }
                finally
                {
                    requireStream.Close();
                }
            }

            return request;
        }
        #endregion

    } // end MultipartRequestDispatcher
} // end Namespace
