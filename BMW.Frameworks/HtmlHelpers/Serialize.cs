using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace BMW.Frameworks.HtmlHelpers
{
    public class DeserializerHelper
    {
        public static T Deserialize<T>(string response)
        {

            XmlDocument xdoc = new XmlDocument();

            try
            {
                xdoc.LoadXml(response);
            }
            catch (IOException ex)
            {
                // 记录IO错误
                var logger = LogManager.GetCurrentClassLogger();
                logger.Error("获取信息错误，url:{0}，错误信息为：", response);
                throw new Exception("反序列化失败！");
            }

            T root = LoadObjFromXML<T>(xdoc.InnerXml);

            return root;
        }

        private static T LoadObjFromXML<T>(string data)
        {
            T t = default(T);
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
                {
                    sw.Write(data);
                    sw.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    t = ((T)serializer.Deserialize(stream));
                }
            }
            return t;
        }
    }
}
