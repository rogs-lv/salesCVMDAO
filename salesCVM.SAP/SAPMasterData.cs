using salesCVM.Models;
using salesCVM.SAP.Interface;
using salesCVM.Utilities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace salesCVM.SAP
{
    public class SAPMasterData : ISAPMasterData
    {
        SAPConnect isap;
        Log lg;
        public SAPMasterData()
        {
            isap = new SAPConnect();
            lg = Log.getIntance();
        }
        public bool CreateBusnessPartner(ref MensajesObj msjCreate, Models.SAP modelo, BP socio, string Usuario) {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo))
                {
                    _oCompany           = isap.GetCompany();
                    BusinessPartners bp = (BusinessPartners)_oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    if (int.Parse(socio.Header.Serie) > 0)
                        bp.Series           = int.Parse(socio.Header.Serie);
                    // bp.CardCode         = socio.Header.CardCode;
                    bp.CardName         = socio.Header.CardName;
                    bp.FederalTaxID     = socio.Header.LicTradNum;
                    bp.CardType         = CardTypes(socio.Header.CardType);
                    bp.EmailAddress     = socio.Header.E_Mail;
                    bp.Website          = socio.Header.IntrntSite;

                    AddUserFieldHeader(bp, socio, "");

                    foreach (Direcciones dir in socio.TabDireccion)
                    {
                        //if (dir.AdresType == 'B') { // Entrega
                        bp.Addresses.AddressName        = dir.Address;
                        bp.Addresses.AddressType        = AdressTypes(dir.AdresType);
                        bp.Addresses.Country            = dir.Country;
                        bp.Addresses.State              = dir.State;
                        bp.Addresses.County             = dir.County;//Delegación/municipio
                        bp.Addresses.City               = dir.City;
                        bp.Addresses.Street             = dir.Street;
                        bp.Addresses.StreetNo           = dir.StreetNo;
                        bp.Addresses.Block              = dir.Block;
                        bp.Addresses.ZipCode            = dir.ZipCode.ToString();
                        bp.Addresses.Add();
                        //} else { // Embarque
                        //}
                    }

                    foreach (Contacto cnt in socio.TabContacto) 
                    {
                        bp.ContactEmployees.Name            = cnt.Name;
                        bp.ContactEmployees.FirstName       = cnt.FirstName;
                        bp.ContactEmployees.Title           = cnt.Title;
                        bp.ContactEmployees.MiddleName      = cnt.MiddleName;
                        bp.ContactEmployees.Position        = cnt.Position;
                        bp.ContactEmployees.LastName        = cnt.LastName;
                        bp.ContactEmployees.Address         = cnt.Address;
                        bp.ContactEmployees.Phone1          = cnt.Tel1;
                        bp.ContactEmployees.MobilePhone     = cnt.Cellolar;
                        bp.ContactEmployees.E_Mail          = cnt.E_MailL;
                        bp.ContactEmployees.Add();
                    }

                    if (bp.Add() != 0)
                    {
                        msjCreate.Mensaje = $"{_oCompany.GetLastErrorCode()} - {_oCompany.GetLastErrorDescription()}";
                        return false;
                    }
                    else
                    {
                        string code = string.Empty;
                        _oCompany.GetNewObjectCode(out code);
                        msjCreate.Code = code;
                        return true;
                    }
                }
                else {
                    msjCreate.Mensaje = msj;
                    return false;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjCreate.Mensaje = ex.Message;
                return false;
            }
            finally
            {
                if (_oCompany != null)
                {
                    if (_oCompany.Connected)
                        _oCompany.Disconnect();
                    Marshal.ReleaseComObject(_oCompany);
                }
            }
        }
        public bool UpdateBusnessPartner(ref MensajesObj msjUpd, Models.SAP modelo, BP socio, string Usuario) {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo)) {
                    _oCompany           = isap.GetCompany();
                    BusinessPartners bp = (BusinessPartners)_oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
                    bp.GetByKey(socio.Header.CardCode);

                    bp.CardName     = socio.Header.CardName;
                    bp.FederalTaxID = socio.Header.LicTradNum;
                    bp.CardType     = CardTypes(socio.Header.CardType);
                    bp.EmailAddress = socio.Header.E_Mail;
                    bp.Website      = socio.Header.IntrntSite;

                    AddUserFieldHeader(bp, socio, "");

                    //Si existen direcciones las actualizamos
                    int countAdress         = bp.Addresses.Count;
                    bool seActRegistros     = false;
                    for (int i = 0; i< countAdress; i++) {
                        bp.Addresses.SetCurrentLine(i);
                        if (bp.Addresses.AddressName != "") { 
                            int indice = IndiceDireccion(socio.TabDireccion, bp.Addresses.AddressName, i);
                            if (indice > -1) {
                                bp.Addresses.AddressName    = socio.TabDireccion[indice].Address;
                                bp.Addresses.AddressType    = AdressTypes(socio.TabDireccion[indice].AdresType);
                                bp.Addresses.Country        = socio.TabDireccion[indice].Country;
                                bp.Addresses.State          = socio.TabDireccion[indice].State;
                                bp.Addresses.County         = socio.TabDireccion[indice].County;//Delegación/municipio
                                bp.Addresses.City           = socio.TabDireccion[indice].City;
                                bp.Addresses.Street         = socio.TabDireccion[indice].Street;
                                bp.Addresses.StreetNo       = socio.TabDireccion[indice].StreetNo;
                                bp.Addresses.Block          = socio.TabDireccion[indice].Block;
                                bp.Addresses.ZipCode        = socio.TabDireccion[indice].ZipCode.ToString();
                            }
                            seActRegistros = true;
                        }
                    }

                    int countCntPerson  = bp.ContactEmployees.Count;
                    bool seActPrsCnt    = false;
                    for (int j = 0; j< countCntPerson; j++) {
                        bp.ContactEmployees.SetCurrentLine(j);
                        if (bp.ContactEmployees.Name != "") {
                            int indiceCnt = IndiceContacto(socio.TabContacto, bp.ContactEmployees.Name, bp.ContactEmployees.InternalCode);
                            if (indiceCnt > -1) {
                                bp.ContactEmployees.Name            = socio.TabContacto[indiceCnt].Name;
                                bp.ContactEmployees.FirstName       = socio.TabContacto[indiceCnt].FirstName;
                                bp.ContactEmployees.Title           = socio.TabContacto[indiceCnt].Title;
                                bp.ContactEmployees.MiddleName      = socio.TabContacto[indiceCnt].MiddleName;
                                bp.ContactEmployees.Position        = socio.TabContacto[indiceCnt].Position;
                                bp.ContactEmployees.LastName        = socio.TabContacto[indiceCnt].LastName;
                                bp.ContactEmployees.Address         = socio.TabContacto[indiceCnt].Address;
                                bp.ContactEmployees.Phone1          = socio.TabContacto[indiceCnt].Tel1;
                                bp.ContactEmployees.MobilePhone     = socio.TabContacto[indiceCnt].Cellolar;
                                bp.ContactEmployees.E_Mail          = socio.TabContacto[indiceCnt].E_MailL;
                            }
                            seActPrsCnt = true;
                        }
                    }

                    //Si no existen direcciones en SAP y el arreglo de direcciones tiene alguna nueva direccion
                    AddDirections(socio.TabDireccion, bp, seActRegistros);

                    //Si no existen contactos en SAP y el arreglo de contactos tiene algun nuevo contacto
                    AddContactos(socio.TabContacto, bp, seActPrsCnt);

                    if (bp.Update() != 0)
                    {
                        msjUpd.Mensaje = $"{_oCompany.GetLastErrorCode()} - {_oCompany.GetLastErrorDescription()}";
                        return false;
                    } else {
                        msjUpd.Code = bp.CardCode;
                        return true;
                    }
                }
                else {
                    msjUpd.Mensaje = msj;
                    return false;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjUpd.Mensaje = ex.Message;
                return false;
            }
            finally
            {
                if (_oCompany != null)
                {
                    if (_oCompany.Connected)
                        _oCompany.Disconnect();
                    Marshal.ReleaseComObject(_oCompany);
                }
            }
        }
        private void AddDirections(List<Direcciones> Direcciones, IBusinessPartners bp, bool seActRegistros) {
            List<Direcciones> listNuevasDireccion = NuevaDireccion(Direcciones);
            if (bp.Addresses.AddressName == "" || listNuevasDireccion.Count > 0)
            {
                foreach (Direcciones dir in listNuevasDireccion)
                {
                    if (seActRegistros)
                        bp.Addresses.Add();

                    bp.Addresses.AddressName    = dir.Address;
                    bp.Addresses.AddressType    = AdressTypes(dir.AdresType);
                    bp.Addresses.Country        = dir.Country;
                    bp.Addresses.State          = dir.State;
                    bp.Addresses.County         = dir.County;//Delegación/municipio
                    bp.Addresses.City           = dir.City;
                    bp.Addresses.Street         = dir.Street;
                    bp.Addresses.StreetNo       = dir.StreetNo;
                    bp.Addresses.Block          = dir.Block;
                    bp.Addresses.ZipCode        = dir.ZipCode.ToString();
                    
                    if(!seActRegistros)
                        bp.Addresses.Add();
                }
            }
        }
        private void AddContactos(List<Contacto> PrsContacto, IBusinessPartners bp, bool seActContacto) {
            List<Contacto> listNuevoContacto = NuevoContacto(PrsContacto);
            if (bp.ContactEmployees.Name != "" || listNuevoContacto.Count > 0) {
                foreach (Contacto cnt in listNuevoContacto) {
                    if (seActContacto)
                        bp.ContactEmployees.Add();
                    bp.ContactEmployees.Name            = cnt.Name;
                    bp.ContactEmployees.FirstName       = cnt.FirstName;
                    bp.ContactEmployees.Title           = cnt.Title;
                    bp.ContactEmployees.MiddleName      = cnt.MiddleName;
                    bp.ContactEmployees.Position        = cnt.Position;
                    bp.ContactEmployees.LastName        = cnt.LastName;
                    bp.ContactEmployees.Address         = cnt.Address;
                    bp.ContactEmployees.Phone1          = cnt.Tel1;
                    bp.ContactEmployees.MobilePhone     = cnt.Cellolar;
                    bp.ContactEmployees.E_Mail          = cnt.E_MailL;

                    if (!seActContacto)
                        bp.ContactEmployees.Add();
                }
            }
        }
        private int IndiceDireccion(List<Direcciones> direccion, string addresName, int LineNum) {
            int index = -1;
            for (int j = 0; j < direccion.Count; j++) {
                if (direccion[j].LineNum != -1)
                {
                    if (direccion[j].Address == addresName && direccion[j].LineNum == LineNum)
                    {
                        index = j;
                        break;
                    }
                }
            }
            return index;
        }
        private int IndiceContacto(List<Contacto> contacto, string Name, int CntctCode) {
            int index = -1;
            for (int c = 0; c < contacto.Count; c++) {
                if (contacto[c].CntctCode != -1) {
                    if (contacto[c].Name == Name && contacto[c].CntctCode == CntctCode) {
                        index = c;
                        break;
                    }
                }
            }
            return index;
        }
        private List<Direcciones> NuevaDireccion(List<Direcciones> direccion) {
            List<Direcciones> nuevaDireccion = new List<Direcciones>();
            foreach (Direcciones dir in direccion) {
                if (dir.LineNum == -1) {
                    nuevaDireccion.Add(dir);
                }
            }
            return nuevaDireccion;
        }
        private List<Contacto> NuevoContacto(List<Contacto> contacto) {
            List<Contacto> nuevoContacto = new List<Contacto>();
            foreach (Contacto cnt in contacto) {
                if (cnt.CntctCode == 0) {
                    nuevoContacto.Add(cnt);
                }
            }
            return nuevoContacto;
        }
        public bool CreateItem(ref MensajesObj msjCreate, Models.SAP modelo, ItemSAP item, string Usuario) {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo))
                {
                    _oCompany = isap.GetCompany();
                    Items oItem = (Items)_oCompany.GetBusinessObject(BoObjectTypes.oItems);

                    oItem.ItemCode = item.Header.ItemCode;
                    oItem.ItemName = item.Header.ItemName;
                    oItem.ItemsGroupCode = item.Header.ItmsGrpCod;
                    oItem.UoMGroupEntry = item.Header.UgpEntry;
                    
                    oItem.SalesItem = typeYN(item.Header.SellItem);
                    oItem.InventoryItem = typeYN(item.Header.InvntItem);
                    oItem.PurchaseItem = typeYN(item.Header.PrchseItem);
                    
                    //oItem.Valid = typeYN(item.Header.validFor);
                    //oItem.Frozen = typeYN(item.Header.validFor);

                    //Precios
                    int ListP = oItem.PriceList.Count;
                    for (int j = 0; j < ListP; j++) {
                        oItem.PriceList.SetCurrentLine(j);
                        if (oItem.PriceList.PriceList == item.Header.ListNum) { 
                            oItem.PriceList.Price = (double)item.Header.Price;
                        }
                    }

                    //Impuestos
                    oItem.WTLiable = typeYN(item.Header.WTLiable);
                    oItem.VatLiable = typeYN(item.Header.VATLiable);

                    //Propiedades
                    for(int p = 0; p < item.TabsProps.Count; p++) {
                        if (item.TabsProps[p].Status == true) {
                            oItem.Properties[item.TabsProps[p].ItmsTypCod] = BoYesNoEnum.tYES;
                        }
                    }
                    //Inventario
                    //foreach (Inventario iw in inventario) {
                    //    oItem.WhsInfo.WarehouseCode = iw.WhsCode;
                    //    oItem.WhsInfo.Locked = (BoYesNoEnum)iw.Locked;
                    //    oItem.WhsInfo.Add();
                    //}

                    if (oItem.Add() != 0)
                    {
                        msjCreate.Mensaje = $"{_oCompany.GetLastErrorCode()} {_oCompany.GetLastErrorDescription()}";
                        return false;
                    }
                    else {
                        string code = string.Empty;
                        _oCompany.GetNewObjectCode(out code);
                        msjCreate.Code = code;
                        return true;
                    }
                }
                else {
                    msjCreate.Mensaje = msj;
                    return false;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjCreate.Mensaje = ex.Message;
                return false;
            }
            finally
            {
                if (_oCompany != null)
                {
                    if (_oCompany.Connected)
                        _oCompany.Disconnect();
                    Marshal.ReleaseComObject(_oCompany);
                }
            }
        }
        public bool UpdateItem(ref MensajesObj msjCreate, Models.SAP modelo, ItemSAP item, string Usuario) {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo))
                {
                    _oCompany = isap.GetCompany();
                    Items oItem = (Items)_oCompany.GetBusinessObject(BoObjectTypes.oItems);

                    oItem.GetByKey(item.Header.ItemCode);

                    oItem.ItemName = item.Header.ItemName;
                    oItem.ItemsGroupCode = item.Header.ItmsGrpCod;
                    oItem.UoMGroupEntry = item.Header.UgpEntry;

                    oItem.SalesItem = typeYN(item.Header.SellItem);
                    oItem.InventoryItem = typeYN(item.Header.InvntItem);
                    oItem.PurchaseItem = typeYN(item.Header.PrchseItem);

                    oItem.Valid = typeYN(item.Header.validFor);

                    //Precios
                    int ListP = oItem.PriceList.Count;
                    for (int j = 0; j < ListP; j++)
                    {
                        oItem.PriceList.SetCurrentLine(j);
                        if (oItem.PriceList.PriceList == item.Header.ListNum)
                        {
                            oItem.PriceList.Price = (double)item.Header.Price;
                        }
                    }

                    //Impuestos
                    oItem.WTLiable = typeYN(item.Header.WTLiable);
                    oItem.VatLiable = typeYN(item.Header.VATLiable);

                    //Propiedades
                    for (int p = 0; p < item.TabsProps.Count; p++)
                    {
                        if (item.TabsProps[p].Status == true)
                        {
                            oItem.Properties[item.TabsProps[p].ItmsTypCod] = BoYesNoEnum.tYES;
                        }
                    }
                    //Inventario
                    //foreach (Inventario iw in inventario)
                    //{
                    //    oItem.WhsInfo.WarehouseCode = iw.WhsCode;
                    //    oItem.WhsInfo.Locked = (BoYesNoEnum)iw.Locked;
                    //    oItem.WhsInfo.Add();
                    //}

                    if (oItem.Update() != 0)
                    {
                        msjCreate.Mensaje = $"{_oCompany.GetLastErrorCode()} {_oCompany.GetLastErrorDescription()}";
                        return false;
                    }
                    else
                    {
                        string code = string.Empty;
                        _oCompany.GetNewObjectCode(out code);
                        msjCreate.Code = code;
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
                msjCreate.Mensaje = ex.Message;
                return false;
            }
            finally
            {
                if (_oCompany != null)
                {
                    if (_oCompany.Connected)
                        _oCompany.Disconnect();
                    Marshal.ReleaseComObject(_oCompany);
                }
            }
        }
        private BoYesNoEnum typeYN(bool value) {
            switch (value)
            {
                case true:
                    return BoYesNoEnum.tYES;
                case false:
                    return BoYesNoEnum.tNO;
                default:
                    return BoYesNoEnum.tNO;
            }
        }
        private BoCardTypes CardTypes(char type) {
            switch (type)
            {
                case 'L':
                    return BoCardTypes.cLid;
                case 'S':
                    return BoCardTypes.cSupplier;
                default:
                    return BoCardTypes.cCustomer;
            }
        }
        private BoAddressType AdressTypes(char type) {
            switch (type)
            {
                case 'B':
                    return BoAddressType.bo_BillTo;
                default:
                    return BoAddressType.bo_ShipTo;
            }
        }
        private void AddUserFieldHeader(BusinessPartners bp, BP _bp, string Usuario)
        {
            bp.UserFields.Fields.Item("U_FormaPago").Value  = _bp.Header.FormaPago == null ? "" : _bp.Header.FormaPago;
            bp.UserFields.Fields.Item("U_MetodoPago").Value = _bp.Header.MetodoPago == null ? "" : _bp.Header.MetodoPago;
            //doc.UserFields.Fields.Item("U_Origen").Value = "P";
            //doc.UserFields.Fields.Item("U_cvmsSucursal").Value = "Matriz";
            //doc.UserFields.Fields.Item("U_cvmsUser").Value = Usuario;
            //doc.UserFields.Fields.Item("U_cvmsHora").Value = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
