using System;

namespace BMW.Frameworks.WebRequest.Exceptions
{
	/// <summary>
	/// 应用程序级异常
	/// </summary>
	public class BaseAppException : System.ApplicationException
	{
		public BaseAppException () : base ()
		{
		}

		public BaseAppException ( String message ) : base ( message )
		{
		}

		public BaseAppException ( String message, Exception innerException ) : base ( message, innerException )
		{
		}

	} 
}