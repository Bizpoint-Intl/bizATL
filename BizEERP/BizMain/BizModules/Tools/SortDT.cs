using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizTools;
using BizRAD.BizAccounts;

/// <summary>
/// Created By Jason
/// Used to Sort DataTables
/// </summary>

namespace ATL.SortTable
{
    class SortDT
    {
        protected DBAccess dbAccess = null;
        private string sql;
        private string Tablename, columnname;
        private DataTable OriTable, SorTable = null;
        private DataView dv = null;

        public SortDT(DataTable dt, string Column)
        {

            this.dbAccess = new DBAccess();

            this.dbAccess.DataSet.Tables.Add(dt.Copy());

            Tablename = dt.ToString();
            columnname = Column;


        }

        public DataTable SortedTable()
        {

            sql = "Select * from " + Tablename + " ";
            OriTable = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, sql);

            dv = OriTable.DefaultView;
            dv.Sort = columnname;
            SorTable = dv.ToTable(Tablename);
            return SorTable;
        }
    }
}
