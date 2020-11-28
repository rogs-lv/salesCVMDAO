using Dapper;
using salesCVM.DAO.Util;
using salesCVM.Models;
using salesCVM.SAP;
using salesCVM.SAP.Interface;
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
        private ISAPCrm iSap;
        private Encrypt encry;
        private Log lg;
        public CrmDAO() {
            this.dBAdapter = DBFactory.GetDefaultAdapter();
            this.iSap = new SAPCrm();
            this.encry = new Encrypt();
            this.lg = Log.getIntance();
        }
        /// <summary>
        /// Get list opportunity
        /// </summary>
        /// <param name="ListOpp">list of opportunity</param>
        /// <param name="msj">Return message error</param>
        /// <returns>true or false</returns>
        public bool GetOpportunities(ref List<Opportunity> ListOpp, ref string msj) {
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
        /// <summary>
        /// Get data opportunity specific and information from tabs opportunity
        /// </summary>
        /// <param name="tabsOpp">information tabs opportunity</param>
        /// <param name="msj">message error</param>
        /// <param name="oppId">specific opportunity</param>
        /// <returns>true or false</returns>
        public bool GetDataTabsOpportunity(ref TabsOpportunity tabsOpp, ref string msj, int oppId ) {
            IDbConnection connection = dBAdapter.GetConnection();
            SqlMapper.GridReader mult;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                mult = connection.QueryMultiple($"{SpGetDatosCRM} 2, {oppId}");

                if (mult != null)
                {
                    tabsOpp.TabPotencial    = mult.Read<Potencial>().FirstOrDefault();
                    tabsOpp.TabGeneral      = mult.Read<General>().FirstOrDefault();
                    tabsOpp.TableEtapas     = mult.Read<Etapas>().ToList();
                    tabsOpp.TablePartner    = mult.Read<Partner>().ToList();
                    tabsOpp.TableCompet     = mult.Read<Competidores>().ToList();
                    tabsOpp.TabResumen      = mult.Read<Resumen>().FirstOrDefault();

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
        /// <summary>
        /// Get List Bussnes Partner to CRM (Only two properties)
        /// </summary>
        /// <param name="ListBP">list bussnes partners</param>
        /// <returns>true or false</returns>
        public bool GetListBpToCRM(ref List<BusinessP> ListBP)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                ListBP = connection.Query<BusinessP>($"{SpGetDatosCRM} 100").ToList();
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
        /// <summary>
        /// Get list oportunities to CRM
        /// </summary>
        /// <param name="ListOpps">return list of opportunities</param>
        /// <returns></returns>
        public bool GetListOpportunity(ref List<Opportunity> ListOpps) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                ListOpps = connection.Query<Opportunity>($"{SpGetDatosCRM} 101").ToList();
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
        /// <summary>
        /// Get options opportunity
        /// </summary>
        /// <param name="optionsHeader">list of options to header document</param>
        /// <returns>true or false</returns>
        public bool GetOptionsOpportunity(ref OptionsHeaderOpp optionsHeader, string cardcode) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                optionsHeader.ListPrsContacto   = connection.Query<PersonaContacto>($"{SpGetDatosCRM} 3,0,{cardcode}").ToList();
                optionsHeader.ListTerritorio    = connection.Query<Territorio>($"{SpGetDatosCRM} 4").ToList();
                optionsHeader.ListVendedor      = connection.Query<Vendedor>($"{SpGetDatosCRM} 5").ToList();

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
        /// <summary>
        /// Get options tabs opportunity
        /// </summary>
        /// <param name="optionsTabDtl">list of options to detail document</param>
        /// <returns>true or false</returns>
        public bool GetOptionsTabsGeneral(ref OptionsTabsDetail optionsTabDtl, int idOpp) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                optionsTabDtl.ListProyectoSN    = connection.Query<ProyectoSN>($"{SpGetDatosCRM} 6").ToList();
                optionsTabDtl.ListaInformacion  = connection.Query<FuenteInformacion>($"{SpGetDatosCRM} 7").ToList();
                optionsTabDtl.ListIndustria     = connection.Query<Industria>($"{SpGetDatosCRM} 8").ToList();
                optionsTabDtl.ListEtapa         = connection.Query<OpcEtapas>($"{SpGetDatosCRM} 9").ToList();
                optionsTabDtl.ListCompetidor    = connection.Query<Competidores>($"{SpGetDatosCRM} 10").ToList();
                optionsTabDtl.ListRazones       = connection.Query<Razones>($"{SpGetDatosCRM} 11, {idOpp}").ToList();
                optionsTabDtl.ListPartner       = connection.Query<Partner>($"{SpGetDatosCRM} 12").ToList();
                optionsTabDtl.ListRelacion      = connection.Query<Relacion>($"{SpGetDatosCRM} 13").ToList();
                optionsTabDtl.ListVendedor      = connection.Query<Vendedor>($"{SpGetDatosCRM} 14").ToList();

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
        /// <summary>
        /// Get list data to NgSelect
        /// </summary>
        /// <param name="listOpDocs">list of data</param>
        /// <param name="type">type document to query</param>
        /// <returns>true or false</returns>
        public bool GetOpenDocuments(ref List<OpenDocs> listOpDocs, int type) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                listOpDocs = connection.Query<OpenDocs>($"{SpGetOpenDocs} {type}").ToList();
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
        /// <summary>
        /// Create document opportunity
        /// </summary>
        /// <param name="msjCreate">message error or success to create document</param>
        /// <param name="document">document with data to sap</param>
        /// <param name="Usuario">user that generated of ducument</param>
        /// <returns>true or false</returns>
        public bool CreateOpportunity(ref MensajesObj msjCreate, OpportunitySAP document, string Usuario) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSap.CreateOportunity(ref msjCreate, modeloSap, document, Usuario))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                msjCreate.Mensaje = ex.Message;
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        /// <summary>
        /// Update document opportunity
        /// </summary>
        /// <param name="msjUpdate">message error or success to update document</param>
        /// <param name="document">document with data to sap</param>
        /// <param name="Usuario">user that generated of ducument</param>
        /// <returns>true or false</returns>
        public bool UpdateOpportunity(ref MensajesObj msjUpdate, OpportunitySAP document, string Usuario) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSap.UpdateOportunity(ref msjUpdate, modeloSap, document, Usuario))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                msjUpdate.Mensaje = ex.Message;
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        /// <summary>
        /// Get data to tabs of opportunity specific
        /// </summary>
        /// <param name="document">return tabs with information</param>
        /// <param name="type">type of document</param>
        /// <param name="idDoc">id document</param>
        /// <returns></returns>
        public bool GetTabsDocumentOpp(ref object document, int type, int idDoc) {
            IDbConnection connection = dBAdapter.GetConnection();
            SqlMapper.GridReader mult;
            try
            {
                switch (type)
                {
                    case 97: // tabs Sales Opportunity
                        mult = connection.QueryMultiple($"{SpGetDatosCRM} '2',{idDoc}");
                        // Potencial
                        Potencial           TabPotencial = mult.Read<Potencial>().FirstOrDefault();
                        General             TabGeneral   = mult.Read<General>().FirstOrDefault();
                        List<Etapas>        TableEtapas  = mult.Read<Etapas>().ToList();
                        List<Partner>       TablePartner = mult.Read<Partner>().ToList();
                        List<Competidores>  TableCompet  = mult.Read<Competidores>().ToList();
                        Resumen             TabResumen   = mult.Read<Resumen>().FirstOrDefault();
                        document = new { TabPotencial, TabGeneral, TableEtapas, TablePartner, TableCompet, TabResumen };
                    return true;
                    default:
                        return false;
                }
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
    }
}
