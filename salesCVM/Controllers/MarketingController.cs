using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using salesCVM.Models;
using salesCVM.DAO.DAO;
using System.Web.Http.Cors;
using System.Security.Policy;

namespace salesCVM.Controllers
{
    [Authorize]
    [RoutePrefix("salesCVM/Marketing")]
    public class MarketingController : ApiController
    {
        MarketingDAO MktDao;
        
        public MarketingController() {
            MktDao = new MarketingDAO();
        }

        [HttpPost]
        [Route("SaveDocument")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult SaveDocument([FromBody]DocSAP document, char typeEvent, int typeDocument) {
            //validar que si no existe registros no pase al metodo
            if (document.Header == null || document.Detail == null)
                return Content(HttpStatusCode.BadRequest, "El parametro de documento no puede ser nulo");
            if (document.Detail.Count == 0)
                return Content(HttpStatusCode.BadRequest, "El documento debe contener por lo menos un artículo");

            Mensajes msj = new Mensajes();

            switch (typeEvent)
            {
                case 'I'://Insert
                    if (MktDao.SaveDocument(ref msj, document, typeDocument))
                        return Content(HttpStatusCode.OK, $"Borrador {msj.DocEntry} guardado correctamente");
                    else
                        return Content(HttpStatusCode.Conflict, msj);
                case 'U'://Update
                    if (MktDao.UpdateDocument(ref msj, document, typeDocument))
                        return Content(HttpStatusCode.OK, $"Borrador {msj.DocEntry} actualizado");
                    else
                        return Content(HttpStatusCode.Conflict, msj);
                default:
                    return Content(HttpStatusCode.BadRequest, "Sentencia no reconocida");
            }
        }
        [HttpPost]
        [Route("CreateDocument")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CreateDocument([FromBody]DocSAP document, int typeDocument, string usuario)
        {
            Mensajes response = new Mensajes();
            if (MktDao.CreateDocumentSAP(ref response, document, typeDocument, usuario)) 
                return Content(HttpStatusCode.OK, response);
            else
                return Content(HttpStatusCode.Conflict, response.Mensaje);
        }
        [HttpGet]
        [Route("GetDocument")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetDocument(int typeDocument, int DocEntry) {
            if(typeDocument <= 0)
                return Content(HttpStatusCode.BadRequest, "Tipo de documento desconocido");

            if(DocEntry <= 0)
                return Content(HttpStatusCode.BadRequest, "Número de documento no valido");

            string msj = string.Empty;
            DocSAP document = new DocSAP();

            if (MktDao.GetDocument(ref msj, ref document, typeDocument, DocEntry)) 
                return Content(HttpStatusCode.OK, document);
            else
                return Content(HttpStatusCode.NotFound, $"No se encontró ningun registro con el documento: {DocEntry} ");
        }
        [HttpGet]
        [Route("GetDocumentSAP")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetDocumentSAP(int docEntry = 0, string cardcode = "", string usuario = "") {
            Mensajes msj = new Mensajes();
            DocSAP doc = new DocSAP();
            List<Document> listDoc = new List<Document>();

            if (MktDao.GetDocumentsSAP(ref msj, ref doc, ref listDoc, docEntry, cardcode, usuario))
            {
                if (docEntry > 0)
                    return Content(HttpStatusCode.OK, doc);
                else
                    return Content(HttpStatusCode.OK, listDoc);
            }
            else
                return Content(HttpStatusCode.NotFound, msj.Mensaje);
        }
    }
}
