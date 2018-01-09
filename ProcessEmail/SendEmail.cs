using System;
using System.Collections.Generic;

using EASendMail;

namespace ProcessEmail
{
    public class SendEmail
    {
		public static string Server = "";
		public static int Port = 25;
		public static string User = "";
		public static string Pwd = "";
		public static string From = "";

        public string to, from,cc,bcc, subject, body;
        SmtpClient smtp = null;

        public SendEmail()
        {
            to = "";
            from = "";
            cc = "";
            bcc = "";
            subject = "";
            body = "";
        }

        public SendEmail(string csubject, string cbody, string cto, string c2, string bc2)
        {
            smtp = new SmtpClient();
            to = cto;
            cc = c2;
            bcc = bc2;
            subject = csubject;
            body = cbody;

        }

        public bool SendMaterialRequestEmail()
        {
            SmtpMail oMail = new SmtpMail("TryIt");
            SmtpClient oSmtp = new SmtpClient();


            // Set sender email address, please change it to yours
            oMail.From = SendEmail.From;

            // Set recipient email address, please change it to yours
            oMail.To = to;

            if (cc != "")
            {

                oMail.Cc = cc;
            }
            if (bcc != "")
            {
                oMail.Bcc = bcc;
            }
        


            // Set email subject
            oMail.Subject = subject;

            // Set email body
            //oMail.TextBody = body;

            oMail.HtmlBody = body;
      

            // Your SMTP server address
            SmtpServer oServer = new SmtpServer(SendEmail.Server);

            // User and password for ESMTP authentication, if your server doesn't require
            // User authentication, please remove the following codes.
            oServer.User = SendEmail.User;
            oServer.Password = SendEmail.Pwd;

            // Set 25 port
            oServer.Port = 25;

            // detect TLS connection automatically
            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

            try
            {
                //Console.WriteLine("start to send email ...");
                oSmtp.SendMail(oServer, oMail);
                //Console.WriteLine("email was sent successfully!");

				return true;
            }
            catch
            {
				return false;
                //Console.WriteLine("failed to send email with the following error:");
                //Console.WriteLine(ep.Message);
            }
        }
      

      

    }
}
