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
        public bool CreateActivity(ref MensajesObj msjCreate, Models.SAP modelo, ActivitySAP _act, string Usuario)
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

                    oAct.CardCode               = _act.CardCode;
                    if(_act.CntctCode > 0)
                        oAct.ContactPersonCode      = _act.CntctCode;
                    oAct.Phone                  = _act.Tel;
                    oAct.HandledBy              = 1;
                    if(_act.CntctType != 0)
                        oAct.ActivityType       = _act.CntctType;
                    if(_act.CntctSbjct != -1)
                        oAct.Subject            = _act.CntctSbjct;
                    if(_act.Location > 0)
                        oAct.Location           = _act.Location;
                    oAct.Priority               = SetPriority(_act.Priority);
                    oAct.Activity               = SetActivity(_act.Action);
                    oAct.StartDate              = DateTime.Parse(_act.Recontact);
                    oAct.EndDuedate             = DateTime.Parse(_act.endDate);
                    oAct.StartTime              = DateTime.ParseExact(_act.BeginTime, "H:mm:ss", null, System.Globalization.DateTimeStyles.None);
                    oAct.EndTime                = DateTime.ParseExact(_act.ENDTime, "H:mm:ss", null, System.Globalization.DateTimeStyles.None);
                    oAct.Notes                  = _act.Notes;

                    AddOppToAct(oAct, _act);
                    int numAct = oActServ.AddActivity(oAct).ActivityCode;
                    if (numAct != 0) // Se genero correctamente
                    {
                        msjCreate.Code = numAct.ToString();
                        msjCreate.Mensaje = string.Empty;
                        return true;
                    } // Hubo algun error
                    else
                    {
                        msjCreate.Mensaje = $"{_oCompany.GetLastErrorCode()} {_oCompany.GetLastErrorDescription()}";
                        return false;
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
        public bool UpdateActivity(ref MensajesObj msjUpdate, Models.SAP modelo, ActivitySAP _act, string Usuario)
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

                    if (oAct.SalesOpportunityId != _act.OprId && oAct.SalesOpportunityId > 0)
                    {
                        msjUpdate.Code = "";
                        msjUpdate.Mensaje = $"La actividad ya tiene una oportunidad ligada y no puede ser modificada";
                        return false;
                    }
                    else
                    {
                        if (oAct.Closed == BoYesNoEnum.tNO)
                        {
                            if (_act.CntctCode > 0)
                                oAct.ContactPersonCode  = _act.CntctCode;
                            oAct.Phone                  = _act.Tel;
                            oAct.HandledBy              = 1;
                            if (_act.CntctType != 0)
                                oAct.ActivityType       = _act.CntctType;
                            if (_act.CntctSbjct != -1)
                                oAct.Subject            = _act.CntctSbjct;
                            if (_act.Location > 0)
                                oAct.Location           = _act.Location;
                            oAct.Priority               = SetPriority(_act.Priority);
                            oAct.Activity               = SetActivity(_act.Action);
                            oAct.StartDate              = DateTime.Parse(_act.Recontact);
                            oAct.EndDuedate             = DateTime.Parse(_act.endDate);
                            oAct.StartTime              = DateTime.ParseExact(_act.BeginTime, "H:mm:ss", null, System.Globalization.DateTimeStyles.None);
                            oAct.EndTime                = DateTime.ParseExact(_act.ENDTime, "H:mm:ss", null, System.Globalization.DateTimeStyles.None);
                            oAct.Notes                  = _act.Notes;

                            AddOppToAct(oAct, _act);

                            oActServ.UpdateActivity(oAct);
                            msjUpdate.Code = _act.ClgCode.ToString();
                            msjUpdate.Mensaje = "Actividad Actualizada";
                            return true;
                        }
                        else
                        {
                            msjUpdate.Code = "";
                            msjUpdate.Mensaje = $"La actividad {_act.ClgCode} tiene un estatus de cancelado";
                            return false;
                        }
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
                    if (_opp.Header.CloseDate != null)
                        oOpp.ClosingDate         = _opp.Header.CloseDate;
                    oOpp.PredictedClosingDate    = _opp.Tabs.TabPotencial.PredDate;
                    if (_opp.Tabs.TabGeneral.PrjCode != "")
                        oOpp.ProjectCode             = _opp.Tabs.TabGeneral.PrjCode;
                    if (_opp.Tabs.TabGeneral.Source > 0)
                        oOpp.Source                  = _opp.Tabs.TabGeneral.Source;
                    if (_opp.Tabs.TabGeneral.Industry > 0)
                        oOpp.Industry                = _opp.Tabs.TabGeneral.Industry;
                    if (_opp.Tabs.TabGeneral.Memo != "")
                        oOpp.Remarks                 = _opp.Tabs.TabGeneral.Memo;
                    oOpp.SalesPerson             = _opp.Header.SlpCode;
                    oOpp.TotalAmountLocal        = _opp.Tabs.TabPotencial.MaxSumLoc;

                    if (_opp.Header.CprCode > 0)
                        oOpp.ContactPerson       = _opp.Header.CprCode;

                    AddStatus(oOpp, _opp);
                    AddStage(oOpp, _opp.Tabs.TableEtapas);
                    AddPartner(oOpp, _opp.Tabs.TablePartner);
                    AddCompet(oOpp, _opp.Tabs.TableCompet);

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
                        oOpp.OpportunityName        = _opp.Header.Name;
                        oOpp.Territory              = _opp.Header.Territory;
                        oOpp.StartDate              = _opp.Header.OpenDate;
                        if (_opp.Header.CloseDate != null)
                            oOpp.ClosingDate        = _opp.Header.CloseDate;
                        oOpp.PredictedClosingDate   = _opp.Tabs.TabPotencial.PredDate;
                        if (_opp.Tabs.TabGeneral.PrjCode != "")
                            oOpp.ProjectCode        = _opp.Tabs.TabGeneral.PrjCode;
                        if (_opp.Tabs.TabGeneral.Source > 0)
                            oOpp.Source             = _opp.Tabs.TabGeneral.Source;
                        if (_opp.Tabs.TabGeneral.Industry > 0)
                            oOpp.Industry           = _opp.Tabs.TabGeneral.Industry;
                        if (_opp.Tabs.TabGeneral.Memo != "")
                            oOpp.Remarks            = _opp.Tabs.TabGeneral.Memo;
                        oOpp.SalesPerson            = _opp.Header.SlpCode;
                        oOpp.TotalAmountLocal       = _opp.Tabs.TabPotencial.MaxSumLoc;

                        if (_opp.Header.CprCode > 0)
                            oOpp.ContactPerson      = _opp.Header.CprCode;

                        bool seActEtapas = false;
                        UpdStage(ref seActEtapas, oOpp, _opp.Tabs.TableEtapas);
                        bool seActPartner = false;
                        UpdPartner(ref seActPartner, oOpp, _opp.Tabs.TablePartner);
                        bool seActCompet = false;
                        UpdCompet(ref seActCompet, oOpp, _opp.Tabs.TableCompet);

                        // AddUserFieldHeader(oOpp, "");

                        // si hay que agregar nuevo registros
                        UpdNewStage(oOpp, _opp.Tabs.TableEtapas, seActEtapas);
                        UpdNewPartner(oOpp, _opp.Tabs.TablePartner, seActPartner);
                        UpdNewCompet(oOpp, _opp.Tabs.TableCompet, seActCompet);

                        UpdStatus(oOpp, _opp);

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
            switch (doc.Header.Status)
            {
                case "O":
                    docSap.Status = BoSoOsStatus.sos_Open;
                    break;
                case "M":
                    docSap.Status = BoSoOsStatus.sos_Missed;
                    docSap.Reasons.Reason = doc.Header.ReasondId;
                    docSap.Reasons.Add();
                    break;
                case "W":
                    docSap.Status = BoSoOsStatus.sos_Sold;
                    docSap.Reasons.Reason = doc.Header.ReasondId;
                    docSap.Reasons.Add();
                    break;

            }
        }
        private void UpdStatus(SalesOpportunities docSap, OpportunitySAP doc) {
            switch (doc.Header.Status)
            {
                case "O":
                    docSap.Status = BoSoOsStatus.sos_Open;
                    break;
                case "M":
                    docSap.Status               = BoSoOsStatus.sos_Missed;
                    //docSap.Reasons.SetCurrentLine(0);
                    //docSap.Reasons.Reason       = int.Parse(doc.Header.ReasondId.ToString());
                    break;
                case "W":
                    docSap.Status               = BoSoOsStatus.sos_Sold;
                    //docSap.Reasons.SetCurrentLine(0);
                    //docSap.Reasons.Reason       = int.Parse(doc.Header.ReasondId.ToString());
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
                docSap.Lines.Add();
            }
        }
        private void AddPartner(SalesOpportunities docSap, List<Partner> TablePartner)
        {
            for(int p = 0; p < TablePartner.Count; p++) {
                docSap.Partners.Partners            = TablePartner[p].ParterId;
                if(TablePartner[p].OrlCode > 0)
                    docSap.Partners.RelationshipCode= TablePartner[p].OrlCode;
                docSap.Partners.Details             = TablePartner[p].Memo;
                docSap.Partners.Add();
            }
        }
        private void AddCompet(SalesOpportunities docSap, List<Competidores> TableCompet)
        {
            for (int c = 0; c < TableCompet.Count; c++) {
                docSap.Competition.Competition  = TableCompet[c].CompetId;
                docSap.Competition.Details      = TableCompet[c].Memo;
                docSap.Competition.WonOrLost    = TableCompet[c].Won ? "Y" : "N";// Validar si es caracter o string
                docSap.Competition.Add();
            }
        }
        private void UpdStage(ref bool seActEtapas, SalesOpportunities docSap, List<Etapas> TableEtapas)
        {
            int countStage = docSap.Lines.Count;
            for (int uS = 0; uS < countStage; uS++)
            {
                docSap.Lines.SetCurrentLine(uS);
                int uD = IndiceEtapas(TableEtapas, docSap.Lines.LineNum);
                if(uD > -1) { 
                    docSap.Lines.SalesPerson = TableEtapas[uD].SlpCode;
                    docSap.Lines.StartDate = TableEtapas[uD].OpenDate;
                    docSap.Lines.ClosingDate = TableEtapas[uD].CloseDate;
                    docSap.Lines.StageKey = TableEtapas[uD].Step_Id;
                    docSap.Lines.PercentageRate = TableEtapas[uD].ClosePrcnt;
                    docSap.Lines.MaxLocalTotal = TableEtapas[uD].WtSumLoc;
                    if (TableEtapas[uD].DocNumber > 0)
                    {
                        docSap.Lines.DocumentType = SetDocType(TableEtapas[uD].ObjType);
                        docSap.Lines.DocumentNumber = TableEtapas[uD].DocNumber;
                    }
                    seActEtapas = true;
                }
            }
        }
        private void UpdNewStage(SalesOpportunities docSap, List<Etapas> TableEtapas, bool seActEtapas) {
            for (int s = 0; s < TableEtapas.Count; s++)
            {
                if (TableEtapas[s].LineNum == -1)
                {
                    if (seActEtapas)
                        docSap.Lines.Add();

                    docSap.Lines.SalesPerson = TableEtapas[s].SlpCode;
                    docSap.Lines.StartDate = TableEtapas[s].OpenDate;
                    docSap.Lines.ClosingDate = TableEtapas[s].CloseDate;
                    docSap.Lines.StageKey = TableEtapas[s].Step_Id;
                    docSap.Lines.PercentageRate = TableEtapas[s].ClosePrcnt;
                    docSap.Lines.MaxLocalTotal = TableEtapas[s].WtSumLoc;
                    if (TableEtapas[s].DocNumber > 0)
                    {
                        docSap.Lines.DocumentType = SetDocType(TableEtapas[s].ObjType);
                        docSap.Lines.DocumentNumber = TableEtapas[s].DocNumber;
                    }
                    // docSap.Lines.Add();
                }
            }
        }
        private int IndiceEtapas(List<Etapas> etapas, int LineNum)
        {
            int index = -1;
            for (int j = 0; j < etapas.Count; j++)
            {
                if (etapas[j].LineNum != -1)
                {
                    if (etapas[j].LineNum == LineNum)
                    {
                        index = j;
                        break;
                    }
                }
            }
            return index;
        }
        private void UpdPartner(ref bool seActPartner, SalesOpportunities docSap, List<Partner> TablePartner)
        {
            int countPartner = docSap.Partners.Count;
            for (int uP = 0; uP < countPartner; uP++) {
                docSap.Partners.SetCurrentLine(uP);
                int idx = IndicePartner(TablePartner, docSap.Partners.RowNo);
                if (idx > -1) {
                    docSap.Partners.Partners = TablePartner[idx].ParterId;
                    if (TablePartner[idx].OrlCode > 0)
                        docSap.Partners.RelationshipCode = int.Parse(TablePartner[idx].OrlCode.ToString());
                    docSap.Partners.Details = TablePartner[idx].Memo.ToString(); ;
                    seActPartner = true;
                }
            }
        }
        private void UpdNewPartner(SalesOpportunities docSap, List<Partner> TablePartner, bool seActPartner) {
            for (int p = 0; p < TablePartner.Count; p++)
            {
                if (TablePartner[p].Line == -1) {
                    
                    if (seActPartner) 
                        docSap.Partners.Add();

                    docSap.Partners.Partners = TablePartner[p].ParterId;
                    if (TablePartner[p].OrlCode > 0)
                        docSap.Partners.RelationshipCode = TablePartner[p].OrlCode;
                    docSap.Partners.Details = TablePartner[p].Memo;
                }
            }
        }
        private int IndicePartner(List<Partner> partner, int Line) {
            int index = -1;
            for (int j = 0; j < partner.Count; j++)
            {
                if (partner[j].Line != -1)
                {
                    if (partner[j].Line == Line)
                    {
                        index = j;
                        break;
                    }
                }
            }
            return index;
        }
        private void UpdCompet(ref bool seActCompet, SalesOpportunities docSap, List<Competidores> TableCompet)
        {
            int countCompt = docSap.Competition.Count;
            for (int uC = 0; uC < countCompt; uC++) {
                docSap.Competition.SetCurrentLine(uC);
                int ind = IndiceCompet(TableCompet, docSap.Competition.RowNo);
                if (ind > -1) {
                    docSap.Competition.Competition = TableCompet[ind].CompetId;
                    docSap.Competition.Details = TableCompet[ind].Memo;
                    docSap.Competition.WonOrLost = TableCompet[ind].Won ? "Y" : "N";
                    seActCompet = true;
                }
            }
        }
        private void UpdNewCompet(SalesOpportunities docSap, List<Competidores> TableCompet, bool seActCompet) {
            for (int c = 0; c < TableCompet.Count; c++)
            {
                if (TableCompet[c].Line == -1) {
                    if (seActCompet)
                        docSap.Competition.Add();
                    
                    docSap.Competition.Competition = TableCompet[c].CompetId;
                    docSap.Competition.Details = TableCompet[c].Memo;
                    docSap.Competition.WonOrLost = TableCompet[c].Won ? "Y" : "N";// Validar si es caracter o string
                }
            }
        }
        private int IndiceCompet(List<Competidores> compets, int Line) {
            int index = -1;
            for (int j = 0; j < compets.Count; j++)
            {
                if (compets[j].Line != -1)
                {
                    if (compets[j].Line == Line)
                    {
                        index = j;
                        break;
                    }
                }
            }
            return index;
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
        private BoActivities SetActivity(string Actividad)
        {
            switch (Actividad)
            {
                case "C":
                    return BoActivities.cn_Conversation;
                case "M":
                    return BoActivities.cn_Meeting;
                case "T":
                    return BoActivities.cn_Task;
                case "E":
                    return BoActivities.cn_Note;
                case "P":
                    return BoActivities.cn_Campaign;
                case "N":
                    return BoActivities.cn_Other;
                default:
                    return BoActivities.cn_Conversation;
            }
        }
        private void AddOppToAct(Activity oAct, ActivitySAP _act) {
            if (_act.OprId > 0)
            {
                oAct.SalesOpportunityId     = _act.OprId;
                oAct.SalesOpportunityLine   = _act.OprLine;
            }
        }
    }
}
