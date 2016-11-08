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

namespace ATL.ExtractWORpics
{
    public partial class ExtractWORpics : Form
    {
        protected DBAccess dbaccess = null;
        protected string Reference, Guid = "";


        public ExtractWORpics(DBAccess da, string refnum, string guid)
        {
            InitializeComponent();
            this.dbaccess = da;
            this.Reference = refnum;
            this.Guid = guid;                
        }

        private void ExtractWORpics_Load(object sender, EventArgs e)
        {
            if (!HasData())
            {
                MessageBox.Show("No Pictures were taken for this Service", "No Pictures", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.dbaccess.DataSet.Tables["getPicsTB"].Dispose();
                this.Dispose();                
                return;
            }
        }

        private bool HasData()
        {
            bool hasData = false;
            string getPics = "Select ID,photourl,remark,created from WOR5 where refnum='" + Reference + "' and [guid]='" + Guid + "'";

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
                        drNew[drNew.Count - 1].CreateCells(PhotoDGV1, null, getPicsTB.Rows[i]["remark"].ToString(), Convert.ToDateTime(getPicsTB.Rows[i]["created"]).ToShortDateString(), getPicsTB.Rows[i]["photourl"].ToString(), getPicsTB.Rows[i]["ID"].ToString(),null);
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
            this.Dispose();
            this.dbaccess.DataSet.Tables["getPicsTB"].Dispose();
            return;
        }

        private void PhotoDGV1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.RowIndex < PhotoDGV1.Rows.Count - 1)
            {
                if (e.ColumnIndex == 0)
                {
                    Process.Start(PhotoDGV1.Rows[e.RowIndex].Cells["photourl"].Value.ToString());
                }
                if (e.ColumnIndex == 5) 
                {
                    this.dbaccess.RemoteStandardSQL.ExecuteNonQuery("Delete from WOR5 WHERE ID="+PhotoDGV1.Rows[e.RowIndex].Cells["ID"].Value.ToString()+"");
                    if (!HasData())
                    {
                        MessageBox.Show("No More Pictures left for this Service", "No More Pictures", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.dbaccess.DataSet.Tables["getPicsTB"].Dispose();
                        this.Dispose();
                        return;
                    }
                }
            }
        }


   
       
    }
}