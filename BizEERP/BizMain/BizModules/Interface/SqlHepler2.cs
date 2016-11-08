using System;
using System.Xml;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Odbc;
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
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizAccounts;
using BizRAD.BizReport;



namespace ATL.ATLInterfaceUI.SqlHelper2
{
    public class Sqlhelper2
    {
        private string ConnectionString;
        private OdbcConnection OdbcConnection;
        private OdbcCommand OdbcCommand;
        private int CommandTimeout = 3000;

        public enum ExpectedType
        {
            StringType = 0,
            NumberType = 1,
            DateType = 2,
            BooleanType = 3,
            ImageType = 4
        }

        public Sqlhelper2(string Connection)
        {
            try
            {
                //ConnectionString = ConfigurationManager.ConnectionStrings["CHConnectionString"].ConnectionString;
                ConnectionString = Connection;
                OdbcConnection = new OdbcConnection(ConnectionString);
                OdbcCommand = new OdbcCommand();
                OdbcCommand.CommandTimeout = CommandTimeout;
                OdbcCommand.Connection = OdbcConnection;
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing data class." + Environment.NewLine + ex.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                //Clean Up Connection Object
                if (OdbcConnection != null)
                {
                    if (OdbcConnection.State != ConnectionState.Closed)
                    {
                        OdbcConnection.Close();
                    }
                    OdbcConnection.Dispose();
                }

                //Clean Up Command Object
                if (OdbcCommand != null)
                {
                    OdbcCommand.Dispose();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error disposing data class." + Environment.NewLine + ex.Message);
            }

        }

        public void CloseConnection()
        {
            if (OdbcConnection.State != ConnectionState.Closed) OdbcConnection.Close();
        }

        public int GetExecuteScalarByCommandSp(string Command)
        {

            object identity = 0;
            try
            {
                OdbcCommand.CommandText = Command;
                OdbcCommand.CommandTimeout = CommandTimeout;
                OdbcCommand.CommandType = CommandType.StoredProcedure;

                OdbcConnection.Open();
                OdbcCommand.Connection = OdbcConnection;
                identity = OdbcCommand.ExecuteScalar();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                //throw ex;
            }
            return Convert.ToInt32(identity);
        }

        public int GetExecuteScalarByCommandTxt(string Command)
        {

            object identity = 0;
            try
            {
                OdbcCommand.CommandText = Command;
                OdbcCommand.CommandTimeout = CommandTimeout;
                OdbcCommand.CommandType = CommandType.Text;

                OdbcConnection.Open();

                OdbcCommand.Connection = OdbcConnection;
                identity = OdbcCommand.ExecuteScalar();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                ////throw ex;
            }
            return Convert.ToInt32(identity);
        }


        public void GetExecuteNonQueryByCommand(string Command)
        {
            try
            {
                OdbcCommand.CommandText = Command;
                OdbcCommand.CommandTimeout = CommandTimeout;
                OdbcCommand.CommandType = CommandType.Text;

                OdbcConnection.Open();

                OdbcCommand.Connection = OdbcConnection;
                OdbcCommand.ExecuteNonQuery();

                CloseConnection();
            }
            //catch (Exception ex)
            //{
            //    CloseConnection();
            //    //throw ex;
            //}

            catch (OdbcException ex)
            {
                MessageBox.Show(ex.Message);
                CloseConnection();
                LogError(Command, ex.Message);
            }

        }

        #region GetDatasetByCommand
        public DataSet GetDatasetByCommand(string Command)
        {
            DataSet ds = new DataSet();
            try
            {
                OdbcCommand.CommandText = Command;
                OdbcCommand.CommandTimeout = CommandTimeout;
                OdbcCommand.CommandType = CommandType.Text;

                OdbcConnection.Open();

                OdbcDataAdapter adpt = new OdbcDataAdapter(OdbcCommand);

                

                adpt.Fill(ds);

            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }
        public DataSet GetDatasetByCommand(Hashtable HashTable)
        {
            DataSet ds = new DataSet();
            try
            {




            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                CloseConnection();
            }

            return ds;
        }
        #endregion

     

        protected string LeftOfLastIndexOf(string source, char c)
        {
            int length = source.LastIndexOf(c);
            if (length != 0)
            {
                return source.Substring(0, length);
            }
            return source;
        }



        #region GetDataTableByCommand
        public DataTable GetDataTableByCommandTxt(string TableName, string txtCommand)
        {
            DataTable dt = new DataTable(TableName);
            try
            {
                OdbcCommand.CommandText = txtCommand;
                OdbcCommand.CommandTimeout = CommandTimeout;
                OdbcCommand.CommandType = CommandType.Text;

                OdbcConnection.Open();

                OdbcDataAdapter adpt = new OdbcDataAdapter(OdbcCommand);
                //DataSet ds = new DataSet();

                adpt.Fill(dt);

            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        public DataTable GetDataTableByCommandSp(string TableName, string spCommand)
        {
            DataTable dt = new DataTable(TableName);
            try
            {
                OdbcCommand.CommandText = spCommand;
                OdbcCommand.CommandTimeout = CommandTimeout;
                OdbcCommand.CommandType = CommandType.StoredProcedure;

                OdbcConnection.Open();

                OdbcDataAdapter adpt = new OdbcDataAdapter(OdbcCommand);
                //DataSet ds = new DataSet();

                adpt.Fill(dt);

            }
            catch (OdbcException ex)
            {
                MessageBox.Show(ex.Message);
                CloseConnection();
                LogError(spCommand, ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }
        #endregion

        public OdbcDataReader GetReaderBySQL(string strSQL)
        {
            if (OdbcConnection.State != ConnectionState.Open)
                OdbcConnection.Open();
            try
            {
                OdbcCommand myCommand = new OdbcCommand(strSQL, OdbcConnection);
                return myCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }


        public OdbcConnection GetReaderBySQLConn()
        {
            if (OdbcConnection.State != ConnectionState.Open)
                OdbcConnection.Open();
            try
            {
                return OdbcConnection;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public OdbcDataReader GetReaderByCmd(string Command)
        {
            OdbcDataReader objOdbcDataReader = null;
            try
            {
                OdbcCommand.CommandText = Command;
                OdbcCommand.CommandType = CommandType.StoredProcedure;
                OdbcCommand.CommandTimeout = CommandTimeout;

                OdbcConnection.Open();
                OdbcCommand.Connection = OdbcConnection;

                objOdbcDataReader = OdbcCommand.ExecuteReader();
                return objOdbcDataReader;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

        }

        public void AddParameterToOdbcCommand(string ParameterName, SqlDbType ParameterType)
        {
            try
            {
                OdbcCommand.Parameters.Add(new SqlParameter(ParameterName, ParameterType));
            }

            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public void AddParameterToOdbcCommand(string ParameterName, SqlDbType ParameterType, int ParameterSize)
        {
            try
            {
                OdbcCommand.Parameters.Add(new SqlParameter(ParameterName, ParameterType, ParameterSize));
            }

            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public void SetOdbcCommandParameterValue(string ParameterName, object Value)
        {
            try
            {
                OdbcCommand.Parameters[ParameterName].Value = Value;
            }

            catch (Exception ex)
            {
                //throw ex;
            }
        }


        private void LogError(string docinfo, string logmessage)
        {
            DBAccess dbaccess = new DBAccess();
            Parameter[] parameters = new Parameter[3];
            parameters[0] = new Parameter("@docinfo", @docinfo);
            parameters[1] = new Parameter("@logmessage", @logmessage);
            parameters[2] = new Parameter("@user", Common.DEFAULT_SYSTEM_USERNAME);


            try
            {
                dbaccess.RemoteStandardSQL.ExecuteNonQuery("sp_Insert_SageMigrateErrorLog", ref parameters);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




    }

}
