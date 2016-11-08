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
using System.Management;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizReport;
using ATL.Network;
#endregion

using ATL.BizLogicTools;

namespace ATL.BizModules.StaCompressFolders
{
    public partial class StaDecommpress : Form
    {
        DBAccess dbaccess = null;
        byte[] buffer;
        String temploc,TableName,refnum,headerTable  = "";
        public StaDecommpress(DBAccess dbaccess,string TnameHeader, string Tname, string Ref)
        {
            this.dbaccess = dbaccess;
            this.TableName = Tname;
            this.headerTable = TnameHeader;
            this.refnum = Ref;
            temploc = System.IO.Path.GetTempPath();
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Select files for subsequent addition to the list of
        /// files to be archived
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // configure the open file dialog
            openFileDialog1.Title = "Add File";
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FileName = "";

            // return if the user cancels the operation
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            // set a local variable to contain the file name
            // captured from the open file dialog
            string sFilePath;
            sFilePath = openFileDialog1.FileName;
            if (sFilePath == "")
                return;

            // make sure the file exists before adding 
            // its path to the list of files to be
            // compressed
            //if (System.IO.File.Exists(sFilePath) == false)
            //    return;
            //else
            //    txtAddFile.Text = sFilePath;

        }
   

        /// <summary>
        /// Button click handler used to set the path to
        /// the zipped file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveBrowse_Click(object sender, EventArgs e)
        {
            // clear the folder path
            txtSaveTo.Text = string.Empty;

            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSaveTo.Text = folderBrowserDialog1.SelectedPath;
            }
        }


        /// <summary>
        /// Collect the files into a common folder location, zip the
        /// files up, and delete the copied files and folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            string sq1 = "Select * from "+ TableName +" where mark=1";
            DataTable TableNameTmp = BizFunctions.ExecuteQuery(this.dbaccess.DataSet, sq1);

            string DriveLetter = NetworkDrive.MapNetworkDrive(System.Configuration.ConfigurationManager.AppSettings.Get("BizERPDocsRepository"));

            if (DriveLetter.Trim() != "")
            {
                if (TableNameTmp.Rows.Count > 0)
                {

                    foreach (DataRow dr in TableNameTmp.Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted)
                        {


                            // make sure there is a destination defined
                            if (txtSaveTo.Text == string.Empty)
                            {
                                MessageBox.Show("No destination file has been defined.", "Save To Empty");
                                return;
                            }

                            // name the zip file whatever the folder is named
                            // by splitting the file path to get the folder name
                            string[] sTemp = txtSaveTo.Text.Split('\\');
                            //string sZipFileName = dr[refnum].ToString() + "_" + BizFunctions.GetSafeDateString(dt);  // Jason 21/01/2015: Problems with downloading more than 3 files
                            string sZipFileName = dr[refnum].ToString() + "_" + BizFunctions.GetSafeDateString(dt) + "_" + BizLogicTools.Tools.getGUID();

                            // check to see if zipped file already exists
                            // user may rename it in the text box if it does.

                            //FileInfo fi = new FileInfo(txtSaveTo.Text + "\\" + sZipFileName + "_" + "001" + "\\");
                            //System.IO.DirectoryInfo fl = new DirectoryInfo(txtSaveTo.Text + "\\" + sZipFileName + "_" + "001" + "\\"); // Jason 21/01/2015: Problems with downloading more than 3 files
                            System.IO.DirectoryInfo fl = new DirectoryInfo(txtSaveTo.Text + "\\" + sZipFileName + "\\");

                            //string currentFilename = sZipFileName + "_" + "001"; // Jason 21/01/2015: Problems with downloading more than 3 files
                            string currentFilename = sZipFileName;
                            while (fl.Exists)
                            {
                                //currentFilename = FileNameGenerator(currentFilename); // Jason 21/01/2015: Problems with downloading more than 3 files
                                currentFilename = dr[refnum].ToString() + "_" + BizFunctions.GetSafeDateString(dt) + "_" + BizLogicTools.Tools.getGUID();
                                fl = new DirectoryInfo(txtSaveTo.Text + "\\" + currentFilename);

                            }

                            fl = new DirectoryInfo(txtSaveTo.Text + "\\" + currentFilename);
                            System.IO.Directory.CreateDirectory(fl.FullName);



                            // Check for the existence of the target folder and
                            // create it if it does not exist
                            if (!System.IO.Directory.Exists(temploc + "\\TempZipFile\\"))
                            {
                                System.IO.Directory.CreateDirectory(temploc + "\\TempZipFile\\");
                            }

                            // Set up a string to hold the path to the temp folder

                            string sZipTmpFileName = BizLogicTools.Tools.getGUID() + sZipFileName;
                            string sTargetFolderPath = (temploc + "\\TempZipFile\\");


                            string sTargetFileFolderPath = (temploc + "\\TempZipFile\\" + sZipTmpFileName + ".zip");


                            // Unzip up the files
                            try
                            {
                                //byte[] file = System.IO.File.ReadAllBytes(tmpLocation.ToString().Trim());
                                byte[] file = System.IO.File.ReadAllBytes(dr["physicalserverlocation"].ToString().Trim());

                                System.IO.File.WriteAllBytes(sTargetFileFolderPath, file);                                

                                ExtractAll(sTargetFileFolderPath, fl.FullName + "\\");

                                System.IO.Directory.Delete(temploc + "\\TempZipFile\\", true);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString(), "Zip Operation Error");
                                this.Dispose();
                            }
                        }
                    }

                    this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("No Items where selected for Download");
            }
            NetworkDrive.DisconnectNetworkDrive(true);
        }


        #region Auto Generate Folder No

        private string FileNameGenerator(string filename)
        {

            string str_Tmp1 = filename.Trim();
            string newFileName = "";
            int newLastDigit = 0;
            int lastcharcount = 0;
            string newLastDigitStr="";
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
                            

                        }
                    }
                }
                newFileName = str_Tmp1.Substring(1, lastcharcount);
            }
        
            return newFileName + "-" + newLastDigitStr;
        }
        #endregion

        #region Extract All

        public void ExtractAll(string CurrZippedPath, string NewZippedPath)
        {
            StringBuilder newPath = new StringBuilder(NewZippedPath);

            ZipInputStream zipIn = new ZipInputStream(File.OpenRead(CurrZippedPath));

            DecompressArchive(newPath, zipIn);

            zipIn.Dispose();
            zipIn.Close();
        }

        private void DecompressArchive(StringBuilder newPath, ZipInputStream zipIn)
        {

            ZipEntry entry;


            while ((entry = zipIn.GetNextEntry()) != null)
            {

                if (entry.Name.EndsWith("/"))
                {

                    Directory.CreateDirectory(String.Format("{0}{1}", newPath, entry.Name.Replace(@"/", @"\")));

                }

                else
                {

                    FileStream streamWriter = File.Create(String.Format("{0}{1}", newPath, entry.Name.Replace(@"/", @"\")));

                    long size = entry.Size;

                    byte[] data = new byte[size];

                    while (true)
                    {

                        size = zipIn.Read(data, 0, data.Length);

                        if (size > 0) streamWriter.Write(data, 0, (int)size);

                        else break;

                    }

                    streamWriter.Close();

                }

            }

        }

        #endregion

         #region Get Files using FileStream(EduDocs)

        //private byte[] FileStreamGet(string trackingno)
        //{
        //    SqlConnection objSqlCon = null;
        //    SqlTransaction objSqlTran = null;
        //    SqlCommand objSqlCmd = null;
        //    SqlFileStream objSqlFileStream = null;
        //    SqlDataReader reader = null;
        //    string filelocation = string.Empty;
        //    string path = string.Empty;
        //    string fileType = string.Empty;
        //    int byteAmount;            

        //    try
        //    {
        //        objSqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["PGConnectionString1"].ConnectionString);
        //        objSqlCon.Open();
        //        objSqlTran = objSqlCon.BeginTransaction();

        //        objSqlCmd = new SqlCommand("FileGet", objSqlCon, objSqlTran);
        //        objSqlCmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter objSqlParam1 = new SqlParameter("@trackingno", SqlDbType.VarChar);
        //        objSqlParam1.Value = trackingno;

        //        objSqlCmd.Parameters.Add(objSqlParam1);
            

        //        using (reader = objSqlCmd.ExecuteReader())
        //        {
        //            while(reader.Read())
        //            {
        //                path = reader[0].ToString();
        //                fileType = reader[1].ToString();
        //            }
        //        }

                
        //        objSqlCmd = new SqlCommand("SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()", objSqlCon, objSqlTran);

        //        byte[] objContext = (byte[])objSqlCmd.ExecuteScalar();

        //        objSqlFileStream = new SqlFileStream(path, objContext, FileAccess.Read);

        //        buffer = new byte[(int)objSqlFileStream.Length];

        //        byteAmount = objSqlFileStream.Read(buffer, 0, buffer.Length);

        //        objSqlFileStream.Close();

        //        objSqlTran.Commit();

        //        reader.Close();

        //        System.Array.Resize<byte>(ref buffer, byteAmount);

            
        //    }
        //    catch (SqlException ex)
        //    {

        //    }
        //    finally
        //    {

        //        objSqlCon.Close();
        //    }

        //    return buffer;

          
        //}
        #endregion



        /// <summary>
        /// Exit the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


    }
}