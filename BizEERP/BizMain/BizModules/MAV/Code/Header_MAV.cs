/***********************************************************
 *	Copyright (C), 1988-2006, Bizpoint International Pte Ltd
 *	File name:		Header_MAV.cs
 *	Description:    Header Page (MAVH)
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * CWK				2006-08-14			Recreate the whole module according to specs.
 * CWK				2006-08-17			Import from excel, match to physical qty. Unmatched is appended to Mav1.
 * 
***********************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Data.OleDb;

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

namespace ATL.MAV
{
    public class Header_MAV
    {
        protected DBAccess dbAccess = null;
        protected DataSet dataSet = null;
        protected Hashtable formsCollection = null;
        protected string documentKey = null;

        protected string projectPath = null;
        protected string matnum_from = null;
        protected string matnum_to = null;
        protected string typnum_from = null;
        protected string typnum_to = null;
        protected string whnum_from = null;

        public Header_MAV(DBAccess dbAccess, Hashtable formsCollection, string DocumentKey)
        {
            this.dbAccess = dbAccess;
            this.dataSet = this.dbAccess.DataSet;
            this.formsCollection = formsCollection;
            this.documentKey = DocumentKey;
        }

        #region Material Transfer Check Box Clicked
        protected void chk_Transfer_Click(object sender, System.EventArgs e)
        {
            CheckBox cb_Transfertype = (CheckBox)BizXmlReader.CurrentInstance.GetControl((this.formsCollection["header"] as Form).Name, "mavh_transferopt");
            ComboBox cb_whfrtype = (ComboBox)BizXmlReader.CurrentInstance.GetControl((this.formsCollection["header"] as Form).Name, "mavh_whnumfrm");
            ComboBox cb_whtotype = (ComboBox)BizXmlReader.CurrentInstance.GetControl((this.formsCollection["header"] as Form).Name, "mavh_whnumto");

            if (cb_Transfertype.Checked)
            {
                cb_whfrtype.Enabled = true;
                cb_whtotype.Enabled = true;
            }
            else
            {
                cb_whfrtype.Enabled = false;
                cb_whtotype.Enabled = false;
            }
        }
        #endregion

        #region Update Clicked

        protected void btn_Update_Click(object sender, System.EventArgs e)
        {
            DataRow mavh = this.dbAccess.DataSet.Tables["mavh"].Rows[0];
            DataTable mav1 = this.dbAccess.DataSet.Tables["mav1"];
            decimal lineNo = 0;

            if (mavh["mavtype"].ToString() == "Stock Take")
            {
                #region Stock Take

                BizFunctions.DeleteAllRows(mav1);

                #region Yushu Commented-20100127
                //DON20071212_Due to DELETEALLROWS Function did not Clear the Error
                //foreach (DataRow dr in this.dbAccess.DataSet.Tables["mav1"].Rows)
                //{
                //    foreach (DataColumn dc in this.dbAccess.DataSet.Tables["mav1"].Columns)
                //    {
                //        dr.SetColumnError(dc.ColumnName, "");
                //    }
                //}
                #endregion

                #region Commented
                //string sqlCommand = "SELECT m.matnum,isnull(w.qty,0) bkqty FROM "
                //                                        + "(SELECT matnum FROM mwt" + Common.DEFAULT_SYSTEM_YEAR
                //                                        + " UNION "
                //                                        + "SELECT matnum FROM matm )m "
                //                                        + "LEFT JOIN "
                //                                        + "(SELECT matnum, isnull(sum(qty),0) qty FROM mwt" + Common.DEFAULT_SYSTEM_YEAR
                //                                        + " WHERE trandate<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' GROUP BY matnum)w "
                //                                        + "ON m.matnum=w.matnum "
                //                                        + "ORDER BY m.matnum";

                //YushuEdited20100716 - Extract only selected warehouse and site's qty in mwt
                #endregion

                string sqlCommand = "";

                #region Select from mwt
                sqlCommand = "SELECT matnum,Sum(qty) As qty,trandate,location,whnum From ("
                    + "SELECT mwt.matnum, mwt.qty As qty, mwt.trandate, mwt.location, mwt.whnum "
                    + "FROM mwt" + Common.DEFAULT_SYSTEM_YEAR + " As mwt) BizResult "
                    + "GROUP BY matnum, trandate, location, whnum";

                sqlCommand = "SELECT mwt.matnum, SUM(mwt.qty) AS bkqty FROM (" + sqlCommand + ") mwt LEFT OUTER JOIN matm ON matm.matnum=mwt.matnum";

                sqlCommand = sqlCommand + " WHERE mwt.location='" + mavh["sitenum"].ToString() + "' AND ISNULL(mwt.matnum,'')<>''  AND mwt.whnum='" + mavh["whnum"].ToString()
                    + "' AND CONVERT(datetime, CONVERT(nvarchar(20),mwt.trandate,112))<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' AND matm.flag='MATMSTK' ";
                sqlCommand = sqlCommand + "GROUP BY mwt.matnum ORDER BY mwt.matnum";
                #endregion

                DataSet dsTmp = this.dbAccess.ReadSQLTemp("mwtTmp", sqlCommand);
                DataRow mavRow = mav1.NewRow();

                foreach (DataRow dr in dsTmp.Tables["mwtTmp"].Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow new_dr = mav1.Rows.Add(new object[] { });
                        BizFunctions.UpdateDataRow(dr, new_dr, "matnum/bkqty");
                    }
                }

                #endregion
            }
            else if (mavh["mavtype"].ToString() == "Stock Adjustment")
            {
                #region Stock Adjustment

                if (MessageBox.Show("Confirm Generate Book Balance?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.projectPath = ConfigurationManager.AppSettings.Get("ProjectPath");
                    Form pt_selection = BizXmlReader.CurrentInstance.Load(this.projectPath + @"\MAV\Code\preview.xml", "preview", this, null) as Form;
                    pt_selection.FormBorderStyle = FormBorderStyle.FixedSingle; // Not resizable.

                    pt_selection.ShowDialog();
                }

                #endregion
            }
            else if (mavh["mavtype"].ToString() == "Opening Balance")
            {
                BizFunctions.DeleteAllRows(mav1);

                #region Stock Openning Sql - Copy  and Amended from View STKAGEALLIN1

                string sqlOpen = "select f.matnum,f.stkdate,f.[1],f.[2],f.[3],f.[4],f.[5],f.[6],f.[7],f.[8],"+
                                                    "CAST(f.agq AS Decimal(16,4)) as phyqty,w.wac,f.agq*w.wac as amt " +
                                                    "from "+
                                                    "("+
                                                    "select *,[1]+[2]+[3]+[4]+[5]+[6]+[7]+[8] as agq "+
                                                    "from "+
                                                    "("+
                                                    "select pr.matnum,pr.stkdate, "+
                                                    "case when([1] is null) then 0 else cast([1] as integer) end as [1],"+
                                                    "case when([2] is null) then 0 else cast([2] as integer) end as [2],"+
                                                    "case when([3] is null) then 0 else cast([3] as integer)end as [3],"+
                                                    "case when([4] is null) then 0 else cast([4] as integer)end as [4],"+
                                                    "case when([5] is null) then 0 else cast([5] as integer)end as [5],"+
                                                    "case when([6] is null) then 0 else cast([6] as integer) end as [6],"+
                                                    "case when([7] is null) then 0 else cast([7] as integer)end as [7],"+
                                                    "case when([8] is null) then 0 else cast( [8] as integer)end as [8] "+
                                                    "from "+
                                                    "("+
                                                    "select * from"+
                                                    "("+
                                                    "select oinv.*, abr.bracketnumber "+
                                                    "from"+
                                                    "("+
                                                    "select matnum,iq,agq,amq,agq+amq as balqty,stkdate,"+
                                                    "case when agq+amq>=iq then iq "+
                                                    "when (agq+amq<iq ) and (agq+amq>0) then (agq+amq) "+
                                                    "when (trec=recno) and (agq+amq<0) then (agq+amq) else 0 end as rmqty,"+
                                                    "trec,recno "+
                                                    "from"+
                                                    "("+
                                                    "select matnum,sum(qty) as iq,"+
                                                    "("+
                                                    "select isnull(sum(qty),0) "+
                                                    "from"+
                                                    "("+
                                                    "select matnum,"+
                                                    "case when stkdate is null then convert(datetime,convert(nvarchar(8),trandate,112)) "+
                                                    "else convert(datetime,convert(nvarchar(8),stkdate,112)) end as stkdate,"+
                                                    "qty "+
                                                    "from "+
                                                    "mwt" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where qty>0 "+
                                                    "and convert(datetime,convert(nvarchar(8),trandate,112)) <= '" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' " +
                                                    "and matnum=pstk.matnum "+
                                                    "and flag not in('tra','tri') "+
                                                    "and whnum<>'sitwh'"+
                                                    ") accstk "+
                                                    "where matnum=pstk.matnum "+
                                                    "and stkdate <= convert(datetime,convert(nvarchar(8),pstk.stkdate,112)) and qty>0 ) as agq, "+
                                                    "("+
                                                    "select isnull(sum(qty),0) "+
                                                    "from "+
                                                    "mwt" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where matnum=pstk.matnum "+
                                                    "and convert(datetime,convert(nvarchar(8),trandate,112)) <= '" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' " +
                                                    "and qty<0 "+
                                                    "and flag not in('tra','tri') "+
                                                    "and whnum<>'sitwh') as amq,stkdate, "+
                                                    "("+
                                                    "select count(*) "+
                                                    "from "+
                                                    "("+
                                                    "select "+
                                                    "convert(datetime,convert(nvarchar(8),stkdate,112)) as stkdate "+
                                                    "from "+
                                                    "("+
                                                    "select matnum,"+
                                                    "case when stkdate is null then convert(datetime,convert(nvarchar(8),trandate,112)) "+
                                                    "else convert(datetime,convert(nvarchar(8),stkdate,112)) end as stkdate, qty "+
                                                    "from mwt" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where qty>0 "+
                                                    "and convert(datetime,convert(nvarchar(8),trandate,112))<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' " +
                                                    "and matnum=pstk.matnum "+
                                                    "and flag not in('tra','tri') "+
                                                    "and whnum<>'sitwh'"+
                                                    ") asdate "+
                                                    "group by stkdate) dsdate) as trec,"+
                                                    "("+
                                                    "select count(*) "+
                                                    "from "+
                                                    "("+
                                                    "select stkdate "+
                                                    "from "+
                                                    "("+
                                                    "select matnum, "+
                                                    "case when stkdate is null then convert(datetime,convert(nvarchar(8),trandate,112)) "+
                                                    "else convert(datetime,convert(nvarchar(8),stkdate,112)) end as stkdate,qty "+
                                                    "from mwt" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where qty>0 and convert(datetime,convert(nvarchar(8),trandate,112))<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' " +
                                                    "and matnum=pstk.matnum "+
                                                    "and flag not in('tra','tri') "+
                                                    "and whnum<>'sitwh'"+
                                                    ") asdate "+
                                                    "where stkdate <= convert(datetime,convert(nvarchar(8),pstk.stkdate,112)) "+
                                                    "group by stkdate) dsdate ) as recno "+
                                                    "from "+
                                                    "("+
                                                    "select matnum,"+
                                                    "case when stkdate is null then convert(datetime,convert(nvarchar(8),trandate,112)) "+
                                                    "else convert(datetime,convert(nvarchar(8),stkdate,112)) end as stkdate, qty "+
                                                    "from mwt" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where qty>0 "+
                                                    "and flag not in('tra','tri') "+
                                                    "and whnum<>'sitwh'"+
                                                    ") pstk "+
                                                    "where stkdate<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' " +
                                                    "group by matnum,stkdate"+
                                                    ") xxs "+
                                                    "union all "+
                                                    "select matnum,0 as iq,0 as accqty,0 as amq,sum(qty) as balqty,"+
                                                    "min(trandate) as stkdate,sum(qty) as rmqty,1 as trec,1 as recno "+
                                                    "from mwt" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where matnum in "+
                                                    "("+
                                                    "select distinct matnum "+
                                                    "from mwt" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where matnum not in "+
                                                    "("+
                                                    "select distinct matnum "+
                                                    "from mwt" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where qty>0 "+
                                                    "and "+
                                                    "convert(datetime,convert(nvarchar(8),trandate,112))<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' " +
                                                    "and matnum is not null) "+
                                                    "and "+
                                                    "convert(datetime,convert(nvarchar(8),trandate,112))<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "') " +
                                                    "and "+
                                                    "convert(datetime,convert(nvarchar(8),trandate,112))<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' " +
                                                    "and flag not in('tra','tri') "+
                                                    "and whnum<>'sitwh' "+
                                                    "group by matnum"+
                                                    ") oinv, sysagingbrackets abr "+
                                                    "where datediff(dd,stkdate,'" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' ) between abr.startday and abr.endday "+
                                                    "and abr.agingreportnumber=0"+
                                                    ") as temp1 "+
                                                    "pivot "+
                                                    "("+
                                                    "sum(rmqty) "+
                                                    "for "+
                                                    "bracketnumber in ([1],[2],[3],[4],[5],[6],[7],[8])"+
                                                    ") as temp2 "+
                                                    ") as pr"+
                                                    ") as rt"+
                                                    ")f   "+
                                                    "left join "+
                                                    "("+
                                                    "select matnum,wac "+
                                                    "from wac" + (Convert.ToInt32(Common.DEFAULT_SYSTEM_YEAR.ToString()) - 1) +
                                                    " where period=dbo.getsysperiod('" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' )"+
                                                    ") w "+
                                                    "on f.matnum=w.matnum " +
                                                    "where f.agq<>0";

                #endregion

                this.dbAccess.ReadSQL("tmpOpen", sqlOpen);
                DataTable dtOpen = this.dbAccess.DataSet.Tables["tmpOpen"];
                if (dtOpen.Rows.Count > 0)
                {
                    foreach (DataRow drOpen in dtOpen.Rows)
                    {
                        if (drOpen.RowState != DataRowState.Deleted && !BizFunctions.IsEmpty(drOpen["matnum"].ToString()))
                        {
                            DataRow newdr = mav1.Rows.Add(new object[] { });
                            BizFunctions.UpdateDataRow(drOpen, newdr, "matnum/stkdate/phyqty");
                        }
                    }
                }
            }

            foreach (DataRow drMAV1 in mav1.Rows)
            {
                if (drMAV1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(drMAV1["line"].ToString()) || (decimal)drMAV1["line"] <= 0)
                    {

                        lineNo = lineNo + 100;
                        drMAV1["line"] = lineNo;
                        //drMAV1["phyqty"] = drMAV1["bkqty"];
                    }
                }
            }
        }

        #endregion

        #region Compute Clicked

        protected void btn_Compute_Click(object sender, System.EventArgs e)
        {
            DataRow mavh = this.dbAccess.DataSet.Tables["mavh"].Rows[0];
            DataTable MAVH = this.dbAccess.DataSet.Tables["mavh"];
            DataTable mav1 = this.dbAccess.DataSet.Tables["mav1"];

            decimal lineNo = 0;

            if (mavh["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSP)
            {
                #region bkqty

                foreach (DataRow drMAV1 in mav1.Rows)
                {
                    if (drMAV1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(drMAV1["line"].ToString()) || (decimal)drMAV1["line"] <= 0)
                        {
                            lineNo = lineNo + 100;
                            drMAV1["line"] = lineNo;
                        }

                        if (!BizFunctions.IsEmpty(drMAV1["matnum"]))
                        {
                            #region Sum System Qty

                            //YushuEdited20100716 - LEFT JOIN mwt with matm
                            string sqlCommand = "";

                            sqlCommand = "SELECT matnum,Sum(qty) As qty,trandate,location,whnum From ("
                                + "SELECT mwt.matnum, mwt.qty As qty, mwt.trandate, mwt.location, mwt.whnum "
                                + "FROM mwt" + Common.DEFAULT_SYSTEM_YEAR + " As mwt) BizResult "
                                + "GROUP BY matnum, trandate, location, whnum";

                            sqlCommand = "SELECT mwt.matnum, SUM(mwt.qty) AS bkqty FROM (" + sqlCommand 
                                + ") mwt LEFT OUTER JOIN matm ON matm.matnum=mwt.matnum";

                            sqlCommand = sqlCommand + " WHERE mwt.location='" + mavh["sitenum"].ToString() + "' AND ISNULL(mwt.matnum,'')<>'' AND mwt.whnum='" 
                                + mavh["whnum"].ToString() + "' AND CONVERT(datetime, CONVERT(nvarchar(20),mwt.trandate,112))<='" 
                                + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' ";
                            sqlCommand = sqlCommand + "GROUP BY mwt.matnum ORDER BY mwt.matnum";

                            DataSet dsTmp = this.dbAccess.ReadSQLTemp("mwtTmp", sqlCommand);

                            foreach (DataRow dr in dsTmp.Tables["mwtTmp"].Rows)
                            {
                                if (dr.RowState != DataRowState.Deleted)
                                {
                                    if (dr["matnum"].ToString().Equals(drMAV1["matnum"].ToString()))
                                    {
                                        BizFunctions.UpdateDataRow(dr, drMAV1, "bkqty");
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion

                #region phyqty & qty

                if (!BizFunctions.IsEmpty(mavh["mavtype"].ToString()))
                {
                    if (mavh["mavtype"].ToString() == "Stock Take" || mavh["mavtype"].ToString() == "Opening Balance")
                    {
                        calculateAdjQty();
                    }
                    else if (mavh["mavtype"].ToString() == "Stock Adjustment")
                    {
                        calculatePhyQty();
                    }
                }

                #endregion

                #region totalqty

                calculateTotalQty();

                #endregion
            }

            #region Mark 0 Qty items

            foreach (DataRow dr in mav1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if ((decimal)dr["bkqty"] == 0 && (decimal)dr["phyqty"] == 0 && (decimal)dr["qty"] == 0)
                    {
                        dr["mark"] = true;
                    }
                    else
                    {
                        dr["mark"] = false;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Update-OK Clicked

        protected void btn_preview_ok_Click(object sender, System.EventArgs e)
        {
            // Collect the information from the dialog box.
            TextBox txt_MatnumFrom = BizXmlReader.CurrentInstance.GetControl("preview", "selMatFrom") as TextBox;
            TextBox txt_MatnumTo = BizXmlReader.CurrentInstance.GetControl("preview", "selMatTo") as TextBox;

            if (txt_MatnumTo.Text == String.Empty)		// Default to search ending string.
                txt_MatnumTo.Text = "ZZZZZZZZZZ";

            DataRow mavh = this.dbAccess.DataSet.Tables["mavh"].Rows[0];
            DataTable mav1 = this.dbAccess.DataSet.Tables["mav1"];

            BizFunctions.DeleteAllRows(mav1);

            #region Yushu Commented-20100127 - Allow to generate items not in Material Master
            //DON20071212_Due to DELETEALLROWS Function did not Clear the Error
            //foreach (DataRow dr in this.dbAccess.DataSet.Tables["mav1"].Rows)
            //{
            //    foreach (DataColumn dc in this.dbAccess.DataSet.Tables["mav1"].Columns)
            //    {
            //        dr.SetColumnError(dc.ColumnName, "");
            //    }
            //}
            #endregion

            string sqlCommand = "SELECT m.matnum,isnull(w.qty,0) bkqty FROM "
                                                    + "(SELECT matnum FROM mwt" + Common.DEFAULT_SYSTEM_YEAR
                                                    + " UNION "
                                                    + "SELECT matnum FROM matm)m "
                                                    + "LEFT JOIN "
                                                    + "(SELECT matnum, isnull(sum(qty),0) qty FROM mwt" + Common.DEFAULT_SYSTEM_YEAR
                                                    + " WHERE trandate<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' GROUP BY matnum)w "
                                                    + "ON m.matnum=w.matnum";

            sqlCommand = sqlCommand
                                        + " WHERE m.matnum IN (SELECT matnum FROM matm WHERE "
                                        + "matnum>='" + txt_MatnumFrom.Text.Trim() + "' AND "
                                        + "matnum<='" + txt_MatnumTo.Text.Trim() + "') ";

            sqlCommand = sqlCommand + "Order By m.matnum";

            #region Commented By Yushu

            //sqlCommand = "Select matnum,Sum(isnull(qty,0)) As qty,trandate From ( "
            //    + "Select mwt.matnum, mwt.qty As qty, mwt.trandate "
            //    + "From mwt" + Common.DEFAULT_SYSTEM_YEAR + " As mwt) BizResult "
            //    + "Group by matnum, trandate";

            //sqlCommand = "SELECT matnum, SUM(isnull(qty,0)) AS bkqty FROM (" + sqlCommand + ") mwt";

            //sqlCommand = sqlCommand + " WHERE trandate<='" + BizFunctions.GetSafeDateString((DateTime)mavh["trandate"]) + "' AND " +
            //                            "matnum IN (SELECT MATNUM FROM MATM WHERE " +
            //                            "matnum>='" + txt_MatnumFrom.Text.Trim() + "' AND " +
            //                            "matnum<='" + txt_MatnumTo.Text.Trim() + "') ";

            //sqlCommand = sqlCommand + "GROUP BY matnum Order By Matnum";

            #endregion

            DataSet dsTmp = this.dbAccess.ReadSQLTemp("mwtTmp", sqlCommand);
            //DataRow mavRow = mav1.NewRow();

            foreach (DataRow dr in dsTmp.Tables["mwtTmp"].Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    DataRow new_dr = mav1.Rows.Add(new object[] { });
                    new_dr.BeginEdit();
                    BizFunctions.UpdateDataRow(dr, new_dr, "matnum/bkqty");
                    new_dr.EndEdit();
                }
            }

            // Closes the dialog box.
            Form form_preview = BizXmlReader.CurrentInstance.GetForm("preview") as Form;
            form_preview.Close();

        }
        #endregion

        #region Update-Cancel Clicked

        protected void btn_preview_cancel_Click(object sender, System.EventArgs e)
        {
            Form form_preview = BizXmlReader.CurrentInstance.GetForm("preview") as Form;
            form_preview.Close();
        }

        #endregion

        #region PREVIEW PAGE (Filtering)

        #region MatnumFrom (F2)

        protected void txt_selMatnumFrom_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_MATM.xml", sender, "matnum", null, null, F2Type.Sort);
                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    this.matnum_from = f2BaseHelper.F2Base.CurrentRow["matnum"].ToString();
                    (sender as TextBox).Text = f2BaseHelper.F2Base.CurrentRow["matnum"].ToString();
                }
            }
        }

        #endregion

        #region MatnumTo (F2)

        protected void txt_selMatnumTo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                F2BaseHelper f2BaseHelper = new F2BaseHelper("F2GridInfo_MATM.xml", sender, "matnum", null, null, F2Type.Sort);
                f2BaseHelper.F2_Load();

                if (f2BaseHelper.F2Base.IsKeyPressF3 == true)
                {
                    this.matnum_to = f2BaseHelper.F2Base.CurrentRow["matnum"].ToString();
                    (sender as TextBox).Text = f2BaseHelper.F2Base.CurrentRow["matnum"].ToString();
                }
            }
        }

        #endregion

        #endregion

        #region Export Clicked

        protected void btn_Export_Click(object sender, System.EventArgs e)
        {
            string defaultPath = Environment.CurrentDirectory;

            try
            {
                DataTable mavExcel = new DataTable();

                mavExcel.Columns.Add("Material Code");
                mavExcel.Columns.Add("Description");
                mavExcel.Columns.Add("UOM");
                mavExcel.Columns.Add("Sys Qty");
                mavExcel.Columns.Add("Phys Qty");

                foreach (DataRow dr in this.dbAccess.DataSet.Tables["mav1"].Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DataRow newRow = mavExcel.NewRow();
                        newRow["Material Code"] = dr["matnum"];
                        newRow["Description"] = dr["matname"];
                        newRow["UOM"] = dr["uom"];
                        newRow["Sys Qty"] = dr["bkqty"];
                        newRow["Phys Qty"] = dr["phyqty"];

                        mavExcel.Rows.Add(newRow);
                    }
                }

                System.Windows.Forms.SaveFileDialog savefile = new SaveFileDialog();
                savefile.DefaultExt = "xls";
                savefile.Filter = "XLS(*.xls)|*.xls|TXT(*.txt)|*.txt";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    DataTableToExcel(mavExcel, savefile.FileName);
                    MessageBox.Show("Data Export Successfully!", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Environment.CurrentDirectory = defaultPath;
            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show("Error occured! Please make sure Microsoft Office is install correctly in this PC", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region DataTableToExcel

        public static void DataTableToExcel(DataTable tmpDataTable, string fileName)
        {
            if (fileName == null)
            {
                return;
            }
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
            xlApp.DefaultFilePath = "";
            xlApp.DisplayAlerts = true;
            xlApp.SheetsInNewWorkbook = 1;
            Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
            //Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(System.Reflection.Missing.Value);
            //xlApp.Cells.NumberFormat = "@";

            for (int i = 0; i < tmpDataTable.Columns.Count; i++)
            {
                xlApp.Cells[1, i + 1] = tmpDataTable.Columns[i].ColumnName.ToString();
            }

            for (int i = 0; i < tmpDataTable.Rows.Count; i++)
            {
                for (int j = 0; j < tmpDataTable.Columns.Count; j++)
                {
                    xlApp.Cells[i + 2, j + 1] = tmpDataTable.Rows[i][j].ToString();
                }
            }
            xlBook.SaveCopyAs(fileName);
        }

        #endregion

        #region Import Clicked

        protected void btn_Import_Click(object sender, System.EventArgs e)
        {
            try
            {
                DataTable mav1 = this.dbAccess.DataSet.Tables["mav1"];
                decimal lineNo = 0;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog_FileOk);
                openFileDialog.Filter = "XLS(*.XLS;*.XLSX;)|*.xls;*.xlsx;|All Files|*.*";
                openFileDialog.ShowDialog();

                foreach (DataRow drMAV1 in mav1.Rows)
                {
                    if (drMAV1.RowState != DataRowState.Deleted)
                    {
                        if (BizFunctions.IsEmpty(drMAV1["line"].ToString()) || (decimal)drMAV1["line"] <= 0)
                        {
                            lineNo = lineNo + 100;
                            drMAV1["line"] = lineNo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region openFileDialog
        protected void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DataRow mavh = this.dbAccess.DataSet.Tables["mavh"].Rows[0];
                DataTable mav1 = this.dbAccess.DataSet.Tables["mav1"];
                string Path = (sender as OpenFileDialog).FileName;
                //Read data from Excel,and return the dataset
                DataSet ds = ExcelToDS(Path, "XSL", 0);

                //Delete the old datas in detail1
                #region Commented
                //int iCount = this.dbAccess.DataSet.Tables["mav1"].Rows.Count;
                //for (int iPos = iCount - 1; iPos >= 0; iPos--)
                //{
                //    DataRow row = this.dbAccess.DataSet.Tables["mav1"].Rows[iPos];
                //    if (row.RowState == DataRowState.Added)
                //    {
                //        this.dbAccess.DataSet.Tables["mav1"].Rows.Remove(row);
                //    }
                //    else if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                //    {
                //        row.Delete();
                //    }
                //}
                #endregion

                //copy the data in dataset(from Excel) to dto1
                foreach (DataRow dr1 in mav1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        foreach (DataRow dr in ds.Tables["XSL"].Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                            {
                                if (dr1["matnum"].ToString().Trim().Equals(dr["Material Code"].ToString().Trim()))
                                {
                                    dr1["phyqty"] = dr["Phys Qty"];
                                    break;//Come out from current loop, go to next matnum in mav1
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //MessageBox.Show("Error occured! Please make sure Microsoft Office is install correctly in this PC", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region ExcelToDS
        public DataSet ExcelToDS(string Path, string tablename, int sheetIndex)
        {
            string strConn = "Provider = Microsoft.Jet.OLEDB.4.0; " + "Data Source = " + Path + "; " + "Extended Properties = Excel 8.0";
            OleDbConnection conn = new OleDbConnection(strConn);

            conn.Open();
            System.Data.DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dbSchema == null || dbSchema.Rows.Count < 1)
            {
                throw new Exception("Error: Could not get Excel schema table.");
            }
            string sheetName = "[" + dbSchema.Rows[sheetIndex]["TABLE_NAME"].ToString() + "]";
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from " + sheetName;
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, tablename);
            conn.Close();
            return ds;
        }
        #endregion

        #region Calculate Phy, Adj, Total Qty

        private void calculatePhyQty()
        {
            DataTable mav1 = this.dbAccess.DataSet.Tables["mav1"];

            foreach (DataRow drV1 in mav1.Rows)
            {
                if (drV1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(drV1["qty"]))
                    {
                        drV1["qty"] = 0;
                    }
                    if (BizFunctions.IsEmpty(drV1["bkqty"]))
                    {
                        drV1["bkqty"] = 0;
                    }

                    drV1["phyqty"] = BizFunctions.Round(Convert.ToDecimal(drV1["qty"]) + Convert.ToDecimal(drV1["bkqty"]), 4);
                }
            }
        }

        private void calculateAdjQty()
        {
            DataTable mav1 = this.dbAccess.DataSet.Tables["mav1"];

            foreach (DataRow drV1 in mav1.Rows)
            {
                if (drV1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(drV1["phyqty"]))
                    {
                        drV1["phyqty"] = 0;
                    }
                    if (BizFunctions.IsEmpty(drV1["bkqty"]))
                    {
                        drV1["bkqty"] = 0;
                    }
                    drV1["qty"] = BizFunctions.Round(Convert.ToDecimal(drV1["phyqty"]) - Convert.ToDecimal(drV1["bkqty"]), 4);
                }
            }
        }

        private void calculateTotalQty()
        {
            DataTable mav1 = this.dbAccess.DataSet.Tables["mav1"];
            DataRow mavh = this.dbAccess.DataSet.Tables["mavh"].Rows[0];

            decimal totalbkqty = 0;
            decimal totalphyqty = 0;
            decimal totalqty = 0;

            foreach (DataRow drV1 in mav1.Rows)
            {
                if (drV1.RowState != DataRowState.Deleted)
                {
                    if (BizFunctions.IsEmpty(drV1["bkqty"]))
                    {
                        drV1["bkqty"] = 0;
                    }
                    if (BizFunctions.IsEmpty(drV1["phyqty"]))
                    {
                        drV1["phyqty"] = 0;
                    }
                    if (BizFunctions.IsEmpty(drV1["qty"]))
                    {
                        drV1["qty"] = 0;
                    }
                    totalbkqty += Convert.ToDecimal(drV1["bkqty"]);
                    totalphyqty += Convert.ToDecimal(drV1["phyqty"]);
                    totalqty += Convert.ToDecimal(drV1["qty"]);
                }
            }

            mavh["totalbkqty"] = totalbkqty;
            mavh["totalphyqty"] = totalphyqty;
            mavh["totalqty"] = totalqty;
        }

        #endregion
    }
}
