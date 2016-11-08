using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
using ATL.GeneralTools;
using BizRAD.BizAccounts;
namespace ATL.ExtractMATM
{
    public partial class ExtractMATM : Form
    {
        #region global variables
        private DataTable oriTable = null;
        protected DBAccess dbaccess = null;
        GenTools genFunctions = new GenTools();
        private DataTable dt_view = null;
        private DataSet ds = null;
        private DataTable dt = null;
        #endregion

        public ExtractMATM(DBAccess dbAccess, DataTable originalTable)
        {
            InitializeComponent();
            this.oriTable = originalTable;

            this.dbaccess = dbAccess;
        }

        private void ExtractMATM_Load(object sender, EventArgs e)
        {
            LoadCombo();
            MakeEnterEvent();

            cbo_Cat.SelectedIndexChanged += new EventHandler(cbo_Cat_SelectedIndexChanged);
            cbo_Code.SelectedIndexChanged += new EventHandler(cbo_Code_SelectedIndexChanged);
        }

        private void LoadCombo()
        {
            string strSQLCat = "SELECT pcatname from pcatm order by pcatcode";
            DataSet dsCat = this.dbaccess.ReadSQLTemp("Cat", strSQLCat);
            DataTable dtCat = dsCat.Tables["Cat"];
            cbo_Cat.Data = dtCat;
            cbo_Cat.ViewColumn = 0;
            cbo_Cat.ColWidthNew(new int[] { 200 });
            cbo_Cat.UpdateIndex();

            string strSQLCode = "SELECT matnum from matm where flag='MATMSTK' order by matnum";
            DataSet dsCode = this.dbaccess.ReadSQLTemp("Code", strSQLCode);
            DataTable dtCode = dsCode.Tables["Code"];
            cbo_Code.Data = dtCode;
            cbo_Code.ViewColumn = 0;
            cbo_Code.ColWidthNew(new int[] { 250 });
            cbo_Code.UpdateIndex();

            //string strSQLCNName = "SELECT cnmatname from matm where isnull(cnmatname,'')<>'' order by cnmatname";
            //DataSet dsCNName = this.dbaccess.ReadSQLTemp("CNName", strSQLCNName);
            //DataTable dtCNName = dsCNName.Tables["CNName"];
            //cbo_CNName.Data = dtCNName;
            //cbo_CNName.ViewColumn = 0;
            //cbo_CNName.ColWidthNew(new int[] { 340 });
            //cbo_CNName.UpdateIndex();

            string strSQLName = "SELECT matname from matm where isnull(matname,'')<>'' and flag='MATMSTK' order by matname";
            DataSet dsName = this.dbaccess.ReadSQLTemp("Name", strSQLName);
            DataTable dtName = dsName.Tables["Name"];
            cbo_Name.Data = dtName;
            cbo_Name.ViewColumn = 0;
            cbo_Name.ColWidthNew(new int[] { 340 });
            cbo_Name.UpdateIndex();
        }

        private void MakeEnterEvent()
        {
            foreach (Control crlControl in grp_Filter.Controls)
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
                    if (cboSender.Name.ToString() == "cbo_Cat") cbo_Cat.Focus();
                    else if (cboSender.Name.ToString() == "cbo_Code") cbo_Code.Focus();
                    else SendKeys.Send("{Tab}");
                    return;
                }
                SendKeys.Send("{Tab}");
            }

            if (e.KeyCode == Keys.Escape)
            {
                cbo_Cat.SelectedIndex = -1;
                cbo_Code.SelectedIndex = -1;
            }
        }

        private void Combo_Validating(object sender, CancelEventArgs e)
        {
            ComboBox cboSender = (ComboBox)sender;

            if (cboSender.Text.Trim() != "")
            {
                if (cboSender.SelectedIndex == -1)
                {
                    MessageBox.Show("Invalid Value! Please select items in the List.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }

        }

        private void cbo_Cat_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strSQL = "";

            if (cbo_Cat.SelectedItem != System.DBNull.Value && cbo_Cat.SelectedItem != null)
            {
                strSQL = "SELECT pcatm.pcatname as pcatname,matnum,matname,uomcode FROM matm " +
                                "left join pcatm on matm.pcatcode=pcatm.pcatcode " +
                                "where pcatname='" + cbo_Cat.SelectedItem.ToString() + "'  AND isnull(matm.status,'')<>'V' and matm.flag='MATMSTK' ORDER BY pcatm.pcatname,matnum";
            }

            if (strSQL != "")
            {
                ds = this.dbaccess.ReadSQLTemp("matm", strSQL);
                dt = ds.Tables["matm"];

                dt_view = dt.Copy();
                dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
                dt_view.Columns[4].DefaultValue = 0;

                this.dgv_Filter.DataSource = dt_view;
            }
        }

        private void cbo_Code_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strSQL = "";

            if (cbo_Code.SelectedItem != System.DBNull.Value && cbo_Code.SelectedItem != null)
            {
                if (cbo_Cat.SelectedItem != System.DBNull.Value && cbo_Cat.SelectedItem != null)
                {
                    strSQL = "SELECT pcatm.pcatname as pcatname,matnum,matname,uomcode FROM matm " +
                                "left join pcatm on matm.pcatcode=pcatm.pcatcode " +
                                "WHERE matnum like '%" + cbo_Code.SelectedItem.ToString() + "%' AND pcatm.pcatname='" + cbo_Cat.SelectedItem.ToString()
                                + "'  AND isnull(matm.status,'')<>'V' and matm.flag='MATMSTK' ORDER BY matnum";
                }

                else
                {
                    strSQL = "SELECT pcatm.pcatname as pcatname,matnum,matname,uomcode FROM matm " +
                                "left join pcatm on matm.pcatcode=pcatm.pcatcode " +
                                "where matnum like '%" + cbo_Code.SelectedItem.ToString()
                                + "%'  AND isnull(matm.status,'')<>'V' and matm.flag='MATMSTK'  ORDER BY matnum";
                }
            }

            else
            {
                if (cbo_Cat.SelectedItem != System.DBNull.Value && cbo_Cat.SelectedItem != null)
                {
                    strSQL = "SELECT pcatm.pcatname as pcatname,matnum,matname,uomcode FROM matm " +
                                "left join pcatm on matm.pcatcode=pcatm.pcatcode " +
                                "where pcatname='" + cbo_Cat.SelectedItem.ToString() + "'  AND isnull(matm.status,'')<>'V' and matm.flag='MATMSTK'  ORDER BY matnum";
                }
            }
           
            if (strSQL != "")
            {
                ds = this.dbaccess.ReadSQLTemp("matm", strSQL);
                dt = ds.Tables["matm"];

                dt_view = dt.Copy();
                dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
                dt_view.Columns[4].DefaultValue = 0;

                this.dgv_Filter.DataSource = dt_view;
            }
        }

        private void cbo_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strSQL = "";
            if (cbo_Name.SelectedItem != System.DBNull.Value && cbo_Name.SelectedItem != null)
            {
                strSQL = "SELECT pcatm.pcatname as pcatname,matnum,matname,uomcode FROM matm " +
                            "left join pcatm on matm.pcatcode=pcatm.pcatcode " +
                            "where matname like '%" + cbo_Name.SelectedItem.ToString() + "%' and matm.flag='MATMSTK'  ORDER BY matname";
            }

            if (strSQL != "")
            {
                ds = this.dbaccess.ReadSQLTemp("matm", strSQL);
                dt = ds.Tables["matm"];

                dt_view = dt.Copy();
                dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
                dt_view.Columns[4].DefaultValue = 0;

                this.dgv_Filter.DataSource = dt_view;
            }
        }

        private void cbo_Name_KeyDown(object sender, KeyEventArgs e)
        {
            string strSQL = "";
            if (cbo_Name.Text !="" && cbo_Name.Text != null)
            {
                strSQL = "SELECT pcatm.pcatname as pcatname,matnum,matname,uomcode FROM matm " +
                            "left join pcatm on matm.pcatcode=pcatm.pcatcode " +
                            "where matname like '%" + cbo_Name.Text.ToString() + "%' and matm.flag='MATMSTK'  ORDER BY matname";
            }

            if (strSQL != "")
            {
                ds = this.dbaccess.ReadSQLTemp("matm", strSQL);
                dt = ds.Tables["matm"];

                dt_view = dt.Copy();
                dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
                dt_view.Columns[4].DefaultValue = 0;

                this.dgv_Filter.DataSource = dt_view;
            }
        }

        //private void cbo_CNName_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string strSQL = "";
        //    if (cbo_Name.SelectedItem != System.DBNull.Value && cbo_CNName.SelectedItem != null)
        //    {
        //        strSQL = "SELECT pcatm.pcatname as pcatname,matnum,matname,uomcode FROM matm " +
        //                    "left join pcatm on matm.pcatcode=pcatm.pcatcode " +
        //                    "where cnmatname like '%" + cbo_CNName.SelectedItem.ToString() + "%'  ORDER BY cnmatname";
        //    }

        //    if (strSQL != "")
        //    {
        //        ds = this.dbaccess.ReadSQLTemp("matm", strSQL);
        //        dt = ds.Tables["matm"];

        //        dt_view = dt.Copy();
        //        dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
        //        dt_view.Columns[4].DefaultValue = 0;

        //        this.dgv_Filter.DataSource = dt_view;
        //    }
        //}

        //private void cbo_CNName_KeyDown(object sender, KeyEventArgs e)
        //{
        //    string strSQL = "";
        //    if (cbo_CNName.Text != "" && cbo_CNName.Text != null)
        //    {
        //        strSQL = "SELECT pcatm.pcatname as pcatname,matnum,matname,cnmatname,uomcode FROM matm " +
        //                    "left join pcatm on matm.pcatcode=pcatm.pcatcode " +
        //                    "where cnmatname like '%" + cbo_CNName.Text.ToString() + "%'  ORDER BY cnmatname";
        //    }

        //    if (strSQL != "")
        //    {
        //        ds = this.dbaccess.ReadSQLTemp("matm", strSQL);
        //        dt = ds.Tables["matm"];

        //        dt_view = dt.Copy();
        //        dt_view.Columns.Add("Mark", System.Type.GetType("System.Boolean"));
        //        dt_view.Columns[4].DefaultValue = 0;

        //        this.dgv_Filter.DataSource = dt_view;
        //    }
        //}

        private void btn_UnmarkAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgr in dgv_Filter.Rows)
            {
                dgr.Cells["Mark"].Value = 0;
                dgr.Selected = false;
            }
        }

        private void btn_MarkAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgr in dgv_Filter.Rows)
            {
                dgr.Cells["Mark"].Value = 1;
                dgr.Selected = true;
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Extract_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow row in this.dt_view.Rows)
                {
                    if (row["Mark"] != System.DBNull.Value)
                    {
                        if ((bool)row["Mark"] == true)
                        {
                            DataRow oriTabletmp = dbaccess.DataSet.Tables[this.oriTable.TableName].NewRow();
                            foreach (DataColumn dc in dt_view.Columns)
                            {
                                if (dbaccess.DataSet.Tables[this.oriTable.TableName].Columns.IndexOf(dc.ColumnName) != -1)
                                {
                                    oriTabletmp[dc.ColumnName] = row[dc.ColumnName];
                                }
                            }
                            oriTabletmp["Mark"] = 0;
                            NewRow(row);
                            //after extract,reset as unmarked
                            row["Mark"] = 0;
                        }
                    }
                }

                MessageBox.Show("Extract is successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                //this.Close();
            }
        }

        protected void NewRow(DataRow dr)
        {
            oriTable = dbaccess.DataSet.Tables[this.oriTable.TableName];
            DataRow newRow = oriTable.NewRow();
            newRow["matnum"] = dr["matnum"];
            oriTable.Rows.Add(newRow);
        }

        private void dgv_Click(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv.CurrentRow != null) // if at least 1 row is selected
            {
                foreach (DataGridViewRow dgr in dgv.Rows)
                {
                    if (dgr.Cells["Mark"].Value.ToString() == "True")
                    {
                        dgr.Selected = true;
                    }
                    if (dgr.Cells["Mark"].Value.ToString() == "False")
                    {
                        dgr.Selected = false;
                    }
                }
            }
        }
    }
}