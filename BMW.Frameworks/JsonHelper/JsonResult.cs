using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMW.Frameworks.JsonHelper
{
    /// <summary>
    /// EXT提交后台后的返回类型
    /// </summary>
    public class JsonRequestResult
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public int id { get; set; }
    }
}
