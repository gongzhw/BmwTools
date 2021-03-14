using System;
using System.Collections.Generic;
using System.Text;

namespace BMW.Frameworks.WebRequest
{
    [Serializable()]
    public abstract class AbstractPostData
    {
        public abstract String Name
        {
            get;
        }

        public abstract String ContentDisposition
        {
            get;
        }

        public abstract String StringValue
        {
            get;
        }

        public abstract byte[] BinaryValue
        {
            get;
        }

		public abstract String TargetCharset
		{
			get;
			set;
		}

    } // end AbstractPostData
} // end Namespace
