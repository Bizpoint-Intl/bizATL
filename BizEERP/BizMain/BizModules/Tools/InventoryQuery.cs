using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizTools;
using BizRAD.DB.Interface;
using BizRAD.BizVoucher;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;

using ATL.MultiColumnComboBox;
using ATL.GeneralTools;
using ATL.BizLogicTools;
using System.Configuration;

namespace ATL.InventoryQuery
{
    public partial class frmInventoryQuery : Form
    {
        DBAccess dbAccess = new DBAccess();
        GenTools genFunctions = new GenTools();
        string posid = ConfigurationManager.AppSettings.Get("POSID");

        public frmInventoryQuery()
        {
            InitializeComponent();
        }

        private void frmInventoryQuery_Load(object sender, System.EventArgs e)
        {
            LoadComboBox();
            rad_Transfer.Checked = true;
            rad_StkBal.Checked = false;
            cboSite.Text = "HQ";
            cboSite.Enabled = false;
            groupBox1.Visible = true;
            groupBox2.Visible = true;
            groupBox3.Visible = true;
            grpStockBal.Visible = false;
            dgQuery.Visible = true;
            dgStkBal.Visible = false;
            MakeEnterEvent();
        }

        private void LoadComboBox()
        {
            string strSQLSite = "SELECT sitenum FROM SITM ORDER BY ID";
            DataSet dsSite = this.dbAccess.ReadSQLTemp("Site", strSQLSite);
            DataTable dtSite = dsSite.Tables["Site"];
            cboSite.Data = dtSite;
            cboSite.ViewColumn = 0;
            cboSite.ColWidthNew(new int[] { 100 });
            cboSite.UpdateIndex();

            string strSQLSiteFrom_TRQ = "SELECT sitenum FROM SITM ORDER BY ID";
            DataSet dsSiteFrom_TRQ = this.dbAccess.ReadSQLTemp("SiteFrom", strSQLSiteFrom_TRQ);
            DataTable dtSiteFrom_TRQ = dsSiteFrom_TRQ.Tables["SiteFrom"];
            cboSiteFrom_TRQ.Data = dtSiteFrom_TRQ;
            cboSiteFrom_TRQ.ViewColumn = 0;
            cboSiteFrom_TRQ.ColWidthNew(new int[] { 100 });
            cboSiteFrom_TRQ.UpdateIndex();

            string strSQLSiteTo_TRQ = "SELECT sitenum FROM SITM ORDER BY ID";
            DataSet dsSiteTo_TRQ = this.dbAccess.ReadSQLTemp("SiteTo", strSQLSiteTo_TRQ);
            DataTable dtSiteTo_TRQ = dsSiteTo_TRQ.Tables["SiteTo"];
            cboSiteTo_TRQ.Data = dtSiteTo_TRQ;
            cboSiteTo_TRQ.ViewColumn = 0;
            cboSiteTo_TRQ.ColWidthNew(new int[] { 100 });
            cboSiteTo_TRQ.UpdateIndex();

            string strSQLSiteFrom_TRA = "SELECT sitenum FROM SITM ORDER BY ID";
            DataSet dsSiteFrom_TRA = this.dbAccess.ReadSQLTemp("SiteFrom", strSQLSiteFrom_TRA);
            DataTable dtSiteFrom_TRA = dsSiteFrom_TRA.Tables["SiteFrom"];
            cboSiteFrom_TRA.Data = dtSiteFrom_TRA;
            cboSiteFrom_TRA.ViewColumn = 0;
            cboSiteFrom_TRA.ColWidthNew(new int[] { 100 });
            cboSiteFrom_TRA.UpdateIndex();

            string strSQLSiteTo_TRA = "SELECT sitenum FROM SITM ORDER BY ID";
            DataSet dsSiteTo_TRA = this.dbAccess.ReadSQLTemp("SiteTo", strSQLSiteTo_TRA);
            DataTable dtSiteTo_TRA = dsSiteTo_TRA.Tables["SiteTo"];
            cboSiteTo_TRA.Data = dtSiteTo_TRA;
            cboSiteTo_TRA.ViewColumn = 0;
            cboSiteTo_TRA.ColWidthNew(new int[] { 100 });
            cboSiteTo_TRA.UpdateIndex();

            string strSQLSiteFrom_TRI = "SELECT sitenum FROM SITM ORDER BY ID";
            DataSet dsSiteFrom_TRI = this.dbAccess.ReadSQLTemp("SiteFrom", strSQLSiteFrom_TRI);
            DataTable dtSiteFrom_TRI = dsSiteFrom_TRI.Tables["SiteFrom"];
            cboSiteFrom_TRI.Data = dtSiteFrom_TRI;
            cboSiteFrom_TRI.ViewColumn = 0;
            cboSiteFrom_TRI.ColWidthNew(new int[] { 100 });
            cboSiteFrom_TRI.UpdateIndex();

            string strSQLSiteTo_TRI = "SELECT sitenum FROM SITM ORDER BY ID";
            DataSet dsSiteTo_TRI = this.dbAccess.ReadSQLTemp("SiteTo", strSQLSiteTo_TRI);
            DataTable dtSiteTo_TRI = dsSiteTo_TRI.Tables["SiteTo"];
            cboSiteTo_TRI.Data = dtSiteTo_TRI;
            cboSiteTo_TRI.ViewColumn = 0;
            cboSiteTo_TRI.ColWidthNew(new int[] { 100 });
            cboSiteTo_TRI.UpdateIndex();

            string strSQLLocation = "SELECT sitenum FROM SITM ORDER BY ID";
            DataSet dsLocation = this.dbAccess.ReadSQLTemp("Location", strSQLLocation);
            DataTable dtLocation = dsLocation.Tables["Location"];
            cboLocation.Data = dtLocation;
            cboLocation.ViewColumn = 0;
            cboLocation.ColWidthNew(new int[] { 100 });
            cboLocation.UpdateIndex();

            string strSQLWH = "SELECT whnum FROM whm ORDER BY ID";
            DataSet dsWH = this.dbAccess.ReadSQLTemp("WH", strSQLWH);
            DataTable dtWH = dsWH.Tables["WH"];
            cboWarehouse.Data = dtWH;
            cboWarehouse.ViewColumn = 0;
            cboWarehouse.ColWidthNew(new int[] { 100 });
            cboWarehouse.UpdateIndex();

            string strSQLMatm = "SELECT MATNAME FROM MATM WHERE STATUS<>'V' ORDER BY MATNAME";
            DataSet dsMatname = this.dbAccess.ReadSQLTemp("Matm", strSQLMatm);
            DataTable dtMatname = dsMatname.Tables["Matm"];
            cboMatname.Data = dtMatname;
            cboMatname.ViewColumn = 0;
            cboMatname.ColWidthNew(new int[] { 120 });
            cboMatname.UpdateIndex();

            string strSQLBrand = "SELECT PBRDNAME FROM PBRDM WHERE ACTIVE=1 ORDER BY PBRDNAME";
            DataSet dsBrand = this.dbAccess.ReadSQLTemp("Brand", strSQLBrand);
            DataTable dtBrand = dsBrand.Tables["Brand"];
            cboBrand.Data = dtBrand;
            cboBrand.ViewColumn = 0;
            cboBrand.ColWidthNew(new int[] { 120 });
            cboBrand.UpdateIndex();

            string strSQLCat = "SELECT PCATNAME FROM PCATM WHERE ACTIVE=1 ORDER BY PCATNAME";
            DataSet dsCat = this.dbAccess.ReadSQLTemp("Cat", strSQLCat);
            DataTable dtCat = dsCat.Tables["Cat"];
            cboCategory.Data = dtCat;
            cboCategory.ViewColumn = 0;
            cboCategory.ColWidthNew(new int[] { 120 });
            cboCategory.UpdateIndex();
        }

        private void MakeEnterEvent()
        {
            foreach (Control crlControl in groupBox1.Controls)
            {
                if (crlControl.GetType().ToString() == "System.Windows.Forms.ComboBox")
                {
                    ComboBox ctrl = (ComboBox)crlControl;
                    //ctrl.SelectedIndexChanged += new System.EventHandler(this.Combo_SelectedIndexChanged);
                    ctrl.Validating += new System.ComponentModel.CancelEventHandler(this.Combo_Validating);
                }
                crlControl.KeyDown += new KeyEventHandler(SendTabForEnter_KeyDown);
            }
            foreach (Control crlControl in groupBox2.Controls)
            {
                if (crlControl.GetType().ToString() == "System.Windows.Forms.ComboBox")
                {
                    ComboBox ctrl = (ComboBox)crlControl;
                    //ctrl.SelectedIndexChanged += new System.EventHandler(this.Combo_SelectedIndexChanged);
                    ctrl.Validating += new System.ComponentModel.CancelEventHandler(this.Combo_Validating);
                }
                crlControl.KeyDown += new KeyEventHandler(SendTabForEnter_KeyDown);
            }
            foreach (Control crlControl in groupBox3.Controls)
            {
                if (crlControl.GetType().ToString() == "System.Windows.Forms.ComboBox")
                {
                    ComboBox ctrl = (ComboBox)crlControl;
                    //ctrl.SelectedIndexChanged += new System.EventHandler(this.Combo_SelectedIndexChanged);
                    ctrl.Validating += new System.ComponentModel.CancelEventHandler(this.Combo_Validating);
                }
                crlControl.KeyDown += new KeyEventHandler(SendTabForEnter_KeyDown);
            }
            foreach (Control crlControl in grpStockBal.Controls)
            {
                if (crlControl.GetType().ToString() == "System.Windows.Forms.ComboBox")
                {
                    ComboBox ctrl = (ComboBox)crlControl;
                    ctrl.Validating += new System.ComponentModel.CancelEventHandler(this.Combo_Validating);
                }
                crlControl.KeyDown += new KeyEventHandler(SendTabForEnter_KeyDown);
            }
        }

        private void SendTabForEnter_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender.GetType().Name.ToString() == "ComboBox")
                {
                    ComboBox cboSender = (ComboBox)sender;
                    if (cboSender.Name.ToString() == "cboSite") cboSite.Focus();
                    else if (cboSender.Name.ToString() == "cboSiteFrom_TRQ") cboSiteFrom_TRQ.Focus();
                    else if (cboSender.Name.ToString() == "cboSiteTo_TRQ") cboSiteTo_TRQ.Focus();
                    else if (cboSender.Name.ToString() == "cboSiteFrom_TRA") cboSiteFrom_TRA.Focus();
                    else if (cboSender.Name.ToString() == "cboSiteTo_TRA") cboSiteTo_TRA.Focus();
                    else if (cboSender.Name.ToString() == "cboSiteFrom_TRI") cboSiteFrom_TRI.Focus();
                    else if (cboSender.Name.ToString() == "cboSiteTo_TRI") cboSiteTo_TRI.Focus();
                    else if (cboSender.Name.ToString() == "cboMatname") cboMatname.Focus();
                    else if (cboSender.Name.ToString() == "cboBrand") cboBrand.Focus();
                    else if (cboSender.Name.ToString() == "cboCategory") cboCategory.Focus();

                    else SendKeys.Send("{Tab}");
                    return;
                }
                SendKeys.Send("{Tab}");
            }

            if (e.KeyCode == Keys.Escape)
            {
                cboSiteFrom_TRQ.Text="";
                cboSiteTo_TRQ.Text = "";
                cboSiteFrom_TRA.Text = "";
                cboSiteTo_TRA.Text = "";
                cboSiteFrom_TRI.Text = "";
                cboSiteTo_TRI.Text = "";
                cboMatname.Text = "";
                cboBrand.Text = "";
                cboCategory.Text = "";
                //LoadComboBox();
            }
        }

        private void Combo_Validating(object sender, CancelEventArgs e)
        {
            ComboBox cboSender = (ComboBox)sender;

            if (cboSender.Text.Trim() != "")
            {
                if (cboSender.SelectedIndex == -1)
                {
                    MessageBox.Show("Invalid Value! Please Select on the List.", "System Message");
                    e.Cancel = true;
                }
            }

        }

        private void btnEnquiry_Click(object sender, EventArgs e)
        {
            string sql = "";
            string connString = "";
            string where="";
            string where1 = "";

            DateTime dtCutoff =dt_Cutoff.Value;

            #region transfer flows
            if (rad_Transfer.Checked)
            {
                #region where condition
                if (cboSiteFrom_TRQ.Text != "")
                {
                    if (where != string.Empty)
                    {
                        where += " AND trqfrom='" + cboSiteFrom_TRQ.Text + "' ";
                    }
                    else
                    {
                        where = " where trqfrom='" + cboSiteFrom_TRQ.Text + "' ";
                    }
                }
                if (cboSiteTo_TRQ.Text != "")
                {
                    if (where != string.Empty)
                    {
                        where += " AND trqto='" + cboSiteTo_TRQ.Text + "' ";
                    }
                    else
                    {
                        where = " where trqto='" + cboSiteTo_TRQ.Text + "' ";
                    }
                }
                if (cboSiteFrom_TRA.Text != "")
                {
                    if (where != string.Empty)
                    {
                        where += " AND trafrom='" + cboSiteFrom_TRA.Text + "' ";
                    }
                    else
                    {
                        where = " where trafrom='" + cboSiteFrom_TRA.Text + "' ";
                    }
                }
                if (cboSiteTo_TRA.Text != "")
                {
                    if (where != string.Empty)
                    {
                        where += " AND trato='" + cboSiteTo_TRA.Text + "' ";
                    }
                    else
                    {
                        where = " where trato='" + cboSiteTo_TRA.Text + "' ";
                    }
                }
                if (cboSiteFrom_TRI.Text != "")
                {
                    if (where != string.Empty)
                    {
                        where += " AND trifrom='" + cboSiteFrom_TRI.Text + "' ";
                    }
                    else
                    {
                        where = " where trifrom='" + cboSiteFrom_TRI.Text + "' ";
                    }
                }
                if (cboSiteTo_TRI.Text != "")
                {
                    if (where != string.Empty)
                    {
                        where += " AND trito='" + cboSiteTo_TRI.Text + "' ";
                    }
                    else
                    {
                        where = " where trito='" + cboSiteTo_TRI.Text + "' ";
                    }
                }
                #endregion

                sql = "select trq.trqnum,trqfrom,trqto,trqqty,trqtrandate,tra.tranum,trafrom,trato,traqty,tratrandate,"+
                        "trinum,trifrom,trito,triqty,tritrandate from (select flag as trqflag,sitenum as trqsite,"+
                        "refnum as trqnum,fromsitenum as trqfrom,"+
                        "tositenum as trqto,ttqty as trqqty,trandate as trqtrandate "+
                        "from trqh where status<>'V') trq full join ("+
                        "select flag as traflag,sitenum as trasite,trqnum,refnum as tranum,fromsitenum as trafrom,"+
                        "tositenum as trato,ttqty as traqty,trandate as tratrandate from trah where status<>'V') tra "+
                        "on trq.trqnum=tra.trqnum full join (select flag as triflag,sitenum as trisite,tranum,"+
                        "refnum as trinum,fromsitenum as trifrom,tositenum as trito,ttqty as triqty,"+
                        "trandate as tritrandate from trih where status<>'V') tri on tra.tranum=tri.tranum "+where;

                connString = GetSite("HQ");
                SqlConnection Connection = new SqlConnection(connString);
                try
                {
                    Connection.Open();
                    SqlCommand Command = new SqlCommand(sql, Connection);
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                    SqlCommandBuilder CommandBuilder = new SqlCommandBuilder(DataAdapter);
                    if (dbAccess.DataSet.Tables.Contains("Query"))
                        dbAccess.DataSet.Tables.Remove("Query");
                    DataAdapter.Fill(dbAccess.DataSet, "Query");
                    DataTable Query = dbAccess.DataSet.Tables["Query"];
                    Connection.Close();

                    BindingSource BindSourceQuery = new BindingSource();
                    BindSourceQuery.DataSource = Query;

                    dgQuery.DataSource = BindSourceQuery;
                    dgQuery.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unavailable  Outlet selected,No connection found.\n" + ex.Message, "Bizpoint Internation Pte Ltd");
                }
            }
            #endregion
            #region stock balance
            else if (rad_StkBal.Checked)
            {
                if (cboSite.Text.Trim() != "")
                {
                    if (cboSite.SelectedIndex == -1)
                    {
                        MessageBox.Show("Invalid Outlet Selected! Please Select on the List.", "System Message");
                        return;
                    }
                }

                if (cboLocation.Text != "")
                {
                    where += " AND location='" + cboLocation.Text + "' ";
                }
                if (cboWarehouse.Text != "")
                {
                    where += " AND whnum='" + cboWarehouse.Text + "' ";
                }
                if (cboMatname.Text != "")
                {
                    if (where1 == string.Empty)
                    {
                        where1 += " where matname like '%" + cboMatname.Text + "%' ";
                    }
                    else
                    {
                        where1 += " AND matname like '%" + cboMatname.Text + "%' ";
                    }
                }
               
                if (cboBrand.Text != "")
                {
                    if (where1 == string.Empty)
                    {
                        where1 += " where pbrdname like '%" + cboBrand.Text + "%' ";
                    }
                    else
                    {
                        where1 += " AND pbrdname like '%" + cboBrand.Text + "%' ";
                    }
                }
                if (cboCategory.Text != "")
                {
                    if (where1 == string.Empty)
                    {
                        where1 += " where pcatname like '%" + cboCategory.Text + "%' ";
                    }
                    else
                    {
                        where1 += " AND pcatname like '%" + cboCategory.Text + "%' ";
                    }
                }
                sql = "select mwt.matnum as matnum,matm.matname as detail,location,whnum,balqty,pbrdname,pcatname from(" +
                       "select matnum,location,whnum,sum(qty) as balqty from mwt" + Common.DEFAULT_SYSTEM_YEAR +
                       " where status<>'V' and trandate<='" + dtCutoff.ToString("yyyyMMdd") + "' " + where + 
                       " group by matnum,location,whnum)mwt left join matm on mwt.matnum=matm.matnum "+
                       " left join pbrdm on matm.pbrdcode=pbrdm.pbrdcode left join pcatm on matm.pcatcode=pcatm.pcatcode"+where1;


                connString = GetSite(cboSite.Text.ToString());
                SqlConnection Connection = new SqlConnection(connString);
                try
                {
                    Connection.Open();
                    SqlCommand Command = new SqlCommand(sql, Connection);
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                    SqlCommandBuilder CommandBuilder = new SqlCommandBuilder(DataAdapter);
                    if (dbAccess.DataSet.Tables.Contains("Query"))
                        dbAccess.DataSet.Tables.Remove("Query");
                    DataAdapter.Fill(dbAccess.DataSet, "Query");
                    DataTable Query = dbAccess.DataSet.Tables["Query"];
                    Connection.Close();

                    BindingSource BindSourceQuery = new BindingSource();
                    BindSourceQuery.DataSource = Query;

                    dgStkBal.DataSource = BindSourceQuery;
                    dgStkBal.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unavailable  Outlet selected,No connection found.\n" + ex.Message, "Bizpoint Internation Pte Ltd");
                }
            }
            #endregion
            else
            {
                MessageBox.Show("Please select TransferFlow or StockBalance first.", "Bizpoint Internation Pte Ltd");
                return;
            }
        }

        //private string GetWarehousenum(string strWarehouse)
        //{
        //    string strSQL = "SELECT * FROM WHM WHERE WHNUM='" + strWarehouse + "'";
        //    DataSet dsWHM = this.dbAccess.ReadSQLTemp("WHM", strSQL);
        //    DataTable dtWHM = dsWHM.Tables["WHM"];

        //    if (dtWHM.Rows.Count > 0)
        //        return dtWHM.Rows[0]["whnum"].ToString();
        //    else
        //        return "";
        //}

        private string GetSite(string strLocation)
        {
            string strSQL = "SELECT * FROM SITM WHERE sitenum='" +cboSite.Text + "'";

            DataSet dsOutlet = this.dbAccess.ReadSQLTemp("Outlet", strSQL);
            DataTable dtOutlet = dsOutlet.Tables["Outlet"];

            if (dtOutlet.Rows.Count > 0)
            {
                return dtOutlet.Rows[0]["connectionstring"].ToString();
            }
            else
            {
                return "";
            }
        }

        private void rad_Transfer_Click(object sender, EventArgs e)
        {
            if (rad_Transfer.Checked)
            {
                dgQuery.Visible = true;
                dgStkBal.Visible = false;
                cboSite.Text = "HQ";
                cboSite.Enabled = false;
                groupBox1.Visible = true;
                groupBox2.Visible = true;
                groupBox3.Visible = true;
                grpStockBal.Visible = false;
            }
        }

        private void rad_StkBal_Click(object sender, EventArgs e)
        {
           if (rad_StkBal.Checked)
            {
                dgQuery.Visible = false;
                dgStkBal.Visible = true;
                cboSite.Enabled = true;
                groupBox1.Visible = false;
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                grpStockBal.Visible = true;
            }
        }

        //private void dgQuery_SelectionChanged(object sender, EventArgs e)
        //{
        //    DataGridView dgv = sender as DataGridView;
        //    if (dgv.CurrentRow != null) // if at least 1 row is selected
        //    {
        //        string mstylecode = dgv.CurrentRow.Cells["mstylecode"].Value.ToString();
        //        string smtypedesc = dgv.CurrentRow.Cells["fabric"].Value.ToString();

        //        foreach (DataGridViewRow dgr in dgv.Rows)
        //        {
        //            if (dgr.Cells["mstylecode"].Value.ToString().Equals(mstylecode) && dgr.Cells["fabric"].Value.ToString().Equals(smtypedesc))
        //            {
        //                //dgr.Selected = true;
        //            }
        //        }
        //    }
        //}


        //private void cboSite_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (cboSite.Text != string.Empty)
        //    {
        //        string strSQLWarehouse = "";

        //        strSQLWarehouse = "SELECT WHNUM FROM WHM WHERE sitenum='" + cboSite.Text + "'";

        //        try
        //        {
        //            if (cboWarehouse.Enabled == false) cboWarehouse.Enabled = true;
        //            DataSet dsWarehouse = this.dbAccess.ReadSQLTemp("Warehouse", strSQLWarehouse);
        //            DataTable dtWarehouse = dsWarehouse.Tables["Warehouse"];
        //            cboWarehouse.Data = dtWarehouse;
        //            cboWarehouse.Text = dtWarehouse.Rows[0]["whnum"].ToString();
        //            cboSite.UpdateIndex();
        //        }
        //        catch
        //        {
        //            if (cboWarehouse.Enabled == true) cboWarehouse.Enabled = false;
        //            cboWarehouse.Text = "<No Available Warehouse>";
        //        }
        //    }
        //}

        //private string GetLocationName(string strLocation)
        //{
        //    //string strSQL = "SELECT * FROM SYSLOCATIONCONNECTION WHERE NAME='" + strLocation + "'";
        //    string strSQL = "SELECT * FROM SITM WHERE SITENAME LIKE'%" + strLocation + "%' ";
        //    DataSet dsLOC = this.dbAccess.ReadSQLTemp("LOC", strSQL);
        //    DataTable dtLOC = dsLOC.Tables["LOC"];

        //    if (dtLOC.Rows.Count > 0)
        //        return dtLOC.Rows[0]["SITENAME"].ToString();
        //    else
        //        return "";
        //}
    }
}