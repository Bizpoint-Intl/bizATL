using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BizRAD.BizReport;
using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizCommon;
using ATL.BizLogicTools;
using BizRAD.BizAccounts;
using BizRAD.BizVoucher;

namespace ATL.BizModules.SADJ
{
    public partial class SetAllocationForm1 : Form
    {
        protected DataTable dt1 = null;
        protected DBAccess dbaccess = null;
        public SetAllocationForm1()
        {
            this.dbaccess = new DBAccess();
            InitializeComponent();
        }

        private void btn_Check_Click(object sender, EventArgs e)
        {
            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow dr1 in dt1.Rows)
                {
                    if (dr1.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(dr1["effectivedate"]))
                        {
                            //DateTime dttest = new DateTime(2015, 9, 30);
                            
                            DateTime datetime1 = Convert.ToDateTime(Convert.ToDateTime(ATL.TimeUtilites.TimeTools.GetSafeDate(dr1["effectivedate"].ToString())).ToShortDateString());
                            if (datetime1 <= DateTime.Today)
                            //if (datetime1 <= dttest)
                            {
                                this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("DELETE FROM SITMT8 WHERE empnum = '" + dr1["empnum"].ToString() + "'");

                                int GetMaxID = BizLogicTools.Tools.getMaxID("SITMT8", this.dbaccess);

                                string getSiteTemplate = "";
                                if (!BizFunctions.IsEmpty(dr1["sitenumt"]))
                                {
                                    getSiteTemplate = "Select * from sitmt where sitenum='" + dr1["newsitenum"].ToString() + "' and sitenumt='" + dr1["sitenumt"].ToString().Trim() + "' and [status]<>'V' ";
                                }
                                else
                                {
                                    getSiteTemplate = "Select * from sitmt where sitenum='" + dr1["newsitenum"].ToString() + "' and [status]<>'V' ";
                                }


                                this.dbaccess.ReadSQL("TempSitmtMain", getSiteTemplate);

                                DataTable TempSitmtMain = this.dbaccess.DataSet.Tables["TempSitmtMain"];

                                if (TempSitmtMain.Rows.Count > 0)
                                {
                                    try
                                    {
                                        string InsertEmp = " INSERT INTO SITMT8 " +
                                                           "( " +
                                                           "id " +
                                                           ",empnum " +
                                                           ",empname " +
                                                           ",sitenum " +
                                                           ",sitenumt " +
                                                           ",ctrnum " +
                                                           ",remark " +
                                                           ",xday1 " +
                                                           ",xday2 " +
                                                           ",xday3 " +
                                                           ",xday4 " +
                                                           ",xday5 " +
                                                           ",xday6 " +
                                                           ",xday7 " +
                                                           ",ispubhol " +
                                                           ",[guid] " +
                                                           ",[status] " +
                                                           ",created" +
                                                           ",modified " +
                                                           ",flag " +
                                                           ",[user] " +
                                                           ") " +
                                                           "VALUES " +
                                                           "( " +
                                                           " " + Convert.ToString(GetMaxID + 1) + " " +
                                                            ",'" + dr1["empnum"].ToString() + "' " +
                                                            ",'" + BizLogicTools.Tools.GetEmpname(dr1["empnum"].ToString(), this.dbaccess) + "' " +                                                            
                                                           ",'" + TempSitmtMain.Rows[0]["sitenum"].ToString() + "' " +
                                                           ",'" + TempSitmtMain.Rows[0]["sitenumt"].ToString() + "' " +
                                                           ",'" + TempSitmtMain.Rows[0]["ctrnum"].ToString() + "' " +
                                                           ",'TRANFER - " + dr1["refnum"].ToString() + " on " + dr1["effectivedate"].ToString() + "' " +
                                                           ",'" + dr1["newMonToFriShiftCode"].ToString() + "' " +
                                                           ",'" + dr1["newMonToFriShiftCode"].ToString() + "' " +
                                                           ",'" + dr1["newMonToFriShiftCode"].ToString() + "' " +
                                                           ",'" + dr1["newMonToFriShiftCode"].ToString() + "' " +
                                                           ",'" + dr1["newMonToFriShiftCode"].ToString() + "' " +
                                                           ",'" + dr1["newSatShiftCode"].ToString() + "' " +
                                                           ",'" + dr1["newSunShiftCode"].ToString() + "' " +
                                                           ",'" + dr1["newPHShiftCode"].ToString() + "' " +
                                                           ",'" + BizLogicTools.Tools.getGUID() + "' " +
                                                           ",'O' " +
                                                           ",GETDATE() " +
                                                           ",GETDATE() " +
                                                           ",'SITMT' " +
                                                           ",'" + Common.DEFAULT_SYSTEM_USERNAME + "' " +
                                                           ") ";
                                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(InsertEmp);

                                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SYSID SET LASTID=(SELECT ISNULL(MAX(ID),0) FROM SITMT8) WHERE TABLENAME='SITMT8'");


                                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SADJH SET transferupdateStatus='TRANSFERED' where refnum='" + dr1["refnum"].ToString() + "' ");
                                        this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("UPDATE SADJH_TEMP SET transferupdateStatus='TRANSFERED' where refnum='" + dr1["refnum"].ToString() + "' ");


                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.ToString(), "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }

                                }
                            }
                        }
                    }
                }
            }

            GetAllocationLists();
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            GetAllocationLists();
        }

        private void GetAllocationLists()
        {
            int month = Convert.ToInt16(Month_Cb.Text);
            int year = Convert.ToInt16(Year_Cb.Text);

            if (dt1.Rows.Count > 0)
            {
                BizFunctions.DeleteAllRows(dt1);
            }

            string get1 = "Select refnum,nric,empnum,empname,sitenum,newsitenum,sitenumt, transferupdateStatus,newsaleffectivedate,newMonToFriShiftCode,newSatShiftCode,newSunShiftCode,newPHShiftCode from sadjh_temp where sadjmcode like '%Transfer%' and transferupdateStatus is null and MONTH(newsaleffectivedate)=" + month.ToString() + " AND YEAR(newsaleffectivedate)=" + year.ToString() + " and (Select case when transferupdateStatus='TRANSFERED' THEN 1 ELSE 0 END AS transferupdateStatus FROM SADJH WHERE SADJH.REFNUM=sadjh_temp.REFNUM)=0 ";

            this.dbaccess.ReadSQL("TempSadjhStat", get1);

            DataTable TempSadjhStat = this.dbaccess.DataSet.Tables["TempSadjhStat"];

            if (TempSadjhStat.Rows.Count > 0)
            {
                foreach (DataRow dr1 in TempSadjhStat.Rows)
                {
                    InsertRowStatus(dr1["refnum"].ToString(), dr1["nric"].ToString(), dr1["empnum"].ToString(), dr1["empname"].ToString(), dr1["sitenum"].ToString(), dr1["newsitenum"].ToString(), dr1["sitenumt"].ToString(), dr1["transferupdateStatus"].ToString(), BizFunctions.GetSafeDateString(Convert.ToDateTime(dr1["newsaleffectivedate"])), dr1["newMonToFriShiftCode"].ToString(), dr1["newSatShiftCode"].ToString(), dr1["newSunShiftCode"].ToString(), dr1["newPHShiftCode"].ToString());
                }
            }
            
        }

        private void setYear()
        {
            int[] arr1 = new int[100];
            DateTime dt = DateTime.Now;

            int Year = dt.AddYears(-10).Year;

            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = Year;
                Year = Year + 1;
            }

            Year_Cb.DataSource = arr1;
            Year_Cb.Text = dt.Year.ToString();
        }


        private void setMonth()
        {
            int[] arr1 = new int[12];
            DateTime dt = DateTime.Now;

            int Month = 0;

            for (int i = 0; i < 12; i++)
            {

                Month = Month + 1;
                arr1[i] = Month;
            }

            Month_Cb.DataSource = arr1;
            Month_Cb.Text = dt.Month.ToString();
          
        }

        private void SetAllocationForm1_Load(object sender, EventArgs e)
        {
            //refnum,nric,empnum,empname,sitenum,newsitenum, transferupdateStatus,newsaleffectivedate
            dt1 = new DataTable("dt1");

            dt1.Columns.Add("refnum", typeof(string));
            dt1.Columns.Add("nric", typeof(string));
            dt1.Columns.Add("empnum", typeof(string));
            dt1.Columns.Add("empname", typeof(string));
            dt1.Columns.Add("sitenum", typeof(string));          
            dt1.Columns.Add("newsitenum", typeof(string));
            dt1.Columns.Add("sitenumt", typeof(string));
            dt1.Columns.Add("status", typeof(string));
            dt1.Columns.Add("effectivedate", typeof(string));

            dt1.Columns.Add("newMonToFriShiftCode", typeof(string));
            dt1.Columns.Add("newSatShiftCode", typeof(string));
            dt1.Columns.Add("newSunShiftCode", typeof(string));
            dt1.Columns.Add("newPHShiftCode", typeof(string));

            BindingSource nbindingSource = new BindingSource();


            dt1.ColumnChanged += new DataColumnChangeEventHandler(dt1_ColumnChanged);
            nbindingSource.DataSource = dt1;
            dataGridView1.DataSource = nbindingSource;

            setMonth();
            setYear();
            GetAllocationLists();
        }

        void dt1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            
        }


        private void InsertRowStatus(string refnum, string nric, string empnum, string empname, string sitenum, string newsitenum, string sitenumt, string status, string date,string newMonToFriShiftCode,string newSatShiftCode,string newSunShiftCode, string newPHShiftCode)
        {
            DataRow dr1 = dt1.NewRow();

            dr1["refnum"] = refnum;
            dr1["nric"] = nric;
            dr1["empnum"] = empnum;
            dr1["empname"] = empname;
            dr1["sitenum"] = sitenum;
            dr1["newsitenum"] = newsitenum;
            dr1["sitenumt"] = sitenumt;
            dr1["status"] = status;
            dr1["effectivedate"] = date;

            dr1["newMonToFriShiftCode"] = newMonToFriShiftCode;
            dr1["newSatShiftCode"] = newSatShiftCode;
            dr1["newSunShiftCode"] = newSunShiftCode;
            dr1["newPHShiftCode"] = newPHShiftCode;



            dt1.Rows.Add(dr1);
        }

    }
}