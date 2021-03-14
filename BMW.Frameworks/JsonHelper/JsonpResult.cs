using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMW.Frameworks.JsonHelper
{
    /// <summary>
    /// EXT提交后台后的返回类型
    /// </summary>
    public class JsonpRequestResult
    {
        public string jsonpcallback { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public int id { get; set; }

        public override string ToString()
        {
            string jsonpString = string.Format("{0}({{\"success\":\"{1}\",\"id\":\"{2}\",\"msg\":\"{3}\"}})", jsonpcallback, success, id.ToString(), msg);
            return jsonpString;
        }
    }
}
