using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BMW.Frameworks.WebRequest
{
    [Serializable()]
    public class FilePostData : AbstractPostData
    {
        #region fields
        private String name;
        private String fileName;
        private String mimeType;
		private String targetCharset;
        private byte[] fileBytes;
        #endregion

        #region ctor
        public FilePostData(String name, String fileName, Stream fileStream) : this(name, fileName, "application/octet-stream", fileStream)
        { 
        }

        public FilePostData(String name, String filePath)
            : this(name, Path.GetFileName(filePath), "application/octet-stream", File.OpenRead(filePath))
        { 
        }

        public FilePostData(String name, String fileName, String mimeType, Stream fileStream)
        {
            this.name = name;
            this.fileName = fileName;
            this.mimeType = mimeType;

            try
            {
                this.fileBytes = this.ReadBytesFromStream(fileStream);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
        #endregion

        #region userfull code
        private void Copy2Array(Array source, Array desc, int srcStartIndex, int descStartIndex, int length)
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
            if (inStream == null)
            {
                return new byte[0];
            }

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
        #endregion

        #region overrides
        public override String Name
        {
            get
            {
                return this.name;
            }
        }
        
        public override string ContentDisposition
        {
            get
            {
                String contentPosition =$"Content-Disposition: form-data; name=\"{name}\"; filename=\"{fileName}\"\r\nContent-Type: {mimeType}";

                return contentPosition;
            }
        }

        public override string StringValue
        {
            get
            {
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return String.Empty;
                }
                else
                {
                    return Convert.ToBase64String(fileBytes);
                }
            }
        }

        public override byte[] BinaryValue
        {
            get
            {
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return new byte[0];
                }
                else
                {
                    return fileBytes;
                }
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

    } // end FilePostData
} // end Namespace
