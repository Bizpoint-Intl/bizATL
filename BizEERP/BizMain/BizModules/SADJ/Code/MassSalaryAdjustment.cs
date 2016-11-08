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
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizAccounts;
using BizRAD.BizReport;
using DEMO.MDT;
using ATL.GeneralTools;
using ATL.FilterOP;
using ATL.MultiColumnComboBox;
using System.IO;
using System.Data.OleDb;

using ATL.BizLogicTools;

namespace ATL.BizModules.SADJ
{
    public partial class MassSalaryAdjustment : Form
    {
        protected DBAccess dbaccess = null;
        protected DataTable importTable;

        public MassSalaryAdjustment()
        {
            InitializeComponent();

            this.dbaccess = new DBAccess();

            importTable = new DataTable("importTable");
            importTable.Columns.Add("no", typeof(int));
            importTable.Columns.Add("refnum", typeof(string));
            importTable.Columns.Add("dateadjusted", typeof(string));
            importTable.Columns.Add("empnum", typeof(string));
            importTable.Columns.Add("empname", typeof(string));
            importTable.Columns.Add("nric", typeof(string));
            importTable.Columns.Add("sitenum", typeof(string));
            importTable.Columns.Add("paytypecode", typeof(string));

            importTable.Columns.Add("Increment", typeof(decimal));
            importTable.Columns.Add("oldBasic", typeof(decimal));
            importTable.Columns.Add("newBasic", typeof(decimal)); //
            importTable.Columns.Add("otallw", typeof(decimal)); //
            importTable.Columns.Add("attnallw", typeof(decimal)); //
            importTable.Columns.Add("accomallw", typeof(decimal)); //
            importTable.Columns.Add("tlallw", typeof(decimal)); //
            importTable.Columns.Add("phoneallw", typeof(decimal));
            importTable.Columns.Add("drvtransallw", typeof(decimal));
            importTable.Columns.Add("loyalty", typeof(decimal)); //
            importTable.Columns.Add("gross", typeof(decimal));


            BindingSource bs = new BindingSource();

            bs.DataSource = importTable;

            this.importDV1.DataSource = bs.DataSource;

            if (importDV1.Columns.Contains("Mark"))
            {
                importDV1.Columns["mark"].Dispose();
                DataGridViewCheckBoxColumn mark = new DataGridViewCheckBoxColumn(false);
                mark.Name = "Mark";
                importDV1.Columns.Add(mark);
            }
            else
            {

                DataGridViewCheckBoxColumn mark = new DataGridViewCheckBoxColumn(false);
                mark.Name = "Mark";
                importDV1.Columns.Add(mark);
            }


            DataGridViewColumn Mark = importDV1.Columns["Mark"];
            Mark.Width = 60;
        }

        private void btn_Import_Click(object sender, EventArgs e)
        {
            try
            {
                //DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
                //DataTable importTable = dbaccess.DataSet.Tables["importTable"];
                decimal lineNo = 0;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog_FileOk);
                openFileDialog.Filter = "XLS(*.XLS;*.XLSX;)|*.xls;*.xlsx;|All Files|*.*";
                openFileDialog.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //DataRow bfavh = dbaccess.DataSet.Tables["bfavh"].Rows[0];
                //DataTable importTable = dbaccess.DataSet.Tables["importTable"];
                string Path = (sender as OpenFileDialog).FileName;
                //Read data from Excel,and return the dataset
                DataSet ds = ExcelToDS(Path, "XSL", 0);



                if (importTable.Rows.Count > 0)
                {
                    BizFunctions.DeleteAllRows(importTable);
                }



                for (int i = 0; i < ds.Tables["XSL"].Rows.Count; i++)
                {
                    DataRow insertimportTable = importTable.NewRow();

                    string empnum = ds.Tables["XSL"].Rows[i][1].ToString();

                    if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][1]))
                    {
                        if (ds.Tables["XSL"].Rows[i][1].ToString().ToUpper().Trim() != string.Empty)
                        {
                            insertimportTable["empnum"] = ds.Tables["XSL"].Rows[i][1].ToString();

                            if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][0]))
                            {
                                if (ds.Tables["XSL"].Rows[i][0].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    insertimportTable["no"] = ds.Tables["XSL"].Rows[i][0].ToString();
                                }
                            }



                            if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][2]))
                            {
                                if (ds.Tables["XSL"].Rows[i][2].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    insertimportTable["sitenum"] = ds.Tables["XSL"].Rows[i][2].ToString();
                                }
                            }

                            if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][4]))
                            {
                                if (ds.Tables["XSL"].Rows[i][4].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    insertimportTable["empname"] = ds.Tables["XSL"].Rows[i][4].ToString();
                                }
                            }

                            if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][5]))
                            {
                                if (ds.Tables["XSL"].Rows[i][5].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    insertimportTable["nric"] = ds.Tables["XSL"].Rows[i][5].ToString();
                                }
                            }

                            if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][6]))
                            {
                                if (ds.Tables["XSL"].Rows[i][6].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    insertimportTable["paytypecode"] = ds.Tables["XSL"].Rows[i][6].ToString();
                                }
                            }

                            ////


                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][7]))
                            {
                                if (ds.Tables["XSL"].Rows[i][7].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][7] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][7] = 0;
                                }
                            }
                            //else if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][7]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][7].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][7] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][7] = 0;
                            //    }
                            //}  
                            insertimportTable["increment"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][7]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][8]))
                            {
                                if (ds.Tables["XSL"].Rows[i][8].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][8] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][8] = 0;
                                }
                            }
                            //else if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][8]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][8].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][8] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][8] = 0;
                            //    }
                            //}
                            insertimportTable["oldBasic"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][8]);
                            /////


                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][9]))
                            {
                                if (ds.Tables["XSL"].Rows[i][9].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][9] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][9] = 0;
                                }
                            }
                            //else if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][9]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][9].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][9] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][9] = 0;
                            //    }
                            //}
                            insertimportTable["newBasic"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][9]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][10]))
                            {
                                if (ds.Tables["XSL"].Rows[i][10].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][10] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][10] = 0;
                                }
                            }
                            //else if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][10]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][10].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][10] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][10] = 0;
                            //    }
                            //}
                            insertimportTable["otallw"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][10]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][11]))
                            {
                                if (ds.Tables["XSL"].Rows[i][11].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][11] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][11] = 0;
                                }
                            }
                            //else if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][11]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][11].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][11] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][11] = 0;
                            //    }
                            //}
                            insertimportTable["attnallw"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][11]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][12]))
                            {
                                if (ds.Tables["XSL"].Rows[i][12].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][12] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][12] = 0;
                                }
                            }
                            //else if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][12]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][12].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][12] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][12] = 0;
                            //    }
                            //}
                            insertimportTable["accomallw"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][12]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][13]))
                            {
                                if (ds.Tables["XSL"].Rows[i][13].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][13] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][13] = 0;
                                }
                            }
                            //else if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][13]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][13].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][13] = 0;
                            //    }
                            //    else
                            //    {
                            //       ds.Tables["XSL"].Rows[i][13] = 0;

                            //    }
                            //}
                            insertimportTable["tlallw"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][13]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][14]))
                            {
                                if (ds.Tables["XSL"].Rows[i][14].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][14] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][14] = 0;
                                }
                            }
                            //else if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][14]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][14].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][14] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][14] = 0;
                            //    }
                            //}
                            insertimportTable["phoneallw"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][14]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][15]))
                            {
                                if (ds.Tables["XSL"].Rows[i][15].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][15] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][15] = 0;
                                }
                            }
                            //else if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][15]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][15].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][15] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][15] = 0;
                            //    }
                            //}
                            insertimportTable["drvtransallw"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][15]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][16]))
                            {
                                if (ds.Tables["XSL"].Rows[i][16].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][16] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][16] = 0;
                                }
                            }
                            //else if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][16]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][16].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][16] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][16] = 0;
                            //    }
                            //}
                            insertimportTable["loyalty"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][16]);
                            /////

                            //////
                            if (BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][17]))
                            {
                                if (ds.Tables["XSL"].Rows[i][17].ToString().ToUpper().Trim() != string.Empty)
                                {
                                    ds.Tables["XSL"].Rows[i][17] = 0;
                                }
                                else
                                {
                                    ds.Tables["XSL"].Rows[i][17] = 0;
                                }
                            }
                            //else if (!BizFunctions.IsEmpty(ds.Tables["XSL"].Rows[i][17]))
                            //{
                            //    if (ds.Tables["XSL"].Rows[i][17].ToString().ToUpper().Trim() != string.Empty)
                            //    {
                            //        ds.Tables["XSL"].Rows[i][17] = 0;
                            //    }
                            //    else
                            //    {
                            //        ds.Tables["XSL"].Rows[i][17] = 0;
                            //    }
                            //}
                            insertimportTable["gross"] = Convert.ToDecimal(ds.Tables["XSL"].Rows[i][17]);
                            /////







                            importTable.Rows.Add(insertimportTable);
                        }

                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #region ExcelToDS
        public DataSet ExcelToDS(string Path, string tablename, int sheetIndex)
        {
            string os_platform = System.Environment.OSVersion.Platform.ToString();
            string strConn = "";

            if (BizLogicTools.Tools.Platform == "x86")
            {
                strConn = "Provider = Microsoft.Jet.OLEDB.4.0; " + "Data Source = " + Path + "; " + "Extended Properties = Excel 8.0";
            }
            else
            {
                strConn = "Provider = Microsoft.ACE.OLEDB.12.0; " + "Data Source = " + Path + "; " + "Extended Properties = Excel 8.0";
            }



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

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            if (importTable.Rows.Count > 0)
            {
                for (int i = 0; i < importTable.Rows.Count; i++)
                {
                    if (importTable.Rows[i].RowState != DataRowState.Deleted)
                    {
                        importTable.Rows[i]["dateadjusted"] = BizFunctions.GetSafeDateString(Convert.ToDateTime(dateAdjustPicker1.Text));

                        //importTable = new DataTable("importTable");
                        //importTable.Columns.Add("no", typeof(int));
                        //importTable.Columns.Add("refnum", typeof(string));
                        //importTable.Columns.Add("dateadjusted", typeof(string));
                        //importTable.Columns.Add("empnum", typeof(string));
                        //importTable.Columns.Add("empname", typeof(string));
                        //importTable.Columns.Add("nric", typeof(string));
                        //importTable.Columns.Add("sitenum", typeof(string));
                        //importTable.Columns.Add("paytypecode", typeof(string));

                        //importTable.Columns.Add("Increment", typeof(decimal));
                        //importTable.Columns.Add("oldBasic", typeof(decimal));
                        //importTable.Columns.Add("newBasic", typeof(decimal)); //
                        //importTable.Columns.Add("otallw", typeof(decimal)); //
                        //importTable.Columns.Add("attnallw", typeof(decimal)); //
                        //importTable.Columns.Add("accomallw", typeof(decimal)); //
                        //importTable.Columns.Add("tlallw", typeof(decimal)); //
                        //importTable.Columns.Add("phoneallw", typeof(decimal));
                        //importTable.Columns.Add("drvtransallw", typeof(decimal));
                        //importTable.Columns.Add("loyalty", typeof(decimal)); //
                        //importTable.Columns.Add("gross", typeof(decimal));


                        Parameter[] parameters1 = new Parameter[15];
                        parameters1[0] = new Parameter("@nric", importTable.Rows[i]["nric"].ToString());
                        parameters1[1] = new Parameter("@empnum", importTable.Rows[i]["empnum"].ToString());
                        parameters1[2] = new Parameter("@trandate", importTable.Rows[i]["dateadjusted"].ToString());
                        parameters1[3] = new Parameter("@SADJmcode", "REVI");
                        parameters1[4] = new Parameter("@newsaleffectivedate", importTable.Rows[i]["dateadjusted"].ToString());
                        parameters1[5] = new Parameter("@remark", "Mass Adjustment for " + BizFunctions.GetSafeDateString(Convert.ToDateTime(dateAdjustPicker1.Text))  + " By "+Common.DEFAULT_SYSTEM_USERNAME+" on "+DateTime.Now.ToShortDateString()+"  ");
                        parameters1[6] = new Parameter("@createdby", Common.DEFAULT_SYSTEM_USERNAME);
                        parameters1[7] = new Parameter("@BasicAmt", importTable.Rows[i]["newBasic"].ToString());
                        parameters1[8] = new Parameter("@OTAllowAmt", importTable.Rows[i]["otallw"].ToString());
                        parameters1[9] = new Parameter("@AttendanceAmt", importTable.Rows[i]["attnallw"].ToString());
                        parameters1[10] = new Parameter("@AccomodationAllwAmt", importTable.Rows[i]["accomallw"].ToString());
                        parameters1[11] = new Parameter("@TransportAllowAmt", importTable.Rows[i]["tlallw"].ToString());
                        parameters1[12] = new Parameter("@PhoneAllw", importTable.Rows[i]["phoneallw"].ToString());
                        parameters1[13] = new Parameter("@DrvTransallw", importTable.Rows[i]["drvtransallw"].ToString());
                        parameters1[14] = new Parameter("@loyalty", importTable.Rows[i]["loyalty"].ToString());
             

                        DataSet ds_Ref = this.dbaccess.RemoteStandardSQL.GetStoredProcedureResult("sp_submit_Salary_Adjustment_Mass", ref parameters1);
                        ds_Ref.Tables[0].TableName = "RefnumTB";

                        DataTable ResultTB = ds_Ref.Tables[0];

                        if (ResultTB != null)
                        {
                            if (ResultTB.Rows.Count > 0)
                            {
                                importTable.Rows[i]["refnum"] = ResultTB.Rows[0]["refnum"].ToString();
                                //try
                                //{
                                //    // Detail
                                //    Parameter[] parameters2 = new Parameter[2];
                                //    parameters2[0] = new Parameter("@refnum", ResultTB.Rows[0]["refnum"].ToString());
                                //    parameters2[1] = new Parameter("@sitenum", SiteDGV1.Rows[i].Cells[1].Value);

                                //    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_Insert_Pest_Tasks_Generate", ref parameters2);
                                //}
                                //catch (Exception ex)
                                //{
                                //}

                            }
                        }
                    }
                }

                btn_Confirm.Enabled = false;
                btn_Import.Enabled = false;
            }
        }

    }
}