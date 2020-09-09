using System;
using salesCVM.Models;
using salesCVM.SAP.Interface;
using SAPbobsCOM;
using salesCVM.Utilities;
using System.Runtime.InteropServices;

namespace salesCVM.SAP
{
    public class SAPMarketing : SAPConnect, ISAPMarketing
    {
        SAPConnect isap;
        Log lg;
        public SAPMarketing() {
            isap = new SAPConnect();
            lg = Log.getIntance();
        }

        public bool CreateDocument(ref Mensajes msjCreate, DocSAP document, Models.SAP modelo, int type) {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo))
                {
                    _oCompany = isap.GetCompany();
                    Documents oDoc = null;
                    if (type == 23)
                        oDoc = (Documents)_oCompany.GetBusinessObject(BoObjectTypes.oQuotations);
                    else if (type == 17)
                        oDoc = (Documents)_oCompany.GetBusinessObject(BoObjectTypes.oOrders);

                    oDoc.CardCode       = document.Header.CardCode;
                    oDoc.DocDate        = document.Header.DocDate;
                    if (type == 17)
                        oDoc.DocDueDate = document.Header.DocDate;
                    oDoc.Reference1     = document.Header.Reference;
                    oDoc.Comments       = document.Header.Comments;

                    AddUserFieldHeader(oDoc);

                    for (int q = 0; q < document.Detail.Count; q++)
                    {
                        oDoc.Lines.ItemCode         = document.Detail[q].ItemCode;
                        oDoc.Lines.Quantity         = document.Detail[q].Quantity;
                        oDoc.Lines.UnitPrice        = (double)document.Detail[q].UnitePrice;
                        oDoc.Lines.DiscountPercent  = document.Detail[q].Discount;
                        oDoc.Lines.TaxCode          = document.Detail[q].TaxCode;
                        oDoc.Lines.Currency         = document.Detail[q].Currency;
                    }

                    if (oDoc.Add() != 0)
                    {
                        msjCreate.Mensaje = $"{_oCompany.GetLastErrorCode()} {_oCompany.GetLastErrorDescription()}";
                        return false;
                    }
                    else
                    {
                        oDoc.GetByKey(int.Parse(_oCompany.GetNewObjectKey()));
                        msjCreate.DocEntry = oDoc.DocEntry;
                        msjCreate.DocNum = oDoc.DocNum;
                        return true;
                    }
                }
                else
                {
                    msjCreate.Mensaje = msj;
                    return false;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return false;
            }
            finally {
                if (_oCompany != null)
                {
                    if (_oCompany.Connected)
                        _oCompany.Disconnect();
                    Marshal.ReleaseComObject(_oCompany);
                }
            }
        }

        private void AddUserFieldHeader(Documents doc) {
            doc.UserFields.Fields.Item("U_Origen").Value = "P";
            doc.UserFields.Fields.Item("U_cvmsSucursal").Value = "Matriz";
            doc.UserFields.Fields.Item("U_cvmsUser").Value = "MLF";
            doc.UserFields.Fields.Item("U_cvmsHora").Value = DateTime.Now.ToString("HH:mm:ss"); ;
        }
    }
}
