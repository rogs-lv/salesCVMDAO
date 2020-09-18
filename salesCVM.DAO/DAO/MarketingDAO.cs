using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using salesCVM.Models;
using salesCVM.Utilities;
using salesCVM.DAO.Util;
using salesCVM.SAP.Interface;
using salesCVM.SAP;

namespace salesCVM.DAO.DAO
{
    public class MarketingDAO : StoreProcedure
    {
        private IDBAdapter dBAdapter;
        private ISAPMarketing iSap;
        private Encrypt encry;
        Log lg;
        public MarketingDAO() {
            lg = Log.getIntance();
            dBAdapter = DBFactory.GetDefaultAdapter();
            encry = new Encrypt();
            iSap = new SAPMarketing();
        }
        /// <summary>
        /// Save documents
        /// </summary>
        /// <param name="msjSQL"></param>
        /// <param name="document"></param>
        /// <param name="typeDocument"></param>
        /// <returns></returns>
        public bool SaveDocument(ref Mensajes msjSQL, DocSAP document, int typeDocument) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                IDbTransaction trans = connection.BeginTransaction();

                List<string> table = TableQuery(typeDocument);
                string qry = $"INSERT INTO \"{table[0]}\" (CardCode, CardName, DocDate, Reference, Comments, Status) VALUES (@CardCode, @CardName, @DocDate, @Reference, @Comments, @Status); SELECT CAST(SCOPE_IDENTITY() as int)";

                DynamicParameters parame = new DynamicParameters();
                parame.Add("@CardCode", document.Header.CardCode);
                parame.Add("@CardName", document.Header.CardName);
                parame.Add("@DocDate", document.Header.DocDate);
                parame.Add("@Reference", document.Header.Reference);
                parame.Add("@Comments", document.Header.Comments);
                parame.Add("@Status", document.Header.Status);

                int DocEntry = connection.QuerySingle<int>(qry, param: parame, transaction:trans, commandType: CommandType.Text);

                if (DocEntry != 0)
                {
                    for (int i = 0; i < document.Detail.Count; i++) {
                        document.Detail[i].DocEntry = DocEntry;
                    }

                    string qryrows = $"INSERT INTO \"{table[1]}\" (DocEntry, ItemCode, ItemName, Quantity, Price, UnitePrice, Discount, Currency, TaxCode) VALUES (@DocEntry, @ItemCode, @ItemName, @Quantity, @Price, @UnitePrice, @Discount, @Currency, @TaxCode)";
                    int rows = connection.Execute(qryrows, document.Detail, transaction: trans, commandType: CommandType.Text);

                    if (rows > 0)
                        trans.Commit();                    
                    else 
                        trans.Rollback();
                    
                    trans.Dispose();
                    msjSQL.DocNum = DocEntry;
                    msjSQL.DocEntry = DocEntry;
                    return true;
                }
                else {
                    msjSQL.Mensaje = "El documento no se pudo guardar";
                    msjSQL.DocEntry = -1;
                    msjSQL.DocNum = -1;
                    trans.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjSQL.Mensaje = ex.Message;
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
        /// <summary>
        /// Update documents
        /// </summary>
        /// <returns></returns>
        public bool UpdateDocument(ref Mensajes msjSQL, DocSAP document, int typeDocument) {
            return true;
        }
        /// <summary>
        /// Get Documents
        /// </summary>
        /// <param name="msjSQL"></param>
        /// <param name="document"></param>
        /// <param name="DocEntry"></param>
        /// <returns></returns>
        public bool GetDocument(ref string msjSQL, ref DocSAP document, int typeDocument, int DocEntry)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            SqlMapper.GridReader mult;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                List<string> tableQry = TableQuery(typeDocument);
                mult = connection.QueryMultiple($"{SpDocument} {DocEntry}, {tableQry[0]}, {tableQry[1]}");

                if (mult != null)
                {
                    document.Header = mult.Read<Document>().First();
                    document.Detail = mult.Read<DocumentLines>().ToList();
                    return true;
                }
                else {
                    msjSQL = $"No se recuperarón registros para el documento {DocEntry}";
                    return false;
                }

            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjSQL = ex.Message;
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
        /// Create document Quotation and Order
        /// </summary>
        /// <param name="msjCreate"></param>
        /// <param name="document"></param>
        /// <param name="modelo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CreateDocumentSAP(ref Mensajes msjCreate, DocSAP document, int type, string Usuario) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                Models.DatosConexion datosSAP = connection.Query<Models.DatosConexion>($"{spDatosConexion}").FirstOrDefault();
                Models.SAP modeloSap = encry.DescryConexionSAP(datosSAP.CadenaConexion);

                if (iSap.CreateDocument(ref msjCreate, document, modeloSap, type, Usuario))
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
        /// Get document creted sap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msjConsulta"></param>
        /// <param name="listDocuments"></param>
        /// <param name="docEntry"></param>
        /// <returns>return true or false to process completed</returns>
        public bool GetDocumentsSAP<T>(ref Mensajes msjConsulta , ref DocSAP document, ref List<T> listDocuments, int docEntry = 0) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    throw new Exception("Connection not available or closed");

                if (docEntry > 0)//documento de sap
                {
                    SqlMapper.GridReader mult;
                    mult = connection.QueryMultiple($"{SpDocumentSAP} {docEntry}");
                    document.Header = mult.Read<Document>().First();
                    document.Detail = mult.Read<DocumentLines>().ToList();
                    if (document.Header != null)
                        return true;
                    else {
                        msjConsulta.Mensaje = "No se recupero ningun documento";
                        return false;
                    }
                } else { //Lista de documentos abiertos
                    listDocuments = connection.Query<T>($"{ SpDocumentSAP}").ToList();
                    if (listDocuments.Count > 0)
                        return true;
                    else {
                        msjConsulta.Mensaje = $"No se recupero ningun documento";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                msjConsulta.Mensaje = ex.Message;
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
        }
        /// <summary>
        /// Create sentence sql to dynamic query
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private List<string> TableQuery(int typeDocument) {
            List<string> result = new List<string>(); ;
            try
            {
                switch (typeDocument)
                {
                    case 23://Cotización
                        result.Add("IOQUT");
                        result.Add("IQUT1");
                        return result;
                    case 17:
                        result.Add("IORDR");
                        result.Add("IRDR1");
                        return result;
                    default:
                        result.Add("IOQUT");
                        result.Add("IQUT1");
                        return result;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                result.Add("");
                result.Add("");
                return result;
            }
        }
    }
}
