#region Namespaces
using System;
using System.Windows.Forms;

using BizRAD.BizLogin;
using BizRAD.BizApplication;
using BizRAD.BizAccounts;
using BizRAD.UI.RibbonStyle;
using BizRAD.BizCommon;
using ATL.BizModules.ATRPRNT;

#region HR

#region HR Modules

using ATL.HEMP;
//using BizERP.HEMPHQ;
using ATL.LVR;

#endregion

#region HR Masters
using ATL.ENM;
using ATL.HACMS;
using ATL.MATM;
using ATL.MATMSTK;
using ATL.HEMPS;
using ATL.HIDEN;
using ATL.HWDM;
using ATL.LVM;
using ATL.LVE;
using ATL.SADJ;
using ATL.TRN;
using ATL.CFR;
using ATL.ERR;
using ATL.HPHM;
using ATL.PBHM;
using ATL.HRAM;
using ATL.HNAM;
using ATL.SADJM;
using ATL.HRPM;
using ATL.HETM;
using ATL.CSEH;
using ATL.GPFM;

#endregion

#endregion

#region Modules
using ATL.CTR;
using ATL.PCTR;
using ATL.QCT;

using ATL.ISM;

using ATL.ATR;
using ATL.WKC;
using ATL.DLA;
using ATL.MTA;
using ATL.TRQ;
#endregion

#region Masters
using ATL.WRR;
using ATL.SITMT;
using ATL.SITMD;
using ATL.EQM;
using ATL.SEM;
using ATL.Coy;
using ATL.COYSP;
using ATL.ARM;
using ATL.SHM;
using ATL.HSGM;
using ATL.HSAM;
using ATL.CRPT;
using ATL.VIEWM;
//using BizERP.LADJ;
using ATL.HRRM;
using ATL.HQAM;
using ATL.UOM;
using ATL.PCAT;
using ATL.PTYPE;
using ATL.PBRD;
using ATL.CTM;
using ATL.PRM;
using ATL.LCM;
using ATL.PMM;
using ATL.FQM;
using ATL.PDM;
using ATL.FLDM;
using ATL.DCM;
using ATL.MNM;
using ATL.LVA;
using ATL.DCTM;
using ATL.WHM;
using ATL.ISN;
#endregion

using ATL.PCS;

#region Finance
using ATL.ACM;
using ATL.ACMG; 
using ATL.ACC;
using ATL.SIV;
using ATL.SCR;
using ATL.SDB;
using ATL.SRC;
using ATL.PIV;
using ATL.PCR;
using ATL.PAYTM;
using ATL.PDB;
using ATL.PAY;
using ATL.MPAY;
using ATL.FAV;
using ATL.FAVP;
using ATL.FAVT;
using ATL.EXRATE;
using ATL.Bank;
using ATL.AccDefaults;
using DEMO.MDT;
using ATL.TDT;
using ATL.SOA;
using ATL.CCM;
using ATL.EXR;
using ATL.PTM;
using ATL.GSTM;
using ATL.PRJM;
using ATL.PTCH;
using ATL.GSTSP;
using BizRAD.Report;
using ATL.GIV;
using ATL.PD;
#endregion

#region Purchases
using ATL.APM;
using ATL.MAV;
#endregion

#region Others

using ATL.BizModules.ATR.TimeControlForm1;
using ATL.ATLInterfaceUI;
#endregion

#region Ops Masters
using ATL.SITM;
#endregion

#region Transfer Modules
using ATL.TRA;
using ATL.TRI;
using ATL.RTN;
#endregion

using ATL.POR;
using ATL.PON;
using ATL.GRN;
using ATL.WAC;
using ATL.WOR;
#endregion

using ATL.DefaultUIFile.DEFAULT_ABOUTBIZERP;
using ATL.BizModules.HEMP.Code;
using ATL.SVC;
using System.IO;


namespace Main
{
    public class MainForm : BizRAD.BizApplication.MainBaseHelper
    {
        #region Class Variables
        Form frmWAC = null;
        protected ReportBaseHelper reportBaseHelper = null;
        Form ATRform1 = null;

        Form ATRform2 = null;

        Form Interface = null;

        Form PDForm = null;

        protected string PendingPageLink = System.Configuration.ConfigurationManager.AppSettings.Get("PendingPageLink");
        #endregion

        #region Constructor

        public MainForm()
            : base("MainForm.xml")
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                string platformVersion = "";
                if (ATL.BizLogicTools.Tools.Platform == "x86")
                {
                    platformVersion = "x86";
                }
                else
                {
                    platformVersion = "x64";
                }
                string environment = System.Configuration.ConfigurationManager.AppSettings.Get("runtimeEnvironment");
                Version version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                string strVersion = version.ToString();
                base.mainBase.Text += "   [ Version No. : " + strVersion + " ] / " + platformVersion + " - " + environment;
            }
            else
            {
                string platformVersion = "";
                if (ATL.BizLogicTools.Tools.Platform == "x86")
                {
                    platformVersion = "x86";
                }
                else
                {
                    platformVersion = "x64";
                }
                string environment = System.Configuration.ConfigurationManager.AppSettings.Get("runtimeEnvironment");               
                base.mainBase.Text +=  " "+ platformVersion + " - " + environment;
            }
        }

        #endregion

        #region MainRibbon Button Clicked

        protected override void Main_RibbonButton_Click(object sender, EventArgs e)
        {
            base.Main_RibbonButton_Click(sender, e);
            string buttonName = ((RibbonButton)sender).Name;
            switch (buttonName)
            {
                /***************************************************************/

                #region H/R

                #region H/R Modules

                case "TRN":		/* Salary Adjustment */
                    VoucherBaseHelper voucher_TRN = new Voucher_TRN(buttonName, this.voucherBaseHelpers);
                    voucher_TRN.Voucher_Load();
                    break;

                case "TRNQ":		/* Salary Adjustment */
                    VoucherBaseHelper voucher_TRNQ = new Voucher_TRN(buttonName, this.voucherBaseHelpers);
                    voucher_TRNQ.Voucher_Load();
                    break;

                case "SADJ":		/* Salary Adjustment */
                    VoucherBaseHelper voucher_SADJ = new Voucher_SADJ(buttonName, this.voucherBaseHelpers);
                    voucher_SADJ.Voucher_Load();
                    break;

                case "APPR":		/* Salary Adjustment */
                    VoucherBaseHelper voucher_APPR = new Voucher_SADJ(buttonName, this.voucherBaseHelpers);
                    voucher_APPR.Voucher_Load();
                    break;

                case "APPH":		/* Salary Adjustment */
                    VoucherBaseHelper voucher_APPH = new Voucher_SADJ(buttonName, this.voucherBaseHelpers);
                    voucher_APPH.Voucher_Load();
                    break;

                case "CFR":		/* Confirmation Record */
                    VoucherBaseHelper voucher_CFR = new Voucher_CFR(buttonName, this.voucherBaseHelpers);
                    voucher_CFR.Voucher_Load();
                    break;

                case "ERR":		/* Resignation Record */
                    VoucherBaseHelper voucher_ERR = new Voucher_ERR(buttonName, this.voucherBaseHelpers);
                    voucher_ERR.Voucher_Load();
                    break;

                case "LVR":		/* Leave Record */
                    VoucherBaseHelper voucher_LVR = new Voucher_LVR(buttonName, this.voucherBaseHelpers);
                    voucher_LVR.Voucher_Load();
                    break;

                case "LVRH":		/* Leave Record */
                    VoucherBaseHelper voucher_LVRH = new Voucher_LVR(buttonName, this.voucherBaseHelpers);
                    voucher_LVRH.Voucher_Load();
                    break;

                //case "LADJ":		/* Leave Adjustment Record */
                //    VoucherBaseHelper voucher_LADJ = new Voucher_LADJ(buttonName, this.voucherBaseHelpers);
                //    voucher_LADJ.Voucher_Load();
                //    break;

                case "SITMTA":                /* Save ATR record */
                    ATL.BizModules.SADJ.SetAllocationForm1 SitmtA = new ATL.BizModules.SADJ.SetAllocationForm1();
                    SitmtA.Show();
                    SitmtA.Focus();
                    break;


                #endregion

                #region H/R Masters

                case "HEMP":		/* Employee Personal Record PO */
                    VoucherBaseHelper voucher_HEMP = new Voucher_HEMP(buttonName, this.voucherBaseHelpers, "HEMP");
                    voucher_HEMP.Voucher_Load();
                    break;

                case "CSE":		/* Employee Personal Record PO */
                    VoucherBaseHelper voucher_CSEH = new Voucher_CSEH(buttonName, this.voucherBaseHelpers);
                    voucher_CSEH.Voucher_Load();
                    break;

                case "HEMPV":		/* Employee Personal Record PO */
                    VoucherBaseHelper voucher_HEMPV = new Voucher_HEMP(buttonName, this.voucherBaseHelpers, "HEMP");
                    voucher_HEMPV.Voucher_Load();
                    break;


                case "HEMPHQ":		/* Employee Personal Record PO */
                    VoucherBaseHelper voucher_HEMPHQ = new Voucher_HEMP(buttonName, this.voucherBaseHelpers, "HEMPHQ");
                    voucher_HEMPHQ.Voucher_Load();
                    break;


                case "HEMPHQS":		/* Employee Personal Record PO */
                    VoucherBaseHelper voucher_HEMPHQS = new Voucher_HEMP(buttonName, this.voucherBaseHelpers, "HEMPHQ");
                    voucher_HEMPHQS.Voucher_Load();
                    break;

                //case "HEMPHQ":		/* Employee Personal Record HQ */
                //    VoucherBaseHelper voucher_HEMPHQ = new Voucher_HEMPHQ(buttonName, this.voucherBaseHelpers);
                //    voucher_HEMPHQ.Voucher_Load();
                //    break;

                case "HEMPN":		/* Employee Personal Record PO */
                    VoucherBaseHelper voucher_HEMPN = new Voucher_HEMP(buttonName, this.voucherBaseHelpers, "HEMPN");
                    voucher_HEMPN.Voucher_Load();
                    break;

                case "HACMS":	/* H/R Appraisal Criteria Main Section */
                    VoucherBaseHelper voucher_HACMS = new Voucher_HACMS(buttonName, this.voucherBaseHelpers);
                    voucher_HACMS.Voucher_Load();
                    break;

                case "MATM":	/* H/R matnum Master */
                    VoucherBaseHelper voucher_MATM = new Voucher_MATM(buttonName, this.voucherBaseHelpers);
                    voucher_MATM.Voucher_Load();
                    break;

                case "MATMSTK":	/* H/R matnum Master */
                    VoucherBaseHelper voucher_MATMSTK = new Voucher_MATMSTK(buttonName, this.voucherBaseHelpers);
                    voucher_MATMSTK.Voucher_Load();
                    break;

                case "UOM":	/* H/R matnum Master */
                    VoucherBaseHelper voucher_UOM = new Voucher_UOM(buttonName, this.voucherBaseHelpers);
                    voucher_UOM.Voucher_Load();
                    break;

                case "PCAT":	/* Product Category Master */
                    VoucherBaseHelper voucher_PCAT = new Voucher_PCAT(buttonName, this.voucherBaseHelpers);
                    voucher_PCAT.Voucher_Load();
                    break;
                case "CTM":	/* Country Master */
                    VoucherBaseHelper voucher_CTM = new Voucher_CTM(buttonName, this.voucherBaseHelpers);
                    voucher_CTM.Voucher_Load();
                    break;

                case "PTYPE":	/* Product Type Master */
                    VoucherBaseHelper voucher_PTYPE = new Voucher_PTYPE(buttonName, this.voucherBaseHelpers);
                    voucher_PTYPE.Voucher_Load();
                    break;
                case "PBRD":	/* Product Brand Master */
                    VoucherBaseHelper voucher_PBRD = new Voucher_PBRD(buttonName, this.voucherBaseHelpers);
                    voucher_PBRD.Voucher_Load();
                    break;



                case "HIDEN":	/* H/R Identify Type Master */
                    VoucherBaseHelper voucher_HIDEN = new Voucher_HIDEN(buttonName, this.voucherBaseHelpers);
                    voucher_HIDEN.Voucher_Load();
                    break;

                case "HPHM":	/* H/R Holiday Master */
                    VoucherBaseHelper voucher_HPHM = new Voucher_HPHM(buttonName, this.voucherBaseHelpers);
                    voucher_HPHM.Voucher_Load();
                    break;

                case "HRAM":	/* H/R Holiday Master */
                    VoucherBaseHelper voucher_HRAM = new Voucher_HRAM(buttonName, this.voucherBaseHelpers);
                    voucher_HRAM.Voucher_Load();
                    break;

                case "HWDM":	/* H/R Working Master */
                    VoucherBaseHelper voucher_HWDM = new Voucher_HWDM(buttonName, this.voucherBaseHelpers);
                    voucher_HWDM.Voucher_Load();
                    break;

                case "LVM":    /* Leave Type Master */
                    VoucherBaseHelper voucher_LVM = new Voucher_LVM(buttonName, base.voucherBaseHelpers);
                    voucher_LVM.Voucher_Load();
                    break;

                case "LVE":    /* Leave Entitlement */
                    VoucherBaseHelper voucher_LVE = new Voucher_LVE(buttonName, base.voucherBaseHelpers);
                    voucher_LVE.Voucher_Load();
                    break;

                case "LVA":    /* Leave Adjustment */
                    VoucherBaseHelper voucher_LVA = new Voucher_LVA(buttonName, base.voucherBaseHelpers);
                    voucher_LVA.Voucher_Load();
                    break;

                case "PBHM":    /* Public Holiday Master */
                    VoucherBaseHelper voucher_PBHM = new Voucher_PBHM(buttonName, base.voucherBaseHelpers);
                    voucher_PBHM.Voucher_Load();
                    break;

                case "HRPM":    /* Public Holiday Master */
                    VoucherBaseHelper voucher_HRPM = new Voucher_HRPM(buttonName, base.voucherBaseHelpers);
                    voucher_HRPM.Voucher_Load();
                    break;

                case "HETM":    /* Public Holiday Master */
                    VoucherBaseHelper voucher_HETM = new Voucher_HETM(buttonName, base.voucherBaseHelpers);
                    voucher_HETM.Voucher_Load();
                    break;

                case "EQM":	/* Site Master */
                    VoucherBaseHelper voucher_EQM = new Voucher_EQM(buttonName, this.voucherBaseHelpers);
                    voucher_EQM.Voucher_Load();
                    break;

                case "SEM":	/* Shift Master */
                    VoucherBaseHelper voucher_SEM = new Voucher_SEM(buttonName, this.voucherBaseHelpers);
                    voucher_SEM.Voucher_Load();
                    break;

                case "HSGM":	/* Shift Master */
                    VoucherBaseHelper voucher_HSGM = new Voucher_HSGM(buttonName, this.voucherBaseHelpers);
                    voucher_HSGM.Voucher_Load();
                    break;

                case "HSAM":	/* Shift Master */
                    VoucherBaseHelper voucher_HSAM = new Voucher_HSAM(buttonName, this.voucherBaseHelpers);
                    voucher_HSAM.Voucher_Load();
                    break;

                case "HNAM":	/* Shift Master */
                    VoucherBaseHelper voucher_HNAM = new Voucher_HNAM(buttonName, this.voucherBaseHelpers);
                    voucher_HNAM.Voucher_Load();
                    break;

                case "HRRM":	/* Shift Master */
                    VoucherBaseHelper voucher_HRRM = new Voucher_HRRM(buttonName, this.voucherBaseHelpers);
                    voucher_HRRM.Voucher_Load();
                    break;

                case "HEMPS":	/* Status Master */
                    VoucherBaseHelper voucher_HEMPS = new Voucher_HEMPS(buttonName, this.voucherBaseHelpers);
                    voucher_HEMPS.Voucher_Load();
                    break;

                case "SHM":	/* Sector Master */
                    VoucherBaseHelper voucher_SHM = new Voucher_SHM(buttonName, this.voucherBaseHelpers);
                    voucher_SHM.Voucher_Load();
                    break;

                case "HQAM":	/* Sector Master */
                    VoucherBaseHelper voucher_HQAM = new Voucher_HQAM(buttonName, this.voucherBaseHelpers);
                    voucher_HQAM.Voucher_Load();
                    break;

                case "SADJM":	/* Sector Master */
                    VoucherBaseHelper voucher_SADJM = new Voucher_SADJM(buttonName, this.voucherBaseHelpers);
                    voucher_SADJM.Voucher_Load();
                    break;


                case "GPFM":		/* Employee Personal Record PO */
                    VoucherBaseHelper voucher_GPFM = new Voucher_GPFM(buttonName, this.voucherBaseHelpers);
                    voucher_GPFM.Voucher_Load();
                    break;


                case "SADJGEN":                /* Save ATR record */
                    ATL.BizModules.SADJ.MassSalaryAdjustment GenSADJ = new ATL.BizModules.SADJ.MassSalaryAdjustment();
                    GenSADJ.Show();
                    GenSADJ.Focus();
                    break;
              
                #endregion

                #endregion

                #region Operations

                #region Operations Masters

                case "WRR":	/* Weekly Rooster Master */
                    VoucherBaseHelper voucher_WRR = new Voucher_WRR(buttonName, this.voucherBaseHelpers);
                    voucher_WRR.Voucher_Load();
                    break;

                    //case "WRRB":	/* Weekly Rooster Master */
                    //    VoucherBaseHelper voucher_WRR_B = new Voucher_WRR(buttonName, this.voucherBaseHelpers,"B");
                    //    voucher_WRR_B.Voucher_Load();
                    //    break;

                    //case "WRRC":	/* Weekly Rooster Master */
                    //    VoucherBaseHelper voucher_WRR_C = new Voucher_WRR(buttonName, this.voucherBaseHelpers,"C");
                    //    voucher_WRR_C.Voucher_Load();
                    //    break;

                    //case "WRRD":	/* Weekly Rooster Master */
                    //    VoucherBaseHelper voucher_WRR_D = new Voucher_WRR(buttonName, this.voucherBaseHelpers,"D");
                    //    voucher_WRR_D.Voucher_Load();
                    //    break;

                    //case "WRRHQ":	/* Weekly Rooster Master */
                    //    VoucherBaseHelper voucher_WRR_HQ = new Voucher_WRR(buttonName, this.voucherBaseHelpers, "HQ");
                    //    voucher_WRR_HQ.Voucher_Load();
                

                case "SITMT":	/* Site Master */
                    VoucherBaseHelper voucher_SITMT = new Voucher_SITMT(buttonName, this.voucherBaseHelpers);
                    voucher_SITMT.Voucher_Load();
                    break;

                case "PSITM":	/* Site Master */
                    VoucherBaseHelper voucher_PSITMT = new Voucher_SITMT(buttonName, this.voucherBaseHelpers);
                    voucher_PSITMT.Voucher_Load();
                    break;

                case "WHM":   /* WHM Master */
                    VoucherBaseHelper voucher_WHM = new Voucher_WHM(buttonName, this.voucherBaseHelpers);
                    voucher_WHM.Voucher_Load();
                    break;

                case "SITMD":	/* Site Master */
                    VoucherBaseHelper voucher_SITMD = new Voucher_SITMD(buttonName, this.voucherBaseHelpers);
                    voucher_SITMD.Voucher_Load();
                    break;

                #endregion

                #region Operations Modules

                case "ATR":	/* Attendance Record */
                    VoucherBaseHelper voucher_ATR_A = new Voucher_ATR(buttonName, this.voucherBaseHelpers, "A");
                    voucher_ATR_A.Voucher_Load();

                    break;

                case "MAV":	/* Attendance Record */
                    VoucherBaseHelper voucher_MAV = new Voucher_MAV(buttonName, this.voucherBaseHelpers);
                    voucher_MAV.Voucher_Load();

                    break;

                case "LMAV":	/* Attendance Record */
                    VoucherBaseHelper voucher_LMAV = new Voucher_MAV(buttonName, this.voucherBaseHelpers);
                    voucher_LMAV.Voucher_Load();

                    break;
                //case "ATRB":	/* Attendance Record */
                //    VoucherBaseHelper voucher_ATR_B = new Voucher_ATR(buttonName, this.voucherBaseHelpers,"B");
                //    voucher_ATR_B.Voucher_Load();
                //    break;


                //case "ATRC":	/* Attendance Record */
                //    VoucherBaseHelper voucher_ATR_C = new Voucher_ATR(buttonName, this.voucherBaseHelpers, "C");
                //    voucher_ATR_C.Voucher_Load();
                //    break;

                //case "ATRD":	/* Attendance Record */
                //    VoucherBaseHelper voucher_ATR_D = new Voucher_ATR(buttonName, this.voucherBaseHelpers, "D");
                //    voucher_ATR_D.Voucher_Load();
                //    break;

                //case "ATRHQ":	/* Attendance Record */
                //    VoucherBaseHelper voucher_ATR_HQ = new Voucher_ATR(buttonName, this.voucherBaseHelpers, "HQ");
                //    voucher_ATR_HQ.Voucher_Load();
                //    break;

                //case "ATR-C":	/* Attendance Record */
                //    VoucherBaseHelper voucher_ATR_C = new Voucher_ATR(buttonName, this.voucherBaseHelpers);
                //    voucher_ATR_C.Voucher_Load();
                //    break;

                //case "ATR-D":	/* Attendance Record */
                //    VoucherBaseHelper voucher_ATR_D = new Voucher_ATR(buttonName, this.voucherBaseHelpers);
                //    voucher_ATR_D.Voucher_Load();
                //    break;

                case "DLA":	/* Attendance Record */
                    VoucherBaseHelper voucher_DLA = new Voucher_DLA(buttonName, this.voucherBaseHelpers);
                    voucher_DLA.Voucher_Load();
                    break;

                ///////////////////////////////////

                case "WKCA":	/* Attendance Record */
                    VoucherBaseHelper voucher_WKC_A = new Voucher_WKC(buttonName, this.voucherBaseHelpers, "A");
                    voucher_WKC_A.Voucher_Load();
                    break;

                case "MTAA":	/* Attendance Record */
                    VoucherBaseHelper voucher_MTA_A = new Voucher_MTA(buttonName, this.voucherBaseHelpers, "A");
                    voucher_MTA_A.Voucher_Load();
                    break;

                ///////////////////////////////////

                case "WKCB":	/* Attendance Record */
                    VoucherBaseHelper voucher_WKC_B = new Voucher_WKC(buttonName, this.voucherBaseHelpers, "B");
                    voucher_WKC_B.Voucher_Load();
                    break;

                case "MTAB":	/* Attendance Record */
                    VoucherBaseHelper voucher_MTA_B = new Voucher_MTA(buttonName, this.voucherBaseHelpers, "B");
                    voucher_MTA_B.Voucher_Load();
                    break;

                ///////////////////////////////////

                case "WKCC":	/* Attendance Record */
                    VoucherBaseHelper voucher_WKC_C = new Voucher_WKC(buttonName, this.voucherBaseHelpers, "C");
                    voucher_WKC_C.Voucher_Load();
                    break;

                case "MTAC":	/* Attendance Record */
                    VoucherBaseHelper voucher_MTA_C = new Voucher_MTA(buttonName, this.voucherBaseHelpers, "C");
                    voucher_MTA_C.Voucher_Load();
                    break;

                ///////////////////////////////////

                case "WKCD":	/* Attendance Record */
                    VoucherBaseHelper voucher_WKC_D = new Voucher_WKC(buttonName, this.voucherBaseHelpers, "D");
                    voucher_WKC_D.Voucher_Load();
                    break;

                case "MTAD":	/* Attendance Record */
                    VoucherBaseHelper voucher_MTA_D = new Voucher_MTA(buttonName, this.voucherBaseHelpers, "D");
                    voucher_MTA_D.Voucher_Load();
                    break;

                ///////////////////////////////////

                case "MTAHQ":	/* Attendance Record */
                    VoucherBaseHelper voucher_MTA_HQ = new Voucher_MTA(buttonName, this.voucherBaseHelpers, "HQ");
                    voucher_MTA_HQ.Voucher_Load();
                    break;

                case "TRQ":	/* Transfer Request */
                    VoucherBaseHelper voucher_TRQ = new Voucher_TRQ("TRQ", this.voucherBaseHelpers);
                    voucher_TRQ.Voucher_Load();
                    break;

                case "TRA":	/* Transfer Allowed */
                    VoucherBaseHelper voucher_TRA = new Voucher_TRA("TRA", this.voucherBaseHelpers);
                    voucher_TRA.Voucher_Load();
                    break;


                case "TRI":	/* Transfer Receive */
                    VoucherBaseHelper voucher_TRI = new Voucher_TRI("TRI", this.voucherBaseHelpers);
                    voucher_TRI.Voucher_Load();
                    break;

                case "RTN":	/* Transfer Receive */
                    VoucherBaseHelper voucher_RTN = new Voucher_RTN("RTN", this.voucherBaseHelpers);
                    voucher_RTN.Voucher_Load();
                    break;

                case "LRTN":	/* Transfer Receive */
                    VoucherBaseHelper voucher_LRTN = new Voucher_RTN("LRTN", this.voucherBaseHelpers);
                    voucher_LRTN.Voucher_Load();
                    break;

                case "LTRQ":	/* Transfer Request */
                    VoucherBaseHelper voucher_LTRQ = new Voucher_TRQ("LTRQ", this.voucherBaseHelpers);
                    voucher_LTRQ.Voucher_Load();
                    break;

                case "LTRA":	/* Transfer Allowed */
                    VoucherBaseHelper voucher_LTRA = new Voucher_TRA("LTRA", this.voucherBaseHelpers);
                    voucher_LTRA.Voucher_Load();
                    break;


                case "LTRI":	/* Transfer Receive */
                    VoucherBaseHelper voucher_LTRI = new Voucher_TRI("LTRI", this.voucherBaseHelpers);
                    voucher_LTRI.Voucher_Load();
                    break;

                case "WOR":	/* Transfer Record */
                    VoucherBaseHelper voucher_WOR = new Voucher_WOR(buttonName, this.voucherBaseHelpers);
                    voucher_WOR.Voucher_Load();
                    break;

                case "PWOR":	/* Transfer Record */
                    VoucherBaseHelper voucher_PWOR = new Voucher_WOR(buttonName, this.voucherBaseHelpers);
                    voucher_PWOR.Voucher_Load();
                    break;

                case "SVC":	/* Transfer Record */
                    VoucherBaseHelper voucher_SVC = new Voucher_SVC(buttonName, this.voucherBaseHelpers);
                    voucher_SVC.Voucher_Load();
                    break;

                case "Pend":	/* Transfer Receive */
                    //WebVro v = new WebVro();
                    //v.Show();

                    if (File.Exists("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"))
                    {
                        System.Diagnostics.Process.Start("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome", PendingPageLink + "?userid=" + Common.DEFAULT_SYSTEM_USERNAME + " ");//"http://192.168.1.227/ci/pending?userid="++""
                    }
                    else if (File.Exists("C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"))
                    {
                        System.Diagnostics.Process.Start("C:\\Program Files\\Google\\Chrome\\Application\\chrome", PendingPageLink + "?userid=" + Common.DEFAULT_SYSTEM_USERNAME + " ");//"http://192.168.1.227/ci/pending?userid="++""
                    }
                    break;


                case "ISM":	/* Transfer Record */
                    VoucherBaseHelper voucher_ISM = new Voucher_ISM(buttonName, this.voucherBaseHelpers);
                    voucher_ISM.Voucher_Load();
                    break;


                case "ISN":	/* Attendance Record */
                    VoucherBaseHelper voucher_ISN= new Voucher_ISN(buttonName, this.voucherBaseHelpers);
                    voucher_ISN.Voucher_Load();
                    break;

                case "WAC":  /* Weighted Average Cost*/
                    if (frmWAC == null || frmWAC.IsDisposed == true) frmWAC = new WAC_Main("");
                    frmWAC.Show();
                    frmWAC.Focus();
                    break;

                #endregion

                #region Attendance Form

                //case "ATRFORM":                /* Save ATR record */
                //    if (ATRform1 == null || ATRform1.IsDisposed == true) ATRform1 = new ATRform1();
                //    ATRform1.Show();
                //    ATRform1.Focus();
                //    break;

                case "TIMECONTROL":                /* Save ATR record */
                    TimeControlForm1 TimeControl1 = new TimeControlForm1();
                    TimeControl1.Show();
                    TimeControl1.Focus();
                    break;


                case "ATRF":                /* Save ATR record */
                    ATL.BizModules.ATRPRNT.ATRFilter ATRfilter = new ATL.BizModules.ATRPRNT.ATRFilter();
                    ATRfilter.Show();
                    ATRfilter.Focus();
                    break;


                case "INT":
                    ATL.ATLInterfaceUI.Interface interfaceForm = new Interface();
                    interfaceForm.Show();
                    interfaceForm.Focus();
                    break;


                case "WORGEN":                /* Save ATR record */
                    ATL.BizModules.WOR.GenWorkOrderForm1_02042015 GenWOR = new ATL.BizModules.WOR.GenWorkOrderForm1_02042015();
                    GenWOR.Show();
                    GenWOR.Focus();
                    break;


                #endregion

                #endregion

                #region Contracts

                #region Contracts & Ad-hoc

                case "QCT":	/* Transfer Record */
                    VoucherBaseHelper voucher_QCT = new Voucher_QCT(buttonName, this.voucherBaseHelpers);
                    voucher_QCT.Voucher_Load();
                    break;

                case "CTR":	/* Contract Record */
                    VoucherBaseHelper voucher_CTR = new Voucher_CTR(buttonName, this.voucherBaseHelpers);
                    voucher_CTR.Voucher_Load();
                    break;

                case "PCTR":	/* Contract Record */
                    VoucherBaseHelper voucher_PCTR = new Voucher_PCTR(buttonName, this.voucherBaseHelpers);
                    voucher_PCTR.Voucher_Load();
                    break;

                case "ADH":	/* Transfer Record */
                    VoucherBaseHelper voucher_ADH = new Voucher_CTR(buttonName, this.voucherBaseHelpers);
                    voucher_ADH.Voucher_Load();
                    break;

                case "PADH":	/* Contract Record */
                    VoucherBaseHelper voucher_PADH = new Voucher_PCTR(buttonName, this.voucherBaseHelpers);
                    voucher_PADH.Voucher_Load();
                    break;

                #endregion

                #region Masters

                case "SITM":    /*  */
                    VoucherBaseHelper voucher_SITM = new Voucher_SITM(buttonName, base.voucherBaseHelpers);
                    voucher_SITM.Voucher_Load();
                    break;

                case "SITME":    /*  */
                    VoucherBaseHelper voucher_SITME = new Voucher_SITM(buttonName, base.voucherBaseHelpers);
                    voucher_SITME.Voucher_Load();
                    break;

                case "PRM":    /*  */
                    VoucherBaseHelper voucher_PRM = new Voucher_PRM(buttonName, base.voucherBaseHelpers);
                    voucher_PRM.Voucher_Load();
                    break;

                case "LCM":    /*  */
                    VoucherBaseHelper voucher_LCM = new Voucher_LCM(buttonName, base.voucherBaseHelpers);
                    voucher_LCM.Voucher_Load();
                    break;

                case "PMM":    /*  */
                    VoucherBaseHelper voucher_PMM = new Voucher_PMM(buttonName, base.voucherBaseHelpers);
                    voucher_PMM.Voucher_Load();
                    break;

                case "MNM":    /*  */
                    VoucherBaseHelper voucher_MNM = new Voucher_MNM(buttonName, base.voucherBaseHelpers);
                    voucher_MNM.Voucher_Load();
                    break;

                #endregion

                case "DCTM":	/* Transfer Record */
                    VoucherBaseHelper voucher_DCTM = new Voucher_DCTM(buttonName, this.voucherBaseHelpers);
                    voucher_DCTM.Voucher_Load();
                    break;

                #endregion

                #region Finance

                #region Finance Modules

                case "SIV":		/* Sales Invoice Module*/
                    VoucherBaseHelper voucher_SIV = new Voucher_SIV(buttonName, this.voucherBaseHelpers);
                    voucher_SIV.Voucher_Load();
                    break;

                case "SCR":		/* Sales Credit Note Module*/
                    VoucherBaseHelper voucher_SCR = new Voucher_SCR(buttonName, this.voucherBaseHelpers);
                    voucher_SCR.Voucher_Load();
                    break;


                case "SDB":		/* Sales Debit Note Module*/
                    VoucherBaseHelper voucher_SDB = new Voucher_SDB(buttonName, this.voucherBaseHelpers);
                    voucher_SDB.Voucher_Load();
                    break;

                case "SRC":		/* Sales Receipt Module*/
                    VoucherBaseHelper voucher_SRC = new Voucher_SRC(buttonName, this.voucherBaseHelpers);
                    voucher_SRC.Voucher_Load();
                    break;


                case "MSRC":		/* Sales Receipt Module*/
                    VoucherBaseHelper voucher_MSRC = new Voucher_SRC(buttonName, this.voucherBaseHelpers);
                    voucher_MSRC.Voucher_Load();
                    break;

                case "PTR":		/* Sales Credit Note Module*/
                    VoucherBaseHelper voucher_PTR = new Voucher_SRC(buttonName, this.voucherBaseHelpers);
                    voucher_PTR.Voucher_Load();
                    break;

                case "PIV":		/* Purchase Invoice */
                    VoucherBaseHelper voucher_PIV = new Voucher_PIV(buttonName, this.voucherBaseHelpers);
                    voucher_PIV.Voucher_Load();
                    break;

                case "GIV":		/* Purchase Invoice */
                    VoucherBaseHelper voucher_GIV = new Voucher_GIV(buttonName, this.voucherBaseHelpers);
                    voucher_GIV.Voucher_Load();
                    break;

                case "PCR":		/* Purchase Credit Note */
                    VoucherBaseHelper voucher_PCR = new Voucher_PCR(buttonName, this.voucherBaseHelpers);
                    voucher_PCR.Voucher_Load();
                    break;

                case "PDB":		/* Purchase Debit Note */
                    VoucherBaseHelper voucher_PDB = new Voucher_PDB(buttonName, this.voucherBaseHelpers);
                    voucher_PDB.Voucher_Load();
                    break;

                case "PAY":		/* Purchase Payment */
                    VoucherBaseHelper voucher_PAY = new Voucher_PAY(buttonName, this.voucherBaseHelpers, "PAY");
                    voucher_PAY.Voucher_Load();
                    break;

                case "MPAY":		/* Purchase Payment */
                    VoucherBaseHelper voucher_MPAY = new Voucher_MPAY(buttonName, this.voucherBaseHelpers, "MPAY");
                    voucher_MPAY.Voucher_Load();
                    break;

                case "GHQC":		/* Purchase Payment 2 */
                    VoucherBaseHelper voucher_GHQC = new Voucher_PAY(buttonName, this.voucherBaseHelpers, "GHQC");
                    voucher_GHQC.Voucher_Load();
                    break;

                case "PTP":		/* Purchase Payment */
                    VoucherBaseHelper voucher_PTP = new Voucher_PAY(buttonName, this.voucherBaseHelpers, "PTP");
                    voucher_PTP.Voucher_Load();
                    break;

                case "PCS":		/* Petty Cash */
                    VoucherBaseHelper voucher_PCS = new Voucher_PCS(buttonName, this.voucherBaseHelpers);
                    voucher_PCS.Voucher_Load();
                    break;

                case "FAV":		/* Journal */
                    VoucherBaseHelper voucher_FAV = new Voucher_FAV(buttonName, this.voucherBaseHelpers, "FAV");
                    voucher_FAV.Voucher_Load();
                    break;

                case "FAVP":		/* Journal */
                    VoucherBaseHelper voucher_FAVP = new Voucher_FAVP(buttonName, this.voucherBaseHelpers, "FAVP");
                    voucher_FAVP.Voucher_Load();
                    break;

                case "PTC":		/* Journal */
                    VoucherBaseHelper voucher_PTC = new Voucher_FAV(buttonName, this.voucherBaseHelpers, "PTC");
                    voucher_PTC.Voucher_Load();
                    break;

                case "FAVT":		/* Journal Template*/
                    VoucherBaseHelper voucher_FAVT = new Voucher_FAVT(buttonName, this.voucherBaseHelpers);
                    voucher_FAVT.Voucher_Load();
                    break;

                case "BANK":	/* BANK RECON */
                    new BANK();
                    break;

                case "EXR":		/* Currency Master */
                    VoucherBaseHelper voucher_EXR = new Voucher_EXR(buttonName, this.voucherBaseHelpers);
                    voucher_EXR.Voucher_Load();
                    break;

                case "EXRATE":		/* System Periods */
                    new EXRATE();
                    break;

                case "PTM":	/* Payment Term Master */
                    VoucherBaseHelper voucher_PTM = new Voucher_PTM(buttonName, this.voucherBaseHelpers);
                    voucher_PTM.Voucher_Load();
                    break;

                case "ARPT":
                    ReportDesigner CReport = new ReportDesigner(true);
                    break;

                case "FINRPT":  /* Financial Worksheet */
                    VoucherBaseHelper voucher = new ATL.FINRPT.Voucher_FINRPT(buttonName, this.voucherBaseHelpers);
                    voucher.Voucher_Load();
                    break;
                #endregion

                #region Finance Settings

                case "ACM":		/* Chart of Account Master */
                    VoucherBaseHelper voucher_ACM = new Voucher_ACM(buttonName, this.voucherBaseHelpers);
                    voucher_ACM.Voucher_Load();
                    break;

                case "ACMG":		/* Chart of Account Master */
                    VoucherBaseHelper voucher_ACMG = new Voucher_ACMG(buttonName, this.voucherBaseHelpers);
                    voucher_ACMG.Voucher_Load();
                    break;

                case "ACC":		/* Default Accounts */
                    new ACC();
                    break;

                case "AccDefaults":     /* Accounting Defaults*/
                    Form frmAccDef = new AccountDefaults();
                    frmAccDef.ShowDialog();
                    break;

                case "CCM":		/* Cost Center Master */
                    VoucherBaseHelper voucher_CCM = new Voucher_CCM(buttonName, this.voucherBaseHelpers);
                    voucher_CCM.Voucher_Load();
                    break;

                case "PRJM":		/* Cost Center Master */
                    VoucherBaseHelper voucher_PRJM = new Voucher_PRJM(buttonName, this.voucherBaseHelpers);
                    voucher_PRJM.Voucher_Load();
                    break;

                case "MDT":		/* Account Module*/
                    VoucherBaseHelper voucher_MDT = new Voucher_MDT(buttonName, this.voucherBaseHelpers);
                    voucher_MDT.Voucher_Load();
                    break;

                case "TDT":		/* Account Module*/
                    VoucherBaseHelper voucher_TDT = new Voucher_TDT(buttonName, this.voucherBaseHelpers);
                    voucher_TDT.Voucher_Load();
                    break;

                case "PAYTM":		/* Account Module*/
                    VoucherBaseHelper voucher_PAYTM = new Voucher_PAYTM(buttonName, this.voucherBaseHelpers);
                    voucher_PAYTM.Voucher_Load();
                    break;

                case "GSTM":		/* GST Master */
                    VoucherBaseHelper voucher_GSTM = new Voucher_GSTM(buttonName, this.voucherBaseHelpers);
                    voucher_GSTM.Voucher_Load();
                    break;

                case "GSTSP":  /*Tax Rates */
                    GSTSP gstsp = new GSTSP();
                    break;

                case "ARM":	/* Customer Record */
                    VoucherBaseHelper voucher_ARM = new Voucher_ARM(buttonName, this.voucherBaseHelpers);
                    voucher_ARM.Voucher_Load();
                    break;

                case "PTCH":	/* Customer Record */
                    VoucherBaseHelper voucher_PTCH = new Voucher_PTCH(buttonName, this.voucherBaseHelpers);
                    voucher_PTCH.Voucher_Load();
                    break;

                case "COY":		/* Company Settings */
                    new COYSP();
                    //new COY();
                    break;

                case "PD":	/*  */
                 new PD();
                    
                    break;

                #endregion

                #endregion

                #region Purchase

                case "APM":	/* Supplier Master */
                    VoucherBaseHelper voucher_APM = new Voucher_APM(buttonName, this.voucherBaseHelpers);
                    voucher_APM.Voucher_Load();
                    break;

                case "POR":      /* Purchase Order*/
                    VoucherBaseHelper voucher_POR = new Voucher_POR(buttonName, this.voucherBaseHelpers);
                    voucher_POR.Voucher_Load();
                    break;
                case "PON":       /* Purchase Order*/
                    VoucherBaseHelper voucher_PON = new Voucher_PON(buttonName, this.voucherBaseHelpers);
                    voucher_PON.Voucher_Load();
                    break;
                case "GRN":       /* Good Receipt Note*/
                    VoucherBaseHelper voucher_GRN = new Voucher_GRN(buttonName, this.voucherBaseHelpers);
                    voucher_GRN.Voucher_Load();
                    break;
                #endregion

                #region Reports

                case "CRPT":
                    VoucherBaseHelper voucher_crpt = new Voucher_CRPT(buttonName, this.voucherBaseHelpers);
                    voucher_crpt.Voucher_Load();
                    break;
                case "VIEWM":
                    VoucherBaseHelper voucher_viewm = new Voucher_VIEW(buttonName, this.voucherBaseHelpers);
                    voucher_viewm.Voucher_Load();
                    break;

                case "FQM":
                    VoucherBaseHelper voucher_FQM = new Voucher_FQM(buttonName, this.voucherBaseHelpers);
                    voucher_FQM.Voucher_Load();
                    break;

                case "PDM":
                    VoucherBaseHelper voucher_PDM = new Voucher_PDM(buttonName, this.voucherBaseHelpers);
                    voucher_PDM.Voucher_Load();
                    break;

                #region ACCOUNTING REPORT
                case "ACCRPT":
                    StandardReports accountingReports = new StandardReports("Accounting");
                    accountingReports.ShowDialog();
                    break;

                case "SOA":     /* Statement of Accounts */
                    SOASP soasp = new SOASP();
                    break;
                #endregion

                #endregion

                #region Settings

                case "FLDM":	/* Folder Master */
                    VoucherBaseHelper voucher_FLDM = new Voucher_FLDM(buttonName, this.voucherBaseHelpers);
                    voucher_FLDM.Voucher_Load();
                    break;

                case "DCM":	/* Document Master */
                    VoucherBaseHelper voucher_DCM = new Voucher_DCM(buttonName, this.voucherBaseHelpers);
                    voucher_DCM.Voucher_Load();
                    break;

                case "ENM":	/* Folder Master */
                    VoucherBaseHelper voucher_ENM = new Voucher_ENM(buttonName, this.voucherBaseHelpers);
                    voucher_ENM.Voucher_Load();
                    break;

                #endregion

                /***************************************************************/
            }
        }

        #endregion

        #region Main_TabControl_ClosePressed & Main_Form_Closing

        protected override void Main_TabControl_ClosePressed(System.Windows.Forms.Form currentForm)
        {
            switch (currentForm.Tag.ToString())
            {
                case "Reports":
                    this.reportBaseHelper.AllReport_Cancel();
                    break;
            }
        }

        protected override void Main_Form_Closing(System.Windows.Forms.Form currentForm)
        {
            switch (currentForm.Tag.ToString())
            {
                case "Reports":
                    this.reportBaseHelper.AllReport_Cancel();
                    break;
            }
        }
        #endregion
    }

    #region Start
    class Start
    {
        [STAThread]
        public static void Main(string[] args)
        {


            Application.EnableVisualStyles();
            SystemConfiguration.Configuration();

            Login login = new Login();
            login.ShowDialog();

            if (login.IsLogin == true)
            {
                MainForm mainForm = new MainForm();

                mainForm.Main_Load();
            }
        }


    }
    #endregion
}