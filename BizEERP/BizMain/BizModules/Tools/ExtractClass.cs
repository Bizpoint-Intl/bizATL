using System;
using System.Collections.Generic;
using System.Text;
using BizRAD.DB.Client;
using System.Windows.Forms;
using System.Data;
using BizRAD.BizCommon;
using BizRAD.BizTools;
using BizRAD.BizXml;
using BizRAD.BizBase;
using System.Collections;
using BizRAD.BizAccounts;

namespace ATL.ExtractTools
{
    public class ExtractClass
    {
        protected DBAccess dbAccess = null;
        protected string moduleName = "";
        protected string infoFile = "";
        protected string formText = "";
        protected string[][] searchColumns = null;
        protected string[][] copyColumns = null;
        protected string[][] compareColumns = null;
        protected Form frmExtract = null;
        protected string mainTable = "";
        protected string strSql = "";
        protected string toTable = "";
        protected DataTable originalTable = null;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="dbaccess">���ݲ�����</param>
        /// <param name="strModuleName">ģ�����Ʊ��磺DOR</param>
        /// <param name="strInfoFile">DATAGRID����ϸ��Ϣ���磺DetailGridInfo_Extract.xml</param>
        /// <param name="strHeaderText">EXTRACT�ı�����磺����������ϸ</param>
        /// <param name="searchcolumns">��ѯ�����б���磺           
        /// string[][] searchcolumns = new string[][] 
        /// { 
        ///     new string[] { "�ͻ�����", "�ͻ�����","������","��Ʒ����"},
        ///     new string[] { "CustomerCode", "CustomerName" ,"refnum","ProductCode"}
        /// }</param>
        /// <param name="copycolumns">��Ҫ���������б���磺����һ����EXTRACT�е��У�����һ������ҪEXTRACT������   
        /// string[][] copycolumns = new string[][] 
        /// { 
        ///     new string[] { "refnum", "ProductCode","Amount","Price"},
        ///     new string[] { "DRQNo", "ProductCode" ,"DRQAmount","Price"}
        /// };</param>
        /// <param name="comparecolumns"> �Ա����б�����ɾ���Ѿ�EXTRACT�������ݱ��磺����һ����EXTRACT�е��У�����һ������ҪEXTRACT������              
        /// string[][] comparecolumns = new string[][] 
        /// { 
        ///     new string[] { "refnum", "ProductCode"},
        ///     new string[] { "DRQNo", "ProductCode" }
        /// };</param>
        /// <param name="sql">��Ҫִ�е�SQL��䣬�����DATAGRID�е�������Դ,ֻ֧�ֲ�ѯ��䲻֧�ִ洢����</param>
        /// <param name="totable">��ҪEXTRACT���ݵ��ı���磺DOR1</param>
        public ExtractClass(DBAccess dbaccess, string strModuleName, string strInfoFile, string strHeaderText, string[][] searchcolumns, string[][] copycolumns, string[][] comparecolumns, string sql, string totable)
        {
            this.dbAccess = dbaccess;
            this.moduleName = strModuleName;
            this.infoFile = strInfoFile;
            this.formText = strHeaderText;
            this.searchColumns = searchcolumns;
            this.copyColumns = copycolumns;
            this.compareColumns = comparecolumns;
            this.strSql = sql;
            this.toTable = totable;
        }

        /// <summary>
        /// FORM�����¼�
        /// </summary>
        public void Form_load()
        {
            #region Binding ��ʾ����
            this.frmExtract = BizXmlReader.CurrentInstance.Load(Common.ProjectPath.Replace("BizModules", "DefaultUIFile") + @"\DEFAULT_DOCUMENT_EXTRACT.xml", "frm_Extract", this, null) as Form;
            BizBinding.Binding(this.frmExtract, this.dbAccess.DataSet);
            this.frmExtract.Text = this.formText;
            //��ѯ����
            DataTable dtComboBox = new DataTable();
            dtComboBox.Columns.Add("Value");
            dtComboBox.Columns.Add("Text");
            DataRow drEmpty = dtComboBox.NewRow();
            drEmpty["Text"] = "";
            drEmpty["Value"] = "";
            dtComboBox.Rows.Add(drEmpty);
            for (int i = 0; i < this.searchColumns[0].Length; i++)
            {
                string columnName = searchColumns[0][i];
                string codeName = searchColumns[1][i];
                if (i > 3)
                {
                    break;
                }
                Label lableTemp = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "lbl_Extract_Filter" + i.ToString()) as Label;
                lableTemp.Visible = true;
                lableTemp.Text = columnName;
                TextBox txtFrom = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "txt_Extract_From" + i.ToString()) as TextBox;
                txtFrom.Visible = true;
                TextBox txtTo = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "txt_Extract_To" + i.ToString()) as TextBox;
                txtTo.Visible = true;
                DataRow drNew = dtComboBox.NewRow();
                drNew["Text"] = columnName;
                drNew["Value"] = codeName;
                dtComboBox.Rows.Add(drNew);
            }
            //��������
            for (int i = 0; i < this.searchColumns[0].Length; i++)
            {
                string columnName = searchColumns[0][i];
                string codeName = searchColumns[1][i];
                if (i > 3)
                {
                    break;
                }
                ComboBox cmbTemp = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "cmb_Extract_Sort" + i.ToString()) as ComboBox;
                cmbTemp.Visible = true;
                DataTable dtTemp = dtComboBox.Copy();
                cmbTemp.DataSource = dtTemp;
                cmbTemp.DisplayMember = "Text";
                cmbTemp.ValueMember = "Value";
                cmbTemp.SelectedIndex = i + 1;
            }
            this.BindDataToGrid(this.strSql);
            //��ʾ����
            this.frmExtract.ShowDialog();
            #endregion
        }

        /// <summary>
        /// ΪDATAGRID������
        /// </summary>
        /// <param name="sql"></param>
        private void BindDataToGrid(string sql) 
        {
            //ΪDataGrid������
            DataGrid dg_Detail = (DataGrid)BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "dg_Extract");
            dg_Detail.TableStyles.Clear();
            BizGridInfo bizGridInfo = (BizGridInfo)BizXmlReader.CurrentInstance.Load(Common.ProjectPath + @"/" + this.moduleName + "/InfoFile/" + this.infoFile + "", "detailGridInfo", null, null);
            this.mainTable = bizGridInfo.MainTable;
            this.dbAccess.ReadSQL(this.mainTable, sql);
            if (!this.dbAccess.DataSet.Tables[this.mainTable].Columns.Contains("mark"))
            {
                this.dbAccess.DataSet.Tables[this.mainTable].Columns.Add("mark", typeof(bool));
            }
            //����Ѿ����˵�����
            foreach (DataRow drTo in this.dbAccess.DataSet.Tables[this.toTable].Rows)
            {
                if (drTo.RowState == DataRowState.Deleted || drTo.RowState == DataRowState.Detached)
                {
                    continue;
                }
                for (int j = this.dbAccess.DataSet.Tables[this.mainTable].Rows.Count - 1; j > -1; j--)
                {
                    DataRow drFrom = this.dbAccess.DataSet.Tables[this.mainTable].Rows[j];
                    if (drFrom.RowState == DataRowState.Deleted || drFrom.RowState == DataRowState.Detached)
                    {
                        continue;
                    }
                    string strFrom = "";
                    string strTo = "";
                    for (int i = 0; i < this.compareColumns[0].Length; i++)
                    {
                        string fromColumn = compareColumns[0][i];
                        string toColumn = compareColumns[1][i];
                        strFrom += drFrom[fromColumn].ToString();
                        strTo += drTo[toColumn].ToString();
                    }
                    if (strFrom.ToUpper() == strTo.ToUpper() && strFrom != "" && strTo != "")
                    {
                        drFrom.Delete();
                    }
                }
            }
            this.originalTable = this.dbAccess.DataSet.Tables[this.mainTable].Copy();
            //���mainTable������������Ϊ0����btn_Extract_Extract��ťΪ����
            int iCount = 0;
            foreach (DataRow dr in this.dbAccess.DataSet.Tables[this.mainTable].Rows)
            {
                if (dr.RowState == DataRowState.Deleted || dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
                iCount++;
            }
            Button btn_Extract = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "btn_Extract_Extract") as Button;
            if (iCount > 0)
            {
                btn_Extract.Enabled = true;
            }
            else 
            {
                btn_Extract.Enabled = false;
                //MessageBox.Show("û������", "ϵͳ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            BizBinding.DataGridBinding(dg_Detail, this.dbAccess.DataSet, bizGridInfo, null, string.Empty, string.Empty, DataViewRowState.CurrentRows);
            (dg_Detail.DataSource as DataView).AllowNew = false;
            DataView dvTemp = (DataView)dg_Detail.DataSource;
            string strSort = "";
            for (int i = 0; i < this.searchColumns[0].Length; i++)
            {
                string columnName = searchColumns[0][i];
                string codeName = searchColumns[1][i];
                if (i > 3)
                {
                    break;
                }
                ComboBox cmbTemp = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "cmb_Extract_Sort" + i.ToString()) as ComboBox;
                if (cmbTemp.SelectedValue.ToString() != "") 
                {
                    if (strSort == "")
                    {
                        strSort += cmbTemp.SelectedValue.ToString();
                    }
                    else 
                    {
                        strSort += "," + cmbTemp.SelectedValue.ToString();
                    }
                }
            }
            dvTemp.Sort = strSort;
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Filter_Click(object sender, EventArgs e)
        {
            string Sql = "";
            string strConditon = "";
            for (int i = 0; i < this.searchColumns[0].Length; i++)
            {
                string columnName = searchColumns[0][i];
                string codeName = searchColumns[1][i];
                if (i > 3)
                {
                    break;
                }
                TextBox txtFrom = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "txt_Extract_From" + i.ToString()) as TextBox;
                if (txtFrom.Text.Trim() != "") 
                {
                    if (strConditon == "")
                    {
                        strConditon += " a." + codeName + " >= '" + txtFrom.Text.Trim().Replace("'", "''") + "' ";
                    }
                    else 
                    {
                        strConditon += " and a." + codeName + " >= '" + txtFrom.Text.Trim().Replace("'", "''") + "' ";
                    }
                }
                TextBox txtTo = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "txt_Extract_To" + i.ToString()) as TextBox;
                if (txtTo.Text.Trim() != "")
                {
                    if (strConditon == "")
                    {
                        strConditon += " a." + codeName + " <= '" + txtTo.Text.Trim().Replace("'", "''") + "' ";
                    }
                    else
                    {
                        strConditon += " and a." + codeName + " <= '" + txtTo.Text.Trim().Replace("'", "''") + "' ";
                    }
                }
            }
            Sql = " select * from " + this.mainTable + " as a  ";
            if (strConditon != "") 
            {
                Sql += " where " + strConditon;
            }
            //this.BindDataToGrid(Sql);
            DataSet ds = new DataSet();
            ds.Tables.Add(this.originalTable.Copy());
            DataTable dt = BizFunctions.ExecuteQuery(ds, Sql);
            dt.TableName = this.mainTable;
            if (this.dbAccess.DataSet.Tables.Contains(this.mainTable)) 
            {
                this.dbAccess.DataSet.Tables.Remove(this.dbAccess.DataSet.Tables[this.mainTable]);
            }
            this.dbAccess.DataSet.Tables.Add(dt);
            //����Ѿ����˵�����
            foreach (DataRow drTo in this.dbAccess.DataSet.Tables[this.toTable].Rows)
            {
                if (drTo.RowState == DataRowState.Deleted || drTo.RowState == DataRowState.Detached)
                {
                    continue;
                }
                for (int j = this.dbAccess.DataSet.Tables[this.mainTable].Rows.Count - 1; j > -1; j--)
                {
                    DataRow drFrom = this.dbAccess.DataSet.Tables[this.mainTable].Rows[j];
                    if (drFrom.RowState == DataRowState.Deleted || drFrom.RowState == DataRowState.Detached)
                    {
                        continue;
                    }
                    string strFrom = "";
                    string strTo = "";
                    for (int i = 0; i < this.compareColumns[0].Length; i++)
                    {
                        string fromColumn = compareColumns[0][i];
                        string toColumn = compareColumns[1][i];
                        strFrom += drFrom[fromColumn].ToString();
                        strTo += drTo[toColumn].ToString();
                    }
                    if (strFrom.ToUpper() == strTo.ToUpper() && strFrom != "" && strTo != "")
                    {
                        drFrom.Delete();
                    }
                }
            }
            DataGrid dg_Detail = (DataGrid)BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "dg_Extract");
            dg_Detail.TableStyles.Clear();
            BizGridInfo bizGridInfo = (BizGridInfo)BizXmlReader.CurrentInstance.Load(Common.ProjectPath + @"/" + this.moduleName + "/InfoFile/" + this.infoFile + "", "detailGridInfo", null, null);
            //���mainTable������������Ϊ0����btn_Extract_Extract��ťΪ����
            int iCount = 0;
            foreach (DataRow dr in this.dbAccess.DataSet.Tables[this.mainTable].Rows)
            {
                if (dr.RowState == DataRowState.Deleted || dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
                iCount++;
            }
            Button btn_Extract = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "btn_Extract_Extract") as Button;
            if (iCount > 0)
            {
                btn_Extract.Enabled = true;
            }
            else
            {
                btn_Extract.Enabled = false;
                //MessageBox.Show("û������", "ϵͳ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            BizBinding.DataGridBinding(dg_Detail, this.dbAccess.DataSet, bizGridInfo, null, string.Empty, string.Empty, DataViewRowState.CurrentRows);
            (dg_Detail.DataSource as DataView).AllowNew = false;
            DataView dvTemp = (DataView)dg_Detail.DataSource;
            string strSort = "";
            for (int i = 0; i < this.searchColumns[0].Length; i++)
            {
                string columnName = searchColumns[0][i];
                string codeName = searchColumns[1][i];
                if (i > 3)
                {
                    break;
                }
                ComboBox cmbTemp = BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "cmb_Extract_Sort" + i.ToString()) as ComboBox;
                if (cmbTemp.SelectedValue.ToString() != "")
                {
                    if (strSort == "")
                    {
                        strSort += cmbTemp.SelectedValue.ToString();
                    }
                    else
                    {
                        strSort += "," + cmbTemp.SelectedValue.ToString();
                    }
                }
            }
            dvTemp.Sort = strSort;
        }

        /// <summary>
        /// ��ȡ�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Extract_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow row in this.dbAccess.DataSet.Tables[this.mainTable].Rows)
                {
                    if (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached)
                    {
                        continue;
                    }
                    if (row["mark"].ToString().ToUpper() != "TRUE")
                    {
                        continue;
                    }
                    DataRow drNew = this.dbAccess.DataSet.Tables[this.toTable].NewRow();
                    drNew["created"] = BizFunctions.GetStandardDateString((DateTime)System.DateTime.Now.Date);
                    for (int i = 0; i < this.copyColumns[0].Length; i++) 
                    {
                        string fromColumn = copyColumns[0][i];
                        string toColumn = copyColumns[1][i];
                        drNew[toColumn] = row[fromColumn];
                    }
                    this.dbAccess.DataSet.Tables[this.toTable].Rows.Add(drNew);
                }
                this.UpdateLineForTable(this.dbAccess.DataSet.Tables[this.toTable]);
                this.btn_Filter_Click(null, null);
                //this.Form_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        /// <summary>
        /// ��ע�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_MarkAll_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in this.dbAccess.DataSet.Tables[this.mainTable].Rows)
            {
                if (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached)
                {
                    continue;
                }
                row["Mark"] = 1;
            }
        }

        /// <summary>
        /// ����ע�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_UnMarkAll_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in this.dbAccess.DataSet.Tables[this.mainTable].Rows)
            {
                if (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached)
                {
                    continue;
                }
                row["Mark"] = 0;
            }
        }

        /// <summary>
        /// �˳��¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Exit_Click(object sender, EventArgs e)
        {
            Form form = BizXmlReader.CurrentInstance.GetForm((sender as Control).Parent.Name) as Form;
            form.Close();
        }

        /// <summary>
        /// update Line For Table
        /// </summary>
        /// <param name="dt">DataTable</param>
        private void UpdateLineForTable(DataTable dt)
        {
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState == DataRowState.Deleted || dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
                dr["Line"] = 100 * i++;
            }
        }

    }
}
