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

namespace salesCVM.DAO.DAO
{
    public class MarketingDAO : StoreProcedure
    {
        private IDBAdapter dBAdapter;
        Log lg;
        public MarketingDAO() {
            lg = Log.getIntance();
            dBAdapter = DBFactory.GetDefaultAdapter();
        }
        /// <summary>
        /// Save documents
        /// </summary>
        /// <param name="msjSQL"></param>
        /// <param name="document"></param>
        /// <param name="typeDocument"></param>
        /// <returns></returns>
        public bool SaveDocument(ref string msjSQL, DocSAP document, int typeDocument) {
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
                    return true;
                }
                else {
                    msjSQL = "El documento no se pudo guardar";
                    trans.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjSQL = ex.Message;
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
        public bool UpdateDocument(ref string msjSQL, DocSAP document, int typeDocument) {
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
