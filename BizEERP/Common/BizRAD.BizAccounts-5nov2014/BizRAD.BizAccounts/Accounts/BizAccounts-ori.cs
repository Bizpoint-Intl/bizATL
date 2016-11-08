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
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;

namespace BizRAD.BizAccounts 
{
	/// <summary>
	/// Class containing procedures related to Accounts Ledgers postings
	/// </summary>
	public class BizAccounts
	{
		#region CalTrnDiff

		/***********************************************************************
		* !! IMPORTANT ASSUMPTIONS !!
		* 1. (ard/apd) always year table
		* 2. one invnum can only have one currency
		* 3. ard/apd detail page does not have duplicated invoice numbers
		***********************************************************************/
		public static void CalTrnDiff(DBAccess dbAccess, string refnum, string dataTableName, string Clause1, string Clause2, string Clause3)
		{
			if(refnum == String.Empty || dataTableName == String.Empty)
			{
				MessageBox.Show("refnum and dataTableName cannot be empty", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else if(Clause1 == String.Empty || Clause2 == String.Empty || Clause3 == String.Empty)
			{
				MessageBox.Show("Clause1, Clause2 and Clause3 cannot be empty", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				try
				{
					DataTable dataTable = dbAccess.DataSet.Tables[dataTableName];
					
					foreach(DataRow dr in dataTable.Rows)
					{
						if(dr.RowState != DataRowState.Deleted)
						{
							string dbName = dataTableName+Common.DEFAULT_SYSTEM_YEAR;
							string sqlCommand = "";
							string selectClause = "";
							string whereClause = "";
							Hashtable selectedCollection = new Hashtable();

							#region Get +ve(SIV, DNR) OR -ve(PIV, DNP) entries

							selectClause = "invnum,SUM(oriamt) AS oriInv,SUM(postamt) AS postInv,"+
								"SUM(oriamt*0) AS oriamt,SUM(oriamt*0) AS postamt,"+
								"SUM(oriamt*0) AS oriTmp,SUM(oriamt*0) AS postTmp,"+
								"SUM(oriamt*0) AS exramt,0 AS advtype";
							whereClause = Clause1 + " AND " + Clause2 + " AND refnum<>'" + refnum  + "' AND invnum='" + dr["invnum"].ToString().Trim() + "'";
							sqlCommand = "SELECT " + selectClause + " FROM " + dbName + 
								" WHERE advtype<>1 AND " + whereClause + " GROUP BY invnum ";
							selectedCollection.Add("dt1",sqlCommand);

							#endregion

							#region Get previous (REC, CNR) OR (PAY, CNP) entries

							selectClause = "invnum,SUM(oriamt*0) AS oriInv,SUM(oriamt*0) AS postInv,"+
								"SUM(oriamt) AS oriamt,SUM(postamt+exramt) AS postamt,"+
								"SUM(oriamt*0) AS oriTmp,SUM(oriamt*0) AS postTmp,"+
								"SUM(oriamt*0) AS exramt,0 AS advtype";
							whereClause = Clause1 + " AND " + Clause3 + " AND refnum<>'" + refnum  + "' AND invnum='" + dr["invnum"].ToString().Trim() + "'";
							sqlCommand = "SELECT " + selectClause + " FROM " + dbName + 
								" WHERE advtype<>1 AND " + whereClause + " GROUP BY invnum ";
							selectedCollection.Add("dt2",sqlCommand);					

							#endregion

							#region Get Advance (REC, CNR) OR (PAY,CNP)

							selectClause = "invnum,SUM(oriamt) AS oriInv,SUM(postamt) AS postInv,"+
								"SUM(oriamt*0) AS oriamt,SUM(oriamt*0) AS postamt,"+
								"SUM(oriamt*0) AS oriTmp,SUM(oriamt*0) AS postTmp,"+
								"SUM(oriamt*0) AS exramt,1 AS advtype";
							whereClause = Clause1 + " AND " + Clause3 + " AND refnum<>'" + refnum  + "' AND invnum='" + dr["invnum"].ToString().Trim() + "'";
							sqlCommand = "SELECT " + selectClause + " FROM " + dbName + 
								" WHERE advtype=1 AND " + whereClause + " GROUP BY invnum ";
							selectedCollection.Add("dt3",sqlCommand);	

							#endregion

							DataSet ardTmp = dbAccess.ReadSQLTemp(selectedCollection);

							#region Create a temporary table for putting results of the 3 selects above & current row */
							
							DataTable dtTmp = new DataTable("dtTmp");
							dtTmp.Columns.Add("invnum",Type.GetType("System.String"));
							dtTmp.Columns.Add("oriInv",Type.GetType("System.Decimal"));
							dtTmp.Columns.Add("postInv",Type.GetType("System.Decimal"));
							dtTmp.Columns.Add("oriamt",Type.GetType("System.Decimal"));
							dtTmp.Columns.Add("postamt",Type.GetType("System.Decimal"));
							dtTmp.Columns.Add("oriTmp",Type.GetType("System.Decimal"));
							dtTmp.Columns.Add("postTmp",Type.GetType("System.Decimal"));
							dtTmp.Columns.Add("exramt",Type.GetType("System.Decimal"));
							dtTmp.Columns.Add("advtype",Type.GetType("System.Int32"));

							#endregion
							
							#region append results of 3 selects

							foreach(DataRow dr1 in ardTmp.Tables["dt1"].Rows)
							{
								if(dr1.RowState != DataRowState.Deleted)
								{
									DataRow dtTmp_dr = dtTmp.Rows.Add(new object [] {});
									BizFunctions.UpdateDataRow(dr1, dtTmp_dr, "invnum/oriInv/postInv/oriamt/postamt/oriTmp/postTmp/exramt/advtype");
								}
							}
							foreach(DataRow dr1 in ardTmp.Tables["dt2"].Rows)
							{
								if(dr1.RowState != DataRowState.Deleted)
								{
									DataRow dtTmp_dr = dtTmp.Rows.Add(new object [] {});
									BizFunctions.UpdateDataRow(dr1, dtTmp_dr, "invnum/oriInv/postInv/oriamt/postamt/oriTmp/postTmp/exramt/advtype");
								}
							}
							foreach(DataRow dr1 in ardTmp.Tables["dt3"].Rows)
							{
								if(dr1.RowState != DataRowState.Deleted)
								{
									DataRow dtTmp_dr = dtTmp.Rows.Add(new object [] {});
									BizFunctions.UpdateDataRow(dr1, dtTmp_dr, "invnum/oriInv/postInv/oriamt/postamt/oriTmp/postTmp/exramt/advtype");
								}
							}
							
							#endregion

							#region append current ard row

							DataRow dtTmp_newdr = dtTmp.Rows.Add(new object [] {});
							dtTmp_newdr["invnum"] = dr["invnum"];
							dtTmp_newdr["oriInv"] = 0;
							dtTmp_newdr["postInv"] = 0;
							dtTmp_newdr["oriamt"] = 0;
							dtTmp_newdr["postamt"] = 0;
							dtTmp_newdr["oriTmp"] = dr["oriamt"];
							dtTmp_newdr["postTmp"] = dr["postamt"];
							dtTmp_newdr["exramt"] = 0;
							dtTmp_newdr["advtype"] = dr["advtype"];

							#endregion

							#region union 4 tables and put resulting row in DataTable sqlReults

							int noOfTables = ardTmp.Tables.Count;
							ardTmp.Tables.Add(dtTmp);
							ardTmp.Tables[noOfTables].TableName="dtTmp";

							sqlCommand = "SELECT invnum,SUM(oriInv) AS oriInv,SUM(postInv) AS postInv,"+
								"SUM(oriamt) AS oriamt,SUM(postamt) AS postamt,"+
								"SUM(oriTmp) AS oriTmp,SUM(postTmp) AS postTmp,"+
								"SUM(exramt) AS exramt,MAX(advtype) as advtype FROM [dtTmp] "+
								"GROUP BY invnum ORDER BY invnum";
							DataTable sqlResults = BizFunctions.ExecuteQuery(ardTmp, sqlCommand);

							ardTmp.Tables.Remove("dtTmp");

							#endregion

							if (sqlResults.Rows.Count != 0)
							{
								DataRow sql_dr = sqlResults.Rows[0];
							
								#region Full Payment
							
								if(((decimal)sql_dr["oriInv"]+(decimal)sql_dr["oriamt"]+(decimal)sql_dr["oriTmp"])==0 &&
									((decimal)sql_dr["postInv"]+(decimal)sql_dr["postamt"]+(decimal)sql_dr["postTmp"])!=0)
								{
									dr["exramt"] = - ((decimal)sql_dr["postInv"] + (decimal)sql_dr["postamt"] + (decimal)sql_dr["postTmp"]);
								}

								#endregion

								#region Partial Receipt

								else if((decimal)sql_dr["oriInv"]+(decimal)sql_dr["oriamt"]+(decimal)sql_dr["oriTmp"]>0 &&
									dataTableName.ToUpper()=="ARD")
								{
									if((decimal)sql_dr["oriInv"] == 0)
										dr["exramt"] = (decimal)sql_dr["oriTmp"] - (decimal)sql_dr["postTmp"];
									else
										dr["exramt"] = BizFunctions.Round(((decimal)sql_dr["oriTmp"]*(decimal)sql_dr["postInv"])/(decimal)sql_dr["oriInv"]) - (decimal)sql_dr["postTmp"];
								}

								#endregion

								#region Partial Payment

								else if((decimal)sql_dr["oriInv"]+(decimal)sql_dr["oriamt"]+(decimal)sql_dr["oriTmp"]<0 &&
									dataTableName.ToUpper()=="APD")
								{
									if((decimal)sql_dr["oriInv"] == 0)
										dr["exramt"] = (decimal)sql_dr["oriTmp"] - (decimal)sql_dr["postTmp"];
									else
										dr["exramt"] = BizFunctions.Round(((decimal)sql_dr["oriTmp"]*(decimal)sql_dr["postInv"])/(decimal)sql_dr["oriInv"]) - (decimal)sql_dr["postTmp"];
								}
							
								#endregion
							
								#region Advance Sales Credit Note

								else if((decimal)sql_dr["oriInv"]+(decimal)sql_dr["oriamt"]+(decimal)sql_dr["oriTmp"]<0 &&
									dataTableName.ToUpper()=="ARD" && (int)sql_dr["advtype"]==1)
								{
									if((decimal)sql_dr["oriInv"] == 0)
										dr["exramt"] = (decimal)sql_dr["oriTmp"] - (decimal)sql_dr["postTmp"];
									else
										dr["exramt"] = BizFunctions.Round(((decimal)sql_dr["oriTmp"]*(decimal)sql_dr["postInv"])/(decimal)sql_dr["oriInv"]) - (decimal)sql_dr["postTmp"];
								}

								#endregion

								#region Advance Purchase Credit Note

								else if((decimal)sql_dr["oriInv"]+(decimal)sql_dr["oriamt"]+(decimal)sql_dr["oriTmp"]>0 &&
									dataTableName.ToUpper()=="APD" && (int)sql_dr["advtype"]==1)
								{
									if((decimal)sql_dr["oriInv"] == 0)
										dr["exramt"] = (decimal)sql_dr["oriTmp"] - (decimal)sql_dr["postTmp"];
									else
										dr["exramt"] = BizFunctions.Round(((decimal)sql_dr["oriTmp"]*(decimal)sql_dr["postInv"])/(decimal)sql_dr["oriInv"]) - (decimal)sql_dr["postTmp"];
								}

								#endregion

								else 
								{
									dr["exramt"] = 0;
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message+"\n"+ex.StackTrace, "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		#endregion

		#region GetDefaultAccount
		/// <summary>
		/// Gets the default account code used by the company for posting
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		/// <param name="refnum">Refrence No in ACC table in sql</param>
		public static string GetDefaultAccount(DBAccess dbAccess, string refnum)
		{
            string tableACC = "ACC";
            if(Convert.ToBoolean(Common.DEFAULT_SYSTEM_TABLEINFO["ACC"]))
			    tableACC = "ACC"+Common.DEFAULT_SYSTEM_YEAR;

			string accnum = "";
			DataSet ds = dbAccess.ReadSQLTemp("BizAccTmp", "SELECT accnum FROM "+tableACC+" WHERE refnum='"+refnum+"'");
			
			if(ds.Tables["BizAccTmp"].Rows.Count!=0)
			{
				accnum = ds.Tables["BizAccTmp"].Rows[0]["accnum"].ToString();
			}
			
			return accnum;
		}

		#endregion
		
		#region GetDefaultCompany
		/// <summary>
		/// Gets the default company code in coy database. Returns the row with the smallest ID if more than 1 rows exist.
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		public static string GetDefaultCompany(DBAccess dbAccess)
		{
			string coynum = "";
			DataSet ds = dbAccess.ReadSQLTemp("BizCoyTmp", "SELECT * FROM coy ORDER BY [ID]");
			
			if(ds.Tables["BizCoyTmp"].Rows.Count!=0)
			{
				coynum = ds.Tables["BizCoyTmp"].Rows[0]["coy"].ToString();
			}
			
			return coynum;
		}

		#endregion

		#region GetDefaultCurrency
		/// <summary>
		/// Gets the default currency used by the company
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		public static string GetDefaultCurrency(DBAccess dbAccess)
		{
			DataSet dsTmp = dbAccess.ReadSQLTemp("SysYear","SELECT oricur FROM SysYear WHERE SystemYear='"+Common.DEFAULT_SYSTEM_YEAR+"'");
			if(dsTmp.Tables["SysYear"].Rows.Count != 0)
				return dsTmp.Tables["SysYear"].Rows[0]["oricur"].ToString().Trim();
			else
				return "SGD";
		}

		#endregion

		#region GetExRate
		/// <summary>
		/// Returns the exchange rate of the currency specified based on date passed in 
		/// from the exchange rate table (EXRD)
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		/// <param name="Currency">Currency used to search for exchange rate</param>
		/// <param name="trandate">Date to get exchange rate</param>
		public static decimal GetExRate(DBAccess dbAccess, string Currency, DateTime trandate)
		{
			decimal exrateValue = 0;

			if (!Convert.IsDBNull(Currency) && !Convert.IsDBNull(trandate))
			{
				if (Currency.Trim() != String.Empty)
				{
					DataSet ds = null;
					string sqlCommand = null;
					string safeDate = BizFunctions.GetSafeDateString(trandate);


					if (UseNewEXRScheme(dbAccess))
					{
						try
						{
							//Get Sysyear & period First
							sqlCommand = string.Format("SELECT SysYear,period FROM pd WHERE pdstart <='{0}' and pdend >='{1}'", safeDate, safeDate);
							ds = dbAccess.RemoteStandardSQL.GetSQLResult(sqlCommand);
							if (ds != null && ds.Tables.Count > 0)
							{
								string SysYr = ds.Tables[0].Rows[0][0].ToString();
								string period = ds.Tables[0].Rows[0][1].ToString();

								sqlCommand = string.Format("select rate{0} as exrate FROM EXR WHERE expire=0 and oricur='{1}' and systemyear={2}", period, Currency, SysYr);
								object result = dbAccess.RemoteStandardSQL.GetScalarResult(sqlCommand, null);
								if (Convert.IsDBNull(result) == true)
									MessageBox.Show("No Exchange Rate defined for currency '" + Currency + "' for " + period.ToString() + " !", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
								else
									exrateValue = (decimal)result;
							}
						}
						catch
						{
							MessageBox.Show("Connection To The Server Has Been Broken!", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
					else
					{
						string tableName = "exrd" + Common.DEFAULT_SYSTEM_YEAR;
						sqlCommand = "SELECT * FROM " + tableName + " WHERE oricur='" + Currency + "' AND effectivedate<='" + safeDate + "' ORDER BY effectivedate DESC";
						ds = dbAccess.ReadSQLTemp("BizExr", sqlCommand);

						if (ds.Tables["Bizexr"].Rows.Count != 0)
						{
							exrateValue = (decimal)ds.Tables["Bizexr"].Rows[0]["exrate"];
						}
						else
							MessageBox.Show("No Exchange Rate defined for currency '" + Currency + "' for " + trandate.ToShortDateString() + " !", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}

			
			return exrateValue;
		}

        /// <summary>
        /// Returns the exchange rate of the currency specified based on period passed in 
        /// from the exchange rate table (EXR)
        /// </summary>
        /// <param name="dbAccess">DBAccess object</param>
        /// <param name="Currency">Currency used to search for exchange rate</param>
        /// <param name="period">period to get exchange rate</param>
        public static decimal GetExRate(DBAccess dbAccess, string Currency, int period)
        {
			decimal exrateValue = 0;

			if (!Convert.IsDBNull(Currency) && !Convert.IsDBNull(period))
			{
				if (Currency.Trim() != String.Empty)
				{
					if (UseNewEXRScheme(dbAccess))
					{
						string sqlCmd = string.Format("select rate{0} as exrate FROM EXR WHERE expire=0 and oricur='{1}' and systemyear={2}",
							 period.ToString(), Currency, Common.DEFAULT_SYSTEM_YEAR);
						try
						{
							object result = dbAccess.RemoteStandardSQL.GetScalarResult(sqlCmd, null);
							if (Convert.IsDBNull(result) == true)
								MessageBox.Show("No Exchange Rate defined for currency '" + Currency + "' for " + period.ToString() + " !", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
							else
								exrateValue = (decimal)result;
						}
						catch
						{
							MessageBox.Show("Connection To The Server Has Been Broken!", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error); 
						}
					}
					else
					{
						string tableName = "exr" + Common.DEFAULT_SYSTEM_YEAR;
						string sqlCommand = "SELECT rate" + period.ToString() + " as exrate FROM " + tableName + " WHERE oricur='" + Currency + "'";
						DataSet ds = dbAccess.ReadSQLTemp("BizExr", sqlCommand);

						if (ds.Tables["Bizexr"].Rows.Count != 0)
						{
							exrateValue = (decimal)ds.Tables["Bizexr"].Rows[0]["exrate"];
						}
						else
							MessageBox.Show("No Exchange Rate defined for currency '" + Currency + "' for " + period.ToString() + " !", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}


			return exrateValue;
        }


		public static bool UseNewEXRScheme(DBAccess dbAccess)
		{
			try
			{
				object result = dbAccess.RemoteStandardSQL.GetScalarResult("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EXR' AND COLUMN_NAME='SystemYear'", null);
				if (Convert.IsDBNull(result) == true)
					return false;

				int count = (int)result;


				return (count > 0);
			}
			catch
			{
				return false;
			}
		}

		#endregion

		#region GetGstRate
		/// <summary>
		/// Returns the system GST rate based on financial year
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		/// <param name="trandate">Date used to search for correct finanical year to retrieve gst percentage</param>
        public static decimal GetGstRate(DBAccess dbAccess, DateTime trandate)
        {
            Parameter[] parameters = new Parameter[2];
            parameters[0] = new Parameter("@trandate", BizFunctions.GetSafeDateString(trandate));
            parameters[1] = new Parameter("@taxrate", decimal.Zero);

            parameters[1].Output = true;

            try
            {
                DataSet ds = dbAccess.RemoteStandardSQL.GetStoredProcedureResult("Biz_GetTaxRate", ref parameters);
                return Decimal.Parse(parameters[1].ParameterValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

		#endregion

		#region GetPeriod


		public static int GetSystemYear(DBAccess dbAccess, DateTime trandate)
		{
			try
			{
				object result = dbAccess.RemoteStandardSQL.GetScalarResult(
					"Select dbo.GetSysYear(@TranDate)",
					new Parameter[] { new Parameter("@TranDate", trandate) });


				if (Convert.IsDBNull(result) == true)
				{
					MessageBox.Show("No Financial Year defined for the date " + trandate.ToShortDateString() + " !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return 0;
				}
				else
				{
					return (int)result;
				}
			}
			catch
			{
				MessageBox.Show("Failed To Retrieve Financial Year From Database!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return 0;
			}
		}


		public static int GetPeriod(DBAccess dbAccess, DateTime trandate, out int SystemYear)
		{
			SystemYear = 0;

			try
			{
				string safeDate = BizFunctions.GetSafeDateString(trandate);
				string sqlCMD = string.Format("Select dbo.GetSysPeriod('{0}'), dbo.GetSysYear('{1}')", safeDate, safeDate);
				DataSet ds = dbAccess.RemoteStandardSQL.GetSQLResult(sqlCMD);
				if (ds != null && ds.Tables.Count > 0)
				{
					SystemYear = (int)ds.Tables[0].Rows[0][1];
					return (int)ds.Tables[0].Rows[0][0];
				}
				else
				{
					MessageBox.Show("No Financial Period defined for the date " + trandate.ToShortDateString() + " !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return 0;
				}
			}
			catch
			{
				MessageBox.Show("Failed To Retrieve Period From Database!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return 0;
			}
		}


		/// <summary>
		/// code for getting the period based on transaction date defined 
		/// used for non-standard period (e.g. period 1 is 26/12/2006 to 25/01/2007)
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		/// <param name="trandate">DateTime used to fetch corresponding period</param>
		/// <param name="usingPdTable">true if want to use the non-standard period, flase if use normal way of handling</param>
		public static int GetPeriod(DBAccess dbAccess, DateTime trandate, bool usingPdTable)
		{
			if (UseNewPeriodScheme(dbAccess))
			{
				try
				{
					object result = dbAccess.RemoteStandardSQL.GetScalarResult(
						"Select dbo.GetSysPeriod(@TranDate)",
						new Parameter[] { new Parameter("@TranDate", trandate) });


					if (Convert.IsDBNull(result) == true)
					{
						MessageBox.Show("No Financial Period defined for the date " + trandate.ToShortDateString() + " !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return 0;
					}
					else
					{
						return (int)result;
					}
				}
				catch
				{
					MessageBox.Show("Failed To Retrieve Period From Database!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return 0;
				}
			}
			else
			{
				if (usingPdTable)
				{
					if (Convert.IsDBNull(trandate))
						return 0;
					else
					{
						DataSet BizPD = dbAccess.ReadSQLTemp("BizPD", "SELECT * FROM pd" + Common.DEFAULT_SYSTEM_YEAR + " WHERE pdstart<='" + BizFunctions.GetSafeDateString(trandate) + "' AND pdend>='" + BizFunctions.GetSafeDateString(trandate) + "'");
						if (BizPD.Tables["BizPD"].Rows.Count != 0)
							return (int)BizPD.Tables["BizPD"].Rows[0]["period"];
						else
						{
							MessageBox.Show("No Period defined for the date " + trandate.ToShortDateString() + " !", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return 0;
						}
					}
				}
				else
					return BizAccounts.GetPeriod(dbAccess, trandate);
			}
		}


		public static bool UseNewPeriodScheme(DBAccess dbAccess)
		{
			try
			{
				object result = dbAccess.RemoteStandardSQL.GetScalarResult("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'pd' AND COLUMN_NAME='SysYear'", null);
				if (Convert.IsDBNull(result) == true)
					return false;

				int count = (int)result;


				return (count > 0);
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// code for direct translation of month to period 
		/// with adjustment based on the financial period start month
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		/// <param name="trandate">DateTime used to fetch corresponding period</param>
		public static int GetPeriod(DBAccess dbAccess, DateTime trandate)
		{
			if (UseNewPeriodScheme(dbAccess))
				return BizAccounts.GetPeriod(dbAccess, trandate, true);
			else
			{
				if (Convert.IsDBNull(trandate))
					return 0;
				else
				{
					DataSet BizSysDefault = dbAccess.ReadSQLTemp("SysDefault", "SELECT DefaultSystemPeriod FROM SysDefault");
					if (BizSysDefault.Tables["SysDefault"].Rows.Count != 0)
					{
						DataRow defaults = BizSysDefault.Tables["SysDefault"].Rows[0];
						return (trandate.Month - (int)defaults["DefaultSystemPeriod"] + 12) % 12 + 1;
					}
					else
					{
						MessageBox.Show("DefaultSystemPeriod not defined in SysDefault !", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return 0;
					}
				}
			}
		}


		public static bool IsPeriodLocked(DBAccess dbAccess, DateTime trandate)
		{
			try
			{
				object result = dbAccess.RemoteStandardSQL.GetScalarResult(
					"Select dbo.IsPeriodLocked(@TranDate)",
					new Parameter[] { new Parameter("@TranDate", trandate) });

				if (Convert.IsDBNull(result) == true)
				{
					MessageBox.Show("There is a database error when system tried to determine whether a Financial Period has been locked!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				else
				{
					return (bool)result;
				}
			}
			catch
			{
				MessageBox.Show("There is a database error when system tried to determine whether a Financial Period has been locked!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
	

		public static bool ValidateAccount(DBAccess dbAccess, string AccCode, string AnalysisCode, string AnalysisType)
		{
			try
			{
				object result = dbAccess.RemoteStandardSQL.GetScalarResult(
					"Select dbo.ValidateACM(@AccCode, @AnalysisCode, @AnalysisType)",
					new Parameter[] { new Parameter("@AccCode", AccCode), new Parameter("@AnalysisCode", AnalysisCode), new Parameter("@AnalysisType", AnalysisType) });

				if (Convert.IsDBNull(result) == true)
				{
					MessageBox.Show("There an error assessing the account!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				else
				{
					return (bool)result;
				}
			}
			catch
			{
				MessageBox.Show("There an error assessing the account!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}


		#endregion

		#region PostFinLgr
		
		#region PostFinLgr
		/// <summary>
		/// Function to post sub-ledgers to the General Ledger used in FAV
		/// </summary>
		/// <param name="dbAccess">The DBAccess object containing GLD, CSH, ARD and APD</param>
		public static void PostFinLgr(DBAccess dbAccess)
		{
			PostFinLgr(dbAccess,"");
		}

		/// <summary>
		/// Function to post sub-ledgers to the General Ledger used in FAV
		/// </summary>
		/// <param name="dbAccess">The DBAccess object containing GLD, CSH, ARD and APD</param>
		/// <param name="extraGrouping">Other parameters to be used in the GROUP BY clause
		/// By default will group by "accnum,oricur,ccnum"</param>
		public static void PostFinLgr(DBAccess dbAccess,string extraGrouping)
		{
			if(dbAccess.DataSet.Tables.Contains("gld") && dbAccess.DataSet.Tables.Contains("csh") && dbAccess.DataSet.Tables.Contains("ard") && dbAccess.DataSet.Tables.Contains("apd"))
			{
				BizFunctions.DeleteRow(dbAccess.DataSet.Tables["gld"],"lgr<>'GLD'");
				if(dbAccess.DataSet.Tables["csh"].Rows.Count!=0) PostGL(dbAccess, "CSH", extraGrouping);
				if(dbAccess.DataSet.Tables["ard"].Rows.Count!=0) PostGL(dbAccess, "ARD", extraGrouping);
				if(dbAccess.DataSet.Tables["apd"].Rows.Count!=0) PostGL(dbAccess, "APD", extraGrouping);
			}
			else
				MessageBox.Show("Dataset missing at least one of the following tables:\nGLD, CSH, ARD, APD", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		#endregion

		#region PostGL
		/// <summary>
		/// Method used to prepare the select statements to be passed into PostGLsum
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		/// <param name="tableName">DataTable Name</param>
		/// <param name="extraGrouping">Other parameters to be used in the GROUP BY clause
		/// By default will group by "accnum,oricur,ccnum"</param>
		private static void PostGL(DBAccess dbAccess, string tableName, string extraGrouping)
		{
			string sqlCommand = "";
			string selectClauseStart = "SELECT accnum,oricur,ccnum,SUM(oriamt) AS oriamt,"+
										"SUM(oriamt)*(CASE WHEN SUM(oriamt)<0 THEN -1 ELSE 0 END) AS oricredit,"+
										"SUM(oriamt)*(CASE WHEN SUM(oriamt)>0 THEN 1 ELSE 0 END) AS oridebit,";
			string selectClauseEnd = (extraGrouping.Trim() == String.Empty) ? "" : ","+extraGrouping;
			string groupByClause1 = "GROUP BY accnum,oricur,ccnum"+selectClauseEnd;
			string groupByClause2 = "GROUP BY oricur,ccnum"+selectClauseEnd;
			string groupByClause3 = "GROUP BY coy";
			string havingClause = "";

			switch(tableName)
			{
				case("CSH"):
					selectClauseStart += "SUM(postamt-exramt) AS postamt"+selectClauseEnd;
					havingClause = " HAVING SUM(oriamt) <> 0 OR SUM(postamt-exramt) <> 0";
					sqlCommand = selectClauseStart + " FROM [" + tableName + "] " + groupByClause1 + havingClause;
					PostGLsum(dbAccess, tableName, sqlCommand, extraGrouping);
					
					selectClauseStart = "SELECT coy,"+
						"SUM(exramt)*(CASE WHEN SUM(exramt)<0 THEN -1 ELSE 0 END) AS oridebit,"+
						"SUM(exramt)*(CASE WHEN SUM(exramt)>0 THEN 1 ELSE 0 END) AS oricredit,"+
						"SUM(exramt) AS oriamt,SUM(exramt) AS postamt";
					havingClause = " HAVING SUM(exramt)<> 0";
					sqlCommand = selectClauseStart + " FROM [" + tableName + "] " + groupByClause3 + havingClause;
					PostGLsum(dbAccess, tableName, sqlCommand, "", GetDefaultAccount(dbAccess, "EXR1"), GetDefaultCurrency(dbAccess));
					break;
				case("ARD"):
					selectClauseStart += "SUM(postamt+exramt) AS postamt"+selectClauseEnd;
					havingClause = " HAVING SUM(oriamt) <> 0 OR SUM(postamt+exramt) <> 0";
					sqlCommand = selectClauseStart + " FROM [" + tableName + "] " + groupByClause1 + havingClause;
					PostGLsum(dbAccess, tableName, sqlCommand, extraGrouping);

					selectClauseStart = "SELECT coy,"+
						"SUM(exramt)*(CASE WHEN SUM(exramt)>0 THEN 1 ELSE 0 END) AS oridebit,"+
						"SUM(exramt)*(CASE WHEN SUM(exramt)<0 THEN -1 ELSE 0 END) AS oricredit,"+
						"SUM(-exramt) AS oriamt,SUM(-exramt) AS postamt";
					havingClause = " HAVING SUM(-exramt) <> 0";
					sqlCommand = selectClauseStart + " FROM [" + tableName + "] " + groupByClause3 + havingClause;			
					PostGLsum(dbAccess, tableName, sqlCommand, "", GetDefaultAccount(dbAccess, "EXR2"), GetDefaultCurrency(dbAccess));
					break;
				case("APD"):
					selectClauseStart += "SUM(postamt+exramt) AS postamt"+selectClauseEnd;
					havingClause = " HAVING SUM(oriamt) <> 0 OR SUM(postamt+exramt) <> 0";
					sqlCommand = selectClauseStart + " FROM [" + tableName + "] " + groupByClause1 + havingClause;
					PostGLsum(dbAccess, tableName, sqlCommand, extraGrouping);

					selectClauseStart = "SELECT coy,"+
						"SUM(exramt)*(CASE WHEN SUM(exramt)>0 THEN 1 ELSE 0 END) AS oridebit,"+
						"SUM(exramt)*(CASE WHEN SUM(exramt)<0 THEN -1 ELSE 0 END) AS oricredit,"+
						"SUM(-exramt) AS oriamt,SUM(-exramt) AS postamt";
					havingClause = " HAVING SUM(-exramt) != 0";
					sqlCommand = selectClauseStart + " FROM [" + tableName + "] " + groupByClause3 + havingClause;		
					PostGLsum(dbAccess, tableName, sqlCommand, "", GetDefaultAccount(dbAccess, "EXR3"), GetDefaultCurrency(dbAccess));
					break;			
			}
		}

		#endregion

		#endregion

		#region PostGLD
		/// <summary>
		/// Function used by accounting-related modules to post their individual Sub Ledgers to the General Ledger
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		/// <param name="subLedgers">Sub-Ledgers to be posted to the General Ledger (i.e. csh, ard, apd, siv1, piv1)</param>
		/// <param name="headerTableName">Name of the header page of the current module (i.e. sivh, recp, pivh, payt)</param>
		public static void PostGLD(DBAccess dbAccess, string subLedgers, string headerTableName)
		{
			PostGLD(dbAccess, subLedgers, headerTableName, "");
		}

		/// <summary>
		/// Function used by accounting-related modules to post their individual Sub Ledgers to the General Ledger
		/// </summary>
		/// <param name="dbAccess">DBAccess object</param>
		/// <param name="subLedgers">Sub-Ledgers to be posted to the General Ledger (i.e. csh, ard, apd, siv1, piv1)</param>
		/// <param name="headerTableName">Name of the header page of the current module (i.e. sivh, recp, pivh, payt)</param>
		/// <param name="extraGrouping">Other parameters to be used in the GROUP BY clause. 
		/// By default will group by "accnum,oricur,ccnum"</param>
		public static void PostGLD(DBAccess dbAccess, string subLedgers, string headerTableName, string extraGrouping)
		{
			try
			{
				DataTable gld	= dbAccess.DataSet.Tables["gld"];
				BizFunctions.DeleteAllRows(gld);

				DataRow header = dbAccess.DataSet.Tables[headerTableName].Rows[0];

				string sqlCommand = null;
				string selectClause = null;
				string selectClauseEnd = (extraGrouping.Trim() == String.Empty) ? "" : ","+extraGrouping;
				string groupByClause1 = " GROUP BY accnum,oricur,ccnum"+selectClauseEnd;
				string groupByClause2 = " GROUP BY ccnum"+selectClauseEnd;
				//string groupByClause3 = " GROUP BY accnum";
				string groupByClause3 = " GROUP BY coy";
				string havingClause = null;

				string columnName = null;
				string defaultAccount = null;
				
				string defaultCurrency1 = GetDefaultCurrency(dbAccess);
				string defaultCurrency = "'" + defaultCurrency1 + "' AS oricur,";
				
				string delimStr = " ,/*"; 
				char [] delimiter = delimStr.ToCharArray();
				string [] split = subLedgers.Split(delimiter);

				foreach(string s in split)
				{
					if(dbAccess.DataSet.Tables.Contains(s))
					{
						#region ARD/APD/SIV1/PIV1 posting to GLD

						selectClause = "SELECT accnum,oricur,ccnum,";
						
						switch(s.ToUpper())
						{
							case "ARD":
								selectClause = selectClause + "SUM(oricredit) AS oricredit,SUM(oridebit) AS oridebit,SUM(oriamt) AS oriamt,SUM(postamt+exramt) AS postamt" + selectClauseEnd;
								havingClause = " HAVING SUM(oriamt)<>0 OR SUM(postamt+exramt)<>0";
								break;
							case "APD":
								selectClause = selectClause + "SUM(oricredit) AS oricredit,SUM(oridebit) AS oridebit,SUM(oriamt) AS oriamt,SUM(postamt+exramt) AS postamt" + selectClauseEnd;
								havingClause = " HAVING SUM(oriamt)<>0 OR SUM(postamt+exramt)<> 0";
								break;
							case "CSH":
								selectClause = selectClause + "SUM(oricredit) AS oricredit,SUM(oridebit) AS oridebit,SUM(oriamt) AS oriamt,SUM(postamt-exramt) AS postamt" + selectClauseEnd;
								havingClause = " HAVING SUM(oriamt)<>0 OR SUM(postamt-exramt)<>0";
								break;
							default:
								selectClause = selectClause + "SUM(oriamt)*(CASE WHEN SUM(oriamt)>0 THEN 1 ELSE 0 END) AS oridebit,"+
									"SUM(oriamt)*(CASE WHEN SUM(oriamt)<0 THEN -1 ELSE 0 END) AS oricredit,"+
									"SUM(oriamt) AS oriamt,SUM(postamt) AS postamt" + selectClauseEnd;
								havingClause = " HAVING SUM(oriamt)<>0 OR SUM(postamt)<>0";
								break;
						}
						
						sqlCommand = selectClause + " FROM [" + s + "]" + groupByClause1 + havingClause;
						PostGLsum(dbAccess, s, sqlCommand, extraGrouping);

						#endregion

						#region [1] Exchange Gain Loss (ARD/APD/CSH); [2] GST (SIV1/PIV1); [3] Cost Of Goods Sold (SIV1)
						switch(s.ToUpper())
						{
								#region ARD -> Exramt -> GLD [default account = EXR2]

							case "ARD":
								columnName = "-exramt";
								selectClause = "SELECT coy," +
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")>0 THEN 1 ELSE 0 END) AS oridebit,"+
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")<0 THEN -1 ELSE 0 END) AS oricredit,"+
									"SUM(" + columnName + ") AS oriamt, SUM(" + columnName + ") AS postamt"; // + selectClauseEnd;
								havingClause = " HAVING SUM(" + columnName + ")<>0";
//								sqlCommand = selectClause + " FROM [" + s + "]" + groupByClause2 + havingClause;
								sqlCommand = selectClause + " FROM [" + s + "]" + groupByClause3 + havingClause;
								PostGLsum(dbAccess, s, sqlCommand, String.Empty, GetDefaultAccount(dbAccess,"EXR2"), defaultCurrency1);
								break;
								
								#endregion

								#region APD -> Exramt -> GLD [default account = EXR3]

							case "APD":
								columnName = "-exramt";
								selectClause = "SELECT coy," +
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")>0 THEN 1 ELSE 0 END) AS oridebit,"+
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")<0 THEN -1 ELSE 0 END) AS oricredit,"+
									"SUM(" + columnName + ") AS oriamt, SUM(" + columnName + ") AS postamt"; // + selectClauseEnd;
								havingClause = " HAVING SUM(" + columnName + ")<>0";
//								sqlCommand = selectClause + " FROM [" + s + "]" + groupByClause2 + havingClause;
								sqlCommand = selectClause + " FROM [" + s + "]" + groupByClause3 + havingClause;
                                PostGLsum(dbAccess, s, sqlCommand, String.Empty, GetDefaultAccount(dbAccess, "EXR3"), defaultCurrency1);
								break;

								#endregion

								#region CSH -> Exramt -> GLD [default account = EXR1]

							case "CSH":
								columnName = "exramt";
								selectClause = "SELECT coy," +
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")>0 THEN 1 ELSE 0 END) AS oridebit,"+
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")<0 THEN -1 ELSE 0 END) AS oricredit,"+
									"SUM(" + columnName + ") AS oriamt, SUM(" + columnName + ") AS postamt"; // + selectClauseEnd;
								havingClause = " HAVING SUM(" + columnName + ")<>0";
//								sqlCommand = selectClause + " FROM [" + s + "] " + groupByClause2 + havingClause;
								sqlCommand = selectClause + " FROM [" + s + "] " + groupByClause3 + havingClause;
                                PostGLsum(dbAccess, s, sqlCommand, String.Empty, GetDefaultAccount(dbAccess, "EXR1"), defaultCurrency1);
								break;

								#endregion

								#region SIV1 -> Gstamt,Cosamt -> GLD [default account = GST2,COS,MAT]

							case "SIV1":
								columnName = "gstamt";
								selectClause = "SELECT coy," +
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")>0 THEN 1 ELSE 0 END) AS oridebit,"+
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")<0 THEN -1 ELSE 0 END) AS oricredit,"+
									"SUM(" + columnName + ") AS oriamt, SUM(" + columnName + ") AS postamt"; // + selectClauseEnd;
								havingClause = " HAVING SUM(" + columnName + ")<>0";
//								sqlCommand = selectClause + " FROM [" + s + "] " + groupByClause2 + havingClause;
								sqlCommand = selectClause + " FROM [" + s + "] " + groupByClause3 + havingClause;
                                PostGLsum(dbAccess, s, sqlCommand, String.Empty, GetDefaultAccount(dbAccess, "GST2"), defaultCurrency1);
	
								columnName = "cosamt";
								selectClause = "SELECT coy," +
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")>0 THEN 1 ELSE 0 END) AS oridebit,"+
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")<0 THEN -1 ELSE 0 END) AS oricredit,"+
									"SUM(" + columnName + ") AS oriamt, SUM(" + columnName + ") AS postamt"; // + selectClauseEnd;
								havingClause = " HAVING SUM(" + columnName + ")<>0";
								sqlCommand = selectClause + " FROM [" + s + "] " + groupByClause3 + havingClause;
                                PostGLsum(dbAccess, s, sqlCommand, String.Empty, GetDefaultAccount(dbAccess, "COS"), defaultCurrency1);
	
								columnName = "-cosamt";
								selectClause = "SELECT coy," +
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")>0 THEN 1 ELSE 0 END) AS oridebit,"+
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")<0 THEN -1 ELSE 0 END) AS oricredit,"+
									"SUM(" + columnName + ") AS oriamt, SUM(" + columnName + ") AS postamt"; // + selectClauseEnd;
								havingClause = " HAVING SUM(" + columnName + ")<>0";
								sqlCommand = selectClause + " FROM [" + s + "] " + groupByClause3 + havingClause;
                                PostGLsum(dbAccess, s, sqlCommand, String.Empty, GetDefaultAccount(dbAccess, "MAT"), defaultCurrency1);
								break;

								#endregion

								#region PIV1 -> Gstamt -> GLD [default account = GST1]

							case "PIV1":
								columnName = "gstamt";
								selectClause = "SELECT coy," +
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")>0 THEN 1 ELSE 0 END) AS oridebit,"+
									"SUM(" + columnName + ")*(CASE WHEN SUM(" + columnName + ")<0 THEN -1 ELSE 0 END) AS oricredit,"+
									"SUM(" + columnName + ") AS oriamt, SUM(" + columnName + ") AS postamt"; // + selectClauseEnd;
								havingClause = " HAVING SUM(" + columnName + ")<>0";
//								sqlCommand = selectClause + " FROM [" + s + "] " + groupByClause2 + " " + havingClause;
								sqlCommand = selectClause + " FROM [" + s + "] " + groupByClause3 + " " + havingClause;
                                PostGLsum(dbAccess, s, sqlCommand, String.Empty, GetDefaultAccount(dbAccess, "GST1"), defaultCurrency1);
								break;

								#endregion

						}
						#endregion
					}
				}
			
				decimal roundingDifference = 0;
				foreach (DataRow dr in gld.Rows)
				{
					if(dr.RowState != DataRowState.Deleted)
					{
						roundingDifference += (decimal)dr["postamt"];
					}
				}
				if(roundingDifference != 0)
				{
					DataRow gld_dr = gld.Rows.Add(new object [] {});
					gld_dr["accnum"] = BizAccounts.GetDefaultAccount(dbAccess,"ROUND");
					if (-roundingDifference>=0)
					{
						gld_dr["oricredit"] = 0;
						gld_dr["oridebit"] = -roundingDifference;
					}
					else
					{
						gld_dr["oricredit"] = roundingDifference;
						gld_dr["oridebit"] = 0;
					}
					gld_dr["oriamt"] = -roundingDifference;
					gld_dr["postamt"] = -roundingDifference;
					gld_dr["lgr"] = "GLD";
//					BizVerify.AppendErrMsg(dbAccess.DataSet,dbAccess.DataSet.Tables[HeadStr].Rows[0]["refnum"].ToString(),"InRound","Rounding Error/"+roundDiff.ToString());
				}
	
				foreach (DataRow dr in gld.Rows)
				{
					if(dr.RowState != DataRowState.Deleted)
					{
						BizFunctions.UpdateDataRow(header, dr);
						if (dr["detail"].ToString().Trim() == String.Empty) 
							dr["detail"] = header["detail"];
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message+"\n"+ex.StackTrace, "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion

		#region PostGLsum
		/// <summary>
		/// Post from Sub-Ledger DataTables to the General Ledger DataTable
		/// </summary>
		/// <param name="dbAccess">The DBAccess object containing GLD, CSH, ARD, APD, SIV1 and PIV1</param>
		/// <param name="tableName">Name of Sub-Ledger database to post to GLD</param>
		/// <param name="sqlcommand">SQL Command. In this case a Select statement</param>
		/// <param name="groupByClauseEnd">Group By Clause in addition to accnum,oricur,ccnum</param>
		private static void PostGLsum(DBAccess dbAccess, string tableName, string sqlcommand, string groupByClauseEnd)
		{
			PostGLsum(dbAccess, tableName, sqlcommand, groupByClauseEnd, "", "");
		}
		
		private static void PostGLsum(DBAccess dbAccess, string tableName, string sqlcommand, string groupByClauseEnd, string accnum, string oricur)
		{
			try
			{
				DataTable gld = dbAccess.DataSet.Tables["gld"];
				
				DataTable GldTmp = BizFunctions.ExecuteQuery(dbAccess.DataSet, sqlcommand);
				
				if (!GldTmp.Columns.Contains("lgr"))
					GldTmp.Columns.Add("lgr");
				foreach (DataRow dr in GldTmp.Rows)
				{
					dr["lgr"] = tableName.ToUpper();
				}		
				
				foreach(DataRow dr in GldTmp.Rows)
				{
					if(dr.RowState != DataRowState.Deleted)
					{
						DataRow dr_gld = gld.Rows.Add(new object [] {});
						if(accnum.Trim() == String.Empty && oricur.Trim() == String.Empty)
						{
							BizFunctions.UpdateDataRow(dr, dr_gld, "accnum/oricur/ccnum");
						}
						else
						{
							dr_gld["accnum"] = accnum;
							dr_gld["oricur"] = oricur;
						}
						BizFunctions.UpdateDataRow(dr, dr_gld, groupByClauseEnd);
						BizFunctions.UpdateDataRow(dr, dr_gld, "oricredit/oridebit/oriamt/postamt/lgr");
					}
				}
			}			
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message+"\n"+ex.StackTrace, "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		
		#endregion

		#region PostSubLgr (WARNING: Not used anymore! and only written for ARD)

		public static void PostSubLgr(DBAccess dbAccess, string fromTable, string subLedgerTable, string headerRow)
		{
			PostSubLgr(dbAccess, fromTable, subLedgerTable, headerRow, "");
		}
		
		public static void PostSubLgr(DBAccess dbAccess, string fromTable, string subLedgerTable, string headerRow, string extraGrouping)
		{
			if (!dbAccess.DataSet.Tables.Contains(subLedgerTable) || !dbAccess.DataSet.Tables.Contains(fromTable) || !dbAccess.DataSet.Tables.Contains(headerRow))
			{
				MessageBox.Show(fromTable+" or "+subLedgerTable+" or "+headerRow+" does not exist in the DataSet!", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else if (subLedgerTable.ToUpper() != "ARD" && subLedgerTable.ToUpper() != "APD" && subLedgerTable.ToUpper() != "CSH")
			{
				MessageBox.Show(subLedgerTable+" is not a Sub-Ledger!", "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				try
				{
					BizFunctions.DeleteAllRows(dbAccess.DataSet.Tables[subLedgerTable]);

					string selectClause = null;
					string selectEnd = (extraGrouping.Trim() == String.Empty) ? "" : ","+extraGrouping;
					string groupByClause = "GROUP BY ccnum"+selectEnd;
					string havingClause = null;
					string sqlcommand = null;
					string updateColumnsFrmHeader = null;
					string updateColumnsFrmDetail = null;

					switch(fromTable.ToUpper())
					{
						case "SIV1":
							selectClause = "SELECT ccnum,SUM(-oriamt-origstamt) AS oriamt,SUM(-postamt-gstamt) AS postamt,SUM(-gstamt) AS gstamt,"+
								"SUM((-oriamt-origstamt)*(case when (oriamt)>0 then 1 else 0 end)) AS oricredit,"+
								"SUM((-oriamt-origstamt)*(case when (oriamt)>0 then 0 else 1 end)) AS oridebit"+selectEnd;
							havingClause = " HAVING SUM(-oriamt-origstamt) !=0 OR SUM(-postamt-gstamt) !=0 OR SUM(-gstamt) !=0";
							break;
					}
					
					sqlcommand = selectClause + " FROM ["+fromTable+"] " + groupByClause + havingClause;
					DataTable dt = BizFunctions.ExecuteQuery(dbAccess.DataSet, sqlcommand);

					switch(subLedgerTable.ToUpper())
					{
						case "ARD":
							updateColumnsFrmHeader = "arnum/docunum/accnum/oricur/invnum";
							updateColumnsFrmDetail = "ccnum/oriamt/postamt/gstamt/oricredit/oridebit";
							break;
					}

					DataTable subLgrDT = dbAccess.DataSet.Tables[subLedgerTable];
					DataRow headerDR = dbAccess.DataSet.Tables[headerRow].Rows[0];
					
					foreach(DataRow dr in dt.Rows)
					{
						if(dr.RowState != DataRowState.Deleted)
						{
							DataRow new_dr = subLgrDT.Rows.Add(new object [] {});
							BizFunctions.UpdateDataRow(headerDR, new_dr);
							BizFunctions.UpdateDataRow(headerDR, new_dr, updateColumnsFrmHeader);
							BizFunctions.UpdateDataRow(dr, new_dr, updateColumnsFrmDetail);
							BizFunctions.UpdateDataRow(dr, new_dr, extraGrouping);
						}
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message+"\n"+ex.StackTrace, "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		#endregion

		#region RefreshAccType
		/// <summary>
		/// Populate AccType variable in gld database
		/// </summary>
		/// <param name="dbAccess">The DBAccess object containing GLD and ACM</param>
		/// <param name="tableName">Name of the database with to do refresh to pull acctype</param>
		public static void RefreshAccType(DBAccess dbAccess, string tableName)
		{
			if(tableName.Trim() != String.Empty)
			{
				try
				{
					DataTable dt = dbAccess.DataSet.Tables[tableName];
					foreach (DataRow dr in dt.Rows)
					{
						if (dr.RowState!=DataRowState.Deleted)
						{
                            string tbName = "acm";
                            if (Convert.ToBoolean(Common.DEFAULT_SYSTEM_TABLEINFO["ACM"]))
                                tbName = "acm" + Common.DEFAULT_SYSTEM_YEAR;

                            DataSet ds = dbAccess.ReadSQLTemp("BizAcmTmp", "SELECT accnum,accname,acctype FROM " + tbName + " WHERE accnum='" + dr["accnum"].ToString().Trim() + "'");
							if(ds.Tables["BizAcmTmp"].Rows.Count!=0) 
							{
								dr["acctype"] = ds.Tables["BizAcmTmp"].Rows[0]["acctype"];
							}
						}					
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message+"\n"+ex.StackTrace, "Bizpoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		#endregion
	}
}
