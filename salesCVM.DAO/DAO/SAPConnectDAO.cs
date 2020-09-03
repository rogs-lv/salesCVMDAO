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

namespace salesCVM.DAO.DAO
{
    public class SAPConnectDAO : StoreProcedure
    {
        IDBAdapter dBAdapter;
        Log lg;
        private ISAPConnect isap;

        public SAPConnectDAO() {
            dBAdapter = DBFactory.GetDefaultAdapter();
            lg = Log.getIntance();
        }

        public bool PingConexionSAP(string msjSap) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modelo = DescryConexionSAP(datosSAP.CadenaConexion);
                if (isap.Conectar(ref msjSap, modelo))
                {
                    msjSap = string.Empty;
                    return true;
                }
                else
                    return false;
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
                    string stringEncryp = EncrytConexionSAP(dataCompany);
                    row = connection.Execute($"{spGuardarConnexion} {2},{codeConnection},'','{stringEncryp}','{dataCompany.Server}','{dataCompany.Server}','{dataCompany.CompanyDB}','{dataCompany.UserName}'");
                }
                else { //La conexión no existe. Aplicamos un insert
                    string stringEncryp = EncrytConexionSAP(dataCompany);
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
                    modelConnection = DescryConexionSAP(datosSAP.CadenaConexion);
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
        private string EncrytConexionSAP(Models.SAP modelo) {
            string stringConexion = ObtenerCadenaConexion(modelo);
            if (stringConexion == null) 
                return null;
            return StringCipher.Encrypt(stringConexion, "#Ida5of$");
        }
        private Models.SAP DescryConexionSAP(string stringConexion) {
            string cadena = StringCipher.Decrypt(stringConexion, "#Ida5of$");
            return DesdeCadenaConexion(cadena);
        }
        private Models.SAP DesdeCadenaConexion(string stringConexion) {
            Models.SAP modelo = new Models.SAP();
            string[] elementosSringConexion = stringConexion.Split(';');
            foreach (string elemento in elementosSringConexion) {
                string[] claveValor = elemento.Split('=');
                if (claveValor[0] == "Server")
                    modelo.Server = claveValor[1];
                else if (claveValor[0] == "Database")
                    modelo.CompanyDB = claveValor[1];
                else if (claveValor[0] == "UserId")
                    modelo.DbUserName = claveValor[1];
                else if (claveValor[0] == "Password")
                    modelo.DbPassword = claveValor[1];
                else if (claveValor[0] == "UserIdSAP")
                    modelo.UserName = claveValor[1];
                else if (claveValor[0] == "PasswordSAP")
                    modelo.Password = claveValor[1];
                else if (claveValor[0] == "ServerType")
                    modelo.DbServerType = claveValor[1];
                else if (claveValor[0] == "LicenseServer")
                    modelo.SLDServer = claveValor[1];
                //else if (claveValor[0] == "PortLicenseServer")
                //    modelo.PortLicenseServer = claveValor[1];
                else
                    return null;
            }
            return modelo;
        }
        private string ObtenerCadenaConexion(Models.SAP model)
        {
            return
                string.Format(
                "Server={0};Database={1};UserId={2};Password={3};UserIdSAP={4};PasswordSAP={5};ServerType={6};LicenseServer={7}",
                model.Server, model.CompanyDB, model.DbUserName, model.DbPassword, model.UserName, model.Password, model.DbServerType, model.SLDServer);
        }
    }
}
