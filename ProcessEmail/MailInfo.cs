using System;
using System.Collections.Generic;
using System.Text;

using BizRAD.Tools.LiteORM;


namespace ProcessEmail
{
	[Table]
	public class webMailServer
	{
		public webMailServer()
		{
		}

		[Column]
		public string Server;
		[Column]
		public int Port;
		[Column]
		public string User;
		[Column]
		public string Password;
		[Column]
		public string From;
	}


	[Table]
	public class webMail
	{
		public webMail()
		{
		}

		[Column, PK]
		public string emailId;
		[Column]
		public string emailTo;
		[Column]
		public string emailCC;
		[Column]
		public string Subject;
		[Column]
		public string emailMessage;
		[Column]
		public bool isSent;
		[Column]
		public DateTime SentDate;
	}
}
