using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Interface;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizTools;
using BizRAD.BizCommon;
using BizRAD.BizBase;
using BizRAD.BizControls.OutLookBar;
using PicoGuards.BizLogicTools;
using BizRAD.BizAccounts;

namespace PicoGuards.EOD
{
	public class EOD
	{
		protected DBAccess	dbAccess	= null;
		protected DataSet	ds			= null;
		protected Hashtable selectsCollection = null;
		protected string	command		= null;
		private string posid = ConfigurationManager.AppSettings.Get("POSID");
		private   string	sitenum		= null;
        protected string syncfilename = "C:\\eod.sync";
		public EOD()
		{
			this.dbAccess			= new DBAccess();
			this.selectsCollection	= new Hashtable();
			
			command = "select sitenum from posm where posnum = '" + posid + "'";
			this.dbAccess.ReadSQL("posm", command);

			this.sitenum = this.dbAccess.DataSet.Tables["posm"].Rows[0]["sitenum"].ToString();

            //Only master POS can perform EOD
            if (System.Configuration.ConfigurationManager.AppSettings["posmaster"] != null)
            {
                if (!bool.Parse(System.Configuration.ConfigurationManager.AppSettings["posmaster"]))
                {
                    MessageBox.Show("Only main POS can perform end of day!", "Cannot perform end of day");
                    return;
                }
            }

			DialogResult result;	
			result = MessageBox.Show("Perform End-of-Day for " + DateTime.Now.ToString("yyyy-MM-dd")+ "\nDo it now?", "Perform End-of-Day", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if(result == DialogResult.OK)
			{
                if (!Tools.isHQ(sitenum, this.dbAccess))
                {
                    // If developer, then can look at log and save log.
                    SYNC.Synchronization endofdaysync = new SYNC.Synchronization(syncfilename, DateTime.Now, DateTime.Now, Common.DEFAULT_SYSTEM_ISADMINISTRATORUSER);

                    ///			 -6: Failure at validation
                    ///			 -5: Failure at export
                    ///			 -4: Failure at zip
                    ///			 -3: Failure at transfer
                    ///			 -2: Failure at unzip
                    ///			 -1: Failure at import
                    ///			 
                    // Only if zipping up for ready manual import ,then counted as success. Else fail.
                    #region auto shutdown pc
                    //if (endofdaysync.complete > -4)
                    //{
                    //    if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Shutdown")) &&
                    //        !Common.DEFAULT_SYSTEM_ISADMINISTRATORUSER)
                    //    {
                    //        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    //        proc.EnableRaisingEvents = false;
                    //        proc.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings.Get("ShutDownPath");
                    //        proc.Start();
                    //    }
                    //}
                    #endregion
                }
			}
		}
	}
}
