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
	/// Class containing Miscellaneous Functions related to Data Entry 
	/// and used to help developer during coding process
	/// </summary>
	public class BizFunctions
	{

		#region DeleteAllRows
		/// <summary>
		/// Function to delete all rows in a DataTable
		/// </summary>
		/// <param name="dataTable">DataTable whose rows are to be deleted</param>
		public static void DeleteAllRows(DataTable dataTable)
		{
			try
			{
				for (int i = dataTable.Rows.Count-1; i >= 0; i--)
				{
					if (dataTable.Rows[i].RowState != DataRowState.Deleted)
					{
						dataTable.Rows[i].Delete();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message+"\n"+ex.StackTrace, "Bizpoint International", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion

		#region DeleteRow
		/// <summary>
		/// Delete rows from a DataTable having which fulfill a certain condition
		/// </summary>
		/// <param name="dataTable">DataTable whose rows are to be deleted</param>
		/// <param name="selectWhereClause">Where clause used to delete rows fulfilling the condition</param>
		public static void DeleteRow(DataTable dataTable, string selectWhereClause) 
		{
			try 
			{		
				if (selectWhereClause != null)
				{
					DataRow[] dataRows = dataTable.Select(selectWhereClause);
		
					foreach (DataRow dataRow in dataRows)
					{
						if (dataRow.RowState != DataRowState.Deleted)
							dataRow.Delete();
					}
				}
				else
					DeleteAllRows(dataTable);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message+"\n"+ex.StackTrace, "Bizpoint International", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion

		#region ExecuteQuery
		/// <summary>
		/// Used to run an SQL Command on DataTables in local DataSet
		/// </summary>
		/// <param name="ds">DataSet containing the DataTables</param>
		/// <param name="sqlCommand">SQL command (generally it will be a select statement)</param>
		public static DataTable ExecuteQuery(DataSet ds, string sqlCommand)
		{
			try
			{
				DataView dv = DsCommand.Execute(sqlCommand, ds);
				return dv.Table;
			}
			catch(Exception eName)
			{
				MessageBox.Show(eName.Message+"\n"+eName.Source+"\n Select Command:"+sqlCommand+"\n"+eName.StackTrace,"Bizpoint International",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
				return null;
			}
		}

		#endregion

		#region F2Condition
		/// <summary>
		/// Used for 'wildcard' searching. Returns a string to be used for the e.Condition 
		/// (i.e. where clause used in the select statement)
		/// </summary>
		/// <param name="fieldsToSearch">fields in the F2 query to search</param>
		/// <param name="textboxValue">the textfield value to search</param>
		public static string F2Condition(string fieldsToSearch, string textboxValue)
		{
			string delimStr = " ,/*"; 
			char [] delimiter = delimStr.ToCharArray();
			string [] split = fieldsToSearch.Split(delimiter);
            
            string spChar = "|";
            char[] specialChar = spChar.ToCharArray();
			
            string result = "";
			int i = 0;

            textboxValue = textboxValue.Replace("'", "''");
			foreach (string s in split) 
			{
                if (i == 0)
                {
                    if (textboxValue.EndsWith("|"))
                    {
                        result = s + " LIKE '" + textboxValue.TrimEnd(specialChar) + "%'";
                    }
                    else if (textboxValue.StartsWith("|"))
                    {
                        result = s + " LIKE '%" + textboxValue.TrimEnd(specialChar) + "'";
                    }
                    else
                    {
                        result = s + " LIKE '%" + textboxValue.TrimEnd(specialChar) + "%'";
                    }
                    i++;
                }
                else
                {
                    if (textboxValue.EndsWith("|"))
                    {
                        result = result + " OR " + s + " LIKE '" + textboxValue.TrimEnd(specialChar) + "%'";
                    }
                    else if (textboxValue.StartsWith("|"))
                    {
                        result = result + " OR " + s + " LIKE '%" + textboxValue.TrimEnd(specialChar) + "'";
                    }
                    else
                    {
                        result = result + " OR " + s + " LIKE '%" + textboxValue.TrimEnd(specialChar) + "%'";
                    }
                }
			}

			return "("+result+")";
		}
		
		#endregion

		#region FindStdCost
		///<summary>
		///Returns the stdocst based on product code and period passed in from the Material Standard Cost table [MATS]
		///</summary>
		///<param name="dbAccess">DBAccess object</param>
		///<param name="matnum">Stock Code to search inside the Std Cost Table</param>
		///<param name="pd">Period</param>
		public static decimal FindStdCost(DBAccess dbAccess,string matnum,string pd)
		{
			DataSet dsTmp = dbAccess.ReadSQLTemp("BizMats","SELECT * FROM mats"+Common.DEFAULT_SYSTEM_YEAR+" WHERE matnum='"+matnum.Trim().Replace("'","")+"'");

            try
            {
                if (dsTmp.Tables["BizMats"].Rows.Count != 0 && pd != String.Empty)
                {
                    decimal returnValue = 0;
                    string vpd = "std" + pd.Trim();
                    DataRow BizMats = dsTmp.Tables["BizMats"].Rows[0];
                    if (BizMats.Table.Columns.Contains(vpd))
                        returnValue = (decimal)BizMats[vpd];
                    else
                        MessageBox.Show(pd.Trim() + " is not a valid period in standard cost table", "Bizpoint International", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return returnValue;
                }
                else
                {
                    return 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "BizPoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
		}
		
		#endregion

		#region GetSafeDateString
		/// <summary>
		/// This returns a safe format of the date which sql will accept no matter what culture/region settings the server/client is on
		/// Mainly used for writing to sql
		/// </summary>
		/// <param name="dateTime">DateTime to convert to safe format for sql</param>
		/// <returns>YYYYMMDD</returns>
		public static string GetSafeDateString(DateTime dateTime)
		{
			return dateTime.Year.ToString() + dateTime.Month.ToString("00") + dateTime.Day.ToString("00");
		}
		
		#endregion

		#region GetSafeTimeString
		/// <summary>
		/// This is mainly used for Select queries
		/// </summary>
		/// <param name="dateTime">DateTime to convert to safetimeformat</param>
		/// <returns>YYYY-MM-DDT00:00:00</returns>
		public static string GetSafeTimeString(DateTime dateTime)
		{
			return GetStandardDateString(dateTime) + "T" + dateTime.Hour.ToString("00") + ":" + dateTime.Minute.ToString("00") + ":" + dateTime.Second.ToString("00");
		}
		
		#endregion

		#region GetStandardDateString
		/// <summary>
		/// This returns a date in the yyyy-mm-dd format
		/// </summary>
		/// <param name="dateTime">DateTime to convert to the standard format</param>
		/// <returns>YYYY-MM-DD</returns>
		public static string GetStandardDateString(DateTime dateTime)
		{
			return dateTime.Year.ToString() + "-" + dateTime.Month.ToString("00") + "-" + dateTime.Day.ToString("00");
		}
		
		#endregion

		#region GetTrandate
		/// <summary>
		/// Reutrn the Trandate in shortdate format
		/// </summary>
		/// <param name="formName">Name of Form</param>
		/// <param name="controlName">The datetime control name declared in xml file</param>
		/// <param name="dataRow">DataRow containing the datetime object</param>
		/// <returns>ShortDate format</returns>
		public static string GetTrandate(string formName, string controlName, DataRow dataRow)
		{
			BizDateTimePicker trandate = (BizDateTimePicker)BizXmlReader.CurrentInstance.GetControl(formName, controlName);
			dataRow["trandate"] = System.DateTime.Today;
			trandate.Value = dataRow["trandate"];
			return ((DateTime)dataRow["trandate"]).ToShortDateString();
		}
		public static string GetTrandate()
		{
			return System.DateTime.Now.Date.ToShortDateString();
		}
		
		#endregion

		#region IsEmpty
		/// <summary>
		/// Check if an object is null/empty
		/// </summary>
		/// <param name="o">object to check</param>
		/// <returns>true if is null/empty, false if not null/empty</returns>
		public static bool IsEmpty(object o)
		{
			if (o == null)
				return true;
			else
				return (o.ToString().Trim() == "");
		}

		#endregion

		#region MailToProtocol
		/// <summary>
		/// emulates the "mailto:" function in webbrowsers
		/// </summary>
		public static void MailTo(string[] email_To)
		{
			MailTo(email_To,null);
		}

		public static void MailTo(string[] email_To, string email_Subject)
		{
			MailTo(email_To,null,null,email_Subject,null);
		}

		public static void MailTo(string[] email_To, string[] email_Cc, string[] email_Bcc, string email_Subject, string email_Body)
		{
			//mailto:mtscf@microsoft.com?CC=davide@davide.it&BCC=pippo@pippo.com&subject=Feedback&body=The InetSDK Site Is Superlative
	
			try
			{
				ExecMailCommand(FormatMailToCommand(email_To,email_Cc,email_Bcc,email_Subject,email_Body));
			}
			catch(Exception err)
			{
				throw new ApplicationException("Failed to execute mailto protocol.", err);
			}
		}

		#region FormatMailToCommand
		/// <summary>
		/// 
		/// </summary>
		/// <param name="p_To">null if not used</param>
		/// <param name="p_Cc">null if not used</param>
		/// <param name="p_Bcc">null if not used</param>
		/// <param name="p_Subject">null if not used</param>
		/// <param name="p_Body">null if not used</param>
		/// <returns></returns>
		private static string FormatMailToCommand(string[] p_To, string[] p_Cc, string[] p_Bcc, string p_Subject, string p_Body)
		{
			string l_To = FormatEMailAddress(p_To);
			string l_CC = FormatEMailAddress(p_Cc);
			string l_Bcc = FormatEMailAddress(p_Bcc);
				
			string l_Command = "mailto:";
			if (l_To!=null)
				l_Command+=l_To;

			System.Collections.ArrayList l_Parameters = new System.Collections.ArrayList();

			if (l_CC!=null)
				l_Parameters.Add("CC="+l_CC);
			if (l_Bcc!=null)
				l_Parameters.Add("BCC="+l_Bcc);
			if (p_Subject!=null)
				l_Parameters.Add("subject="+p_Subject);
			if (p_Body!=null)
				l_Parameters.Add("body="+p_Body);

			if (l_Parameters.Count>0)
			{
				string[] l_tmp = new string[l_Parameters.Count];
				l_Parameters.CopyTo(l_tmp,0);
				l_Command+="?";
				l_Command+=string.Join("&",l_tmp);
			}

			return l_Command;
		}
		#endregion

		#region FormatEMailAddress
		/// <summary>
		/// Joins multiple emails with the ; seperator
		/// </summary>
		/// <param name="p_EMails">array of email addresses</param>
		private static string FormatEMailAddress(string[] p_EMails)
		{
			if (p_EMails==null || p_EMails.Length <= 0)
				return null;
			else
				return string.Join(";",p_EMails);
		}
		#endregion

		#region ExecMailCommand
		/// <summary>
		/// Load the default email client with the values passed in
		/// </summary>
		/// <param name="p_Command"></param>
		private static void ExecMailCommand(string p_Command)
		{
			System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(p_Command);
			p.UseShellExecute = true;
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo = p;
			process.Start();
		}
		#endregion

		#endregion

		#region Round
		/// <summary>
		/// Method for rounding a number to a specified number of deicmal places using logical 
		/// Arithmetic rounding instead of using .NET's Math.Round which implements Banker 
        /// Rounding. By default method will round to 2 decimal places</summary>
		/// <param name="dblNumber">Decimal to be rounded to 2 decimal places</param>
		public static decimal Round(decimal numberToRound) 
		{ 
			return Round(numberToRound, 2);
		}

		/// <summary>
		/// Method for rounding a number to a specified number of deicmal places using logical 
		/// Arithmetic rounding instead of using .NET's Math.Round which implements Banker 
        /// Rounding. By default method will round to 2 decimal places</summary>
		/// <param name="dblNumber">Decimal to be rounded to n decimal places</param>
		/// <param name="decimalPlaces">Number of decimal places to round</param>
		public static decimal Round(decimal numberToRound, int decimalPlaces) 
		{ 
			if(Convert.IsDBNull(decimalPlaces))
				decimalPlaces = 2;

			if(Convert.IsDBNull(numberToRound) || numberToRound == 0)
				return 0;
			else
				return Decimal.Round((decimal) System.Data.SqlTypes.SqlDecimal.Round((System.Data.SqlTypes.SqlDecimal) numberToRound, decimalPlaces ), decimalPlaces);
		}
		#endregion

		#region SetCoyForPrinting
		/// <summary>
		/// Method pulling coy into local DataSet and retrieve the company logo defined 
        /// in app.config - "PrintingImageFile" into column "coyimage" in the DataTable coy</summary>
		public static void SetCoyForPrinting(DBAccess dbAccess)
		{
			SetCoyForPrinting(dbAccess, "");
		}

		public static void SetCoyForPrinting(DBAccess dbAccess, string whereClause)
		{
			string sqlCommand = "SELECT * FROM coy";
			if(!IsEmpty(whereClause))
				sqlCommand += " WHERE "+whereClause;
			dbAccess.ReadSQL("coy",sqlCommand);

            string imagepath = ConfigurationManager.AppSettings.Get("ImagePath") + ConfigurationManager.AppSettings.Get("PrintingImageFile");
			try
			{
				FileStream fs = new FileStream(imagepath, FileMode.Open);	// create a file stream
				BinaryReader br = new BinaryReader(fs);						// create binary reader

				DataTable coy = dbAccess.DataSet.Tables["coy"];
				if(coy.Columns.Contains("coyimage"))
					coy.Columns.Remove("coyimage");
				coy.Columns.Add("coyimage", System.Type.GetType("System.Byte[]"));
				coy.Rows[0]["coyimage"] = br.ReadBytes((int)br.BaseStream.Length);

				fs.Close();
				br.Close();
			}
			catch (Exception ex)
			{
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "BizPoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}

		#endregion

		#region SetDisplayTotal
        #region old version
        /// <summary>
		/// Set a Control which is used to display a numeric value to have 2 deicmal places only
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="total">decimal amount to use to set display</param>
		public static void SetDisplayTotal(object sender, decimal total)
		{
			SetDisplayTotal(sender,total,"txt_Document_Total");
		}

		/// <summary>
		/// Set a Control which is used to display a numeric value to have 2 deicmal places only
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="controlName">control to set</param>
		/// <param name="total">decimal amount to use to set display</param>
		public static void SetDisplayTotal(object sender, decimal total, string controlName)
		{
			if (controlName != String.Empty)
			{
				try
				{
					TextBox tbtotal = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name,controlName) as TextBox;
					if(total!=0)
					{
						int decptindex = total.ToString().IndexOf(".",0);
						if (decptindex <= 0)
							tbtotal.Text = total.ToString().Trim()+".00";
						else
							tbtotal.Text = total.ToString().Trim().Substring(0,decptindex+3);
					}
					else
						tbtotal.Text = "0.00";
				}
				catch
				{}
			}
			else
				MessageBox.Show("ControlName : '"+"' is not valid", "Bizpoint International", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        public static void SetDisplayTotal(object sender, string labelnumber, decimal total)
        {
            SetDisplayTotal(sender, labelnumber, total, null);
        }

        public static void SetDisplayTotal(object sender, string labelnumber, decimal total, string labelname)
        {
            if (labelnumber != String.Empty)
            {
                try
                {
                    Label lbltotalcaption = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "lbl_Document_Total" + labelnumber + "_Caption") as Label;
                    Label lbltotal = BizXmlReader.CurrentInstance.GetControl((sender as Control).Parent.Name, "lbl_Document_Total" + labelnumber) as Label;
                    if (total != 0)
                    {
                        int decptindex = total.ToString().IndexOf(".", 0);
                        if (decptindex <= 0)
                            lbltotal.Text = total.ToString().Trim() + ".00";
                        else
                            lbltotal.Text = total.ToString().Trim().Substring(0, decptindex + 3);
                    }
                    else
                        lbltotal.Text = "0.00";

                    
                    if (labelname != null)
                    {
                        lbltotalcaption.Text = labelname;
                    }
                    lbltotalcaption.Visible = true;
                    lbltotal.Visible = true;
                }
                catch
                { }
            }
            else
                MessageBox.Show("ControlName : '" + "' is not valid", "Bizpoint International", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region ToggleSign
        /// <summary>
		/// Changes values of columns in a datatable from +ve to -ve and vice versa.
		/// </summary>
		/// <param name="tableToToggle">datatable whose columns are to be toggled</param>
		/// <param name="fieldsToToggle">column names in table of type decimal or int</param>
		public static void ToggleSign(DataTable tableToToggle, string fieldsToToggle)
		{		
			foreach (DataRow dr in tableToToggle.Rows)
			{
				if(dr.RowState != DataRowState.Deleted)
				{
					string delimStr = " ,/*"; 
					char [] delimiter = delimStr.ToCharArray();
					string [] split = fieldsToToggle.Split(delimiter);
					foreach (string s in split) 
					{
						if(tableToToggle.Columns.Contains(s))
						{
							switch(tableToToggle.Columns[s].DataType.ToString())
							{
								case "System.Decimal":
									if(Convert.IsDBNull(dr[s]))
										dr[s] = 0;
									dr[s] = (-1)*(decimal)dr[s];
									break;
								case "System.Int32":
									if(Convert.IsDBNull(dr[s]))
										dr[s] = 0;
									dr[s] = (-1)*(int)dr[s];
									break;
							}
						}
					}					
				}
			}
		}

		#endregion

		#region UpdateDataRow
		/// <summary>
		/// Updates detail datarow with the header datarow's refnum,trandate,period,flag,status,user,modified,coy
		/// </summary>
		/// <param name="dataRowFrom">Header DataRow</param>
		/// <param name="dataRowTo">Detail DataRow</param>
		public static void UpdateDataRow(DataRow dataRowFrom, DataRow dataRowTo)
		{
			UpdateDataRow(dataRowFrom, dataRowTo, "refnum/trandate/period/flag/status/user/modified/coy");
		}

		/// <summary>
		/// Updates detail datarow with the header datarow
		/// </summary>
		/// <param name="dataRowFrom">Header DataRow</param>
		/// <param name="dataRowTo">Detail DataRow</param>
		/// <param name="fieldsToUpdate">List if fields to update from header to detail</param>
		public static void UpdateDataRow(DataRow dataRowFrom, DataRow dataRowTo, string fieldsToUpdate)
		{
			if(fieldsToUpdate.Trim() != String.Empty)
			{
				try
				{
					string delimStr = ",/*"; 
					char [] delimiter = delimStr.ToCharArray();
					string [] split = fieldsToUpdate.Split(delimiter);
					foreach (string s in split) 
					{
						string str = s.Trim();
						if(dataRowFrom.Table.Columns.Contains(str) && dataRowTo.Table.Columns.Contains(str))
						{
							if(dataRowFrom.Table.Columns[str].DataType == dataRowTo.Table.Columns[str].DataType)
								dataRowTo[str] = dataRowFrom[str];
							else
								MessageBox.Show("Column "+str+" dataType mismatch in the 2 datarows","Bizpoint International", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
						{
							MessageBox.Show("Column "+str+" does not exist in the dataRow","Bizpoint International", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message+"\n"+ex.StackTrace,"BizPoint International Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		#endregion

	}
}
