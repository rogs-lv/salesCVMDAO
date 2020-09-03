using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using salesCVM.Utilities;

namespace salesCVM.DAO.Implements
{
    public class SQLServerAdapter : IDBAdapter
    {
        Log lg;
        public SQLServerAdapter() {
            lg = Log.getIntance();
        }
        public IDbConnection GetConnection() {
            try
            {
                string connectionString = CreateConnectionString();
                DbConnection connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return null;
            }
        }

        private string CreateConnectionString() {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
            return connectionString;
        }
    }
}
