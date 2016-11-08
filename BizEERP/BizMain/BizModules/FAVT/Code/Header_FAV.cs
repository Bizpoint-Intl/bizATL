/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Header_FAV.cs
 *	Description:    Header Page (GENJ)
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 *
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
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
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;

namespace DeskRight.FAV
{
	public class Header_FAV
	{
		protected DBAccess	dbAccess		= null;
		protected DataSet	dataSet			= null;
		protected Hashtable	formsCollection	= null;
		protected string documentKey		= null;

		public Header_FAV(DBAccess dbAccess, Hashtable formsCollection, string DocumentKey)
		{
			this.dbAccess			= dbAccess;
			this.dataSet			= this.dbAccess.DataSet;
			this.formsCollection	= formsCollection;
			this.documentKey		= DocumentKey;
		}

		#region Opening Balance Check Box Clicked

		protected void chk_Opbal_Click(object sender, System.EventArgs e)
		{
			CheckBox cb_opbal = (CheckBox)BizXmlReader.CurrentInstance.GetControl((this.formsCollection["header"] as Form).Name, "genj_opbal");
			ComboBox cb_opbaltype = (ComboBox)BizXmlReader.CurrentInstance.GetControl((this.formsCollection["header"] as Form).Name, "genj_opbaltype");
			Button btn_update = (Button)BizXmlReader.CurrentInstance.GetControl((this.formsCollection["header"] as Form).Name, "Btn_Update");

			if (cb_opbal.Checked)
			{
				cb_opbaltype.Enabled = true;
				btn_update.Enabled = true;
			}
			else
			{
				cb_opbaltype.Enabled = false;
				btn_update.Enabled = false;
			}
		}

		#endregion

		#region Extract Opening Balances

		protected void btn_Update_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("All data previouly entered will be lost\nContinue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DataRow genj = this.dataSet.Tables["genj"].Rows[0];
				DataTable gld	= this.dataSet.Tables["gld"];
				DataTable csh	= this.dataSet.Tables["csh"];
				DataTable ard	= this.dataSet.Tables["ard"];
				DataTable apd	= this.dataSet.Tables["apd"];

				BizFunctions.DeleteAllRows(gld);
				BizFunctions.DeleteAllRows(csh);
				BizFunctions.DeleteAllRows(ard);
				BizFunctions.DeleteAllRows(apd);

				string selectClause = "";
				string fromClause = "";
				string whereClause = "";
				string groupbyClause = "";
				string havingClause = "";
				string orderbyClause = "";
				string lastYear = Convert.ToString(Int32.Parse(Common.DEFAULT_SYSTEM_YEAR)-1);
				string tablename = "";

				switch(genj["opbaltype"].ToString())
				{
					case "1":
						tablename = "csh";
						selectClause = "SELECT accnum,oricur,SUM(oriamt) AS oriamt,SUM(postamt) AS postamt,SUM(bankamt) AS bankamt ";
						fromClause = "FROM "+tablename+lastYear+" ";
						groupbyClause = "GROUP BY accnum,oricur ";
						havingClause = "HAVING SUM(oriamt)<>0 ";
						orderbyClause = "ORDER BY accnum,oricur ";
						break;
					case "2":
						tablename = "ard";
						selectClause = "SELECT arnum,invnum,accnum,ccnum,MAX(empnum) AS empnum,MAX(oricur) AS oricur,'GLD' AS lgr,SUM(oriamt) AS oriamt,SUM(postamt) AS postamt,MIN(trandate) AS trandate ";
						fromClause = "FROM "+tablename+lastYear+" ";
						groupbyClause = "GROUP BY arnum,invnum,accnum,ccnum ";
						havingClause = "HAVING SUM(oriamt)<>0 ";
						orderbyClause = "ORDER BY arnum,invnum,accnum,ccnum ";
						break;
					case "3":
						tablename = "apd";
						selectClause = "SELECT apnum,invnum,accnum,ccnum,MAX(oricur) AS oricur,'GLD' AS lgr,SUM(oriamt) AS oriamt,SUM(postamt) AS postamt,MIN(trandate) AS trandate ";
						fromClause = "FROM "+tablename+lastYear+" ";
						groupbyClause = "GROUP BY apnum,invnum,accnum,ccnum ";
						havingClause = "HAVING SUM(oriamt)<>0 ";
						orderbyClause = "ORDER BY apnum,invnum,accnum,ccnum ";
						break;
					default:
						tablename = "gld";
						//updated select statement so that exrate is calcuated in sql statement - Ivan 121107
						selectClause = "select accnum, oricur, ccnum, lgr, oriamt, postamt, sum(postamt/oriamt) as exrate from " +
							"(SELECT accnum,oricur,ccnum,'GLD' AS lgr,SUM(oriamt) AS oriamt,SUM(postamt) AS postamt " +
							"FROM " + tablename + lastYear + " WHERE acctype='5' OR acctype='6' OR acctype='7' OR acctype='8' " +
							"OR acctype='9' OR acctype='10' OR acctype='11' OR acctype='12' OR acctype='13' OR acctype='14' " +
							"GROUP BY accnum,oricur,ccnum HAVING SUM(oriamt)<>0)as temp1 group by accnum, oricur, ccnum, lgr, oriamt, " +
							"postamt";
						fromClause = "";
						whereClause = "";
						groupbyClause = "";
						havingClause = "";
						orderbyClause = "";
						//selectClause = "SELECT accnum,oricur,ccnum,'GLD' AS lgr,SUM(oriamt) AS oriamt,SUM(postamt) AS postamt ";
						//fromClause = "FROM "+tablename+lastYear+" ";
						////whereClause = "WHERE acctype='A' OR acctype='L' ";
						//whereClause = "WHERE  acctype='5' OR acctype='6' OR acctype='7' OR acctype='8' OR acctype='9' OR acctype='10' OR acctype='11' OR acctype='12' OR acctype='13' OR acctype='14' ";
						//groupbyClause = "GROUP BY accnum,oricur,ccnum ";
						//havingClause = "HAVING SUM(oriamt)<>0 ";
						//orderbyClause = "ORDER BY accnum,oricur,ccnum ";
						break;
				}
                string mainclause = selectClause+fromClause+whereClause+groupbyClause+havingClause+orderbyClause;
				if (dbAccess.DataSet.Tables.Contains("ob_db"))
					dbAccess.DataSet.Tables.Remove("ob_db");
                this.dbAccess.ReadSQL("ob_db", mainclause);
				if(dbAccess.DataSet.Tables.Contains("ob_db"))
				{
					DataTable ob_db = dbAccess.DataSet.Tables["ob_db"];

					CopyColumns.CopyDataRow(this.dataSet,"ob_db",tablename);
				}

				//if oriamt is positive, copy an instance in debit column of the row, 
				//if not copy into credit column - Ivan 121107
				if (dbAccess.DataSet.Tables[tablename].Select().Length > 0)
					foreach (DataRow dr in dbAccess.DataSet.Tables[tablename].Rows)
						if(dr.RowState != DataRowState.Deleted)
							if ((decimal)dr["oriamt"] > 0)
								dr["oridebit"] = dr["oriamt"];
							else
								dr["oricredit"] = -(decimal)dr["oriamt"];
			}
		}

		#endregion
	}
}
