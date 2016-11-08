using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.Configuration;

using BizRAD.BizXml;
using BizRAD.DB.Client;
using BizRAD.BizDocument;
using BizRAD.DB.Interface;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizCommon;

namespace ATL.GeneralTools
{
    class GenTools
    {
        DBAccess dbAccess = new DBAccess();

        public void BindComboBox(ComboBox cboData, string strSQLStatement,
                string strDisplayMember, string strValueMember)
        {
            DataSet DS = dbAccess.ReadSQLTemp("DATA", strSQLStatement);
            BindingSource BindSourceData = new BindingSource();
            BindSourceData.DataSource = DS.Tables["DATA"];

            cboData.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboData.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboData.DropDownStyle = ComboBoxStyle.DropDown;

            cboData.ValueMember = strValueMember;
            cboData.DisplayMember = strDisplayMember;

            cboData.DataSource = BindSourceData;
            cboData.Refresh();
        }

        public static void SetDefaultPrinter(string name)
        {
            try
            {
                //late bind to the wsh network com object
                //create an instance of a reflection type
                Type t = Type.GetTypeFromProgID("WScript.Network");
                //create an instance using the system activator, consuming the type
                Object o = Activator.CreateInstance(t);
                //invoke the method using the object we created
                t.InvokeMember("SetDefaultPrinter", System.Reflection.BindingFlags.InvokeMethod, null, o, new object[] { name });
            }
            catch (Exception)
            {
                throw new Exception("Unable to set a default printer. Incorrect printer path " + name + ".");
            }
        }

        public bool isAdministratorUser(string strUserName)
        {
            string strSQL = "SELECT * FROM SYSUSERGROUP " +
                " WHERE UPPER(GROUPNAME)='ADMINISTRATOR' " +
                " AND UPPER(USERNAME)=UPPER('" + strUserName + "')";

            DataSet dsUSER = this.dbAccess.ReadSQLTemp("USER", strSQL);
            DataTable dtUSER = dsUSER.Tables["USER"];

            if (dtUSER.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public string getViewAccess(string strUserName)
        {
            string strSQL = "SELECT * FROM HEMP1 WHERE EMPNUM='" + strUserName + "'";

            DataSet dsUSER = this.dbAccess.ReadSQLTemp("USER", strSQL);
            DataTable dtUSER = dsUSER.Tables["USER"];

            string strAccess = "'" + strUserName +"'";
            if (dtUSER.Rows.Count > 0)
            {
                foreach (DataRow dr in dtUSER.Rows)
                {
                    strAccess = strAccess + ",'" + dr["viewempnum"].ToString() + "'";
                }
            }

            return strAccess;
        }

        public bool isPowerUser(string strUserName)
        {
            string strSQL = "SELECT * FROM SYSUSERGROUP " +
                " WHERE UPPER(GROUPNAME)='POWERUSER' " +
                " AND UPPER(USERNAME)=UPPER('" + strUserName + "')";

            DataSet dsUSER = this.dbAccess.ReadSQLTemp("USER", strSQL);
            DataTable dtUSER = dsUSER.Tables["USER"];

            if (dtUSER.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public bool isProcessAdmin(string strUserName)
        {
            string strSQL = "SELECT * FROM SYSUSERGROUP " +
                " WHERE UPPER(GROUPNAME) IN ('ADMINISTRATOR','PROCESSADMIN') " +
                " AND UPPER(USERNAME)=UPPER('" + strUserName + "')";

            DataSet dsUSER = this.dbAccess.ReadSQLTemp("USER", strSQL);
            DataTable dtUSER = dsUSER.Tables["USER"];

            if (dtUSER.Rows.Count > 0)
                return true;
            else
                return false;

        }
    }
}
