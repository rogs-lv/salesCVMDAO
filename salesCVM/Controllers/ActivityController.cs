using salesCVM.DAO.DAO;
using salesCVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace salesCVM.Controllers
{
    [Authorize]
    [RoutePrefix("salesCVM/Activity")]
    public class ActivityController : ApiController
    {
        private readonly ActivityDAO actDAO;
        public ActivityController()
        {
            actDAO = new ActivityDAO();
        }

        [HttpGet]
        [Route("GetOptionsActivity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetOptionsActivity(int action) {
            switch (action)
            {
                case 1: // Get values to drop downs activity
                    object list = null;
                    if (actDAO.GetOptionsActivity(ref list))
                        return Content(HttpStatusCode.OK, list);
                    else
                        return Content(HttpStatusCode.NotFound, "No se recupero información para la actividad");
                default:
                    return Content(HttpStatusCode.InternalServerError, "Valor desconocido");
            }
        }

        [HttpGet]
        [Route("GetContactsActivity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetContactsActivity(string cardCode)
        {
            List<Contactos> ListPers = new List<Contactos>();
            if (actDAO.GetContactPerson(ref ListPers, cardCode))
                return Content(HttpStatusCode.OK, ListPers);
            else
                return Content(HttpStatusCode.OK, ListPers);
        }

        [HttpGet]
        [Route("GetListasActivity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetListasActivity(int type, int idDoc = 0) {
            switch (type)
            {
                case 97: // List of opportunity
                    List<OportunidadAct> listOpp = new List<OportunidadAct>();
                    if (actDAO.GetListasActivity(ref listOpp, type))
                        return Content(HttpStatusCode.OK, listOpp);
                    else
                        return Content(HttpStatusCode.OK, listOpp);
                case 2: // List of bussnes partner
                    List<SocioNegocioAct> listPartners = new List<SocioNegocioAct>();
                    if (actDAO.GetListasActivity(ref listPartners, type))
                        return Content(HttpStatusCode.OK, listPartners);
                    else
                        return Content(HttpStatusCode.OK, listPartners);
                case 3: // list of activities
                    List<Actividad> listActs = new List<Actividad>();
                    if (actDAO.GetListasActivity(ref listActs, type))
                        return Content(HttpStatusCode.OK, listActs);
                    else
                        return Content(HttpStatusCode.OK, listActs);
                case 8: // list of stages
                    List<Stage> listStage = new List<Stage>();
                    if(idDoc <= 0 )
                        return Content(HttpStatusCode.BadRequest, "La acción requiere un documento");

                    if (actDAO.GetListasActivity(ref listStage, type, idDoc))
                        return Content(HttpStatusCode.OK, listStage);
                    else
                        return Content(HttpStatusCode.OK, listStage);
                default:
                    return Content(HttpStatusCode.InternalServerError, "Tipo desconocido");
            }
        }

        [HttpGet]
        [Route("GetActivityId")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetActivityId(int idDoc) {
            ActivitySAP document = new ActivitySAP();
            if (idDoc <= 0)
                return Content(HttpStatusCode.BadRequest, "Indique id de documento");

            if (actDAO.GetActivityId(ref document, idDoc))
                return Content(HttpStatusCode.OK, document);
            else
                return Content(HttpStatusCode.OK, document);
        }

        [HttpPost]
        [Route("CreateActivity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CreateActivity([FromBody] ActivitySAP document, string usuario)
        {
            if(document.OprId != -1 || document.OprLine != -1)
                return Content(HttpStatusCode.BadRequest, "Una actividad no puede ser ligada a una oportunidad al momento de ser creada");

            MensajesObj response = new MensajesObj();
            if (actDAO.CreateActivity(ref response, document, usuario))
                return Content(HttpStatusCode.OK, response);
            else
                return Content(HttpStatusCode.Conflict, response.Mensaje);
        }

        [HttpPatch]
        [Route("UpdateActivity")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateActivity([FromBody] ActivitySAP document, string usuario)
        {
            MensajesObj response = new MensajesObj();
            if (actDAO.UpdateActivity(ref response, document, usuario))
                return Content(HttpStatusCode.OK, response);
            else
                return Content(HttpStatusCode.Conflict, response.Mensaje);
        }

    }
}
