using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class StoreProcedure
    {
        /// <summary>
        /// Tipo de comando a ejecutar para la BD
        /// </summary>
        string command = string.Empty;
        public StoreProcedure() {
            command = ConfigurationManager.AppSettings["cmd"];
        }

        #region Login
        /// <summary>
        /// Comando para ejecutar procedimiento de login
        /// </summary>
        public string spLogin {
            get {
                return $"{command} \"IDS_Login\" ";
            }
        }
        #endregion

        #region Configuracion
        public string spConfiguracion { 
            get {
                return $"{command} \"IDS_Configuracion\" ";
            } 
        }

        public string spGuardarConnexion {
            get {
                return $"{command} \"IDS_GuardarConexion\" ";
            }
        }
        #endregion

        #region SAP
        public string spDatosConexion {
            get {
                return $"{command} \"IDS_ConexionSAP\"";
            }
        }
        #endregion

        #region Marketing
        public string SpDocument {
            get {
                return $"{command} \"IDS_GetDocument\" ";
            }
        }
        public string SpDocumentSAP
        {
            get
            {
                return $"{command} \"IDS_GetDocumentSAP\" ";
            }
        }
        #endregion

        #region Datos Maestros
        public string SpDatosMaestros
        {
            get
            {
                return $"{command} \"IDS_GetDatosMaestros\" ";
            }
        }
        public string SpGetSN
        {
            get
            {
                return $"{command} \"IDS_GetSN\" ";
            }
        }
        public string SpGetItems
        {
            get
            {
                return $"{command} \"IDS_GetItems\" ";
            }
        }
        public string SpDestinos
        {
            get
            {
                return $"{command} \"IDS_Destinos\" ";
            }
        }
        public string SpGetPrecios
        {
            get
            {
                return $"{command} \"IDS_GetPrecios\" ";
            }
        }
        public string SpGetOpciones
        {
            get
            {
                return $"{command} \"IDS_GetOpciones\" ";
            }
        }
        public string SpGetEmpleados
        {
            get
            {
                return $"{command} \"IDS_GetEmpleados\" ";
            }
        }
        public string SpDocumentsNumbering { 
            get 
            {
                return $"{command} \"IDS_DocumentsNumbering\" ";
            } 
        }
        public string SpExiste
        {
            get
            {
                return $"{command} \"IDS_Existe\" ";
            }
        }
        public string QryPricePartner { 
            get {
                return $"SELECT \"ListNum\" FROM OCRD WHERE \"CardCode\" = ";
            } 
        }
        #endregion

        #region CRM
        public string SpGetDatosCRM
        {
            get
            {
                return $"{command} \"IDS_GetDatosCRM\" ";
            }
        }
        public string SpGetOpenDocs
        {
            get
            {
                return $"{command} \"GetOpenDocs\" ";
            }
        }
        #endregion

        #region Activity
        public string SpGetDatosActividad
        {
            get
            {
                return $"{command} \"IDS_GetDatosActividad\" ";
            }
        }
        #endregion

        #region Impresion
        public string SpGetReporte
        {
            get
            {
                return $"{command} \"IDS_GetReporte\" ";
            }
        }
        #endregion

        #region Dashboard
        public string SpGetPromNoticias
        {
            get
            {
                return $"{command} \"IDS_GetPromNoticias\" ";
            }
        }
        public string SpCuotasVentas
        {
            get
            {
                return $"{command} \"IDS_CuotasVentas\" ";
            }
        }
        public string SpGetCotDashboard
        {
            get
            {
                return $"{command} \"IDS_GetCotDashboard\" ";
            }
        }
        #endregion
    }
}
