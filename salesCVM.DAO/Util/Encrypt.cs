using salesCVM.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.DAO.Util
{
    public class Encrypt
    {
        public string EncrytConexionSAP(Models.SAP modelo)
        {
            string stringConexion = ObtenerCadenaConexion(modelo);
            if (stringConexion == null)
                return null;
            return StringCipher.Encrypt(stringConexion, "#Ida5of$");
        }
        public Models.SAP DescryConexionSAP(string stringConexion)
        {
            string cadena = StringCipher.Decrypt(stringConexion, "#Ida5of$");
            return DesdeCadenaConexion(cadena);
        }
        private Models.SAP DesdeCadenaConexion(string stringConexion)
        {
            Models.SAP modelo = new Models.SAP();
            string[] elementosSringConexion = stringConexion.Split(';');
            foreach (string elemento in elementosSringConexion)
            {
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
