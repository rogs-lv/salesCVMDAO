using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Models
{
    public class SAP
    {
        public string Server { get; set; }
        public string DbServerType { get; set; }
        public string SLDServer { get; set; }
        public string CompanyDB { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public string language { get; set; }
    }
    public class DatosConexion
    {
        public int CodigoConexion { get; set; }
        public string NombreConexion { get; set; }
        public string Servidor { get; set; }
        public string BaseDeDatos { get; set; }
        public string UsuarioSAP { get; set; }
        public string CadenaConexion { get; set; }
        public char VersionSAP { get; set; }
    }
}
