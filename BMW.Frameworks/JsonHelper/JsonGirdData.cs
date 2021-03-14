using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BMW.Frameworks.JsonHelper
{
    public class JsonGridData 
    {
        public long total { get; set; }
        public IEnumerable data { get; set; }
    }
}
