using System;
using System.Drawing;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizBase;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizDetail;
using BizRAD.BizApplication;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizTools;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizControls.BizDateTimePicker;

using QueryADataSet;

namespace BizRAD.BizAccounts
{
	/// <summary>
	/// Class containing Functions used for validation of data entry input
	/// </summary>
	public class BizValidate
	{
		#region CheckColumnError

		public static bool CheckColumnError(DataSet ds, string tableNames)
		{
			string delimStr = " ,/*"; 
			char [] delimiter = delimStr.ToCharArray();
			string [] split = tableNames.Split(delimiter);
			foreach (string s in split) 
			{
				if(ds.Tables.Contains(s))
				{
					foreach(DataRow dr in ds.Tables[s].Rows)
					{
						if (dr.RowState != DataRowState.Deleted)
						{
							if (dr.GetColumnsInError().Length > 0)
								return true;
						}
					}
				}
			}
			return false;
		}

		#endregion

		#region CheckInvoiceBalance
		
		/***********************************************************************
		* !! IMPORTANT ASSUMPTIONS !!
		* 1. (ard/apd) always year table
		* 2. one invnum can only have one currency
		* 3. ard/apd detail page does not have duplicated invoice numbers
		***********************************************************************/
		public static void CheckInvoiceBalance(DBAccess dbAccess, string subLedger, string localDataTable, string ar_ap_code, string refnum)
		{
			if(subLedger == String.Empty || localDataTable == String.Empty || ar_ap_code == String.Empty || refnum == String.Empty)
			{
			}
			else
			{
				DataTable currentTable = dbAccess.DataSet.Tables[localDataTable];
				if(currentTable.Rows.Count>0)
				{
					try
					{
						string dbTableName = subLedger.Trim()+Common.DEFAULT_SYSTEM_YEAR;
						string errInvnums = "";

						foreach(DataRow dr in currentTable.Rows)
						{
							if(dr.RowState != DataRowState.Deleted)
							{
								string sqlCommand = null;
								string currentRowInvnum = dr["invnum"].ToString().Trim();
								decimal oriamt = 0;

								switch(subLedger.ToUpper())
								{
									case "ARD":
										sqlCommand = "SELECT arnum,invnum,oriamt FROM " + dbTableName + " WHERE refnum<>'"+refnum+"' AND arnum='"+ar_ap_code+"' AND invnum='"+currentRowInvnum+"'";
										oriamt += (-1)*(decimal)dr["oriamt"];
										break;
									case "APD":
										sqlCommand = "SELECT apnum,invnum,oriamt FROM " + dbTableName + " WHERE refnum<>'"+refnum+"' AND apnum='"+ar_ap_code+"' AND invnum='"+currentRowInvnum+"'";
										oriamt += (decimal)dr["oriamt"];
										break;
								}

								DataTable dt1 = (dbAccess.ReadSQLTemp("dt1",sqlCommand)).Tables["dt1"];
								if(dt1.Rows.Count!=0)
									oriamt += (decimal)dt1.Rows[0]["oriamt"];
							
								switch(subLedger.ToUpper())
								{
									case "ARD":
										if(oriamt<0)
										{
											if(errInvnums == String.Empty)
												errInvnums = currentRowInvnum + "/" + oriamt.ToString().Trim();
											else
												errInvnums = errInvnums + "\n" + currentRowInvnum + "/" + oriamt.ToString().Trim();
										}
										break;
									case "APD":
										if(oriamt>0)
										{
											if(errInvnums == String.Empty)
												errInvnums = currentRowInvnum + "/" + oriamt.ToString().Trim();
											else
												errInvnums = errInvnums + "\n" + currentRowInvnum + "/" + oriamt.ToString().Trim();
										}
										break;
								}
							}
						}

						if(errInvnums != String.Empty)
						{
							if(subLedger.ToUpper() == "ARD")
								MessageBox.Show(errInvnums, "Credit Invoice Balance A/R", MessageBoxButtons.OK, MessageBoxIcon.Error);
							else if (subLedger.ToUpper() == "APD")
								MessageBox.Show(errInvnums, "Debit Invoice Balanace A/P", MessageBoxButtons.OK, MessageBoxIcon.Error);
							else
							{}
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message+"\n"+ex.StackTrace, "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		#endregion

		#region ChkPeriod

		public static bool ChkPeriodLocked(DBAccess dbAccess, string period)
		{
			DataSet dsTmp = dbAccess.ReadSQLTemp("BizChkPdLock","SELECT * FROM pd"+Common.DEFAULT_SYSTEM_YEAR+" WHERE period="+period+" AND locked=1");
			if(dsTmp.Tables["BizChkPdLock"].Rows.Count!=0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region CheckRowState

		public static bool CheckRowState(DataSet ds, string tableNames)
		{
			string delimStr = " ,/*"; 
			char [] delimiter = delimStr.ToCharArray();
			string [] split = tableNames.Split(delimiter);
			foreach (string s in split) 
			{
				if(ds.Tables.Contains(s))
				{
					foreach(DataRow dr in ds.Tables[s].Rows)
					{
						if(dr.RowState == DataRowState.Modified || dr.RowState == DataRowState.Added)
							return true;
					}
				}
			}
			return false;
		}

		#endregion

        #region CheckTableIsValid

        public static bool CheckTableIsValid(DBAccess dbAccess, string tableName, string columnName, string valueToCheck)
        {
            return CheckTableIsValid(dbAccess, tableName, columnName, valueToCheck, "");
        }

        public static bool CheckTableIsValid(DBAccess dbAccess, string tableName, string columnName, string valueToCheck, string whereClause)
        {
            if ((bool)Common.DEFAULT_SYSTEM_TABLEINFO[tableName.ToUpper()])
                tableName += Common.DEFAULT_SYSTEM_YEAR;

            Parameter[] parameters = new Parameter[4];
            parameters[0] = new Parameter("@valuetocheck", valueToCheck);
            parameters[1] = new Parameter("@columntocheck", columnName);
            parameters[2] = new Parameter("@tablename", tableName);
            parameters[3] = new Parameter("@whereClause", whereClause);

            try
            {
                DataSet ds = dbAccess.RemoteStandardSQL.GetStoredProcedureResult("Biz_CheckTableIsValid", ref parameters);
                if ((int)ds.Tables[0].Rows[0][0] > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        #endregion
	}
}
