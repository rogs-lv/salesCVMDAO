using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using salesCVM.Models;
using salesCVM.Utilities;

namespace salesCVM.DAO.DAO
{
    public class ConfiguracionDAO : StoreProcedure
    {
        private IDBAdapter dBAdapter;
        Log lg;

        public ConfiguracionDAO() {
            dBAdapter = DBFactory.GetDefaultAdapter();
            lg = Log.getIntance();
        }

        public bool ConfiguracionOpciones(ref Configuracion.Menu menu, ref Configuracion.Adicional adicional, string usuario = "" ) {
            SqlMapper.GridReader mult;
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                mult = connection.QueryMultiple($"{spConfiguracion} '{usuario}'");
                if (mult != null)
                {
                    menu = mult.Read<Configuracion.Menu>().FirstOrDefault();
                    adicional = mult.Read<Configuracion.Adicional>().FirstOrDefault();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
            finally {
                if (connection != null) {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

    }
}
