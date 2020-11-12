using salesCVM.Models;
using salesCVM.SAP.Interface;
using salesCVM.Utilities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.SAP
{
    public class SAPCrm : ISAPCrm
    {
        SAPConnect isap;
        Log lg;
        public SAPCrm() {
            isap = new SAPConnect();
            lg = Log.getIntance();
        }
        public bool CreateActivity(ref MensajesObj msjCreate, Models.SAP modelo, ActivitySap _act, string Usuario)
        {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo))
                {
                    _oCompany = isap.GetCompany();

                    CompanyService oCompServ    = _oCompany.GetCompanyService();
                    ActivitiesService oActServ  = (ActivitiesService)oCompServ.GetBusinessService(ServiceTypes.ActivitiesService);
                    Activity oAct               = (Activity)oActServ.GetDataInterface(ActivitiesServiceDataInterfaces.asActivity);

                    oAct.CardCode               = _act.Socio.CardCode;
                    oAct.ContactPersonCode      = _act.Contacto.CntctCode;
                    oAct.Phone                  = _act.Telefono;
                    oAct.HandledBy              = 1;
                    oAct.Location               = int.Parse(_act.TabGeneral.Location);
                    oAct.Subject                = _act.Asunto.Code;
                    oAct.Priority               = SetPriority(int.Parse(_act.TabGeneral.Priority));
                    oAct.Activity               = SetActivity(_act.Actividad.Code);
                    oAct.StartDate              = _act.TabGeneral.Recontact.Date;
                    oAct.EndDuedate             = _act.TabGeneral.EndDate.Date;
                    oAct.StartTime              = _act.TabGeneral.Recontact;
                    oAct.EndTime                = _act.TabGeneral.EndDate;
                    oAct.Notes                  = _act.TabContenido.Notes;

                    AddOppToAct(oAct, _act);

                    if (oActServ.AddActivity(oAct).ActivityCode != 0)
                    {
                        msjCreate.Mensaje   = $"{_oCompany.GetLastErrorCode()} {_oCompany.GetLastErrorDescription()}";
                        return false;
                    }
                    else
                    {
                        string idAct        = string.Empty;
                        _oCompany.GetNewObjectCode(out idAct);
                        msjCreate.Code      = idAct;
                        msjCreate.Mensaje   = string.Empty;
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
        public bool UpdateActivity(ref MensajesObj msjUpdate, Models.SAP modelo, ActivitySap _act, string Usuario)
        {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo))
                {
                    _oCompany = isap.GetCompany();

                    CompanyService oCompServ    = _oCompany.GetCompanyService();
                    ActivitiesService oActServ  = (ActivitiesService)oCompServ.GetBusinessService(ServiceTypes.ActivitiesService);
                    ActivityParams oActParams   = (ActivityParams)oActServ.GetDataInterface(ActivitiesServiceDataInterfaces.asActivityParams);
                    oActParams.ActivityCode     = _act.ClgCode;
                    Activity oAct               = oActServ.GetActivity(oActParams);
                    
                    if (oAct.Closed == BoYesNoEnum.tNO)
                    {
                        oAct.CardCode               = _act.Socio.CardCode;
                        oAct.ContactPersonCode      = _act.Contacto.CntctCode;
                        oAct.Phone                  = _act.Telefono;
                        oAct.HandledBy              = 1;
                        oAct.Location               = int.Parse(_act.TabGeneral.Location);
                        oAct.Subject                = _act.Asunto.Code;
                        oAct.Priority               = SetPriority(int.Parse(_act.TabGeneral.Priority));
                        oAct.Activity               = SetActivity(_act.Actividad.Code);
                        oAct.StartDate              = _act.TabGeneral.Recontact.Date;
                        oAct.EndDuedate             = _act.TabGeneral.EndDate.Date;
                        oAct.StartTime              = _act.TabGeneral.Recontact;
                        oAct.EndTime                = _act.TabGeneral.EndDate;
                        oAct.Notes                  = _act.TabContenido.Notes;

                        AddOppToAct(oAct, _act);

                        oActServ.UpdateActivity(oAct);
                        msjUpdate.Code      = _act.ClgCode.ToString();
                        msjUpdate.Mensaje   = "Actividad Actualizada";
                        return true;
                    }
                    else {
                        msjUpdate.Code      = "";
                        msjUpdate.Mensaje   = $"La actividad {_act.ClgCode} tiene un estatus de cancelado";
                        return false;
                    }
                }
                else {
                    msjUpdate.Mensaje       = msj;
                    return false;
                }
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                msjUpdate.Mensaje = ex.Message;
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
        public bool CreateOportunity(ref MensajesObj msjCreate, Models.SAP modelo, OpportunitySAP _opp, string Usuario)
        {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo))
                {
                    _oCompany               = isap.GetCompany();
                    SalesOpportunities oOpp = (SalesOpportunities)_oCompany.GetBusinessObject(BoObjectTypes.oSalesOpportunities);

                    oOpp.OpportunityName         = _opp.Header.Name;
                    oOpp.CardCode                = _opp.Header.CardCode;
                    oOpp.Territory               = _opp.Header.Territory;
                    oOpp.StartDate               = _opp.Header.OpenDate;
                    // Opp.ClosingType = SetDiftType(_opp.Detail.TabPotencial.);
                    oOpp.PredictedClosingDate    = _opp.Detail.TabPotencial.PredDate;
                    oOpp.ProjectCode             = _opp.Detail.TabGeneral.PrjCode.ToString();
                    oOpp.Source                  = _opp.Detail.TabGeneral.Source;
                    oOpp.Industry                = _opp.Detail.TabGeneral.Industry;
                    oOpp.Remarks                 = _opp.Detail.TabGeneral.Memo;
                    oOpp.SalesPerson             = _opp.Header.SlpCode;
                    if (_opp.Header.CprCode > 0)
                        oOpp.ContactPerson       = _opp.Header.CprCode;

                    AddStatus(oOpp, _opp);
                    AddStage(oOpp, _opp.Detail.TableEtapas);
                    AddPartner(oOpp, _opp.Detail.TablePartner);
                    AddCompet(oOpp, _opp.Detail.TableCompet);

                    // AddUserFieldHeader(Opp, "");
                    if (oOpp.Add() != 0)
                    {
                        msjCreate.Mensaje   = $"{_oCompany.GetLastErrorCode()} {_oCompany.GetLastErrorDescription()}";
                        return false;
                    }
                    else {
                        string idOpp        = string.Empty;
                        _oCompany.GetNewObjectCode(out idOpp);
                        msjCreate.Code      = idOpp;
                        msjCreate.Mensaje   = string.Empty;
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
        public bool UpdateOportunity(ref MensajesObj msjUpd, Models.SAP modelo, OpportunitySAP _opp, string Usuario)
        {
            string msj = string.Empty;
            Company _oCompany = null;
            try
            {
                if (isap.Conectar(ref msj, modelo))
                {
                    _oCompany               = isap.GetCompany();
                    SalesOpportunities oOpp = (SalesOpportunities)_oCompany.GetBusinessObject(BoObjectTypes.oSalesOpportunities);

                    if (oOpp.GetByKey(_opp.Header.OpprId))
                    {
                        oOpp.ProjectCode = _opp.Detail.TabGeneral.PrjCode.ToString();
                        oOpp.Source = _opp.Detail.TabGeneral.Source;
                        oOpp.Industry = _opp.Detail.TabGeneral.Industry;
                        oOpp.Remarks = _opp.Detail.TabGeneral.Memo;
                        oOpp.SalesPerson = _opp.Header.SlpCode;
                        oOpp.Territory = _opp.Header.Territory;
                        if (_opp.Header.CprCode > 0)
                            oOpp.ContactPerson = _opp.Header.CprCode;

                        AddStatus(oOpp, _opp);
                        UpdStage(oOpp, _opp.Detail.TableEtapas);
                        UpdPartner(oOpp, _opp.Detail.TablePartner);
                        UpdCompet(oOpp, _opp.Detail.TableCompet);
                        // AddUserFieldHeader(oOpp, "");

                        if (oOpp.Update() != 0)
                        {
                            msjUpd.Mensaje = $"{_oCompany.GetLastErrorCode()} - {_oCompany.GetLastErrorDescription()}";
                            return false;
                        }
                        else {
                            msjUpd.Code = _opp.Header.OpprId.ToString();
                            return true;
                        }
                    }
                    else {
                        msjUpd.Mensaje = $"El documento {_opp.Header.OpprId} NO existe";
                        return false;
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
        private BoSoClosedInTypes SetDiftType(string DifType) {
            switch (DifType) {
                case "Dias":
                    return BoSoClosedInTypes.sos_Days;
                case "Meses":
                    return BoSoClosedInTypes.sos_Months;
                case "Semanas":
                    return BoSoClosedInTypes.sos_Weeks;
                default:
                    return BoSoClosedInTypes.sos_Days;
            }
        }
        private BoAPARDocumentTypes SetDocType(int ObjType)
        {
            switch (ObjType)
            {
                case 17:
                    return BoAPARDocumentTypes.bodt_Order;
                case 23:
                    return BoAPARDocumentTypes.bodt_Quotation;
                case 13:
                    return BoAPARDocumentTypes.bodt_Invoice;
                case 15:
                    return BoAPARDocumentTypes.bodt_DeliveryNote;
                case 22:
                    return BoAPARDocumentTypes.bodt_PurchaseOrder;
                case 18:
                    return BoAPARDocumentTypes.bodt_PurchaseInvoice;
                default:
                    return BoAPARDocumentTypes.bodt_Order;
            }
        }
        private void AddUserFieldHeader(SalesOpportunities doc, string Usuario)
        {
        }
        private void AddStatus(SalesOpportunities docSap, OpportunitySAP doc) {
            switch (doc.Detail.TabResumen.Status)
            {
                case 'O':
                    docSap.Status = BoSoOsStatus.sos_Open;
                    break;
                case 'M':
                    docSap.Status = BoSoOsStatus.sos_Missed;
                    docSap.Reasons.Reason = doc.Detail.TabResumen.ReasondId;
                    docSap.Add();
                    break;
                case 'W':
                    docSap.Status = BoSoOsStatus.sos_Sold;
                    docSap.Reasons.Reason = doc.Detail.TabResumen.ReasondId;
                    docSap.Add();
                    break;

            }
        }
        private void AddStage(SalesOpportunities docSap, List<Etapas> TableEtapas) {
            for (int s = 0; s < TableEtapas.Count; s++) {
                docSap.Lines.SalesPerson        = TableEtapas[s].SlpCode;
                docSap.Lines.StartDate          = TableEtapas[s].OpenDate;
                docSap.Lines.ClosingDate        = TableEtapas[s].CloseDate;
                docSap.Lines.StageKey           = TableEtapas[s].Step_Id;
                docSap.Lines.PercentageRate     = TableEtapas[s].ClosePrcnt;
                docSap.Lines.MaxLocalTotal      = TableEtapas[s].WtSumLoc;
                if (TableEtapas[s].DocNumber > 0) {
                    docSap.Lines.DocumentType   = SetDocType(TableEtapas[s].ObjType);
                    docSap.Lines.DocumentNumber = TableEtapas[s].DocNumber;
                }
                docSap.Add();
            }
        }
        private void AddPartner(SalesOpportunities docSap, List<Partner> TablePartner)
        {
            for(int p = 0; p < TablePartner.Count; p++) {
                docSap.Partners.Partners            = TablePartner[p].ParterId;
                docSap.Partners.RelationshipCode    = 4;
                docSap.Partners.Details             = TablePartner[p].Memo;
                docSap.Partners.Add();
            }
        }
        private void AddCompet(SalesOpportunities docSap, List<Competidores> TableCompet)
        {
            for (int c = 0; c < TableCompet.Count; c++) {
                docSap.Competition.Competition  = TableCompet[c].CompetId;
                docSap.Competition.Details      = TableCompet[c].Memo;
                docSap.Competition.WonOrLost    = TableCompet[c].ThreatLevi; // Validar si es caracter o string
                docSap.Add();
            }
        }
        private void UpdStage(SalesOpportunities docSap, List<Etapas> TableEtapas)
        {
            int countStage = docSap.Lines.Count;
            if (countStage == 0)
            {
                AddStage(docSap, TableEtapas);
            }
            else
            {
                for (int uS = 0; uS < countStage; uS++)
                {
                    docSap.Lines.SetCurrentLine(uS);
                    for (int uD = 0; uD < TableEtapas.Count; uD++) {
                        if (docSap.Lines.DocumentNumber == TableEtapas[uD].DocNumber && docSap.Lines.LineNum == TableEtapas[uD].LineNum) {
                            docSap.Lines.SalesPerson    = TableEtapas[uD].SlpCode;
                            docSap.Lines.StartDate      = TableEtapas[uD].OpenDate;
                            docSap.Lines.ClosingDate    = TableEtapas[uD].CloseDate;
                            docSap.Lines.StageKey       = TableEtapas[uD].Step_Id;
                            docSap.Lines.PercentageRate = TableEtapas[uD].ClosePrcnt;
                            docSap.Lines.MaxLocalTotal  = TableEtapas[uD].WtSumLoc;
                            if (TableEtapas[uD].DocNumber > 0)
                            {
                                docSap.Lines.DocumentType   = SetDocType(TableEtapas[uD].ObjType);
                                docSap.Lines.DocumentNumber = TableEtapas[uD].DocNumber;
                            }
                        }
                    }
                }
            }
        }
        private void UpdPartner(SalesOpportunities docSap, List<Partner> TablePartner)
        {
            int countPartner = docSap.Partners.Count;
            if (countPartner == 0)
            {
                AddPartner(docSap, TablePartner);
            }
            else {
                for (int uP = 0; uP < countPartner; uP++) {
                    docSap.Partners.SetCurrentLine(uP);
                    for (int uD = 0; uD < TablePartner.Count; uD++) {
                        if (docSap.Partners.Partners == TablePartner[uD].ParterId) {
                            docSap.Partners.RelationshipCode    = 4;
                            docSap.Partners.Details             = TablePartner[uD].Memo;
                        }
                    }
                }
            }

        }
        private void UpdCompet(SalesOpportunities docSap, List<Competidores> TableCompet)
        {
            int countCompt = docSap.Partners.Count;
            if (countCompt == 0)
            {
                AddCompet(docSap, TableCompet);
            }
            else {
                for (int uC = 0; uC < countCompt; uC++) {
                    docSap.Competition.SetCurrentLine(uC);
                    for (int uD = 0; uD < TableCompet.Count; uD++) {
                        if (docSap.Competition.Competition == TableCompet[uD].CompetId) {
                            docSap.Competition.Details      = TableCompet[uD].Memo;
                            docSap.Competition.WonOrLost    = TableCompet[uD].ThreatLevi;
                        }
                    }
                }
            }
        }
        private BoMsgPriorities SetPriority(int Prioridad)
        {
            switch (Prioridad)
            {
                case 0:
                    return BoMsgPriorities.pr_Low;
                case 1:
                    return BoMsgPriorities.pr_Normal;
                case 2:
                    return BoMsgPriorities.pr_High;
                default:
                    return BoMsgPriorities.pr_Low;
            }
        }
        private BoActivities SetActivity(char Actividad)
        {
            switch (Actividad)
            {
                case 'C':
                    return BoActivities.cn_Conversation;
                case 'M':
                    return BoActivities.cn_Meeting;
                case 'T':
                    return BoActivities.cn_Task;
                case 'E':
                    return BoActivities.cn_Note;
                case 'P':
                    return BoActivities.cn_Campaign;
                case 'N':
                    return BoActivities.cn_Other;
                default:
                    return BoActivities.cn_Conversation;
            }
        }
        private void AddOppToAct(Activity oAct, ActivitySap _act) {
            if (_act.IdOpp > 0)
            {
                oAct.SalesOpportunityId = _act.IdOpp;
                oAct.SalesOpportunityLine = _act.LineOpp;
            }
        }
    }
}
