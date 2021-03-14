//===============================================================================
// 类功能概述
//===============================================================================
// 版权所有 北京世纪摇篮网络技术有限公司 2007
//===============================================================================

#region using
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using BMW.Frameworks.Logs;
using BMW.Frameworks.WebRequest.Exceptions;
using BMW.Frameworks.WebRequest;
#endregion

namespace BMW.Frameworks.WebRequest
{
    /// <remarks>
    /// 对指定网址发送请求的基类，具体实现类包括：发送 GET 请求，发送普通 POST 请求以及发送包括文件流的 POST 请求
    /// </remarks>
    /// 创 建 人: 范宝胤
    /// 创建日期: 2007-10-26 16:06:04
    /// 版    本:
    /// 历史修改记录:
    ///     修改人  日期    版本    描述
    public abstract class RequestDispatcher
    {
        #region fields
        protected static readonly NLogLogger log = new NLogLogger();
		private IList<AbstractPostData> parameters;
        private String targetUrl;
        private String targetEncoding;
        private String returnEncoding;
        private CookieContainer cookie;
        private Int32 maxRetryTimes;
        private Int32 retryWaitTime;

        private const String DEFAULT_TARGET_ENCODING = "utf-8";
        private const String DEFAULT_RETURN_ENCODING = "utf-8";
        private const Int32 DEFAULT_MAX_RETRY_TIMES = 0;
        private const Int32 DEFAULT_RETRY_WAIT_TIME = 0;

        //Request 响应时间
        /// <summary>
        /// 5s （改成120秒了）
        /// </summary>
        protected const Int32 DEFAULT_REQUEST_TIMEOUT_TIME =120 * 1000;
        //protected const Int32 DEFAULT_REQUEST_TIMEOUT_TIME = 120 * 1000;
        #endregion

        #region ctor
        /// <summary>
        /// 创建将 HttpRequest 请求转发至目标地址的 RequestDispatcher 实例
        /// </summary>
        /// <param name="request">Asp.Net 中的 HttpRequest 对象</param>
        /// <param name="targetUrl">请求的目标地址，必填参数</param>
        public RequestDispatcher(HttpRequest request, String targetUrl)
            : this(request, targetUrl, DEFAULT_TARGET_ENCODING, DEFAULT_RETURN_ENCODING, null, DEFAULT_MAX_RETRY_TIMES, DEFAULT_RETRY_WAIT_TIME)
        {
        }

        /// <summary>
        /// 创建将 HttpRequest 请求转发至目标地址的 RequestDispatcher 实例
        /// </summary>
        /// <param name="request">Asp.Net 中的 HttpRequest 对象</param>
        /// <param name="targetUrl">请求的目标地址，必填参数</param>
        /// <param name="targetEncoding">目标地址对应的编码，默认为 utf-8</param>
        /// <param name="returnEncoding">返回字符串的编码格式</param>
        /// <param name="cookie">cookie 对象容器</param>
        /// <param name="maxRetryTimes">请求失败时最大重试次数，默认为 10 次</param>
        /// <param name="retryWaitTime">请求失败两次重试的时间间隔，默认为 10 毫秒</param>
        public RequestDispatcher(HttpRequest request, String targetUrl, String targetEncoding, String returnEncoding, 
            CookieContainer cookie, Int32 maxRetryTimes, Int32 retryWaitTime)
        {
            if (request != null)
            {
                this.parameters = Request2Params(request);
            }
            else
            {
                this.parameters = null;
            }
            this.targetUrl = targetUrl;
            this.targetEncoding = string.IsNullOrEmpty(targetEncoding) ? DEFAULT_TARGET_ENCODING : targetEncoding;
            this.returnEncoding = string.IsNullOrEmpty(returnEncoding) ? DEFAULT_RETURN_ENCODING : returnEncoding;
            this.cookie = cookie;
            this.maxRetryTimes = maxRetryTimes < 0 ? DEFAULT_MAX_RETRY_TIMES : maxRetryTimes;
            this.retryWaitTime = retryWaitTime <= 0 ? DEFAULT_RETRY_WAIT_TIME : retryWaitTime;
        }

        /// <summary>
        /// 创建将自定义参数转发至目标地址的 RequestDispatcher 实例
        /// </summary>
        /// <param name="parameters">自定义参数集合</param>
        /// <param name="targetUrl">请求的目标地址，必填参数</param>
        public RequestDispatcher(IList<AbstractPostData> parameters, String targetUrl)
            : this(parameters, targetUrl, DEFAULT_TARGET_ENCODING, DEFAULT_RETURN_ENCODING, null, DEFAULT_MAX_RETRY_TIMES, DEFAULT_RETRY_WAIT_TIME)
        {
        }

        /// <summary>
        /// 创建将自定义参数转发至目标地址的 RequestDispatcher 实例
        /// </summary>
        /// <param name="parameters">自定义参数集合</param>
        /// <param name="targetUrl">请求的目标地址，必填参数</param>
        /// <param name="targetEncoding">目标地址对应的编码，默认为 utf-8</param>
        /// <param name="returnEncoding">返回字符串的编码格式</param>
        /// <param name="cookie">cookie 对象容器</param>
        /// <param name="maxRetryTimes">请求失败时最大重试次数，默认为 10 次</param>
        /// <param name="retryWaitTime">请求失败两次重试的时间间隔，默认为 10 毫秒</param>
        public RequestDispatcher(IList<AbstractPostData> parameters, String targetUrl, String targetEncoding, String returnEncoding, 
            CookieContainer cookie, Int32 maxRetryTimes, Int32 retryWaitTime)
        {
            this.parameters = parameters;
            this.targetUrl = targetUrl;
            this.targetEncoding = string.IsNullOrEmpty(targetEncoding) ? DEFAULT_TARGET_ENCODING : targetEncoding;
            this.returnEncoding = string.IsNullOrEmpty(returnEncoding) ? DEFAULT_RETURN_ENCODING : returnEncoding;
            this.cookie = cookie;
            this.maxRetryTimes = maxRetryTimes < 0 ? DEFAULT_MAX_RETRY_TIMES : maxRetryTimes;
            this.retryWaitTime = retryWaitTime <= 0 ? DEFAULT_RETRY_WAIT_TIME : retryWaitTime;
        }
        #endregion

        public static IList<AbstractPostData> Request2Params(HttpRequest request)
        {
            if (request == null) return null;

            List<AbstractPostData> parameters = new List<AbstractPostData>();

            FormPostData formParam = null;
            foreach (String key in request.Params)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                formParam = new FormPostData(key, request.Params[key]);
                parameters.Add(formParam);
            }

            FilePostData fileParam = null;
            HttpPostedFile file = null;
            foreach (String key in request.Files.Keys)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                file = request.Files[key];
                fileParam = new FilePostData(key, file.FileName, file.ContentType, file.InputStream );
                parameters.Add(fileParam);
            }

            return parameters;
        }

        protected abstract HttpWebRequest GetHttpWebRequest(String uri, CookieContainer cookieContainer, IList<AbstractPostData> parameters, String targetEncoding);

        private String ReadResponse(HttpWebResponse response, String encoding)
        {
            Int32 charArraySize = 4096;
            Int32 count = -1;
            String htmlContent = String.Empty;
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                Char[] buffer = new Char[charArraySize];
                while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    htmlContent += new String(buffer, 0, count);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return htmlContent;
        }

        private String GetResponseFromUrl(String url, IList<AbstractPostData> parameters, String targetEncoding, CookieContainer cookie, String returnEncoding, Int32 retryTimes, Int32 maxRetryTimes, Int32 retryWaitTime)
        {
            String htmlContent = null;
            HttpWebResponse response = null;
            try
            {
                cookie = cookie ?? new CookieContainer();
                HttpWebRequest request = GetHttpWebRequest(url, cookie, parameters, targetEncoding);
                response = (HttpWebResponse)request.GetResponse();
                htmlContent = ReadResponse(response, returnEncoding);
            }
            catch (Exception e)
            {
				String msg = $"第{retryTimes}次访问页面{url}时出现异常: {e.Message + e.StackTrace}";
				log.Debug(msg);
                if (retryTimes < maxRetryTimes)
                {
                    Thread.Sleep(retryWaitTime);
                    retryTimes++;
                    htmlContent = GetResponseFromUrl(url, parameters, targetEncoding, cookie, returnEncoding, retryTimes, maxRetryTimes, retryWaitTime);
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return htmlContent;
        }

        private byte[] GetResponseFromUrl(String url, IList<AbstractPostData> parameters, String targetEncoding, CookieContainer cookie, Int32 retryTimes, Int32 maxRetryTimes, Int32 retryWaitTime)
        {
            byte[] byteContent = null;
            HttpWebResponse response = null;
            try
            {
                cookie = cookie ?? new CookieContainer();
                HttpWebRequest request = GetHttpWebRequest(url, cookie, parameters, targetEncoding);
                response = (HttpWebResponse)request.GetResponse();
                byteContent = ReadBytesFromStream(response.GetResponseStream());
            }
            catch (Exception e)
            {
				String msg = $"第{retryTimes}次访问页面{url}时出现异常: {e.Message + e.StackTrace}";
				log.Debug(msg);
                if (retryTimes < maxRetryTimes)
                {
                    Thread.Sleep(retryWaitTime);
                    retryTimes++;
                    byteContent = GetResponseFromUrl(url, parameters, targetEncoding, cookie, retryTimes, maxRetryTimes, retryWaitTime);
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return byteContent;
        }

        private static void Copy2Array(Array source, Array desc, int srcStartIndex, int descStartIndex, int length)
        {
            if (source == null || desc == null)
                throw new ArgumentNullException("source array or desc array can not be null");
            if (srcStartIndex < 0 || srcStartIndex >= source.Length)
                throw new ArgumentOutOfRangeException("srcStartIndex has been out of the source array range");
            if (descStartIndex < 0 || descStartIndex >= desc.Length)
                throw new ArgumentOutOfRangeException("descStartIndex has been out of the desc array range");
            if (srcStartIndex + length > source.Length)
                throw new ArgumentOutOfRangeException("length has been out of the source array range");
            if (descStartIndex + length > desc.Length)
                throw new ArgumentOutOfRangeException("length has been out of the desc array range");

            for (int i = 0; i < length; i++)
            {
                desc.SetValue(source.GetValue(i), descStartIndex + i);
            }
        }

        private byte[] ReadBytesFromStream(Stream inStream)
        {
            byte[] bytes = null;
            byte[] b = new byte[10240];

            Int32 i = 0;
            while ((i = inStream.Read(b, 0, b.Length)) != 0)
            {
                if (bytes == null)
                {
                    bytes = new byte[i];
                    Copy2Array(b, bytes, 0, 0, i);
                }
                else
                {
                    byte[] temp = new byte[bytes.Length + i];
                    bytes.CopyTo(temp, 0);
                    Copy2Array(b, temp, 0, bytes.Length, i);
                    bytes = temp;
                }
            }

            return bytes;
        }

        /// <summary>
        /// 将请求发送至目标地址，并获取响应的字符串
        /// </summary>
        /// <returns>返回响应的字符串，如果目标地址为空，将抛出 YaoLan.Share.SystemFramework.Exceptions.BaseAppException</returns>
        public String ForwardForString()
        {
            if (string.IsNullOrEmpty(this.targetUrl))
            {
                throw new BaseAppException("目标网址不能为空");
            }

            String responseStr = GetResponseFromUrl(this.targetUrl, this.parameters, this.targetEncoding, this.cookie, 
                this.returnEncoding, 0, this.maxRetryTimes, this.retryWaitTime);

            return responseStr;
        }

        /// <summary>
        /// 将请求发送至目标地址，并获取响应的二进制数组
        /// </summary>
        /// <returns>返回响应的二进制数组，如果目标地址为空，将抛出 YaoLan.Share.SystemFramework.Exceptions.BaseAppException</returns>
        public byte[] ForwardForByte()
        {
            if (string.IsNullOrEmpty(this.targetUrl))
            {
                throw new BaseAppException("目标网址不能为空");
            }

            byte[] byteResponses = GetResponseFromUrl(this.targetUrl, this.parameters, this.targetEncoding, this.cookie, 
                0, this.maxRetryTimes, this.retryWaitTime);

            return byteResponses;
        }

        /// <summary>
        /// 自动获取转发器
        /// </summary>
        /// <param name="request">Asp.Net 中的 HttpRequest 对象</param>
        /// <param name="targetUrl">请求的目标地址，必填参数</param>
        /// <returns>若 HttpRequest 对象为空，将返回 GET 转发器，否则根据 HttpRequest 对象的 HttpMethod、ContentType 属性，返回特定转发器。若未找到特定转发器，将返回 Null。</returns>
        public static RequestDispatcher GetDispatcher(HttpRequest request, String targetUrl)
        {
            RequestDispatcher dispatcher = null;

            if (request == null)
            {
                dispatcher = new GetRequestDispatcher(request, targetUrl);
            }
            else
            {
                dispatcher = GetDispatcher(request.HttpMethod, request.ContentType, Request2Params(request), targetUrl);
            }

            return dispatcher;
        }

        /// <summary>
        /// 自动获取转发器
        /// </summary>
        /// <param name="method">Http 请求方式，如：GET、POST等，默认为 GET</param>
        /// <param name="contentType">Http 请求的内容类型，仅支持：application/x-www-form-urlencoded、multipart/form-data</param>
        /// <param name="parameters">Http 请求的参数集合</param>
        /// <param name="targetUrl">请求的目标地址，必填参数</param>
        /// <returns>若 method 或 parameters 对象为空，将返回 GET 转发器，否则根据 method、contentType 参数，返回特定转发器。若未找到特定转发器，将返回 Null。</returns>
        public static RequestDispatcher GetDispatcher(String method, String contentType, IList<AbstractPostData> parameters, String targetUrl)
        {
            if (String.IsNullOrEmpty(targetUrl))
            {
                throw new BaseAppException("目标网址不能为空");
            }

            RequestDispatcher dispatcher = null;

            if (string.IsNullOrEmpty(method) || parameters == null)
            {
                dispatcher = new GetRequestDispatcher(parameters, targetUrl);
            }
            else
            {
                if (String.Compare(method, "GET", true) == 0)
                {
                    dispatcher = new GetRequestDispatcher(parameters, targetUrl);
                }
                else if (String.Compare(method, "POST", true) == 0)
                {
                    if (String.Compare(contentType, "application/x-www-form-urlencoded", true) == 0)
                    {
                        dispatcher = new XwwwRequestDispatcher(parameters, targetUrl);
                    }
                    else if (contentType.StartsWith("multipart/form-data"))
                    {
                        dispatcher = new MultipartRequestDispatcher(parameters, targetUrl);
                    }
                }
            }

            return dispatcher;
        }

    } // end RequestDispather
} // end HttpServer
