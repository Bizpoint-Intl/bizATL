#region Namespaces
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Data.SqlTypes;


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
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizReport;

using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

using System.Text.RegularExpressions;
using ATL.BizModules.CompressFolders;
using ATL.TimeUtilites;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
using ATL.Network;
using ATL.TimeUtilites;

#endregion



namespace ATL.BizModules.FileAcc3
{
    class FileSendGet3
    {


        private string tmpLocation, ArNum, ServerPhysicalLocation, Type,DrvLtr = "";
        private bool UploadSuccess,DownloadSuccess = false;
        private DateTime commenceDate;
        public string finalfileName = "";
        private byte[] Buffer = null;


        public FileSendGet3()
        {
        }

        public FileSendGet3(string DriveLetter, byte[] buffer, string arnum, DateTime cDate, string tyPe)
        {
            this.DrvLtr = DriveLetter;
            this.Type = tyPe;
            this.ArNum = arnum;
            this.commenceDate = cDate;
            this.Buffer = buffer;
            FileUpload();
        }


        private void FileUpload()
        {
            
                            
          
            if (Buffer != null)
            {
                try
                {
                    string fileName = ArNum + "-" + Type + "-" + TimeTools.GetStandardSafeDateOnly3(commenceDate);

           
                    string SaveLoc = DrvLtr + ":";

                    System.IO.DirectoryInfo fl = new DirectoryInfo(SaveLoc + @"\\" + Convert.ToString(commenceDate.Year) + "\\" + ArNum + "\\" + Type + "\\" + BizFunctions.GetSafeDateString(commenceDate) + "\\");

                    if (!fl.Exists)
                    {
                        System.IO.Directory.CreateDirectory(fl.FullName);
                    }


                    FileInfo fi = new FileInfo(fl.FullName + "\\" + fileName + ".html");


                    File.WriteAllBytes(fl.FullName + "\\" + fileName + ".html", Buffer);

                    if (File.Exists(fl.FullName + "\\" + fileName + ".html"))
                    {
                        ServerPhysicalLocation = System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository") + "\\" + Convert.ToString(commenceDate.Year) + "\\" + ArNum + "\\" + Type + "\\" + BizFunctions.GetSafeDateString(commenceDate) + "\\" + fileName + ".html";
                        finalfileName = fileName + ".html";
                        UploadSuccess = true;
                    }

                    

                }
                catch (Exception ex)
                {
                    UploadSuccess = false;
                    MessageBox.Show("" + ex.ToString() + ",Unable to Upload Documents.  Please Call SSI Holdings", "BizERP", MessageBoxButtons.OK);
                }
            }           
        }

       
        private string FileNameGenerator(string filename)
        {

            string str_Tmp1 = filename.Trim();
            string newFileName = "";
            int newLastDigit = 0;
            int lastcharcount = 0;
            string newLastDigitStr = "";
            if (str_Tmp1.Trim().Length != 0)
            {
                int target = 0;
                int str_Index = 0;
                foreach (char c in str_Tmp1.Trim())
                {
                    str_Index = str_Index + 1;
                    if (c == '-')
                    {

                        target = target + 1;
                        if (target == 3)
                        {
                            newLastDigit = Convert.ToInt32(str_Tmp1.Substring(str_Index)) + 1;
                            lastcharcount = str_Index - 2;

                            if (newLastDigit < 10)
                            {
                                newLastDigitStr = "00" + Convert.ToString(newLastDigit);
                            }
                            else
                            {
                                newLastDigitStr = "0" + Convert.ToString(newLastDigit);
                            }
                            break;


                        }
                    }
                }
                newFileName = str_Tmp1.Substring(0, lastcharcount+1);
            }

            return newFileName + "-" + newLastDigitStr;
        }


        public string FileInServerLocation
        {
            get
            {
                return ServerPhysicalLocation;
            }
            set
            {
                ServerPhysicalLocation = value;
            }
        }

        public bool FileUploadSuccess
        {
            get
            {
                return UploadSuccess;
            }
            set
            {
                UploadSuccess = value;
            }
        }

        public bool FileDownloadSuccess
        {
            get
            {
                return DownloadSuccess;
            }
            set
            {
                DownloadSuccess = value;
            }
        }

    
    }
}
