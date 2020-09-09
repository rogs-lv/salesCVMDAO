using salesCVM.Models;
using salesCVM.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using salesCVM.SAP.Interface;
using salesCVM.DAO.Util;
using salesCVM.SAP;

namespace salesCVM.DAO.DAO
{
    public class SAPConnectDAO : StoreProcedure
    {
        private Encrypt encry;
        private ISAPConnect ISap;
        IDBAdapter dBAdapter;
        Log lg;

        public SAPConnectDAO() {
            dBAdapter = DBFactory.GetDefaultAdapter();
            encry = new Encrypt();
            lg = Log.getIntance();
            ISap = new SAPConnect();
        }

        public bool PingConexionSAP(ref string msjSap) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modelo = encry.DescryConexionSAP(datosSAP.CadenaConexion);
                if (ISap.Conectar(ref msjSap, modelo))
                {
                    msjSap = string.Empty;
                    return true;
                }
                else
                {
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjSap = ex.Message;
                return false;
            }
            finally {
                if (connection != null) {
                    connection.Dispose();
                    connection.Close();
                }
            }
        }
        public bool GuardarConexionSAP(ref string msjSave, Models.SAP dataCompany, int codeConnection) {
            IDbConnection connection = dBAdapter.GetConnection();
            int row = 0;
            try
            {
                if(connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");
                
                if (codeConnection > 0) {//La conexión existe. Aplicamos un update
                    string stringEncryp = encry.EncrytConexionSAP(dataCompany);
                    row = connection.Execute($"{spGuardarConnexion} {2},{codeConnection},'','{stringEncryp}','{dataCompany.Server}','{dataCompany.Server}','{dataCompany.CompanyDB}','{dataCompany.UserName}'");
                }
                else { //La conexión no existe. Aplicamos un insert
                    string stringEncryp = encry.EncrytConexionSAP(dataCompany);
                    row = connection.Execute($"{spGuardarConnexion} {1},{1},'','{stringEncryp}','{dataCompany.Server}','{dataCompany.Server}','{dataCompany.CompanyDB}','{dataCompany.UserName}'");
                }

                return row > 0 ? true : false;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjSave = ex.Message;
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
        }
        public bool RecuperarConexionSAP(ref Models.SAP modelConnection, int codeConnection) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if(connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                if (datosSAP != null)
                {
                    modelConnection = encry.DescryConexionSAP(datosSAP.CadenaConexion);
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
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
        }
        
    }
}
