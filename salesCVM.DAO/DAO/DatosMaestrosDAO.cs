using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salesCVM.Utilities;
using salesCVM.Models;
using System.Data;
using salesCVM.DAO.Util;
using salesCVM.SAP.Interface;
using salesCVM.SAP;

namespace salesCVM.DAO.DAO
{
    public class DatosMaestrosDAO: StoreProcedure
    {
        IDBAdapter DBAdapter;
        Log Lg;
        private Encrypt Encry;
        private ISAPMasterData iSapMD;
        public DatosMaestrosDAO() {
            DBAdapter   = DBFactory.GetDefaultAdapter();
            Lg          = Log.getIntance();
            Encry       = new Encrypt();
            iSapMD      = new SAPMasterData();
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
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
        public bool GetInformacionSocios<T>(ref List<T> ListaInformacion, ref string msj, int type, string cardcode) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                ListaInformacion = connection.Query<T>($"{SpGetSN} {type}, {cardcode}").ToList();
                
                return ListaInformacion.Count > 0 ? true : false;
            }
            catch (Exception ex) {
                Lg.Registrar(ex, this.GetType().FullName);
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
        public bool GetInformacionArticulo(ref List<ItemData> ListArticulos, ref string msj, int type) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                ListArticulos = connection.Query<ItemData>($"{SpGetItems}  {type}").ToList();
                return true;
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
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
        public bool TabsArticulos<T>(ref List<T> ListTabs, ref string msj, string itemcode,int type) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");
                
                ListTabs = connection.Query<T>($"{SpGetItems} {type}, '{itemcode}'").ToList();
                return true;
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
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
        public bool GetDestinos(ref List<Country> ListPaises, ref List<State> ListEstados, ref string msj) {
            IDbConnection connection = DBAdapter.GetConnection();
            SqlMapper.GridReader mult;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");
                
                mult = connection.QueryMultiple($"{SpDestinos}");

                if (mult != null)
                {
                    ListPaises = mult.Read<Country>().ToList();
                    ListEstados = mult.Read<State>().ToList();
                    return true;
                }
                else
                {
                    msj = $"No se recuperarón destinos";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
                msj = ex.Message;
                return false;
            }
            finally {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
        public bool CrearSocioSAP(ref MensajesObj msjCreate, BP document, string usuario) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = Encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSapMD.CreateBusnessPartner(ref msjCreate, modeloSap, document, usuario))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                msjCreate.Mensaje = ex.Message;
                Lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        public bool UpdateSocioSAP(ref MensajesObj msjUpdate, BP document, string usuario) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = Encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSapMD.UpdateBusnessPartner(ref msjUpdate, modeloSap, document, usuario))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                msjUpdate.Mensaje = ex.Message;
                Lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        public bool GetPrecios<T>(ref List<T> lista, ref string msj, int accion, string itemcode, int listnum) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                lista = connection.Query<T>($"{SpGetPrecios} {accion},'{itemcode}',{listnum}").ToList(); ;
                return true;
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
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
        public bool GetOpciones(ref List<UoM> UnidadMedida, ref List<GrupoArticulos> GrpArticulos, ref string msj, int accion)
        {
            IDbConnection connection = DBAdapter.GetConnection();
            SqlMapper.GridReader mult;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                mult = connection.QueryMultiple($"{SpGetOpciones} {accion}");
                
                if (mult != null)
                {
                    UnidadMedida = mult.Read<UoM>().ToList();
                    GrpArticulos = mult.Read<GrupoArticulos>().ToList();
                    return true;
                }
                else
                {
                    msj = $"No se recuperarón opciones";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
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
        public bool CrearArticuloSAP(ref MensajesObj msjCreate, ItemSAP document, string usuario) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = Encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSapMD.CreateItem(ref msjCreate, modeloSap, document, usuario))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                msjCreate.Mensaje = ex.Message;
                Lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        public bool UpdateArticuloSAP(ref MensajesObj msjCreate, ItemSAP document, string usuario) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = Encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSapMD.UpdateItem(ref msjCreate, modeloSap, document, usuario))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                msjCreate.Mensaje = ex.Message;
                Lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        public bool GetEmpleadosVts(ref List<Vendedor> ListVendedor, ref string msj, int type) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                ListVendedor = connection.Query<Vendedor>($"{SpGetEmpleados}  {type}").ToList();
                return true;
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
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
        public bool GetDocumentsNumbering(ref DocumentNumbering documentNum, ref string msj, string type, string subtype) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                documentNum = connection.Query<DocumentNumbering>($"{SpDocumentsNumbering}  {type}, {subtype}").SingleOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
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
        public bool Existe(ref bool resp, string value, string type) {
            IDbConnection connection = DBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                resp = connection.Query<bool>($"{SpExiste} {type}, {value}").Single();
                return true;
            }
            catch (Exception ex)
            {
                Lg.Registrar(ex, this.GetType().FullName);
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
