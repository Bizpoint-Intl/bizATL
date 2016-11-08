using System;
using System.Collections.Generic;

using System.Net.Mail;

namespace ATL.BizModules.Tools
{
    public class SendEmail
    {
        public string to, from, subject, body;
        SmtpClient smtp = null;

        public SendEmail()
        {
            to = "";
            from = "";
            subject = "";
            body = "";
        }

        public SendEmail(string cto, string cfrom, string csubject, string cbody)
        {
            smtp = new SmtpClient();
            cto = to;
            cfrom = from;
            csubject = subject;
            cbody = body;
        }
        public void MailSenderFeedBack()
        {
    
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;


            smtp.Credentials = new System.Net.NetworkCredential("JAOcommerce@gmail.com", "123abc!@#");
            smtp.EnableSsl = true;

            to = "JAOcommerce@gmail.com";


            System.Net.Mail.MailMessage message = new MailMessage(from, to, subject, body);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            try
            {
                smtp.Send(message);


            }
            catch (Exception ex)
            {


            }
        }

        public void MailRespond()
        {

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;


            smtp.Credentials = new System.Net.NetworkCredential("JAOcommerce@gmail.com", "123abc!@#");
            smtp.EnableSsl = true;

            from = "JAOcommerce@gmail.com";


            System.Net.Mail.MailMessage message = new MailMessage(from, to, subject, body);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            try
            {
                smtp.Send(message);

            }
            catch (Exception ex)
            {


            }
        }


        //public void SendMaterialRequestEmail()
        //{


        //    smtp.Host = "outlook.office365.com";
        //    smtp.Port = 25;


        //    smtp.Credentials = new System.Net.NetworkCredential("ricoh@atlmaintenance.com.sg", "ATLfax68413182");
        //    smtp.EnableSsl = true;

        //    from = "ricoh@atlmaintenance.com.sg";


        //    System.Net.Mail.MailMessage message = new MailMessage(from, to, subject, body);
        //    message.BodyEncoding = System.Text.hem
        //    message.IsBodyHtml = true;
        //    try
        //    {
        //        smtp.Send(message);

        //    }
        //    catch (Exception ex)
        //    {


        //    }

        //    //SmtpMail oMail = new SmtpMail("TryIt");
        //    //SmtpClient oSmtp = new SmtpClient();


        //    //// Set sender email address, please change it to yours
        //    //oMail.From = "ricoh@atlmaintenance.com.sg";

        //    //// Set recipient email address, please change it to yours
        //    //oMail.To = to;

        //    //oMail.Cc = "felix@atlmaintenance.com.sg; clarence@atlmaintenance.com.sg";
        //    ////oMail.Cc = "jayobina@gmail.com";


        //    //// Set email subject
        //    //oMail.Subject = subject;

        //    //// Set email body
        //    //oMail.TextBody = body;


        //    //// Your SMTP server address
        //    //SmtpServer oServer = new SmtpServer("outlook.office365.com");

        //    //// User and password for ESMTP authentication, if your server doesn't require
        //    //// User authentication, please remove the following codes.
        //    //oServer.User = "ricoh@atlmaintenance.com.sg";
        //    //oServer.Password = "ATLfax68413182";

        //    //// Set 25 port
        //    //oServer.Port = 25;

        //    //// detect TLS connection automatically
        //    //oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

        //    //try
        //    //{
        //    //    oSmtp.SendMail(oServer, oMail);
        //    //}
        //    //catch (Exception ep)
        //    //{
        //    //}
        //}


    }
}
