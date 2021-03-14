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
	/// <summary>
	/// 注意 TransEncoding 与 TargetEncoding 的区别
	/// </summary>
    [Serializable()]
    public class XwwwRequestDispatcher : RequestDispatcher
    {
        #region ctor
        public XwwwRequestDispatcher(HttpRequest request, String targetUrl)
            : base(request, targetUrl)
        {
        }

        public XwwwRequestDispatcher(HttpRequest request, String targetUrl, String targetEncoding, String returnEncoding,
            CookieContainer cookie, Int32 maxRetryTimes, Int32 retryWaitTime)
            : base(request, targetUrl, targetEncoding, returnEncoding, cookie, maxRetryTimes, retryWaitTime)
        {
        }

        public XwwwRequestDispatcher(IList<AbstractPostData> parameters, String targetUrl)
            : base(parameters, targetUrl)
        {
        }

        public XwwwRequestDispatcher(IList<AbstractPostData> parameters, String targetUrl, String targetEncoding, String returnEncoding,
            CookieContainer cookie, Int32 maxRetryTimes, Int32 retryWaitTime) 
            : base(parameters, targetUrl, targetEncoding, returnEncoding, cookie, maxRetryTimes, retryWaitTime)
        {
        }
        #endregion

        #region usefull code
        private string GetPostedParams(IList<AbstractPostData> parameters, String charset)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return String.Empty;
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
            if (uri == null || uri == String.Empty)
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

            if (request.CookieContainer == null)
            {
                request.CookieContainer = cookieContainer;
            }
			if (String.IsNullOrEmpty(targetEncoding))
			{
				targetEncoding = "UTF-8";
			}

            String param = GetPostedParams(parameters, targetEncoding);
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(param); // 总是以 UTF-8 传送数据
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            Stream requireStream = request.GetRequestStream();
            try
            {
                requireStream.Write(postData, 0, postData.Length);
            }
            finally
            { 
                requireStream.Close();
            }

            return request;
        }
        #endregion

    } // end XwwwRequestDispatcher
} // end Namespace
