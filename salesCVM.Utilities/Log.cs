using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Utilities
{
    public class Log
    {
        private static Log _intanceLog;
        private Log() {
        }
        /// <summary>
        /// Singleton
        /// </summary>
        /// <returns></returns>
        public static Log getIntance() {
            if (_intanceLog == null)
                CreateInstance();
            return _intanceLog;
        }

        private static void CreateInstance() {
            if (_intanceLog == null)
                _intanceLog = new Log();
        }

        #region Metodos
        public void Registrar(Exception ex, string nameSpace)
        {
            StreamWriter Wr = null;
            string pathLog = ConfigurationManager.AppSettings["RutaLog"];
            string pathFile = $"{pathLog}\\log.log";
            try
            {
                if (!Directory.Exists($"{pathLog}"))
                    Directory.CreateDirectory($"{pathLog}");
                if (!File.Exists($"{pathFile} "))
                    File.CreateText(pathFile).Dispose();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("{0}", ":::::::::::::::::::::::::::::::::::::::"));
                sb.AppendLine(string.Format("Fecha > {0}", DateTime.Now));
                sb.AppendLine(string.Format("Clase/Método > {0}", ex.StackTrace));
                sb.AppendLine(string.Format("Proyecto > {0}", nameSpace));
                sb.AppendLine(string.Format("Excepción > {0}", ex.InnerException == null ? "---" : ex.InnerException.ToString()));
                sb.AppendLine(string.Format("Mensaje > {0}", ex.Message));

                Wr = File.AppendText(pathFile);
                Wr.Write(sb.ToString());
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (Wr != null)
                {
                    Wr.Flush();
                    Wr.Close();
                }
            }
        }
        #endregion
    }
}
