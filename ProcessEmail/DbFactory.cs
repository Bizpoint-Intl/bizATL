using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using BizRAD.Tools.LiteORM;
using BizRAD.Tools.LiteORM.SQLServer;


namespace ProcessEmail
{
	public class DbFactory
	{
		public static IDb DB;
		public static string connectString;
		public static readonly DbFactory Instance = new DbFactory();

		private SqlProvider provider;
		private SqlProvider provider2;	//for Background worker thread usage!!


		private DbFactory()
		{
			DbFactory.connectString = ConfigurationManager.ConnectionStrings["dbconn"].ConnectionString;
			provider = new SqlProvider(DbFactory.connectString);
			provider2 = new SqlProvider(DbFactory.connectString);
		}


		public void DBAlternateFactory(string connectString)
		{
			DbFactory.connectString = connectString;
			provider = new SqlProvider(DbFactory.connectString);
			provider2 = new SqlProvider(DbFactory.connectString);
		}


		public IDb GetDb()
		{
			return provider.OpenDb();
		}

		
		public IDb GetDb2()
		{
			return provider2.OpenDb();
		}


		public void KeepAliveDB(IDb DB)
		{
			if (DB.Connection.State == System.Data.ConnectionState.Closed)
			{
				DB.Connection.Open();
				BizRAD.Tools.BizLogger.WriteLog("DB re-opened!");
			}
		}


		public IList<T> GetList<T>(IQuery qry)
		{
			IList<T> list;

			try
			{
				list = DB.Select<T>(qry);

				if (list == null || list.Count == 0)
					return null;
			}
			catch 
			{
				return null;
			}


			return list;
		}

	}
}
