using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

using BizRAD.Tools.LiteORM;


namespace ProcessEmail
{
	class Program
	{
		static void Main(string[] args)
		{
            int TimeOutSec = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("TimeOutSec"));
            int SleepTime = 0;
            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out SleepTime) == false)
                    SleepTime = TimeOutSec * 1000;
                else
                    SleepTime = SleepTime * 1000;
            }

			DbFactory.DB = DbFactory.Instance.GetDb();
			//
			List<webMailServer> server = DbFactory.Instance.GetList<webMailServer>(null) as List<webMailServer>;
			if (server.Count > 0)
			{
				SendEmail.Server = server[0].Server;
				SendEmail.Port = server[0].Port;
				SendEmail.User = server[0].User;
				SendEmail.Pwd = server[0].Password;
				SendEmail.From = server[0].From;
			}

			//Jason: 15/11/2017
            //IQuery qry = DbFactory.DB.Query();
            //qry.Constrain("isSent").Equal(false);
            //List<webMail> mails = DbFactory.Instance.GetList<webMail>(qry) as List<webMail>;

            //SendEmail emailMSG;
            //foreach (webMail mail in mails)
            //{
            //    emailMSG = new SendEmail(mail.Subject, mail.emailMessage, mail.emailTo, mail.emailCC, "");
            //    if (emailMSG.SendMaterialRequestEmail() == true)
            //    {
            //        mail.isSent = true;
            //        mail.SentDate = DateTime.Now;
            //        DbFactory.DB.Update(mail);
            //    }
            //}

            //Jason: 15/11/2017 - Added improved if statements and sleeptime
            if (DbFactory.DB != null)
            {
                IQuery qry = DbFactory.DB.Query();
                qry.Constrain("isSent").Equal(false);
                List<webMail> mails;

                while (true)
                {
                    mails = DbFactory.Instance.GetList<webMail>(qry) as List<webMail>;
                    if (mails == null)
                    {
                        //sleep for X period
                        System.Threading.Thread.Sleep(SleepTime);
                    }
                    else
                    {
                        SendEmail emailMSG;
                        foreach (webMail mail in mails)
                        {
                            emailMSG = new SendEmail(mail.Subject, mail.emailMessage, mail.emailTo, mail.emailCC, "");
                            if (emailMSG.SendMaterialRequestEmail() == true)
                            {
                                mail.isSent = true;
                                mail.SentDate = DateTime.Now;
                                DbFactory.DB.Update(mail);
                            }
                        }
                    }
                }
            }
			Console.ReadLine();
		}
	}


}
