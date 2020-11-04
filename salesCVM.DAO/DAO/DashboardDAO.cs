using Dapper;
using salesCVM.Models;
using salesCVM.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.DAO.DAO
{
    public class DashboardDAO : StoreProcedure
    {
        private IDBAdapter dBAdapter;
        private Log lg;
        public DashboardDAO() {
            dBAdapter = DBFactory.GetDefaultAdapter();
            lg = Log.getIntance();
        }
        public bool GetPromocionesNoticias<T>(ref List<T> PromocionesNoticas, ref string msj, string type, string user) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                PromocionesNoticas = connection.Query<T>($"{SpGetPromNoticias} '{type}','{user}'").ToList();
                return true;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msj = ex.Message;
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
        public bool GetDatosGrafica<T>(ref List<T> Grafica, ref string msj, string usuario) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Grafica = connection.Query<T>($"{SpCuotasVentas} '{usuario}'").ToList();
                if (Grafica.Count == 0)
                {
                    msj = $"No se encontraron registros para el usuario {usuario}";
                    return false;
                }    
                return true;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msj = ex.Message;
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
        public bool GetCotizaciones(ref List<Cotizaciones> ListCot, ref string msj, string usuario)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                ListCot = connection.Query<Cotizaciones>($"{SpGetCotDashboard} '{usuario}'").ToList();
                return true;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msj = ex.Message;
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
    }
}
