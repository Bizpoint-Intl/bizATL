using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.ComponentModel;

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
using BizRAD.BizAccounts;
using BizRAD.BizVoucher;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Text.RegularExpressions;
using ATL.SortTable;
using ATL.TimeUtilites;
using ATL.BizModules.TextValidator;
using NodaTime;
using System.Diagnostics;
using System.Collections.Generic;

using ATL.BizModules.Tools;

namespace ATL.ExtractHEMPpics1
{
    public partial class ExtractHEMPpics1 : Form
    {
        protected DBAccess dbaccess = null;
        protected string Reference, Guid = "";


        public ExtractHEMPpics1(DBAccess da, string refnum)
        {
            InitializeComponent();
            this.dbaccess = da;
            this.Reference = refnum;
               
        }

        private void ExtractHEMPpics1_Load(object sender, EventArgs e)
        {
            if (!HasData())
            {
                MessageBox.Show("No Pictures were taken for this Employee", "No Pictures", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.dbaccess.DataSet.Tables["getPicsTB"].Dispose();
                this.Dispose();                
                return;
            }
        }

        private bool HasData()
        {
            bool hasData = false;
            string getPics = " Select * from (Select ID,photourl,remark,created,ISNULL(isappear,0) as isappear,ISNULL(orderno,0) as orderno,0 as isNew from HEMP1 where empnum='" + Reference + "')a order by orderno ";

            this.dbaccess.ReadSQL("getPicsTB", getPics);

            DataTable getPicsTB = this.dbaccess.DataSet.Tables["getPicsTB"];

            if (getPicsTB != null)
            {
                if (getPicsTB.Rows.Count > 0)
                {
                    if (PhotoDGV1.Rows.Count > 0)
                    {
                        PhotoDGV1.Rows.Clear();
                    }
                    List<DataGridViewRow> drNew = new List<DataGridViewRow>();
                    for (int i = 0; i < getPicsTB.Rows.Count; i++)
                    {
                        drNew.Add(new DataGridViewRow());
                        //drNew[drNew.Count - 1].CreateCells(PhotoDGV1, null, getPicsTB.Rows[i]["remark"].ToString(), Convert.ToDateTime(getPicsTB.Rows[i]["created"]).ToShortDateString(), getPicsTB.Rows[i]["photourl"].ToString(), getPicsTB.Rows[i]["ID"].ToString(), null);
                        drNew[drNew.Count - 1].CreateCells(PhotoDGV1, null, getPicsTB.Rows[i]["remark"].ToString(), Convert.ToDateTime(getPicsTB.Rows[i]["created"]).ToShortDateString(), getPicsTB.Rows[i]["photourl"].ToString(), getPicsTB.Rows[i]["ID"].ToString(), null, getPicsTB.Rows[i]["isappear"].ToString(), getPicsTB.Rows[i]["orderno"].ToString(), getPicsTB.Rows[i]["isnew"].ToString());

                       
                     

                    }

                    PhotoDGV1.Rows.AddRange(drNew.ToArray());

                    PhotoDGV1.Refresh();
                 
                    hasData = true;

                }
                else
                {
                    hasData = false;
                }
            }

            return hasData;
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            DataTable getPicsTB = this.dbaccess.DataSet.Tables["getPicsTB"];

            if (getPicsTB != null)
            {
                if (getPicsTB.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in getPicsTB.Rows)
                    {
                        if (dr1.RowState != DataRowState.Deleted)
                        {
                            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Update HEMP1 set isappear=" + dr1["isappear"].ToString() + ", orderno=" + dr1["orderno"].ToString() + ", isnew=" + dr1["isNew"].ToString() + " WHERE ID=" + dr1["ID"].ToString() + " ");
                        }
                    }
                }

            }
            this.Dispose();
            this.dbaccess.DataSet.Tables["getPicsTB"].Dispose();
            return;
        }

        private void PhotoDGV1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.RowIndex < PhotoDGV1.Rows.Count)
            {
                if (e.ColumnIndex == 0)
                {
                    Process.Start(PhotoDGV1.Rows[e.RowIndex].Cells["photourl"].Value.ToString());
                }
                if (e.ColumnIndex == 5) 
                {
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from HEMP1 WHERE ID="+PhotoDGV1.Rows[e.RowIndex].Cells["ID"].Value.ToString()+"");
                    if (!HasData())
                    {
                        MessageBox.Show("No More Pictures left for this Employee", "No More Pictures", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.dbaccess.DataSet.Tables["getPicsTB"].Dispose();
                        this.Dispose();
                        return;
                    }
                }
            }
        }

        private void ExtractHEMPpics1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PhotoDGV1.Rows.Count > 0)
            {
                for (int i = 0; i < PhotoDGV1.Rows.Count; i++)
                {
                    int isAppear = 0;
                    if (BizFunctions.IsEmpty(PhotoDGV1.Rows[i].Cells["isappear"].Value))
                    {
                        PhotoDGV1.Rows[i].Cells["isappear"].Value = 0;
                    }
                    if (!BizFunctions.IsEmpty(PhotoDGV1.Rows[i].Cells["isappear"].Value))
                    {
                       if (PhotoDGV1.Rows[i].Cells["isappear"].Value.ToString() =="True")
                       {
                           isAppear = 1;
                       }
                       else
                       {
                           isAppear = 0;
                       }
                    }

                    string query = "Update HEMP1 set isappear=" + isAppear.ToString() + ", orderno=" + PhotoDGV1.Rows[i].Cells["orderno"].Value + ", isnew=" + PhotoDGV1.Rows[i].Cells["isNew"].Value + " WHERE ID=" + PhotoDGV1.Rows[i].Cells["ID"].Value + " ";
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(query);

                }
            }
        }




    }
}