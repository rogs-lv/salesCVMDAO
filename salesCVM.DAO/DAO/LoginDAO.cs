using System;
using System.Linq;
using System.Data;
using Dapper;
using salesCVM.Utilities;
using salesCVM.Models;

namespace salesCVM.DAO.DAO
{ 
    public class LoginDAO : StoreProcedure
    {
        IDBAdapter dBAdapter;
        Log lg;

        public LoginDAO() {
            dBAdapter = DBFactory.GetDefaultAdapter();
            lg = Log.getIntance();
        }

        public bool Login(UserLogin userLogin, ref User userData) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    throw new Exception("Connection not available or closed");
                }

                userData = connection.Query<User>($"{spLogin} '{userLogin.IdUser}','{userLogin.Password}'").FirstOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
            finally
            {
                if (connection != null) {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
    }
}
