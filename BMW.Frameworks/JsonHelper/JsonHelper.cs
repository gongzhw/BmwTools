using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web;
using System.IO;
using System.Globalization;

namespace BMW.Frameworks.JsonHelper
{
    public static class FormatJsonExtension
    {
        public static FormatJsonResult JsonFormat(this Controller c, object data, string[] exceptMemberName)
        {
            FormatJsonResult result = new FormatJsonResult
            {
                Data = data,
                ExceptMemberName = exceptMemberName
            };

            return result;
        }

        public static FormatJsonResult JsonFormat(this Controller c, object data)
        {
            return JsonFormat(c, data, null);
        }
    }

    public class FormatJsonResult : ActionResult
    {
        public string[] ExceptMemberName { get; set; }
        public Object Data { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";

            StringWriter sw = new StringWriter();
            JsonSerializer serializer = JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    Converters = new JsonConverter[] { new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter() },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new JsonContractResolver(ExceptMemberName)
                }
                );

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                serializer.Serialize(jsonWriter, Data);
            }
            response.Write(sw.ToString());
        }
    }
}
