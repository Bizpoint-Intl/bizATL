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
        /// 构造函数
        /// </summary>
        /// <param name="dbaccess">数据操作类</param>
        /// <param name="strModuleName">模块名称比如：DOR</param>
        /// <param name="strInfoFile">DATAGRID的纤细信息比如：DetailGridInfo_Extract.xml</param>
        /// <param name="strHeaderText">EXTRACT的标题比如：查找申请详细</param>
        /// <param name="searchcolumns">查询的列列表比如：           
        /// string[][] searchcolumns = new string[][] 
        /// { 
        ///     new string[] { "客户编码", "客户名称","申请编号","商品编码"},
        ///     new string[] { "CustomerCode", "CustomerName" ,"refnum","ProductCode"}
        /// }</param>
        /// <param name="copycolumns">需要拷贝的列列表比如：上面一行是EXTRACT中的列，下面一行是需要EXTRACT到的列   
        /// string[][] copycolumns = new string[][] 
        /// { 
        ///     new string[] { "refnum", "ProductCode","Amount","Price"},
        ///     new string[] { "DRQNo", "ProductCode" ,"DRQAmount","Price"}
        /// };</param>
        /// <param name="comparecolumns"> 对比列列表，用来删除已经EXTRACT过的数据比如：上面一行是EXTRACT中的列，下面一行是需要EXTRACT到的列              
        /// string[][] comparecolumns = new string[][] 
        /// { 
        ///     new string[] { "refnum", "ProductCode"},
        ///     new string[] { "DRQNo", "ProductCode" }
        /// };</param>
        /// <param name="sql">需要执行的SQL语句，这个是DATAGRID列的数据来源,只支持查询语句不支持存储过程</param>
        /// <param name="totable">需要EXTRACT数据到的表比如：DOR1</param>
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
        /// FORM加载事件
        /// </summary>
        public void Form_load()
        {
            #region Binding 显示窗体
            this.frmExtract = BizXmlReader.CurrentInstance.Load(Common.ProjectPath.Replace("BizModules", "DefaultUIFile") + @"\DEFAULT_DOCUMENT_EXTRACT.xml", "frm_Extract", this, null) as Form;
            BizBinding.Binding(this.frmExtract, this.dbAccess.DataSet);
            this.frmExtract.Text = this.formText;
            //查询条件
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
            //排序条件
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
            //显示窗体
            this.frmExtract.ShowDialog();
            #endregion
        }

        /// <summary>
        /// 为DATAGRID绑定数据
        /// </summary>
        /// <param name="sql"></param>
        private void BindDataToGrid(string sql) 
        {
            //为DataGrid绑定数据
            DataGrid dg_Detail = (DataGrid)BizXmlReader.CurrentInstance.GetControl(this.frmExtract.Name, "dg_Extract");
            dg_Detail.TableStyles.Clear();
            BizGridInfo bizGridInfo = (BizGridInfo)BizXmlReader.CurrentInstance.Load(Common.ProjectPath + @"/" + this.moduleName + "/InfoFile/" + this.infoFile + "", "detailGridInfo", null, null);
            this.mainTable = bizGridInfo.MainTable;
            this.dbAccess.ReadSQL(this.mainTable, sql);
            if (!this.dbAccess.DataSet.Tables[this.mainTable].Columns.Contains("mark"))
            {
                this.dbAccess.DataSet.Tables[this.mainTable].Columns.Add("mark", typeof(bool));
            }
            //清除已经有了的数据
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
            //如果mainTable的数据行数不为0，则btn_Extract_Extract按钮为可用
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
                //MessageBox.Show("没有数据", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 过滤事件
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
            //清除已经有了的数据
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
            //如果mainTable的数据行数不为0，则btn_Extract_Extract按钮为可用
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
                //MessageBox.Show("没有数据", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 抽取事件
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
                MessageBox.Show( ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        /// <summary>
        /// 标注事件
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
        /// 反标注事件
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
        /// 退出事件
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
