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
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.DAO.DAO
{
    public class ActivityDAO : StoreProcedure
    {
        private IDBAdapter dBAdapter;
        private ISAPCrm iSap;
        private Encrypt encry;
        private Log lg;
        public ActivityDAO()
        {
            this.dBAdapter = DBFactory.GetDefaultAdapter();
            this.iSap = new SAPCrm();
            this.encry = new Encrypt();
            this.lg = Log.getIntance();
        }
        /// <summary>
        /// Get objects with lists to drop downs
        /// </summary>
        /// <param name="Listas">returns lists of type T</param>
        /// <returns></returns>
        public bool GetOptionsActivity(ref object Listas) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                List<DropDownActivity> ListTipo         = connection.Query<DropDownActivity>($"{SpGetDatosActividad} 4").ToList();
                List<DropDownActivity> ListAsunto       = connection.Query<DropDownActivity>($"{SpGetDatosActividad} 5").ToList();
                List<DropDownActivity> ListLocalidad    = connection.Query<DropDownActivity>($"{SpGetDatosActividad} 6").ToList();

                Listas = new { ListTipo, ListAsunto, ListLocalidad };
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
        /// Get list of documents to interface activity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Lista">returns lists of type T</param>
        /// <param name="type"> action to do</param>
        /// <returns></returns>
        public bool GetListasActivity<T>(ref List<T> Lista, int type, int idOpp = 0) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");
                if (idOpp > 0)
                    Lista = connection.Query<T>($"{SpGetDatosActividad} @accion={type}, @IdOpp = {idOpp}").ToList();
                else
                    Lista = connection.Query<T>($"{SpGetDatosActividad} {type}").ToList();
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
        /// Get document activity by ID
        /// </summary>
        /// <param name="document">return document of type ActivitySAP</param>
        /// <param name="idAct">id document</param>
        /// <returns></returns>
        public bool GetActivityId(ref ActivitySAP document, int idAct) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");
                
                document = connection.Query<ActivitySAP>($"{SpGetDatosActividad} @accion=0, @IdAct = {idAct}").FirstOrDefault();
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
        /// Get list contacts of partner
        /// </summary>
        /// <param name="ListaContact">return list of contact person</param>
        /// <param name="cardCode">code of partner to filter list</param>
        /// <returns></returns>
        public bool GetContactPerson(ref List<Contactos> ListaContact, string cardCode) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                ListaContact = connection.Query<Contactos>($"{SpGetDatosActividad} @accion = 7, @cardCode = {cardCode}").ToList();
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
        public bool CreateActivity(ref MensajesObj msjCreate, ActivitySAP document, string Usuario)
        {
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
        public bool UpdateActivity(ref MensajesObj msjUpdate, ActivitySAP document, string Usuario)
        {
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
