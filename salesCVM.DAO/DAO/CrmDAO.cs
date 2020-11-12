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
        public bool GetOptionsTabsGeneral(ref OptionsTabsDetail optionsTabDtl) {
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
        /// Get options to header activity
        /// </summary>
        /// <param name="optionsHdActivity">return list of options (Tipo, Asunto, Contacto, Etapa)</param>
        /// <param name="cardcode">code to filter list person contact</param>
        /// <returns>true or false</returns>
        public bool GetOptionsActivity(ref OptionsHeadActivity optionsHdActivity, string cardcode) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                optionsHdActivity.ListTipo      = connection.Query<Tipo>($"{SpGetDatosCRM} 15").ToList();
                optionsHdActivity.ListAsunto    = connection.Query<Asunto>($"{SpGetDatosCRM} 16").ToList();
                optionsHdActivity.ListContacto  = connection.Query<PersonaContacto>($"{SpGetDatosCRM} 3,0,'{cardcode}'").ToList();
                optionsHdActivity.ListEtapa     = connection.Query<OpcEtapas>($"{SpGetDatosCRM} 9").ToList();

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
        /// Get options to tab general
        /// </summary>
        /// <param name="optionsTabsActivity"></param>
        /// <returns>true or false</returns>
        public bool GetOptionsTabsActivity(ref OptionsTabsActivity optionsTabsActivity) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                optionsTabsActivity.ListLocalidad   = connection.Query<Localidad>($"{SpGetDatosCRM} 16").ToList();

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
        /// Create document type activity in SAP
        /// </summary>
        /// <param name="msjCreate">return message of conflict or success</param>
        /// <param name="document">data of document to create in SAP</param>
        /// <param name="Usuario">user that generated of ducument</param>
        /// <returns>true or false</returns>
        public bool CreateActivity(ref MensajesObj msjCreate, ActivitySap document, string Usuario) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSap.CreateActivity(ref msjCreate, modeloSap, document, Usuario))
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
        /// Update document activity in SAP
        /// </summary>
        /// <param name="msjUpdate">return message of conflict or success</param>
        /// <param name="document">data of document to update in SAP</param>
        /// <param name="Usuario">user that generated of ducument</param>
        /// <returns>true or false</returns>
        public bool UpdateActivity(ref MensajesObj msjUpdate, ActivitySap document, string Usuario) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSap.UpdateActivity(ref msjUpdate, modeloSap, document, Usuario))
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
    }
}
