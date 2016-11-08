/***********************************************************
 *	Copyright (C), 1988-2007, Bizpoint International Pte Ltd
 *	File name:		Voucher_PBHM.cs
 *	Description:    Public Holiday Master
 *	Function List:	
 * 
 * History			历史修改添加记录
 * ---------------------------------------------------------
 * Author			Time				Description
 * Joshua           20070207            Change to new core
***********************************************************/

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;

namespace ATL.PBHM
{
    public class Voucher_PBHM : BizRAD.BizApplication.VoucherBaseHelper
    {
        public Voucher_PBHM(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_PBHM.xml", moduleName, voucherBaseHelpers)
        {
        }

        #region Document Handle

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);
            DataRow PBHM = e.DBAccess.DataSet.Tables["PBHM"].Rows[0];

            if (BizFunctions.IsEmpty(PBHM["PBHYEAR"]))
            {
                MessageBox.Show("Empty Field 'Public Holiday Year' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handle = false;
                return;
            }
        }

        #endregion

        #region Document Event

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

            DataRow pbhm = e.DBAccess.DataSet.Tables["pbhm"].Rows[0];
            DataTable pbhm1 = e.DBAccess.DataSet.Tables["pbhm1"];

            #region PBHM1

            foreach (DataRow dr in pbhm1.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    BizFunctions.UpdateDataRow(pbhm, dr, "status/user/modified/created/flag/year/pbhyear");

                    if (Convert.IsDBNull(dr["wkend"])) dr["wkend"] = false;

                    if (Convert.IsDBNull(dr["pbhdate"]))
                        dr["pbhdate"] = System.DateTime.Now.ToShortDateString();
                    else
                        dr["pbhdate"] = BizFunctions.GetStandardDateString((DateTime)dr["pbhdate"]);

                    dr["pbhmth"] = ((DateTime)dr["pbhdate"]).Month;
                }
            }

            #endregion
        }

        #endregion
    }
}