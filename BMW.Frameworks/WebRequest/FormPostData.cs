using BMW.Frameworks.WebRequest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace BMW.Frameworks.WebRequest
{
    [Serializable()]
    public class FormPostData : AbstractPostData
    {
        #region fields
        private String name;
        private String value;
		private String targetCharset;
        #endregion

        #region ctor
        public FormPostData(String name, String value)
        { 
            this.name = name;
            this.value = value;
        }
        #endregion

        #region usefull code
        private String UriEncode(String uri, String charset)
        {
            String encodedUri = String.Empty;

            if (uri == null || uri == String.Empty)
                return encodedUri;

            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(charset);
            String[] getParams = uri.Split('&');
            foreach (String getParam in getParams)
            {
                Int32 index = getParam.IndexOf('=');
                if (index != -1)
                {
                    String name = getParam.Substring(0, index).Trim();
                    String value = String.Empty;
                    if (getParam.Length > index + 1)
                    {
                        value = getParam.Substring(index + 1, getParam.Length - index - 1).Trim();
                    }
                    encodedUri += encodedUri == String.Empty ? String.Empty : "&";
                    encodedUri += HttpUtility.UrlEncode(name, encoding) + "=" + HttpUtility.UrlEncode(value, encoding);
                }
            }

            return encodedUri;
        }
        #endregion

        #region overrides
        public override string ContentDisposition
        {
            get
            {
				String contentPosition = $"Content-Disposition: form-data; name=\"{name}\"";

                return contentPosition;
            }
        }

        public override byte[] BinaryValue
        {
            get
            {
				if (String.IsNullOrEmpty(TargetCharset))
				{
					TargetCharset = "utf-8";
				}
                if (value != null)
                {
					Encoding encoding = System.Text.Encoding.GetEncoding(TargetCharset);

					return encoding.GetBytes(value);
                }
                else
                {
                    return new byte[0];
                }
            }
        }

        public override string StringValue
        {
            get
            {
                return value;
            }
        }

        public override String Name
        {
            get
            {
                return this.name;
            }
        }

		public override string TargetCharset
		{
			get
			{
				return this.targetCharset;
			}
			set
			{
				this.targetCharset = value;
			}
		}
        #endregion

    } // end FormPostData
} // end Namespace
