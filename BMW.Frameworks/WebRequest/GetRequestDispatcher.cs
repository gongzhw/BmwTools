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
    public class GetRequestDispatcher : RequestDispatcher
    {
        #region ctor
        public GetRequestDispatcher(HttpRequest request, String targetUrl) : base(request, targetUrl)
        {
        }

        public GetRequestDispatcher(HttpRequest request, String targetUrl, String targetEncoding, String returnEncoding,
            CookieContainer cookie, Int32 maxRetryTimes, Int32 retryWaitTime)
            : base(request, targetUrl, targetEncoding, returnEncoding, cookie, maxRetryTimes, retryWaitTime)
        {
        }

        public GetRequestDispatcher(IList<AbstractPostData> parameters, String targetUrl)
            : base(parameters, targetUrl)
        {
        }

        public GetRequestDispatcher(IList<AbstractPostData> parameters, String targetUrl, String targetEncoding, String returnEncoding,
            CookieContainer cookie, Int32 maxRetryTimes, Int32 retryWaitTime) 
            : base(parameters, targetUrl, targetEncoding, returnEncoding, cookie, maxRetryTimes, retryWaitTime)
        {
        }
        #endregion

        #region usefull code
        /// <summary>
        /// �в������ϻ�ȡ GET ��������Ҫ��������
        /// </summary>
        /// <param name="parameters">��ת����������</param>
        /// <returns>�������Ϊ�գ����ؿ��ַ��������򣬷��� GET ��������������磺a=1&b=2&....</returns>
        private string GetParams(IList<AbstractPostData> parameters, String charset)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return String.Empty;
            }

			if (String.IsNullOrEmpty(charset))
			{
				charset = "UTF-8";
			}

            String param = String.Empty;
			Encoding encoding = Encoding.GetEncoding(charset);

            foreach (AbstractPostData parameter in parameters)
            {
                param += HttpUtility.UrlEncode(parameter.Name, encoding) + "=" + HttpUtility.UrlEncode(parameter.StringValue, encoding);
                param += "&";
            }

            return param.Substring(0, param.Length - 1);
        }
        #endregion

        #region overrides
        protected override HttpWebRequest GetHttpWebRequest(string uri, CookieContainer cookieContainer, IList<AbstractPostData> parameters, string targetEncoding)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new BaseAppException("Ŀ����ַ����Ϊ��");
            }

			if (String.IsNullOrEmpty(targetEncoding))
			{
				targetEncoding = "UTF-8";
			}

            if (parameters != null && parameters.Count > 0)
            {
                uri += "?" + GetParams(parameters, targetEncoding);
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "GET";
            request.KeepAlive = true;
            request.Timeout = DEFAULT_REQUEST_TIMEOUT_TIME;
            request.ReadWriteTimeout = DEFAULT_REQUEST_TIMEOUT_TIME;

            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; InfoPath.1)";

            if (request.CookieContainer == null)
            { 
                request.CookieContainer = cookieContainer;
            }

            return request;
        }
        #endregion

    } // end GetRequestDispatcher
} // end Namespace
