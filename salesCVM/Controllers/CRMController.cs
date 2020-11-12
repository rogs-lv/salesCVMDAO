﻿using salesCVM.DAO.DAO;
using salesCVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace salesCVM.Controllers
{
    [Authorize]
    [RoutePrefix("salesCVM/CRM")]
    public class CRMController : ApiController
    {
        private CrmDAO crmDAO;
        public CRMController() {
            crmDAO = new CrmDAO();
        }
        
        [HttpGet]
        [Route("GetOpportunity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetOpportunity(int type, int idOpp = 0) {
            string msj = string.Empty;
            switch (type)
            {
                case 1:
                    List<Opportunity> listOpps = new List<Opportunity>();
                    if (crmDAO.GetOpportunities(ref listOpps, ref msj))
                        return Content(HttpStatusCode.OK, listOpps);
                    else
                        return Content(HttpStatusCode.InternalServerError, msj);
                case 2:
                    TabsOpportunity dataTabsOpp = new TabsOpportunity();
                    if (crmDAO.GetDataTabsOpportunity(ref dataTabsOpp, ref msj, idOpp))
                        return Content(HttpStatusCode.OK, dataTabsOpp);
                    else
                        return Content(HttpStatusCode.InternalServerError, msj);
                default:
                    return Content(HttpStatusCode.BadRequest, "Opción desconocida");
            }
        }
        
        [HttpGet]
        [Route("GetListaPartner")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetListaPartner() {
            List<BusinessP> ListBPtoCRM = new List<BusinessP>();
            if (crmDAO.GetListBpToCRM(ref ListBPtoCRM))
                return Content(HttpStatusCode.OK, ListBPtoCRM);
            else
                return Content(HttpStatusCode.BadRequest, "Error al obtener socios de negocios");
        }
        
        [HttpGet]
        [Route("GetOptions")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetOptions(int type, string cardcode = "") {
            switch (type)
            {
                case 1: //Options to header document opportunity
                    OptionsHeaderOpp optionsHeader = new OptionsHeaderOpp();
                    if (crmDAO.GetOptionsOpportunity(ref optionsHeader, cardcode))
                        return Content(HttpStatusCode.OK, optionsHeader);
                    else
                        return Content(HttpStatusCode.InternalServerError, "Error al cargar datos para el documento");
                case 2: //Options to detail document opportunity
                    OptionsTabsDetail optionsDetail = new OptionsTabsDetail();
                    if (crmDAO.GetOptionsTabsGeneral(ref optionsDetail))
                        return Content(HttpStatusCode.OK, optionsDetail);
                    else
                        return Content(HttpStatusCode.InternalServerError, "Error al cargar datos para el detalle del documento");
                case 3://Options to header document Activity
                    OptionsHeadActivity optionsActivity = new OptionsHeadActivity();
                    if (crmDAO.GetOptionsActivity(ref optionsActivity, cardcode))
                        return Content(HttpStatusCode.OK, optionsActivity);
                    else
                        return Content(HttpStatusCode.InternalServerError, "Error al cargar datos para la actividad");
                case 4://Options to detail document Activity
                    OptionsTabsActivity optionTabsActivity = new OptionsTabsActivity();
                    if (crmDAO.GetOptionsTabsActivity(ref optionTabsActivity))
                        return Content(HttpStatusCode.OK, optionTabsActivity);
                    else
                        return Content(HttpStatusCode.InternalServerError, "Error al cargar datos para el detalle de la actividad");

                default:
                    return Content(HttpStatusCode.BadRequest, "Opción desconocida");
            }
        }

        [HttpPost]
        [Route("CreateOpportunity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CreateOpportunity([FromBody] OpportunitySAP document, string usuario) {
            MensajesObj response = new MensajesObj();
            if (crmDAO.CreateOpportunity(ref response, document, usuario))
                return Content(HttpStatusCode.OK, response);
            else
                return Content(HttpStatusCode.Conflict, response.Mensaje);
        }
        
        [HttpPatch]
        [Route("UpdateOpportunity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateOpportunity([FromBody] OpportunitySAP document, string usuario) {
            MensajesObj response = new MensajesObj();
            if (crmDAO.UpdateOpportunity(ref response, document, usuario))
                return Content(HttpStatusCode.OK, response);
            else
                return Content(HttpStatusCode.Conflict, response.Mensaje);
        }

        [HttpPost]
        [Route("CreateActivity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CreateActivity([FromBody] ActivitySap document, string usuario)
        {
            MensajesObj response = new MensajesObj();
            if (crmDAO.CreateActivity(ref response, document, usuario))
                return Content(HttpStatusCode.OK, response);
            else
                return Content(HttpStatusCode.Conflict, response.Mensaje);
        }
        
        [HttpPatch]
        [Route("UpdateActivity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateActivity([FromBody] ActivitySap document, string usuario)
        {
            MensajesObj response = new MensajesObj();
            if (crmDAO.UpdateActivity(ref response, document, usuario))
                return Content(HttpStatusCode.OK, response);
            else
                return Content(HttpStatusCode.Conflict, response.Mensaje);
        }

    }
}
