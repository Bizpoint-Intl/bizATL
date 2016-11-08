using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

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
using BizRAD.BizAccounts;

namespace ATL.WAC
{
    public partial class WAC_Document : Form
    {
		DBAccess dbAccess = new DBAccess();
		bool blnFlag = false;
		int intPeriod = 0;
		protected string company = "";

        public WAC_Document(string moduleName, bool blnNew,int intPrd)
        {
            InitializeComponent();
			blnFlag = blnNew;
			intPeriod = intPrd;
			company = moduleName;
        }

		private void WAC_Document_Load(object sender, EventArgs e)
		{
			if (blnFlag)
			{
				dtClosing.Value = DateTime.Now;
				dtTrandate.Value = DateTime.Now;
				txtPeriod.Text = BizAccounts.GetPeriod(this.dbAccess, dtClosing.Value).ToString();
			}
			else
			{
				txtPeriod.Text = intPeriod.ToString();

				if (!allowCompute())
				{
					dtClosing.Enabled = false;
					dtTrandate.Enabled = false;
					txtRemarks.Enabled = false;
					cmdCompute.Enabled = false;
					cmdConfirm.Enabled = false;
				}

				LoadFromWAC();
				LoadDetail();
			}
		}

		private void LoadDetail()
		{
			string strSQL = "SELECT " +
					" trandate," +
					" closingdate," +
					" remarks" +
			" FROM " +
					" wac" + Common.DEFAULT_SYSTEM_YEAR +
					" where " +
						" period=" + intPeriod +
						" and year =" + Common.DEFAULT_SYSTEM_YEAR +" and isnull(coy,'')='"+company+"'";

			DataSet dsTotal = this.dbAccess.ReadSQLTemp("WAC", strSQL);
			DataTable dtTotal = dsTotal.Tables["WAC"];
			if (dtTotal.Rows.Count > 0)
			{
				txtRemarks.Text = dtTotal.Rows[0]["remarks"].ToString();
				dtClosing.Value = (DateTime)dtTotal.Rows[0]["closingdate"];
				dtTrandate.Value = (DateTime)dtTotal.Rows[0]["trandate"];
			}
		}

		private void LoadFromWAC()
		{
			string strSQL = "SELECT matnum, matname, openingqty, inqty," +
					" inamt, openingcost, wac, openingamt, closingamt " +
				" FROM " +
					" wac" + Common.DEFAULT_SYSTEM_YEAR +
				" where " +
					" period=" + intPeriod +
					" and year =" + Common.DEFAULT_SYSTEM_YEAR + " and isnull(coy,'')='"+company+"'" + 
				" ORDER BY " +
					" matname";

			DataSet DS = dbAccess.ReadSQLTemp("wac", strSQL);
			BindingSource BindSourceWAC = new BindingSource();
			BindSourceWAC.DataSource = DS.Tables["wac"];

			dgWAC.DataSource = BindSourceWAC;
			dgWAC.Refresh();

			TotalFromWAC();
		}

		private void TotalFromWAC()
		{
			string strSQL = "SELECT " +
					" sum(isnull(OPENINGQTY,0)) opnqty, " +
					" sum(isnull(INQTY,0)) inqty," +
					" sum(isnull(INAMT,0)) inamt," +
					" sum((isnull(OPENINGQTY,0) * isnull(OPENINGCOST,0))) openamt," +
					" sum(((isnull(OPENINGQTY ,0)* isnull(OPENINGCOST,0)) + isnull(INAMT,0))) AS ClosingAmt " +
			" FROM " +
					" wac" + Common.DEFAULT_SYSTEM_YEAR +
					" where " +
						" period=" + intPeriod +
						" and year =" + Common.DEFAULT_SYSTEM_YEAR + " and isnull(coy,'') = '"+company+"'";

			DataSet dsTotal = this.dbAccess.ReadSQLTemp("WAC", strSQL);
			DataTable dtTotal = dsTotal.Tables["WAC"];

			txtOpeningQty.Text = dtTotal.Rows[0]["opnqty"].ToString();
			txtInQty.Text = dtTotal.Rows[0]["inqty"].ToString();
			txtInAmt.Text = dtTotal.Rows[0]["inamt"].ToString();
			txtOpenAmt.Text = dtTotal.Rows[0]["openamt"].ToString();
			txtClosingAmt.Text = dtTotal.Rows[0]["ClosingAmt"].ToString();
		}

		private void dtClosing_ValueChanged(object sender, EventArgs e)
		{
			txtPeriod.Text = BizAccounts.GetPeriod(this.dbAccess, dtClosing.Value).ToString();
		}

		private void cmdClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cmdCompute_Click(object sender, EventArgs e)
		{
			if (!allowCompute())
			{
				MessageBox.Show("Unable to process this Period, Please select the correct period.",
					"System Message");
				return;
			}

			if (MessageBox.Show("Compute WAC for Period " + txtPeriod.Text + "?", "System Message",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
                string strSQL = "";
                if (Convert.ToInt32(txtPeriod.Text) == 1)
                {
                    strSQL = "select matnum,matname,openingqty,inqty,inamt,openingcost, " +
               "	case when (openingqty + inqty)=0 THEN openingcost   " +
               "			ELSE  ((openingqty* openingcost) + inamt) / (openingqty + inqty)  END AS wac, " +
               "		openingqty* openingcost as openingamt,  " +
               "	case when (openingqty + inqty)=0 THEN openingcost*closingqty   " +
               "			ELSE  (((openingqty* openingcost) + inamt) / (openingqty + inqty))*closingqty  END AS closingamt  " +
               "		from  " +
               "		(  " +
               "select matnum,max(matname) as matname,SUM(openingqty) as openingqty,  " +
               "		SUM(inqty) as inqty, SUM(inamt) as inamt, MAX(openingcost) as  openingcost,  " +
               "		sum(wac) as wac, sum(openingamt) as openingamt,sum(closingqty) as closingqty   " +
               "	from  " +
               "	(  " +
               "		select distinct matnum,matname, 0 as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty  " +
               "			from matm " +
               "		UNION ALL " +
               "		select matnum,'' as matname, SUM(qty) as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty  " +
               "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
               "			where period=0 group by matnum " +
               "		UNION ALL " +
               "		select matnum,'' as matname, 0 as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt,SUM(qty) as closingqty  " +
               "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
               "			where trandate<(convert(datetime,'" + dtClosing.Value.ToString("yyyyMMdd") + "')+1) group by matnum " +
               "		UNION ALL " +
               "		select matnum,'' as matname, 0 as openingqty, SUM(qty) as inqty, SUM((stdcost*qty)-discamt) as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0  as closingqty  " +
               "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
               "			where dbo.GetSysPeriod(trandate)=" + Convert.ToString(Convert.ToInt32(txtPeriod.Text)) + " and (flag='GRN' or flag='GRNV' or flag='GRNS' or flag='GRNP') " +
               "				and trandate<(convert(datetime,'" + dtClosing.Value.ToString("yyyyMMdd") + "')+1) group by matnum " +
               "		UNION ALL " +
               "		select distinct matnum,'' as matname, 0 as openingqty, 0 as inqty, 0 as inamt,  " +
               "			isnull(std" + Convert.ToString(Convert.ToInt32(txtPeriod.Text) - 1) + ",0.00) as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty   " +
               "			from mats" + Common.DEFAULT_SYSTEM_YEAR +
               "	) result1 " +
               "	group by matnum " +
               ") result2 " +
               "where matnum <>'' and matnum is not null " +
               "order by matnum ";
                }
                else
                {
                    strSQL = "select matnum,matname,openingqty,inqty,inamt,openingcost, " +
                   "	case when (openingqty + inqty)=0 THEN openingcost   " +
                   "			ELSE  ((openingqty* openingcost) + inamt) / (openingqty + inqty)  END AS wac, " +
                   "		openingqty* openingcost as openingamt,  " +
                   "	case when (openingqty + inqty)=0 THEN openingcost*closingqty   " +
                   "			ELSE  (((openingqty* openingcost) + inamt) / (openingqty + inqty))*closingqty  END AS closingamt  " +
                   "		from  " +
                   "		(  " +
                   "select matnum,max(matname) as matname,SUM(openingqty) as openingqty,  " +
                   "		SUM(inqty) as inqty, SUM(inamt) as inamt, MAX(openingcost) as  openingcost,  " +
                   "		sum(wac) as wac, sum(openingamt) as openingamt,sum(closingqty) as closingqty   " +
                   "	from  " +
                   "	(  " +
                   "		select distinct matnum,matname, 0 as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty  " +
                   "			from matm " +
                   "		UNION ALL " +
                   "		select matnum,'' as matname, SUM(qty) as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty  " +
                   "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                   "			where dbo.GetSysPeriod(trandate)<" + Convert.ToString(Convert.ToInt32(txtPeriod.Text)) + " group by matnum " +
                   "		UNION ALL " +
                   "		select matnum,'' as matname, 0 as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt,SUM(qty) as closingqty  " +
                   "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                   "			where trandate<(convert(datetime,'" + dtClosing.Value.ToString("yyyyMMdd") + "')+1) group by matnum " +
                   "		UNION ALL " +
                   "		select matnum,'' as matname, 0 as openingqty, SUM(qty) as inqty, SUM((stdcost*qty)-discamt) as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0  as closingqty  " +
                   "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                   "			where dbo.GetSysPeriod(trandate)=" + Convert.ToString(Convert.ToInt32(txtPeriod.Text)) + " and (flag='GRN' or flag='GRNV' or flag='GRNS' or flag='GRNP') " +
                   "				and trandate<(convert(datetime,'" + dtClosing.Value.ToString("yyyyMMdd") + "')+1) group by matnum " +
                   "		UNION ALL " +
                   "		select distinct matnum,'' as matname, 0 as openingqty, 0 as inqty, 0 as inamt,  " +
                   "			isnull(std" + Convert.ToString(Convert.ToInt32(txtPeriod.Text) - 1) + ",0.00) as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty   " +
                   "			from mats" + Common.DEFAULT_SYSTEM_YEAR +
                   "	) result1 " +
                   "	group by matnum " +
                   ") result2 " +
                   "where matnum <>'' and matnum is not null " +
                   "order by matnum ";
                }

				DataSet DS = dbAccess.ReadSQLTemp("wac", strSQL);
				BindingSource BindSourceWAC = new BindingSource();
				BindSourceWAC.DataSource = DS.Tables["wac"];

				dgWAC.DataSource = BindSourceWAC;
				dgWAC.Refresh();

                ShowTotalInCompute(DS.Tables["wac"]);

				cmdConfirm.Enabled = true;
			}
		}

		private void ShowTotalInCompute(DataTable twac)
		{
//            string strSQL = "SELECT " +
//                    " sum(isnull(OPENINGQTY,0)) opnqty, " +
//                    " sum(isnull(INQTY,0)) inqty," +
//                    " sum(isnull(INAMT,0)) inamt," +
//                    " sum((isnull(OPENINGQTY,0) * isnull(OPENINGCOST,0))) openamt," +
//                    " sum(((isnull(OPENINGQTY ,0)* isnull(OPENINGCOST,0)) + isnull(INAMT,0))) AS ClosingAmt " +
//            " FROM " +
//                    " ( " +
//                    " SELECT     matnum, matname," +
//                        " (select isnull(sum(qty),0) from (SELECT xmwt.flag as flag,xmwt.period as period,xmwt.coy as coy,xmwt.matnum as matnum,xmwt.uom as uom, " +
//" isnull(SUM(xmwt.qty),0) " +
//" AS qty FROM    mwt" + Common.DEFAULT_SYSTEM_YEAR + " xmwt " +
//" left outer join matm xmatm on xmwt.matnum = xmatm.matnum and xmatm.uom = xmwt.uom " +
//"group by xmwt.matnum,xmwt.uom, xmatm.uom,xmwt.period,xmwt.coy,xmwt.flag) xste " +
//                            " where period<" + txtPeriod.Text + " and matnum=matm.matnum  and isnull(coy,'') = '') as OpeningQty, " +
//                        " (select isnull(sum(qty),0) from (SELECT xmwt.flag as flag,xmwt.period as period,xmwt.coy as coy,xmwt.matnum as matnum,xmwt.uom as uom, " +
//" isnull(SUM(xmwt.qty),0) " +
//" AS qty FROM    mwt" + Common.DEFAULT_SYSTEM_YEAR + " xmwt  " +
//" left outer join matm xmatm on xmwt.matnum = xmatm.matnum and xmatm.uom = xmwt.uom " +
//"group by xmwt.matnum,xmwt.uom, xmatm.uom,xmwt.period,xmwt.coy,xmwt.flag) xste" +
//                            " where period = " + txtPeriod.Text + " and flag='GRN' and matnum=matm.matnum  and isnull(coy,'') = '') as INQty," +
//                        " (select isnull(sum(postamt),0) from (select flag,period, coy, matnum,uom,qty, (qty*price) - discamt as postamt from" +
//"(SELECT xmwt.flag,xmwt.period as period,xmwt.coy as coy, xmwt.matnum as matnum,xmwt.uom as uom, " +
//"isnull(SUM(xmwt.qty),0) " +
//" AS qty,isnull(MAX(xmwt.price),0) AS price,xmwt.discamt FROM    mwt" + Common.DEFAULT_SYSTEM_YEAR + "  xmwt  " +
//"left outer join matm xmatm on xmwt.matnum = xmatm.matnum and xmatm.uom = xmwt.uom " +
//"group by xmwt.matnum,xmwt.uom,xmatm.uom,xmwt.price,xmwt.period,xmwt.coy,xmwt.flag,xmwt.discamt)x)xste3" +
//                            " where period = " + txtPeriod.Text + " and flag='GRN' and matnum=matm.matnum  and isnull(coy,'') = '') as INAmt," +
//                        " isnull((select std" + Convert.ToString(Convert.ToInt32(txtPeriod.Text) - 1) + " from mats" + Common.DEFAULT_SYSTEM_YEAR + " where matnum=matm.matnum  and isnull(coy,'') = ''),0) as OpeningCost" +
//                    " FROM         matm " +
//                    " ) COSTING ";

//            DataSet dsTotal = this.dbAccess.ReadSQLTemp("WAC", strSQL);
//            DataTable dtTotal = dsTotal.Tables["WAC"];

            // replace by spencer
           
            txtOpeningQty.Text = twac.Compute("Sum(openingqty)", "").ToString();
            txtInQty.Text = twac.Compute("Sum(inqty)", "").ToString();
            txtInAmt.Text = twac.Compute("Sum(inamt)", "").ToString();
            txtOpenAmt.Text = twac.Compute("Sum(openingamt)", "").ToString();
            txtClosingAmt.Text = twac.Compute("Sum(closingamt)", "").ToString();
            
		}

		private bool allowCompute()
		{
			string strSQL = "select isnull(max(period),0) period from wac" + Common.DEFAULT_SYSTEM_YEAR +" WHERE isnull(coy,'') = '"+company+"'";

			DataSet dsWAC = this.dbAccess.ReadSQLTemp("WAC", strSQL);
			DataTable dtWAC = dsWAC.Tables["WAC"];

			int intP=(int)dtWAC.Rows[0]["period"] + 1;

			if (txtPeriod.Text != intP.ToString())
				if (txtPeriod.Text == dtWAC.Rows[0]["period"].ToString())
					return true;
				else
					return false;
			else
				return true;
		}

		private void cmdConfirm_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure do you want to confirm this Record?",
				"System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				//fill mats by new matnum
				FillNewMatNum();
				InsertToWACTable();
				UpdateMATSData();
				MessageBox.Show("Successfully Processed!", "System Message");
			}
		}

		private void FillNewMatNum()
		{
			string strSQL = "select * from matm where matnum not in(select matnum from mats" + Common.DEFAULT_SYSTEM_YEAR + ")";
			DataSet dsMATS = this.dbAccess.ReadSQLTemp("MATS", strSQL);
			DataTable dtMATS = dsMATS.Tables["MATS"];

			if (this.dbAccess.DataSet.Tables.Contains("MATS" + Common.DEFAULT_SYSTEM_YEAR))
				this.dbAccess.DataSet.Tables.Remove("MATS" + Common.DEFAULT_SYSTEM_YEAR);

			dbAccess.ReadSQL("MATS", "SELECT * FROM MATS" + Common.DEFAULT_SYSTEM_YEAR + " WHERE ID=-1");

			DataTable mats = dbAccess.DataSet.Tables["MATS"];
			DataRow drMATS;

			foreach (DataRow dr in dtMATS.Rows)
			{
				drMATS = mats.NewRow();
				drMATS["matnum"] = dr["matnum"];
				mats.Rows.Add(drMATS);
			}

			dbAccess.SetID("MATS", "MATS" + Common.DEFAULT_SYSTEM_YEAR);
			mats.TableName = "MATS" + Common.DEFAULT_SYSTEM_YEAR;
			dbAccess.Update(new DataTable[] { mats });
			mats.AcceptChanges();
		}

		private void InsertToWACTable()
		{
			string strSQLDelete = "DELETE FROM WAC" + Common.DEFAULT_SYSTEM_YEAR +
				" WHERE PERIOD=" + txtPeriod.Text + " AND YEAR=" + Common.DEFAULT_SYSTEM_YEAR + " and isnull(coy,'') = '"+company+"'";
			dbAccess.RemoteStandardSQL.ExecuteNonQuery(strSQLDelete);

            string strSQL="";
            if (Convert.ToInt32(txtPeriod.Text) == 1)
            {
                strSQL = "select  " +
                                 "'" + txtPeriod.Text + "'," +
                                "'" + txtRemarks.Text + "'," +
                                "'" + dtTrandate.Value.ToString("yyyy-MM-dd") + "'," +
                                "'" + dtClosing.Value.ToString("yyyy-MM-dd") + "'," +
                "matnum,matname,openingqty,inqty,inamt,openingcost, " +
                "	case when (openingqty + inqty)=0 THEN openingcost   " +
                "			ELSE  ((openingqty* openingcost) + inamt) / (openingqty + inqty)  END AS wac, " +
                "		openingqty* openingcost as openingamt,  " +
                "	case when (openingqty + inqty)=0 THEN openingcost*closingqty   " +
                "			ELSE  (((openingqty* openingcost) + inamt) / (openingqty + inqty))*closingqty  END AS closingamt,  " +
                                            "'" + Common.DEFAULT_SYSTEM_YEAR + "'," +
                                "'" + Common.DEFAULT_SYSTEM_USERNAME + "',GETDATE(), " +
                                "'" + company + "'" +
                "		from  " +
                "		(  " +
                "select matnum,max(matname) as matname,SUM(openingqty) as openingqty,  " +
                "		SUM(inqty) as inqty, SUM(inamt) as inamt, MAX(openingcost) as  openingcost,  " +
                "		sum(wac) as wac, sum(openingamt) as openingamt,sum(closingqty) as closingqty   " +
                "	from  " +
                "	(  " +
                "		select distinct matnum,matname, 0 as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty  " +
                "			from matm " +
                "		UNION ALL " +
                "		select matnum,'' as matname, SUM(qty) as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty  " +
                "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                "			where period=0 group by matnum " +
                "		UNION ALL " +
                "		select matnum,'' as matname, 0 as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt,SUM(qty) as closingqty  " +
                "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                "			where trandate<(convert(datetime,'" + dtClosing.Value.ToString("yyyyMMdd") + "')+1) group by matnum " +
                "		UNION ALL " +
                "		select matnum,'' as matname, 0 as openingqty, SUM(qty) as inqty, SUM((stdcost*qty)-discamt) as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0  as closingqty  " +
                "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                "			where dbo.GetSysPeriod(trandate)=" + Convert.ToString(Convert.ToInt32(txtPeriod.Text)) + " and (flag='GRN' or flag='GRNV' or flag='GRNS' or flag='GRNP') " +
                "				and trandate<(convert(datetime,'" + dtClosing.Value.ToString("yyyyMMdd") + "')+1) group by matnum " +
                "		UNION ALL " +
                "		select distinct matnum,'' as matname, 0 as openingqty, 0 as inqty, 0 as inamt,  " +
                "			isnull(std" + Convert.ToString(Convert.ToInt32(txtPeriod.Text) - 1) + ",0.00) as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty   " +
                "			from mats" + Common.DEFAULT_SYSTEM_YEAR +
                "	) result1 " +
                "	group by matnum " +
                ") result2 " +
                "where matnum <>'' and matnum is not null " +
                "order by matnum ";
            }
            else
            {
                strSQL = "select  " +
                                "'" + txtPeriod.Text + "'," +
                               "'" + txtRemarks.Text + "'," +
                               "'" + dtTrandate.Value.ToString("yyyy-MM-dd") + "'," +
                               "'" + dtClosing.Value.ToString("yyyy-MM-dd") + "'," +
               "matnum,matname,openingqty,inqty,inamt,openingcost, " +
               "	case when (openingqty + inqty)=0 THEN openingcost   " +
               "			ELSE  ((openingqty* openingcost) + inamt) / (openingqty + inqty)  END AS wac, " +
               "		openingqty* openingcost as openingamt,  " +
               "	case when (openingqty + inqty)=0 THEN openingcost*closingqty   " +
               "			ELSE  (((openingqty* openingcost) + inamt) / (openingqty + inqty))*closingqty  END AS closingamt,  " +
                                           "'" + Common.DEFAULT_SYSTEM_YEAR + "'," +
                               "'" + Common.DEFAULT_SYSTEM_USERNAME + "',GETDATE(), " +
                               "'" + company + "'" +
               "		from  " +
               "		(  " +
               "select matnum,max(matname) as matname,SUM(openingqty) as openingqty,  " +
               "		SUM(inqty) as inqty, SUM(inamt) as inamt, MAX(openingcost) as  openingcost,  " +
               "		sum(wac) as wac, sum(openingamt) as openingamt,sum(closingqty) as closingqty   " +
               "	from  " +
               "	(  " +
               "		select distinct matnum,matname, 0 as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty  " +
               "			from matm " +
               "		UNION ALL " +
               "		select matnum,'' as matname, SUM(qty) as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty  " +
               "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
               "			where dbo.GetSysPeriod(trandate)<" + Convert.ToString(Convert.ToInt32(txtPeriod.Text)) + " group by matnum " +
               "		UNION ALL " +
               "		select matnum,'' as matname, 0 as openingqty, 0 as inqty, 0.00 as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt,SUM(qty) as closingqty  " +
               "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
               "			where trandate<(convert(datetime,'" + dtClosing.Value.ToString("yyyyMMdd") + "')+1) group by matnum " +
               "		UNION ALL " +
               "		select matnum,'' as matname, 0 as openingqty, SUM(qty) as inqty, SUM((stdcost*qty)-discamt) as inamt, 0.00 as openingcost, 0.00 as wac, 0.00 as openingamt, 0  as closingqty  " +
               "			from mwt" + Common.DEFAULT_SYSTEM_YEAR +
               "			where dbo.GetSysPeriod(trandate)=" + Convert.ToString(Convert.ToInt32(txtPeriod.Text)) + " and (flag='GRN' or flag='GRNV' or flag='GRNS' or flag='GRNP') " +
               "				and trandate<(convert(datetime,'" + dtClosing.Value.ToString("yyyyMMdd") + "')+1) group by matnum " +
               "		UNION ALL " +
               "		select distinct matnum,'' as matname, 0 as openingqty, 0 as inqty, 0 as inamt,  " +
               "			isnull(std" + Convert.ToString(Convert.ToInt32(txtPeriod.Text) - 1) + ",0.00) as openingcost, 0.00 as wac, 0.00 as openingamt, 0 as closingqty   " +
               "			from mats" + Common.DEFAULT_SYSTEM_YEAR +
               "	) result1 " +
               "	group by matnum " +
               ") result2 " +
               "where matnum <>'' and matnum is not null " +
               "order by matnum ";
            }


			string strSQLInsert = "INSERT INTO WAC" + Common.DEFAULT_SYSTEM_YEAR +
					  " (period, remarks, trandate, closingdate, matnum, " +
					  " matname, openingqty, inqty, inamt, openingcost, wac, " +
					  " openingamt, closingamt, year, [user], created,coy) " +
				strSQL;

			dbAccess.RemoteStandardSQL.ExecuteNonQuery(strSQLInsert);
		}

		private void UpdateMATSData()
		{
			string strSQLUpate="update mats" + Common.DEFAULT_SYSTEM_YEAR +
					" set " +
					" lateststdcost=(SELECT TOP 1 WAC FROM WAC" + Common.DEFAULT_SYSTEM_YEAR  +
							" WHERE MATNUM=mats" + Common.DEFAULT_SYSTEM_YEAR + ".matnum " +
							" AND period=" + txtPeriod.Text +
							" AND YEAR=" + Common.DEFAULT_SYSTEM_YEAR + " and isnull(coy,'') = '"+company+"') ," +
					" std" + txtPeriod.Text + " = " +
							" (SELECT TOP 1 WAC FROM WAC" + Common.DEFAULT_SYSTEM_YEAR +
							" WHERE MATNUM=mats" + Common.DEFAULT_SYSTEM_YEAR + ".matnum " +
							" AND period=" + txtPeriod.Text +
							" AND YEAR=" + Common.DEFAULT_SYSTEM_YEAR + " and isnull(coy,'') = '"+company+"')";

			dbAccess.RemoteStandardSQL.ExecuteNonQuery(strSQLUpate);
		}
    }
}