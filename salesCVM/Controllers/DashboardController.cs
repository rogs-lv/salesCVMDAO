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
    [RoutePrefix("salesCVM/Dashboard")]
    public class DashboardController : ApiController
    {
        private DashboardDAO dashDAO;
        public DashboardController() {
            dashDAO = new DashboardDAO();
        }
        [HttpGet]
        [Route("GetPromocion")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetPromocion(string usuario) {
            if (string.IsNullOrEmpty(usuario))
                return Content(HttpStatusCode.BadRequest, "El usuario es requerido");

            List<Anuncio> anuncios = new List<Anuncio>();
            string msj = string.Empty;

            if (dashDAO.GetPromocionesNoticias(ref anuncios, ref msj, "P", usuario))
                return Content(HttpStatusCode.OK, new { codigo = 0, mensaje = anuncios });
            else
                return Content(HttpStatusCode.NotFound, new { codigo = -1, mensaje = msj});
        }
        [HttpGet]
        [Route("GetNoticia")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetNoticias(string usuario) {
            if (string.IsNullOrEmpty(usuario))
                return Content(HttpStatusCode.BadRequest, "El usuario es requerido");
            
            List<Anuncio> anuncios = new List<Anuncio>();
            string msj = string.Empty;

            if (dashDAO.GetPromocionesNoticias(ref anuncios, ref msj, "N", usuario))
                return Content(HttpStatusCode.OK, new { codigo = 0, mensaje = anuncios });
            else
                return Content(HttpStatusCode.NotFound, new { codigo = -1, mensaje = msj });
        }
        [HttpGet]
        [Route("GetGrafica")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetGrafica(string usuario) {
            if (string.IsNullOrEmpty(usuario))
                return Content(HttpStatusCode.BadRequest, "El usuario es requerido");

            List<Grafica> DtsGrafica = new List<Grafica>();
            string msj = string.Empty;

            if (dashDAO.GetDatosGrafica(ref DtsGrafica, ref msj, usuario))
                return Content(HttpStatusCode.OK, new { codigo = 0, mensaje = DtsGrafica });
            else
                return Content(HttpStatusCode.NotFound, new { codigo = -1, mensaje = msj});
        }
    }
}
