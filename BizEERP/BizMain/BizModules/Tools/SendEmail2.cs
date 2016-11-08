using System;
using System.Collections.Generic;
using BizRAD.BizReport;
using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizCommon;
using ATL.BizLogicTools;
using BizRAD.BizAccounts;
using BizRAD.BizVoucher;



namespace ATL.BizModules.Tools
{
    public class SendEmail2
    {
        public string to, from,cc,bcc, subject, body, user;
        DBAccess dbaccess = null;

        public SendEmail2()
        {
            to = "";
            from = "";
            cc = "";
            bcc = "";
            subject = "";
            body = "";
            user = "";

        }

        public SendEmail2(string csubject, string cbody, string cto, string c2, string bc2,string User, DBAccess dbAccess)
        {
            to = cto;
            cc = c2;
            bcc = bc2;
            subject = csubject;
            body = cbody;
            user = User;
            this.dbaccess = dbAccess;

        }

        //public void SendMaterialRequestEmail()
        //{
        //    SmtpMail oMail = new SmtpMail("TryIt");
        //    SmtpClient oSmtp = new SmtpClient();


        //    // Set sender email address, please change it to yours
        //    oMail.From = "ricoh@atlmaintenance.com.sg";

        //    // Set recipient email address, please change it to yours
        //    oMail.To = to;

        //    if (cc != "")
        //    {

        //        oMail.Cc = cc;
        //    }
        //    if (bcc != "")
        //    {
        //        oMail.Bcc = bcc;
        //    }
        


        //    // Set email subject
        //    oMail.Subject = subject;

        //    // Set email body
        //    //oMail.TextBody = body;

        //    oMail.HtmlBody = body;
      

        //    // Your SMTP server address
        //    SmtpServer oServer = new SmtpServer("outlook.office365.com");

        //    // User and password for ESMTP authentication, if your server doesn't require
        //    // User authentication, please remove the following codes.
        //    oServer.User = "ricoh@atlmaintenance.com.sg";
        //    oServer.Password = "ATLfax68413182";

        //    // Set 25 port
        //    oServer.Port = 25;

        //    // detect TLS connection automatically
        //    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

        //    try
        //    {
        //        //Console.WriteLine("start to send email ...");
        //        oSmtp.SendMail(oServer, oMail);
        //        //Console.WriteLine("email was sent successfully!");
        //    }
        //    catch (Exception ep)
        //    {
        //        //Console.WriteLine("failed to send email with the following error:");
        //        //Console.WriteLine(ep.Message);
        //    }
        //}


        public void SendEmail()
        {
            string str = "INSERT INTO [webEmail] "+
                               "([subject] "+
                               ",[emailTo] "+
                               ",[emailCc] "+
                               ",[emailMessage] "+
                               ",[isSent] "+
                               ",[createdOn] "+
                               ",[createdBy]) "+
                         "VALUES "+
                               "( "+
                               "'"+subject+"' "+
                               ",'"+to+"' "+
                               ",'"+cc+"' "+
                               ",'"+body+"' "+
                               ",0 "+
                               ",GETDATE() "+
                               ",'"+user+"' "+
			                    ")";

            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(str);
        }
      

      

    }
}
