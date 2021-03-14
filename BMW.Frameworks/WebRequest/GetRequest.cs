using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using BMW.Frameworks.WebRequest;

namespace BMW.Frameworks
{
    public class UrlRequest
    {
        /// <summary>
        /// 获取一个url地址返回的信息（参数不支持中文）
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="par">参数 如： enterpriseId=300658&hotline=01085858585</param>
        /// <returns>返回string类型信息</returns>
        public static string GetUrlRequest(string url,string par)
        {
            //更新微信端配置
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(par);
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(par);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 获取url地址返回的信息（参数支持中英文）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetUrlRequest(string url, List<AbstractPostData> parameters)
        {
            RequestDispatcher dispatcher = new MultipartRequestDispatcher(parameters, url);
            return dispatcher.ForwardForString();
        }
    }
}