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
        string command;
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
    }
}
