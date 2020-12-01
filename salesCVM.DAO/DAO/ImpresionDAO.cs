using CrystalDecisions.CrystalReports.Engine;
using salesCVM.DAO.Util;
using salesCVM.Models;
using salesCVM.Utilities;
using System;
using System.Data;
using System.IO;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using CrystalDecisions.Shared;

namespace salesCVM.DAO.DAO
{
    public class ImpresionDAO : StoreProcedure
    {
        private Encrypt encry;
        private Log lg;
        private IDBAdapter dBAdapter;
        public ImpresionDAO() {
            this.dBAdapter = DBFactory.GetDefaultAdapter();
            this.encry = new Encrypt();
            this.lg = Log.getIntance();
        }
        public Stream GetFormatPrint(int idDoc, string _nameRep, int _typeRep) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Reporte _rep = connection.Query<Reporte>($"{SpGetReporte} '{_nameRep}', {_typeRep}").FirstOrDefault();
                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = encry.DescryConexionSAP(datosSAP.CadenaConexion);
                return Print(idDoc, _rep.TypeRep, "", @"" + _rep.PathRep + _rep.NameRep + "", modeloSap);
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return null;
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
        private Stream Print(int idDoc, int typeDoc, string usuario, string fullPath, Models.SAP connSAP) {
            ReportDocument _report = new ReportDocument();

            try
            {
                _report.Load(fullPath);
                _report.Refresh();
                _report.SetParameterValue("DocKey@", idDoc);
                _report.SetParameterValue("ObjectId@", typeDoc);

                OdbcConnectionStringBuilder odbc = new OdbcConnectionStringBuilder();

                odbc.ConnectionString = $"Data Source={connSAP.Server};Initial Catalog={connSAP.CompanyDB};User Id={connSAP.DbUserName};Password={connSAP.DbPassword}";
                CrystalReportsUtilerias.RecurseAndRemap(_report, (string)odbc["Data Source"], (string)odbc["Initial Catalog"], (string)odbc["User Id"], (string)odbc["Password"]);
                return _report.ExportToStream(ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return null;
            }
            finally
            {
                _report.Close();
                _report.Dispose();
            }
        }
    }
}
