using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salesCVM.Utilities;
using salesCVM.Models;
using System.Data;

namespace salesCVM.DAO.DAO
{
    public class DatosMaestrosDAO: StoreProcedure
    {
        IDBAdapter DBAdapter;
        Log Lg;
        public DatosMaestrosDAO() {
            DBAdapter = DBFactory.GetDefaultAdapter();
            Lg = Log.getIntance();
        }

        public bool GetDatosMaestros<T>(ref List<T> ListDatosMaestros, int type, string WhsCode, string PriceList)
        {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                if(type == 2)
                    ListDatosMaestros = connection.Query<T>($"{SpDatosMaestros} {type},'','',''").ToList();
                else
                    ListDatosMaestros = connection.Query<T>($"{SpDatosMaestros} {type},'{WhsCode}','{PriceList}',''").ToList();

                return true;
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }

    }
}
