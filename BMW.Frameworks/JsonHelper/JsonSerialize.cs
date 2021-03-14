using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace BMW.Frameworks.JsonHelper
{
    public class JsonHelper
    {
        public static string JsonClear(string text)
        {
            return text.Substring(text.IndexOf("{"), text.LastIndexOf("}") - text.IndexOf("{") + 1);
        }

        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream();
            contractJsonSerializer.WriteObject((Stream)memoryStream, (object)t);
            string @string = Encoding.UTF8.GetString(memoryStream.ToArray());
            memoryStream.Close();
            return @string;
        }

        public static T JsonDeserialize<T>(string jsonString)
        {
            return (T)new DataContractJsonSerializer(typeof(T)).ReadObject((Stream)new MemoryStream(Encoding.UTF8.GetBytes(jsonString)));
        }
    }
}
