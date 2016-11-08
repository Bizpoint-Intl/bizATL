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
using BizRAD.BizCommon;
using ATL.BizLogicTools;
using BizRAD.BizAccounts;

namespace ATL.WOR
{
    public partial class sitequickf2form : Form
    {
        DataTable trqtable = new DataTable();
        DBAccess targetdbaccess, remotedbaccess;
        String sourcecolumn, targetcolumn;

        public sitequickf2form(DBAccess targetdbaccess, DBAccess remotedbaccess, String sql, String sourcecolumn, String targetcolumn)
        {
            try
            {
                InitializeComponent();

                this.targetdbaccess = targetdbaccess;
                this.remotedbaccess = remotedbaccess;
                this.sourcecolumn = sourcecolumn;
                this.targetcolumn = targetcolumn;

                // Launch the sql populate the table to show.
                DataSet dstmp = remotedbaccess.ReadSQLTemp("trqf2", sql);
                trqtable = dstmp.Tables["trqf2"];

                dgv_trqf2.DataSource = trqtable;

                SizeColumns(dgv_trqf2);

                #region Set Colors and Styles
                dgv_trqf2.RowsDefaultCellStyle.BackColor = Color.Bisque;
                dgv_trqf2.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;
                dgv_trqf2.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                dgv_trqf2.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
                dgv_trqf2.DefaultCellStyle.SelectionForeColor = Color.Black;
                #endregion

                dgv_trqf2.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dgv_trqf2_CellMouseDoubleClick);
                this.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Your connection to HQ is not available! \n\n" + "Can't retrieve TRQ list" + "\n\nPlease try again.", "Transveiw Pte Ltd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void dgv_trqf2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Get the refnum from the row double clicked
            targetdbaccess.DataSet.Tables["trah"].Rows[0][targetcolumn] = trqtable.Rows[e.RowIndex][sourcecolumn];
            this.Close();
            this.Dispose();
        }



        protected void SizeColumns(DataGridView grid)
        {
            Graphics g = CreateGraphics();
            DataTable dataTable = (DataTable)grid.DataSource;

            foreach (DataGridViewColumn dataColumn in grid.Columns)
            {
                int maxSize = 0;

                SizeF size = g.MeasureString(
                                dataColumn.HeaderText,
                                grid.Font
                             );

                if (size.Width > maxSize)
                    maxSize = (int)size.Width;

                foreach (DataRow row in dataTable.Rows)
                {
                    size = g.MeasureString(
                              row[dataColumn.Name].ToString().Trim(),
                              grid.Font
                        );

                    if (size.Width > maxSize)
                        maxSize = (int)size.Width;
                }
                dataColumn.Width = maxSize + 5;
            }
            g.Dispose();
        }
    }
}