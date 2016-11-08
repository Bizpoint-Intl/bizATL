using System;
using System.Xml;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;



namespace ATL.ATLInterfaceUI.SqlHelper
{
    public class Sqlhelper1
    {
        private string ConnectionString;
        private SqlConnection SqlConnection;
        private SqlCommand SqlCommand;
        private int CommandTimeout = 30;

        public enum ExpectedType
        {
            StringType = 0,
            NumberType = 1,
            DateType = 2,
            BooleanType = 3,
            ImageType = 4
        }

        public Sqlhelper1(string Connection)
        {
            try
            {
                //ConnectionString = ConfigurationManager.ConnectionStrings["CHConnectionString"].ConnectionString;
                ConnectionString = Connection;
                SqlConnection = new SqlConnection(ConnectionString);
                SqlCommand = new SqlCommand();
                SqlCommand.CommandTimeout = CommandTimeout;
                SqlCommand.Connection = SqlConnection;
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
                if (SqlConnection != null)
                {
                    if (SqlConnection.State != ConnectionState.Closed)
                    {
                        SqlConnection.Close();
                    }
                    SqlConnection.Dispose();
                }

                //Clean Up Command Object
                if (SqlCommand != null)
                {
                    SqlCommand.Dispose();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error disposing data class." + Environment.NewLine + ex.Message);
            }

        }

        public void CloseConnection()
        {
            if (SqlConnection.State != ConnectionState.Closed) SqlConnection.Close();
        }

        public int GetExecuteScalarByCommandSp(string Command)
        {

            object identity = 0;
            try
            {
                SqlCommand.CommandText = Command;
                SqlCommand.CommandTimeout = CommandTimeout;
                SqlCommand.CommandType = CommandType.StoredProcedure;

                SqlConnection.Open();
                SqlCommand.Connection = SqlConnection;
                identity = SqlCommand.ExecuteScalar();
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
                SqlCommand.CommandText = Command;
                SqlCommand.CommandTimeout = CommandTimeout;
                SqlCommand.CommandType = CommandType.Text;

                SqlConnection.Open();

                SqlCommand.Connection = SqlConnection;
                identity = SqlCommand.ExecuteScalar();
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
                SqlCommand.CommandText = Command;
                SqlCommand.CommandTimeout = CommandTimeout;
                SqlCommand.CommandType = CommandType.Text;

                SqlConnection.Open();

                SqlCommand.Connection = SqlConnection;
                SqlCommand.ExecuteNonQuery();

                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                //throw ex;
            }
        }

        #region GetDatasetByCommand
        public DataSet GetDatasetByCommand(string Command)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand.CommandText = Command;
                SqlCommand.CommandTimeout = CommandTimeout;
                SqlCommand.CommandType = CommandType.Text;

                SqlConnection.Open();

                SqlDataAdapter adpt = new SqlDataAdapter(SqlCommand);

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

        //public void ReadSQL(Hashtable selectsCollection)
        //{
        //    int num = 0;
        //    string source = string.Empty;
        //    foreach (DictionaryEntry entry in selectsCollection)
        //    {
        //        source = source + entry.Value.ToString() + "|";
        //    }
        //    source = this.LeftOfLastIndexOf(source, '|').Trim();
        //    bool isError = false;
        //    string errorMessage = null;
        //    DataSet set = this.remoteDataAccess.ReadSQL(source, ref isError, ref errorMessage);
        //    if (isError)
        //    {
        //        if (errorMessage == null)
        //        {
        //            errorMessage = "ReadSQL Is Error !";
        //        }
        //        throw new Exception(errorMessage);
        //    }
        //    if (set != null)
        //    {
        //        foreach (DictionaryEntry entry2 in selectsCollection)
        //        {
        //            set.Tables[num].TableName = entry2.Key.ToString();
        //            num++;
        //        }
        //        for (int i = 0; i < num; i++)
        //        {
        //            DataTable table = set.Tables[0];
        //            set.Tables.RemoveAt(0);
        //            if (this.ds_DBAccess.Tables.Contains(table.TableName))
        //            {
        //                this.ds_DBAccess.Tables[table.TableName].Rows.Clear();
        //                foreach (DataRow row in table.Rows)
        //                {
        //                    this.ds_DBAccess.Tables[table.TableName].ImportRow(row);
        //                }
        //            }
        //            else
        //            {
        //                this.ds_DBAccess.Tables.Add(table);
        //            }
        //        }
        //    }
        //}

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
                SqlCommand.CommandText = txtCommand;
                SqlCommand.CommandTimeout = CommandTimeout;
                SqlCommand.CommandType = CommandType.Text;

                SqlConnection.Open();

                SqlDataAdapter adpt = new SqlDataAdapter(SqlCommand);
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
                SqlCommand.CommandText = spCommand;
                SqlCommand.CommandTimeout = CommandTimeout;
                SqlCommand.CommandType = CommandType.StoredProcedure;

                SqlConnection.Open();

                SqlDataAdapter adpt = new SqlDataAdapter(SqlCommand);
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
        #endregion

        public SqlDataReader GetReaderBySQL(string strSQL)
        {
            if (SqlConnection.State != ConnectionState.Open)
                SqlConnection.Open();
            try
            {
                SqlCommand myCommand = new SqlCommand(strSQL, SqlConnection);
                return myCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }


        public SqlConnection GetReaderBySQLConn()
        {
            if (SqlConnection.State != ConnectionState.Open)
                SqlConnection.Open();
            try
            {
                return SqlConnection;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public SqlDataReader GetReaderByCmd(string Command)
        {
            SqlDataReader objSqlDataReader = null;
            try
            {
                SqlCommand.CommandText = Command;
                SqlCommand.CommandType = CommandType.StoredProcedure;
                SqlCommand.CommandTimeout = CommandTimeout;

                SqlConnection.Open();
                SqlCommand.Connection = SqlConnection;

                objSqlDataReader = SqlCommand.ExecuteReader();
                return objSqlDataReader;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

        }

        public void AddParameterToSQLCommand(string ParameterName, SqlDbType ParameterType)
        {
            try
            {
                SqlCommand.Parameters.Add(new SqlParameter(ParameterName, ParameterType));
            }

            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public void AddParameterToSQLCommand(string ParameterName, SqlDbType ParameterType, int ParameterSize)
        {
            try
            {
                SqlCommand.Parameters.Add(new SqlParameter(ParameterName, ParameterType, ParameterSize));
            }

            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public void SetSQLCommandParameterValue(string ParameterName, object Value)
        {
            try
            {
                SqlCommand.Parameters[ParameterName].Value = Value;
            }

            catch (Exception ex)
            {
                //throw ex;
            }
        }




    }

}
