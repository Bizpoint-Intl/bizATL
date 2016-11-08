using System;
using System.Drawing;
using System.Collections;
using System.Data;
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
//Added By Yushu-To use in Barcode Print
using LabelGalleryPlus3WR;
using System.Configuration;



namespace ATL.MATMSTK
{
    //ALL STATUS OF A PRODUCT ITEM
    public class PSTATUS
    {
        public const string NEW = "NEW";
        public const string LIVE = "LIVE";
        public const string DISCONT = "DISCONT";
        public const string OBSOLETE = "OBSOLETE";
    }

    public class Voucher_MATMSTK : BizRAD.BizApplication.VoucherBaseHelper
    {
        Button btnMatmSetting = null;
        int NettDefault = 80;
        int FloorDefault = 50;


        protected bool opened = false;
        protected bool IsNEW = false;
        protected bool NewItemSAVED = false;
        protected string NewMatnum;


        protected DBAccess dbaccess;
        protected DataRow matm;
        protected DataTable matm1;

        protected Panel pnInfo;
        protected Panel pnStockInfo;
        protected GroupBox gpParam;
        protected Button btnAvail;
        protected GroupBox gpStatus;
        protected DataGrid dgPrice;

        protected RadioButton rbNEW;
        protected RadioButton rbLIVE;
        protected RadioButton rbDisCont;
        protected RadioButton rbObsolete;

        protected TextBox txtStatus;
        protected TextBox txtUOM;


        //Document Buttons
        protected Button btnRefresh;
        protected Button btnSave;
        protected Button btnConfirm;
        protected Button btnClose;
        protected Button btnVoid;
        protected Button btnInsert;
        protected LXFClassLibrary.Controls.TabControl tabcontrol;
        protected LXFClassLibrary.Controls.TabPage tpSuppliers;
        protected LXFClassLibrary.Controls.TabPage tpUOM;
        //Added By Yushu
        protected LXFClassLibrary.Controls.TabPage tpPrint;
        protected DialogResult result = DialogResult.No;


        //For New Item Only
        private string pcat;
        private string pbrd;
        private string ploft;
        private string pflex;
        private string pshf;
        private string pcol;
        private string psz;
        private string model;

        //added by Yushu-For Barcode Printing
        protected Button btn_PrintBarcode = null;
        protected Button btn_SelectItems = null;
        protected Button btn_PrintMultiple = null;
        protected DialogResult printresult = DialogResult.No;

        //added by Yushu20100429-Validate items
        protected bool validated = false;

        string mtype = "STK";

        public Voucher_MATMSTK(string moduleName, Hashtable voucherBaseHelpers)
            : base("VoucherGridInfo_MATM.xml", moduleName, voucherBaseHelpers)
        {
            //LOAD the default nett/floor price FROM matmsetting TABLE
            try
            {
                DataSet dsPriceDefault = this.VoucherBase.DBAccess.RemoteStandardSQL.GetSQLResult("select top 1 * from matmsetting");
                if (dsPriceDefault != null)
                {
                    this.NettDefault = (int)dsPriceDefault.Tables[0].Rows[0][0];
                    this.FloorDefault = (int)dsPriceDefault.Tables[0].Rows[0][1];
                }
            }
            catch { }
        }


        protected override void Voucher_Form_OnLoad(object sender, VoucherEventArgs e)
        {
            base.Voucher_Form_OnLoad(sender, e);
            DataRow matm = e.DBAccess.DataSet.Tables["matm"].Rows[0];

            //Add a Button at the Voucher Page - FOR "DEFAULT SETTING" Button
            this.btnMatmSetting = new Button();
            this.btnMatmSetting.Text = "Default Settings";
            this.btnMatmSetting.Size = new System.Drawing.Size(75, 35);
            this.btnMatmSetting.Location = new System.Drawing.Point(930, 540);
            this.btnMatmSetting.FlatStyle = FlatStyle.System;
            //			this.btnMatmSetting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(7)))), ((int)(((byte)(213)))));
            this.btnMatmSetting.Font = new System.Drawing.Font("Tahoma", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMatmSetting.Click += new EventHandler(btnMatmSetting_Click);

            this.VoucherBase.VoucherForm.Controls.Add(this.btnMatmSetting);
            this.VoucherBase.VoucherForm.Refresh();

           
            
        }

        protected override void Document_Reopen_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Reopen_Handle(sender, e);

        }



        #region Default Setting BUTTON @ Voucher Page

        private void btnMatmSetting_Click(object sender, EventArgs e)
        {
            frmMatmSetting matmSet = new frmMatmSetting();
            matmSet.Nett = this.NettDefault;
            matmSet.Floor = this.FloorDefault;
            matmSet.ShowDialog();
            if (matmSet.DialogResult == DialogResult.OK)
            {
                this.NettDefault = matmSet.Nett;
                this.FloorDefault = matmSet.Floor;

                string sqlupd = String.Format("update matmsetting set nettprice={0},floorprice={1}", this.NettDefault, this.FloorDefault);
                try
                {
                    this.VoucherBase.DBAccess.RemoteStandardSQL.ExecuteNonQuery(sqlupd);
                }
                catch { }
            }
        }

        #endregion Default Setting BUTTON @ Voucher Page


        #region Voucher Default/ALL

        protected override void AddVoucherDefaultCondition(BizRAD.BizVoucher.VoucherConditionEventArgs e)
        {
            base.AddVoucherDefaultCondition(e);

            e.Condition = String.Format("mtype='" + mtype + "' and (pstatus='{0}' or pstatus='{1}')", PSTATUS.LIVE, PSTATUS.NEW);
        }

        protected override void AddVoucherAllCondition(VoucherConditionEventArgs e)
        {
            //For status 'O' & 'P'
            base.AddVoucherAllCondition(e);
            e.Condition = " MATM.mtype='" + mtype + "' ";
        }

        #endregion


        #region Voucher Handle - To trick core not to input a new code in search textbox

        protected override void Voucher_New_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_New_Handle(sender, e);

            if (opened)
            {
                MessageBox.Show("You cannot open two documents at the same time.\n" +
                    "To Edit/New a Document, you have to close the document that's currently opened for this module.",
                    "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //else
            //{
            //    //core 'thought' it is input by user
            //    this.VoucherBase.SearchTextBox.Text = "NEW ITEM";
            //}

            e.Handle = !opened;
        }


        protected override void Voucher_Edit_Handle(object sender, BizRAD.BizVoucher.VoucherHandleEventArgs e)
        {
            base.Voucher_Edit_Handle(sender, e);

            if (opened)
            {
                MessageBox.Show("You cannot open two documents at the same time.\n" +
                    "To Edit/New a Document, you have to close the document that's currently opened for this module.",
                    "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            e.Handle = !opened;
        }


        #endregion Voucher Handle - To trick core not to input a new code in search textbox


        #region DOCUMENT RELATED
        /****
		 * UI:
		 * Controls inside
		 * Panel pnInfo & GroupBox gpParam 
		 * are for NEW ITEM only
		 * Will be disabled once Product Code is confirmed and saved
		 ****/


        protected override void Document_Form_OnLoad(object sender, DocumentEventArgs e)
        {
            base.Document_Form_OnLoad(sender, e);
            opened = true;


            //initialize
            this.InitializeForm((sender as Form).Name, e);
        }



        private void InitializeForm(string formname, DocumentEventArgs e)
        {
            this.dbaccess = e.DBAccess;
            this.NewMatnum = "";


            this.pnInfo = BizXmlReader.CurrentInstance.GetControl(formname, "pnInfo") as Panel;
            this.pnStockInfo = BizXmlReader.CurrentInstance.GetControl(formname, "pnStockInfo") as Panel;
            this.gpParam = BizXmlReader.CurrentInstance.GetControl(formname, "gpParam") as GroupBox;
            this.btnAvail = BizXmlReader.CurrentInstance.GetControl(formname, "btnAvail") as Button;
            this.gpStatus = BizXmlReader.CurrentInstance.GetControl(formname, "gpStatus") as GroupBox;
            this.dgPrice = BizXmlReader.CurrentInstance.GetControl(formname, "dgPrice") as DataGrid;

            this.rbNEW = BizXmlReader.CurrentInstance.GetControl(formname, "rbNEW") as RadioButton;
            this.rbLIVE = BizXmlReader.CurrentInstance.GetControl(formname, "rbLIVE") as RadioButton;
            this.rbDisCont = BizXmlReader.CurrentInstance.GetControl(formname, "rbDisCont") as RadioButton;
            this.rbObsolete = BizXmlReader.CurrentInstance.GetControl(formname, "rbObsolete") as RadioButton;

            this.txtStatus = BizXmlReader.CurrentInstance.GetControl(formname, "txtStatus") as TextBox;
            this.txtUOM = BizXmlReader.CurrentInstance.GetControl(formname, "matm_uomcode") as TextBox;

            this.btnRefresh = BizXmlReader.CurrentInstance.GetControl(formname, "btn_Document_Refresh") as Button;
            this.btnSave = BizXmlReader.CurrentInstance.GetControl(formname, "btn_Document_Save") as Button;
            this.btnClose = BizXmlReader.CurrentInstance.GetControl(formname, "btn_Document_Close") as Button;
            this.btnConfirm = BizXmlReader.CurrentInstance.GetControl(formname, "btn_Document_Confirm") as Button;
            this.btnVoid = BizXmlReader.CurrentInstance.GetControl(formname, "btn_Document_Void") as Button;
            this.btnInsert = BizXmlReader.CurrentInstance.GetControl(formname, "btn_Document_Insert") as Button;

            this.tabcontrol = BizXmlReader.CurrentInstance.GetControl(formname, Common.DEFAULT_DOCUMENT_TABCONTROL) as LXFClassLibrary.Controls.TabControl;
            this.tpSuppliers = null;
            this.tpUOM = null;
            //Added By Yushu
            this.tpPrint = null;

            //Added By Yushu
            btn_PrintBarcode = BizXmlReader.CurrentInstance.GetControl(formname, "btnPrintBarcode") as Button;
            btn_PrintBarcode.Click += new EventHandler(btn_PrintBarcode_Click);

            btn_SelectItems = BizXmlReader.CurrentInstance.GetControl(formname, "btnSelectItem") as Button;
            btn_SelectItems.Click += new EventHandler(btn_SelectItems_Click);

            btn_PrintMultiple = BizXmlReader.CurrentInstance.GetControl(formname, "btnPrintMultiple") as Button;
            btn_PrintMultiple.Click += new EventHandler(btn_PrintMultiple_Click);

            //By Yushu20100127-Only Administrator can void Material Master items
            //disable unecessary buttons
            //this.btnConfirm.Enabled = false;
            //if (this.dbaccess.DataSet.Tables["matm"].Rows[0]["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSV)
            //{
            //    if (Common.DEFAULT_SYSTEM_ISADMINISTRATORUSER)
            //    {
            //        this.btnVoid.Enabled = true;
            //    }
            //    else
            //    {
            //        if (Common.DEFAULT_SYSTEM_ISADMINISTRATORGROUP)
            //        {
            //            this.btnVoid.Enabled = true;
            //        }
            //        else
            //            this.btnVoid.Enabled = false;
            //    }
            //}

            //DataTable control
            this.matm = this.dbaccess.DataSet.Tables["matm"].Rows[0];
            this.matm.Table.ColumnChanged += new DataColumnChangeEventHandler(MATM_ColumnChanged);
            this.matm1 = this.dbaccess.DataSet.Tables["matm1"];
            this.matm1.ColumnChanged += new DataColumnChangeEventHandler(MATM1_ColumnChanged);


            //Check if this is a new item!
            //this.SetControlsForNEWOnly();
            //if (this.matm["status"].ToString() != Common.DEFAULT_DOCUMENT_STATUSN)
            //{
            //    this.SetControlsForEditOnly();
            //    this.BindsUI();
            //}

            this.rbNEW.CheckedChanged += new EventHandler(PSTATUS_Changed);
            this.rbLIVE.CheckedChanged += new EventHandler(PSTATUS_Changed);
            this.rbDisCont.CheckedChanged += new EventHandler(PSTATUS_Changed);
            this.rbObsolete.CheckedChanged += new EventHandler(PSTATUS_Changed);
        }

        private void BindsUI()
        {
            //For Editing Document only

            //For RADIO Buttons
            string pstatus = this.matm["pstatus"].ToString();
            switch (pstatus)
            {
                case PSTATUS.NEW:
                    this.rbNEW.Checked = true;
                    break;

                case PSTATUS.LIVE:
                    this.rbLIVE.Checked = true;
                    break;

                case PSTATUS.DISCONT:
                    this.rbDisCont.Checked = true;
                    break;

                default:
                    this.rbObsolete.Checked = true;
                    break;
            }
        }


        private void SetControlsForNEWOnly()
        {
            this.IsNEW = true;

            //this.pnInfo.Enabled = true;
            //this.gpParam.Enabled = true;
            //this.btnAvail.Enabled = true;
            this.btnAvail.Click += new EventHandler(btnAvail_Click);

            //this.gpStatus.Enabled = false;
            //this.dgPrice.Enabled = false;

            //disable 2nd tabpage
            this.tpSuppliers = this.tabcontrol.TabPages["Suppliers"];
            //this.tpUOM = this.tabcontrol.TabPages["UOM Conversion"];
            ////Added By Yushu
            this.tpPrint = this.tabcontrol.TabPages["Print Multiple Items"];
            this.tabcontrol.TabPages.Remove(this.tpSuppliers);
            //this.tabcontrol.TabPages.Remove(this.tpUOM);
            //Added By Yushu
            this.tabcontrol.TabPages.Remove(this.tpPrint);

            //temporary disable these buttons
            //this.btnRefresh.Enabled = false;
            //this.btnSave.Enabled = false;
            //this.btnClose.Enabled = false;

            //			this.btnInsert.Enabled = false; //core reset enabled back to true!?
            this.btnInsert.Visible = false;
        }


        /****************************************************************
        STOCK / Non-STOCK ITEMS
 

        /*****************************************************************/


        private void SetControlsForEditOnly()
        {
            //this.IsNEW = false;

            //this.pnInfo.Enabled = false;
            //this.gpParam.Enabled = false;
            //this.btnAvail.Enabled = false;

            //this.gpStatus.Enabled = true;

            //enable these buttons back
            //this.btnRefresh.Enabled = true;
            //this.btnSave.Enabled = true;
            //this.btnClose.Enabled = true;

            //this.dgPrice.Enabled = true;	//enable grid in 1st page
            //this.btnInsert.Visible = true;	//enable Insert button

            if (BizFunctions.IsEmpty(matm["nonstock"]))
            {
                this.matm["nonstock"] = 1;
            }

            //for Stock Items only
            if ((bool)(this.matm["nonstock"]) == false)
            {
                //enable Suppliers tabpage
                if (this.tpSuppliers != null)
                    this.tabcontrol.TabPages.Add(this.tpSuppliers);

                ////enable UOM tabpage
                //if (this.tpUOM != null)
                //    this.tabcontrol.TabPages.Add(this.tpUOM);

                //Added By Yushu-enable Print tabpage
                if (this.tpPrint != null)
                    this.tabcontrol.TabPages.Add(this.tpPrint);
            }
            else
            {
                //disable pnStockInfo
                this.pnStockInfo.Enabled = false;
            }


            // disable NEW radio button IF it is still in NEW
            if (this.matm["pstatus"].ToString() == PSTATUS.NEW)
            {
                this.rbNEW.Enabled = false;
            }

            this.rbNEW.CheckedChanged += new EventHandler(PSTATUS_Changed);
            this.rbLIVE.CheckedChanged += new EventHandler(PSTATUS_Changed);
            this.rbDisCont.CheckedChanged += new EventHandler(PSTATUS_Changed);
            this.rbObsolete.CheckedChanged += new EventHandler(PSTATUS_Changed);
        }


        #region MATM Change control

        private void MATM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            switch (e.Column.ColumnName)
            {
                case "pstatus":
                    this.txtStatus.Text = e.ProposedValue.ToString();
                    break;

                default:
                    break;
            }
        }

        //
        // PSTATUS NEW CANNOT BE CHANGED TO NEW 
        // TO Disable 'NEW' radio button
        //

        private void PSTATUS_Changed(object sender, EventArgs e)
        {
            if (this.rbNEW.Checked)
            {
                this.matm["pstatus"] = PSTATUS.NEW;
            }
            else if (this.rbLIVE.Checked)
            {
                this.matm["pstatus"] = PSTATUS.LIVE;
            }
            else if (this.rbDisCont.Checked)
            {
                this.matm["pstatus"] = PSTATUS.DISCONT;
            }
            else
            {
                this.matm["pstatus"] = PSTATUS.OBSOLETE;
            }
        }

        #endregion MATM Change control

        #region Detail Grid Insert Handles

        protected override void Document_Insert_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Insert_Handle(sender, e);

            //For UOM Conversion TabPage
            if (this.tabcontrol.SelectedTab == this.tpUOM)
            {
                //if UOM in Header Page is empty, STOP
                if (this.matm["uomcode"].ToString() == "")
                {
                    MessageBox.Show("The UOM is not defined in the 1st Page.\nPlease define a base UOM before proceeding here.", "Insert Row NOT Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Handle = false;
                }
            }
        }


        protected override void Document_Insert_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Insert_OnClick(sender, e);

            //Set matnum value for ALL sub tables
            if (this.NewMatnum != "")
            {
                e.CurrentRow["matnum"] = this.NewMatnum;
            }

            //For UOM Conversion TabPage
            if (e.CurrentRow.Table.TableName == "matmcf")
            {
                //Set Defautls
                e.CurrentRow["baseuom"] = this.matm["uomcode"];
            }
        }

        #endregion Detail Grid Insert Handles


        #region MATM1 Change Control

        private void MATM1_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            decimal retail = 0.0M;
            decimal nett = 0.0M;
            decimal floor = 0.0M;


            switch (e.Column.ColumnName)
            {
                case "retail":
                case "nett":
                case "floor":
                    if (e.Column.ColumnName == "retail")
                    {
                        e.Row["pnett"] = (this.NettDefault / 100.0M) * (decimal)e.ProposedValue;
                        e.Row["pfloor"] = (this.FloorDefault / 100.0M) * (decimal)e.ProposedValue;

                        //copy if values = 0
                        if ((decimal)(e.Row["nett"]) == 0.0M)
                            e.Row["nett"] = e.Row["pnett"];
                        if ((decimal)(e.Row["floor"]) == 0.0M)
                            e.Row["floor"] = e.Row["pfloor"];
                    }


                    //
                    retail = (decimal)(e.Row["retail"]);
                    nett = (decimal)(e.Row["nett"]);
                    floor = (decimal)(e.Row["floor"]);

                    //
                    // condition: retail <= nett <= floor
                    if (retail < nett || retail < floor || nett < floor)
                    {
                        e.Row.SetColumnError(e.Column, "Retail$ < Nett$ < Floor$ !!");
                    }
                    else
                    {
                        e.Row.SetColumnError(e.Column, "");
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion MATM1 Change Control


        #region METHODS FOR NEW ITEM

        private void btnAvail_Click(object sender, EventArgs e)
        {
            //Validates for NEW ITEM
            if (this.ValidatesForNEWItem() == false)
                return;

            //Validates for Common
            if (this.ValidateCommon() == false)
                return;

            //Formulate New ProductCode (matnum) from user selection
            if (this.FormulateMatnum() == false)
                return;

            //Confirm with User
            if (MessageBox.Show("New Product Code is Valid!\nProceed to commit into Database?", "Confirm New Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.btnSave.Enabled = true;
                this.NewItemSAVED = false;
                this.btnSave.PerformClick();

                if (this.NewItemSAVED == false)	//disable back Save button IFF SAVE FAILS
                    this.btnSave.Enabled = false;
            }
            validated = true;
        }


        private bool ValidatesForNEWItem()
        {
            //init values
            this.pcat = "";
            this.pbrd = "";
            this.ploft = "";
            this.pflex = "";
            this.pshf = "";
            this.pcol = "";
            this.psz = "";
            this.model = "";


            //Product Cateogry
            if (!BizValidate.CheckTableIsValid(this.dbaccess, "pcatm", "pcatcode", this.matm["pcatcode"].ToString()))
            {
                MessageBox.Show("Invalid 'Category Type' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            this.pcat = this.matm["pcatcode"].ToString();

            //Product Brand
            //for Stock Items only
            if ((bool)(this.matm["nonstock"]) == false)
            {
                if (!BizValidate.CheckTableIsValid(this.dbaccess, "pbrdm", "pbrdcode", this.matm["pbrdcode"].ToString()))
                {
                    MessageBox.Show("Invalid 'Brand' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                this.pbrd = this.matm["pbrdcode"].ToString();
            }
            else //NON-STOCK set to empty
            {
                this.matm["pbrdcode"] = "";
            }

            //Product Model Code
            if (BizFunctions.IsEmpty(this.matm["modelcode"]))
            {
                MessageBox.Show("Empty Field 'Model Code' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            this.model = this.matm["modelcode"].ToString();

            //Loft - if not blank, check for valid F2 entries
            if (BizFunctions.IsEmpty(this.matm["ploftcode"]) == false)
            {
                if (!BizValidate.CheckTableIsValid(this.dbaccess, "ploftm", "ploftcode", this.matm["ploftcode"].ToString()))
                {
                    MessageBox.Show("Invalid 'Loft' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                this.ploft = this.matm["ploftcode"].ToString();
            }

            //Flex - if not blank, check for valid F2 entries
            if (BizFunctions.IsEmpty(this.matm["pflexcode"]) == false)
            {
                if (!BizValidate.CheckTableIsValid(this.dbaccess, "pflexm", "pflexcode", this.matm["pflexcode"].ToString()))
                {
                    MessageBox.Show("Invalid 'Flex' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                this.pflex = this.matm["pflexcode"].ToString();
            }

            //Shaft - if not blank, check for valid F2 entries
            if (BizFunctions.IsEmpty(this.matm["pshfcode"]) == false)
            {
                if (!BizValidate.CheckTableIsValid(this.dbaccess, "pshfm", "pshfcode", this.matm["pshfcode"].ToString()))
                {
                    MessageBox.Show("Invalid 'Shaft' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                this.pshf = this.matm["pshfcode"].ToString();
            }

            //Color - if not blank, check for valid F2 entries
            if (BizFunctions.IsEmpty(this.matm["pcolcode"]) == false)
            {
                if (!BizValidate.CheckTableIsValid(this.dbaccess, "pcolm", "pcolcode", this.matm["pcolcode"].ToString()))
                {
                    MessageBox.Show("Invalid 'Color' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                this.pcol = this.matm["pcolcode"].ToString();
            }

            //Size - if not blank, check for valid F2 entries
            if (BizFunctions.IsEmpty(this.matm["pszcode"]) == false)
            {
                if (!BizValidate.CheckTableIsValid(this.dbaccess, "pszm", "pszcode", this.matm["pszcode"].ToString()))
                {
                    MessageBox.Show("Invalid 'Size' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                this.psz = this.matm["pszcode"].ToString();
            }

            return true;
        }


        private bool FormulateMatnum()
        {
            //formulate accordingly
            this.NewMatnum = string.Format(
                "{0}{1}{2}{3}{4}{5}{6}{7}",
                this.pcat,
                this.pbrd,
                this.ploft,
                this.pflex,
                this.pshf,
                this.pcol,
                this.psz,
                this.model);

            #region Yushu Commented20100125 - User requested to remove '-' between parameters
            //formulate accordingly
            //this.NewMatnum = string.Format(
            //    "{0}{1}-{2}{3}{4}{5}{6}-{7}", 
            //    this.pcat,
            //    this.pbrd,
            //    this.ploft,
            //    this.pflex,
            //    this.pshf,
            //    this.pcol,
            //    this.psz,
            //    this.model);
            #endregion

            //remove 2 '-' if there are no parameters selected
            this.NewMatnum = this.NewMatnum.Replace("--", "-");
            this.NewMatnum = this.NewMatnum.Replace(" ", "");	//remove blanks

            //Check length of New Matnum
            if (this.NewMatnum.Length > 20)
            {
                MessageBox.Show("The Length of Product Code '" + this.NewMatnum + "' is Too Long (> 20)!\nPlease select LESS Parameters!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            //Check for uniqueness of matnum!
            if (BizValidate.CheckTableIsValid(this.dbaccess, "matm", "matnum", this.NewMatnum) == true)
            {
                MessageBox.Show("The Product Code '" + this.NewMatnum + "' is NOT Unique!\nPlease choose another Model Code!", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            this.matm.BeginEdit();
            this.matm["matnum"] = this.NewMatnum;
            this.matm["pstatus"] = PSTATUS.NEW;
            this.matm.EndEdit();

            return true;
        }

        #endregion METHODS FOR NEW ITEM


        #region Common Validates

        private bool ValidateCommon()
        {
            //Product Description
            if (BizFunctions.IsEmpty(this.matm["matname"]))
            {
                MessageBox.Show("Empty Field 'Product Description' !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //UOM - if not blank, check for valid F2 entries
            if (BizFunctions.IsEmpty(this.matm["uomcode"]) == false)
            {
                if (!BizValidate.CheckTableIsValid(this.dbaccess, "uom", "uomcode", this.matm["uomcode"].ToString()))
                {
                    MessageBox.Show("Invalid 'UOM' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            //Country Code - if not blank, check for valid F2 entries
            if (BizFunctions.IsEmpty(this.matm["oricountry"]) == false)
            {
                if (!BizValidate.CheckTableIsValid(this.dbaccess, "countrycode", "countrycode", this.matm["oricountry"].ToString()))
                {
                    MessageBox.Show("Invalid 'Country' selected !", "Save Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }


            return true;
        }

        #endregion Common Validates


        #region Validates for EDIT item

        private bool ValidatesForEditItem()
        {
            //Check the pstatus - redundant?
            string pstatus = this.matm["pstatus"].ToString();
            if (pstatus != PSTATUS.NEW && pstatus != PSTATUS.LIVE && pstatus != PSTATUS.DISCONT && pstatus != PSTATUS.OBSOLETE)
            {
                MessageBox.Show("Please select one of the STATUS for this Product.", "Error in Product Status", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            //matm1 Table Check
            if (this.matm1.HasErrors)
            {
                MessageBox.Show("There is Error(s) in the Price Table.\nPlease clear it before saving/exiting.", "Error in Price Table", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //matm1 - check retail/nett/floor != 0
            foreach (DataRow drmatm1 in this.matm1.Rows)
            {
                if ((decimal)drmatm1["retail"] <= 0.0M)
                {
                    drmatm1.SetColumnError("retail", "Value <= 0");
                    return false;
                }

                if ((decimal)drmatm1["nett"] <= 0.0M)
                {
                    drmatm1.SetColumnError("nett", "Value <= 0");
                    return false;
                }

                if ((decimal)drmatm1["floor"] <= 0.0M)
                {
                    drmatm1.SetColumnError("floor", "Value <= 0");
                    return false;
                }
            }


            //matm2 Table Check
            if (this.dbaccess.DataSet.Tables["matm2"].HasErrors)
            {
                MessageBox.Show("There is Error(s) in the Supplier List.\nPlease clear it before saving/exiting.", "Error in Supplier List", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            //matmcf Table Check
            if (this.dbaccess.DataSet.Tables["matmcf"].HasErrors)
            {
                MessageBox.Show("There is Error(s) in the UOM Conversion.\nPlease clear it before saving/exiting.", "Error in UOM Conversion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        #endregion Validates for EDIT item

        #region Document Refresh

        protected override void Document_Refresh_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Refresh_OnClick(sender, e);

            DataTable mats = dbaccess.DataSet.Tables["mats"];

            #region Add an entry into mats if it's empty
            if (mats.Rows.Count == 0)
            {
                DataRow addMats = mats.Rows.Add(new object[] { });
                BizFunctions.UpdateDataRow(matm, addMats, "matnum/user/modified/created/flag");
            }
            #endregion
        }

        #endregion

        #region Document SAVE

        protected override void Document_Save_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Save_Handle(sender, e);

            DataRow matm = this.dbaccess.DataSet.Tables["matm"].Rows[0];
            DataTable matm2 = this.dbaccess.DataSet.Tables["matm2"];

            if (matm2.Rows.Count > 0)
            {
                foreach (DataRow dr2 in matm2.Rows)
                {
                    if (dr2.RowState != DataRowState.Deleted)
                    {
                        dr2["matnum"] = matm["matnum"];

                        if (!BizFunctions.IsEmpty(dr2["dpriceConvert"]) && !BizFunctions.IsEmpty(dr2["uomqty"]))
                        {
                            if (Convert.ToDecimal(dr2["dpriceConvert"]) > 0 && Convert.ToDecimal(dr2["uomqty"]) > 0)
                            {

                                dr2["baseunitprice"] = Convert.ToDecimal(dr2["dpriceConvert"]) / Convert.ToDecimal(dr2["uomqty"]);
                            }
                        }
                    }
                }
            }

            //validate only if not new
            //if (BizFunctions.IsEmpty(matm["ploftcode"]) || matm["ploftcode"].ToString() == "")
            //{

            //    MessageBox.Show("Please check Convertion Factor before saving !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    e.Handle = false;
            //}

            ////YushuAdded20100429-Validate Before Saving

            if (BizFunctions.IsEmpty(matm["purcAcc"]))
            {
                matm["purcAcc"] = "14001";
            }

            if (BizFunctions.IsEmpty(matm["puom"]))
            {
                matm["puom"] = matm["uomcode"];
            }

            if (BizFunctions.IsEmpty(matm["uomcode"]) || BizFunctions.IsEmpty(matm["puom"]))
            {
                //   if (!this.validated)
                {
                    MessageBox.Show("Please check UOM before saving !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handle = false;
                }
            }
        }

        protected override void Document_SaveBegin_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveBegin_OnClick(sender, e);

            DataRow matm = this.dbaccess.DataSet.Tables["matm"].Rows[0];
            DataTable matm2 = this.dbaccess.DataSet.Tables["matm2"];



            //Added By Yushu20091118-Generate Barcode with 8 Digits
            //#region Generate barcode
            //if (BizFunctions.IsEmpty(matm["barcode"]))
            //{
            //    DataTable dtbarcode = dbaccess.ReadSQLTemp("dtbarcode", "select isnull(max(barcode),0) as barcode from matm").Tables["dtbarcode"];
            //    int code = Int32.Parse(dtbarcode.Rows[0]["barcode"].ToString()) + 1;
            //    matm["barcode"] = code.ToString("D8");
            //}
            //else if (!BizFunctions.IsEmpty(matm["barcode"]))
            //{
            //    int barcode = Int32.Parse(matm["barcode"].ToString());
            //    matm["barcode"] = barcode.ToString("D8");
            //}
            //#endregion

            matm["mtype"] = mtype;

        }

        protected override void Document_SaveEnd_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_SaveEnd_OnClick(sender, e);

            //SAVED - data successfully saved into DB
            //if (this.IsNEW)
            //{
            //    this.NewItemSAVED = true;
            //    this.SetControlsForEditOnly();

            //    //Set the NEW ITEM PSTATUS TO "NEW"
            //    this.rbNEW.Checked = true;
            //}

            if (isExistMats())
            {
                UpdateMats();
            }
            else
            {
                InsertMats();
            }
        }

        #endregion Document SAVE

        #region Document Cancel

        protected override void Document_Cancel_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Cancel_Handle(sender, e);

            //data is modified , STILL EXIT ????
            //if (this.matm.Table.GetChanges() != null)
            //{
            //    if (MessageBox.Show("Data is modified and not saved.\nDo you want to exit without saving?", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            //    {
            //        e.Handle = false;
            //    }
            //}
        }


        protected override void Document_Cancel_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Cancel_OnClick(sender, e);

            //To Reset Voucher Opened back to false
            opened = false;
        }

        #endregion Document Cancel

        #region Document VOID

        protected override void Document_Void_Handle(object sender, DocumentHandleEventArgs e)
        {
            base.Document_Void_Handle(sender, e);

            //By Yushu20100127-DELETE from database when user click 'VOID'
            this.result = MessageBox.Show("Are you sure you want to permanently delete this item from the database?", "Delete item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (this.result == DialogResult.No)
            {
                e.Handle = false;
            }
        }

        protected override void Document_Void_OnClick(object sender, DocumentEventArgs e)
        {
            base.Document_Void_OnClick(sender, e);

            if (this.result == DialogResult.Yes)
            {
                DataRow matm = e.DBAccess.DataSet.Tables["matm"].Rows[0];
                String sql = "Delete FROM MATM where matnum = '" + matm["matnum"].ToString().Trim() + "'";
                e.DBAccess.RemoteStandardSQL.ExecuteNonQuery(sql);

                DataTable matm1 = e.DBAccess.DataSet.Tables["matm1"];
                String sql2 = "Delete FROM MATM1 where matnum = '" + matm["matnum"].ToString().Trim() + "'";
                e.DBAccess.RemoteStandardSQL.ExecuteNonQuery(sql);
            }
        }

        #endregion


        #endregion  DOCUMENT RELATED

        #region Yushu- btn_PrintBarcode_Click

        private void btn_PrintBarcode_Click(object sender, EventArgs e)
        {
            #region Variables

            DataTable dtBarcode;
            DataRow drBarcode = null;

            string matname = "";
            string matnum = "";
            string parameter = "";
            string barcode = "";
            int qty = 0;
            decimal price = -1;
            string sql = "";
            bool isEmpty = false;

            #endregion

            this.printresult = MessageBox.Show("Start Barcode Printing?", "Print Barcode?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            #region Start Print

            if (this.printresult == DialogResult.Yes)
            {
                #region Save Before Printing

                //if (matm["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP)
                //{
                if (matm["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP && BizValidate.CheckRowState(this.dbaccess.DataSet, "matm/matm1"))
                {
                    MessageBox.Show("Please save before Printing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    if (BizFunctions.IsEmpty(matm["qty"]))
                    {
                        matm["qty"] = 0;
                    }
                    qty = Convert.ToInt32(matm["qty"]);

                    if (qty > 0)
                    {
                        #region Select Barcode data and assign to dtBarcode table

                        sql = "select matnum,matname,ploftcode+pflexcode+pshfcode+pcolcode+pszcode as parameter,barcode,qty,price "
                            + "from (select m.matnum,m.matname,"
                            + "case when isnull(m.ploftcode,'')='' or isnull(m.ploftcode,'')='***' " + "then ''+ SPACE(1) else m.ploftcode+ SPACE(1) end as ploftcode,"
                            + "case when isnull(m.pflexcode,'')='' or isnull(m.pflexcode,'')='**' then ''+ SPACE(1) else m.pflexcode+ SPACE(1) end as pflexcode,"
                            + "case when isnull(m.pshfcode,'')='' or isnull(m.pshfcode,'')='***' then ''+ SPACE(1) else m.pshfcode+ SPACE(1) end as pshfcode,"
                            + "case when isnull(m.pcolcode,'')='' or isnull(m.pcolcode,'')='****' then ''+ SPACE(1) else m.pcolcode+ SPACE(1) end as pcolcode,"
                            + "case when isnull(m.pszcode,'')='' or isnull(m.pszcode,'')='****' then ''+ SPACE(1) else m.pszcode+ SPACE(1) end as pszcode,"
                            + "m.barcode,m.qty,s.retail as price "
                            + "from matm m "
                            + "left join Stocks s on s.matnum=m.matnum "
                            + "where m.matnum='" + matm["matnum"].ToString() + "' ) t";

                        this.dbaccess.ReadSQL("tmpBarcode", sql);
                        dtBarcode = this.dbaccess.DataSet.Tables["tmpBarcode"];

                        #endregion

                        if (dtBarcode.Rows.Count > 0)
                        {
                            drBarcode = dtBarcode.Rows[0];

                            if (drBarcode["matnum"].ToString().ToUpper().Trim() == matm["matnum"].ToString().ToUpper().Trim())
                            {
                                #region Create New Label, point to Label Path and Barcode Printer

                                LGApp nice = new LGApp();
                                LGLabel labelintf = new LGLabel();

                                bool setprintx;

                                if (BizFunctions.IsEmpty(matm["printPrice"]))
                                {
                                    matm["printPrice"] = false;
                                }
                                long labelID = 0;
                                if ((bool)matm["printPrice"] == true)
                                {
                                    labelID = nice.LabelOpen(@"C:\TVBarcode\TVBarcode.lbl");
                                }
                                else
                                {
                                    labelID = nice.LabelOpen(@"C:\TVBarcode\TVBarcode(NoPrice).lbl");
                                }

                                // get printer
                                setprintx = nice.LabelSetPrinter((int)labelID, ConfigurationManager.AppSettings.Get("BarcodePrinter"));

                                #endregion

                                #region Assign Label Data

                                barcode = drBarcode["barcode"].ToString();
                                matname = drBarcode["matname"].ToString();
                                matnum = drBarcode["matnum"].ToString();
                                parameter = drBarcode["parameter"].ToString();
                                if (!BizFunctions.IsEmpty(drBarcode["price"]))
                                {
                                    price = Convert.ToDecimal(drBarcode["price"]);
                                }
                                if (!BizFunctions.IsEmpty(drBarcode["qty"]))
                                {
                                    qty = Convert.ToInt32(drBarcode["qty"]);
                                }

                                #endregion

                                #region Call PrintBarcode Method

                                if ((bool)matm["printPrice"] == true)
                                {
                                    if (!(BizFunctions.IsEmpty(matnum) && BizFunctions.IsEmpty(matname)
                                        && BizFunctions.IsEmpty(barcode)) && price > -1 && qty > 0)
                                    {
                                        PrintBarcode(drBarcode, nice, labelintf, setprintx, labelID, matname, matnum, parameter, barcode, qty, price);
                                    }
                                    else
                                    {
                                        isEmpty = true;
                                    }
                                }
                                else
                                {
                                    if (!(BizFunctions.IsEmpty(matnum) && BizFunctions.IsEmpty(matname)
                                        && BizFunctions.IsEmpty(barcode)) && qty > 0)
                                    {
                                        PrintBarcode2(drBarcode, nice, labelintf, setprintx, labelID, matname, matnum, parameter, barcode, qty);
                                    }
                                    else
                                    {
                                        isEmpty = true;
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #region If Empty or Invalid Data detected, Prompt Error

                    if (isEmpty)
                    {
                        MessageBox.Show("Material Code/Material Name/Barcode/Price or Qty has empty or invalid data!", "Print Failed For Some Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion
                }
                //}

                #endregion
            }
            #endregion
        }

        private void btn_SelectItems_Click(object sender, EventArgs e)
        {
            DataRow matm = dbaccess.DataSet.Tables["matm"].Rows[0];
            DataTable matm3 = dbaccess.DataSet.Tables["matm3"];

            Form frm = BizXmlReader.CurrentInstance.GetForm("Header") as Form;
            DataTable oriTable = matm3;
            try
            {
                // Open Extract Form
                ExtractMATM.ExtractMATM extract = new ExtractMATM.ExtractMATM(this.dbaccess, oriTable);
                extract.ShowDialog(frm);

                foreach (DataRow row3 in matm3.Rows)
                {
                    if (row3.RowState != DataRowState.Deleted)
                    {
                        if (!BizFunctions.IsEmpty(row3["matnum"]))
                        {
                            string sqlDetail = "Select matnum,matname,uomcode from matm where matnum='" + row3["matnum"].ToString() + "'";
                            this.dbaccess.ReadSQL("tmpDetail", sqlDetail);
                            DataTable dtDetail = this.dbaccess.DataSet.Tables["tmpDetail"];
                            if (dtDetail.Rows.Count > 0)
                            {
                                row3["matname"] = dtDetail.Rows[0]["matname"];
                                row3["uomcode"] = dtDetail.Rows[0]["uomcode"];
                            }
                        }
                        if (BizFunctions.IsEmpty(row3["printqty"]) && !BizFunctions.IsEmpty(matm["qty"]))
                        {
                            row3["printqty"] = Convert.ToInt32(matm["qty"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(frm, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #region Print Multiple Items

        private void btn_PrintMultiple_Click(object sender, EventArgs e)
        {
            #region Variables

            DataTable matm3 = dbaccess.DataSet.Tables["matm3"];
            DataTable dtBarcode;
            DataRow drBarcode = null;

            string matname = "";
            string matnum = "";
            string parameter = "";
            string barcode = "";
            int qty = 0;
            decimal price = -1;
            string sql = "";
            bool isEmpty = false;

            #endregion

            this.result = MessageBox.Show("Start Barcode Printing?", "Print Barcode?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (this.result == DialogResult.Yes)
            {
                #region Save Before Printing

                if (matm["status"].ToString().Trim() != Common.DEFAULT_DOCUMENT_STATUSP && BizValidate.CheckRowState(this.dbaccess.DataSet, "matm/matm1"))
                {
                    MessageBox.Show("Please save before Printing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    foreach (DataRow dr3 in matm3.Rows)
                    {
                        if (dr3.RowState != DataRowState.Deleted)
                        {
                            if (!BizFunctions.IsEmpty(dr3["printqty"]))
                            {
                                qty = Convert.ToInt32(dr3["printqty"]);
                            }

                            #region Select Barcode data and assign to dtBarcode table

                            sql = "select matnum,matname,ploftcode+pflexcode+pshfcode+pcolcode+pszcode as parameter,barcode,price "
                                + "from (select m.matnum,m.matname,"
                                + "case when isnull(m.ploftcode,'')='' or isnull(m.ploftcode,'')='***' then ''+ SPACE(1) else m.ploftcode+ SPACE(1) end as ploftcode,"
                                + "case when isnull(m.pflexcode,'')='' or isnull(m.pflexcode,'')='**' then ''+ SPACE(1) else m.pflexcode+ SPACE(1) end as pflexcode,"
                                + "case when isnull(m.pshfcode,'')='' or isnull(m.pshfcode,'')='***' then ''+ SPACE(1) else m.pshfcode+ SPACE(1) end as pshfcode,"
                                + "case when isnull(m.pcolcode,'')='' or isnull(m.pcolcode,'')='****' then ''+ SPACE(1) else m.pcolcode+ SPACE(1) end as pcolcode,"
                                + "case when isnull(m.pszcode,'')='' or isnull(m.pszcode,'')='****' then ''+ SPACE(1) else m.pszcode+ SPACE(1) end as pszcode,"
                                + "m.barcode,s.retail as price "
                                + "from matm m "
                                + "left join Stocks s on s.matnum=m.matnum "
                                + "where m.matnum='"
                                + dr3["matnum"].ToString() + "') p";
                            #endregion

                            this.dbaccess.ReadSQL("tmpBarcode", sql);
                            dtBarcode = this.dbaccess.DataSet.Tables["tmpBarcode"];

                            if (dtBarcode.Rows.Count > 0)
                            {
                                drBarcode = dtBarcode.Rows[0];

                                if (drBarcode["matnum"].ToString().ToUpper().Trim() == dr3["matnum"].ToString().ToUpper().Trim())
                                {
                                    #region Create New Label, point to Label Path and Barcode Printer

                                    LGApp nice = new LGApp();
                                    LGLabel labelintf = new LGLabel();

                                    bool setprintx;
                                    if (BizFunctions.IsEmpty(matm["printPrice"]))
                                    {
                                        matm["printPrice"] = false;
                                    }
                                    long labelID = 0;
                                    if ((bool)matm["printPrice"] == true)
                                    {
                                        labelID = nice.LabelOpen(@"C:\TVBarcode\TVBarcode.lbl");
                                    }
                                    else
                                    {
                                        labelID = nice.LabelOpen(@"C:\TVBarcode\TVBarcode(NoPrice).lbl");
                                    }

                                    // get printer
                                    setprintx = nice.LabelSetPrinter((int)labelID, ConfigurationManager.AppSettings.Get("BarcodePrinter"));

                                    #endregion

                                    #region Assign Label Data

                                    barcode = drBarcode["barcode"].ToString();
                                    matname = drBarcode["matname"].ToString();
                                    matnum = drBarcode["matnum"].ToString();
                                    parameter = drBarcode["parameter"].ToString();
                                    if (!BizFunctions.IsEmpty(drBarcode["price"]))
                                    {
                                        price = Convert.ToDecimal(drBarcode["price"]);
                                    }

                                    #endregion

                                    #region Call PrintBarcode Method

                                    if ((bool)matm["printPrice"] == true)
                                    {
                                        if (!(BizFunctions.IsEmpty(matnum) && BizFunctions.IsEmpty(matname)
                                            && BizFunctions.IsEmpty(barcode)) && price > -1 && qty > 0)
                                        {
                                            PrintBarcode(drBarcode, nice, labelintf, setprintx, labelID, matname, matnum, parameter, barcode, qty, price);
                                        }
                                        else
                                        {
                                            isEmpty = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!(BizFunctions.IsEmpty(matnum) && BizFunctions.IsEmpty(matname)
                                            && BizFunctions.IsEmpty(barcode)) && qty > 0)
                                        {
                                            PrintBarcode2(drBarcode, nice, labelintf, setprintx, labelID, matname, matnum, parameter, barcode, qty);
                                        }
                                        else
                                        {
                                            isEmpty = true;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #region If Empty or Invalid Data detected, Prompt Error

                    if (isEmpty)
                    {
                        MessageBox.Show("Invalid Material Code/Material Name/Barcode,Negative Price or 0 Qty detected!", "No Label Printed For Some Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion
                }

                #endregion
            }
        }

        #endregion

        #region Print Barcode

        private void PrintBarcode(DataRow drBarcode, LGApp nice, LGLabel labelintf, bool setprintx, long labelID, string matname, string matnum, string parameter, string barcode, int qty, decimal price)
        {
            #region Split Parameter into Multiline

            if (parameter.Length > 51)
                parameter = parameter.Substring(0, 51);

            if (matname.Length > 45)
                //matname = matname.Substring(0, 45);
                matname = matname.Replace(" ", "");

            //if (parameter.Length > 17 && parameter.Substring(17, 1) != " ")
            //{
            //    parameter = matname.Substring(0, 17) + " " + parameter.Substring(17);
            //}

            #endregion

            int multiples = qty / 5;
            int remainder = qty % 5;

            #region set label
            setprintx = nice.LabelSetVar((int)labelID, "matname", matname.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "matnum", matnum.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "parameter", parameter.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "barcode", barcode, -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "price", price.ToString(), -9999, -9999);

            //setprintx = nice.LabelSetVar((int)labelID, "matname", matname.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "matnum", matnum.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "parameter", parameter.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "barcode", barcode, 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "price", price.ToString(), 5000, 5000);
            #endregion

            for (int i = 0; i < multiples; i++)
            {
                setprintx = nice.LabelPrint((int)labelID, "5");
            }

            setprintx = nice.LabelPrint((int)labelID, remainder.ToString());

            labelintf.Free();
        }

        #endregion

        #region Print Barcode2

        private void PrintBarcode2(DataRow drBarcode, LGApp nice, LGLabel labelintf, bool setprintx, long labelID, string matname, string matnum, string parameter, string barcode, int qty)
        {
            #region Split Parameter into Multiline

            if (parameter.Length > 51)
                parameter = parameter.Substring(0, 51);

            if (matname.Length > 45)
                //matname = matname.Substring(0, 45);
                matname = matname.Replace(" ", "");

            //if (parameter.Length > 17 && parameter.Substring(17, 1) != " ")
            //{
            //    parameter = matname.Substring(0, 17) + " " + parameter.Substring(17);
            //}

            #endregion

            int multiples = qty / 5;
            int remainder = qty % 5;

            #region set label
            setprintx = nice.LabelSetVar((int)labelID, "matname", matname.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "matnum", matnum.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "parameter", parameter.ToString().ToUpper(), -9999, -9999);
            setprintx = nice.LabelSetVar((int)labelID, "barcode", barcode, -9999, -9999);

            //setprintx = nice.LabelSetVar((int)labelID, "matname", matname.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "matnum", matnum.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "parameter", parameter.ToString().ToUpper(), 5000, 5000);
            //setprintx = nice.LabelSetVar((int)labelID, "barcode", barcode, 5000, 5000);
            #endregion

            for (int i = 0; i < multiples; i++)
            {
                setprintx = nice.LabelPrint((int)labelID, "5");
            }

            setprintx = nice.LabelPrint((int)labelID, remainder.ToString());

            labelintf.Free();
        }

        #endregion

        #endregion

        private bool isExistMats()
        {
            bool isExist = false;

            DataRow matm = this.dbaccess.DataSet.Tables["matm"].Rows[0];

            string str = "Select matnum from mats"+Common.DEFAULT_SYSTEM_YEAR+" where matnum='"+matm["matnum"].ToString()+"' ";

            this.dbaccess.ReadSQL("TempMats", str);

            DataTable TempMats = this.dbaccess.DataSet.Tables["TempMats"];

            if (TempMats.Rows.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }

        private void UpdateMats()
        {
            DataRow matm = this.dbaccess.DataSet.Tables["matm"].Rows[0];

            decimal cost = 0;

            cost = GetHighestPrice();

            string str = "Update mats" + Common.DEFAULT_SYSTEM_YEAR + " set lateststdcost=" + cost.ToString() + ", std0=" + cost.ToString() + ", "+
                         "std1=" + cost.ToString() + ", std2=" + cost.ToString() + ", std3=" + cost.ToString() + ", std4=" + cost.ToString() + ",  "+
                         "std5=" + cost.ToString() + ", std6=" + cost.ToString() + ", std7=" + cost.ToString() + ", std8=" + cost.ToString() + ",  "+ 
                         "std9=" + cost.ToString() + ", std10=" + cost.ToString() + ", std11=" + cost.ToString() + " , std12=" + cost.ToString() + ",   "+
                         "[user]='"+Common.DEFAULT_SYSTEM_USERNAME+"',modified='"+BizFunctions.GetSafeDateString(Convert.ToDateTime(DateTime.Now))+"' "+
                         "where matnum='" + matm["matnum"].ToString() + "'";

            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(str);

        }

        private void InsertMats()
        {
            DataRow matm = this.dbaccess.DataSet.Tables["matm"].Rows[0];

            int LastCount = BizLogicTools.Tools.getMaxID("mats" + Common.DEFAULT_SYSTEM_YEAR + "", this.dbaccess);

            decimal cost = 0;

            cost = GetHighestPrice();

            string str = "Insert into mats" + Common.DEFAULT_SYSTEM_YEAR + " "+
                            "(id,matnum,lateststdcost,std0,std1,std2,std3,std4,std5,std6,std7,std8,std9,std10,std11,std12,[user],flag,created,modified) "+
                            "VALUES "+
                            "(" + LastCount.ToString() + ",'" + matm["matnum"].ToString() + "', " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ", " + cost.ToString() + ",'"+ Common.DEFAULT_SYSTEM_USERNAME +"',GETDATE(),GETDATE()) ";


            this.dbaccess.RemoteStandardSQL.ExecuteNonQuery(str);

            BizLogicTools.Tools.updateID(this.dbaccess, "mats" + Common.DEFAULT_SYSTEM_YEAR + "", (LastCount + 1));

        }


        private decimal GetHighestPrice()
        {
            DataRow matm = this.dbaccess.DataSet.Tables["matm"].Rows[0];
            decimal amount = 0;

            string str = "SELECT "+
	                            "A.baseunitprice "+
                            "from "+
                            "( "+
                            "select  ROW_NUMBER() OVER (Order BY ISNULL(baseunitprice,0)) as ForTop,ROW_NUMBER() OVER (Order BY ISNULL(baseunitprice,0) Desc) as ForBottom,* from matm2 where matnum='" + matm["matnum"].ToString() + "' " +
                            ")A "+
                            "where A.ForBottom=1";

            this.dbaccess.ReadSQL("TempMatmPrice", str);

            DataTable TempMatmPrice = this.dbaccess.DataSet.Tables["TempMatmPrice"];

            if (TempMatmPrice.Rows.Count > 0)
            {
                amount = Convert.ToDecimal(this.dbaccess.DataSet.Tables["TempMatmPrice"].Rows[0]["baseunitprice"]);
            }

            return amount;

        }
    }
}

