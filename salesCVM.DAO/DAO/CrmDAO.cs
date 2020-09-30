using Dapper;
using salesCVM.Models;
using salesCVM.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace salesCVM.DAO.DAO
{
    public class CrmDAO : StoreProcedure
    {
        private IDBAdapter dBAdapter;
        private Log lg;
        public CrmDAO() {
            this.dBAdapter = DBFactory.GetDefaultAdapter();
            this.lg = Log.getIntance();
        }
        public bool GetOpportunity(ref List<Opportunity> ListOpp, ref string msj, int OppId) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                ListOpp = connection.Query<Opportunity>($"{SpGetDatosCRM} 1").ToList();
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
        public bool GetBPOpportunity(ref List<BussnessPartner> ListBP) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                ListBP = connection.Query<BussnessPartner>($"{SpGetDatosCRM} 100").ToList();
                return true;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
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
        public bool GetDataTabsOpportunity(ref Potencial potencial, ref General general, ref List<Etapas> etapas, ref List<Partner> partner, ref List<Competidores> competidor, ref Resumen resumen, ref string msj, int oppId ) {
            IDbConnection connection = dBAdapter.GetConnection();
            SqlMapper.GridReader mult;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                mult = connection.QueryMultiple($"{SpGetDatosCRM} 2, {oppId}");

                if (mult != null)
                {
                    potencial   = mult.Read<Potencial>().First();
                    general     = mult.Read<General>().First();
                    etapas      = mult.Read<Etapas>().ToList();
                    partner     = mult.Read<Partner>().ToList();
                    competidor  = mult.Read<Competidores>().ToList();
                    resumen     = mult.Read<Resumen>().First();

                    return true;
                }
                else
                {
                    msj = $"No se recuperarón registros para el documento {oppId}";
                    return false;
                }

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
        public bool GetOptionsOpp(ref List<PersonaContacto> persnCont, ref List<Territorio> territorio, ref List<Vendedor> vendedor) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                persnCont   = connection.Query<PersonaContacto>($"{SpGetDatosCRM} 3").ToList();
                territorio  = connection.Query<Territorio>($"{SpGetDatosCRM} 4").ToList();
                vendedor    = connection.Query<Vendedor>($"{SpGetDatosCRM} 5").ToList();

                return true;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
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
        public bool GetOptionsTabsGeneral() {
            return true;
        }
    }
}
