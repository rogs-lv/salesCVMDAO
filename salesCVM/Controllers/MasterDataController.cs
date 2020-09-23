using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using salesCVM.Models;
using salesCVM.DAO;
using salesCVM.DAO.DAO;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;

namespace salesCVM.Controllers
{
    [Authorize]
    [RoutePrefix("salesCVM/MasterData")]
    public class MasterDataController : ApiController
    {
        DatosMaestrosDAO DtsDao;
        public MasterDataController() {
            DtsDao = new DatosMaestrosDAO();
        }

        [HttpGet]
        [Route("GetMasterData")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetMasterData(int type, string whsCode = "", string priceList = "") {
            switch (type)
            {
                case 2:
                    List<BusnessPartner> ListBp = new List<BusnessPartner>();
                    if (whsCode.Length > 0)
                        return Content(HttpStatusCode.BadRequest, "El almacén no es valido");
                    if (priceList.Length > 0)
                        return Content(HttpStatusCode.BadRequest, "La lista de precios no es valido");

                    if (DtsDao.GetDatosMaestros(ref ListBp, type, "", "" ))
                        return Content(HttpStatusCode.OK, ListBp);
                    else
                        return Content(HttpStatusCode.NoContent, "No se recuperarón socios de negocios");
                case 4:
                    List<Item> ListItem = new List<Item>();
                    if (DtsDao.GetDatosMaestros(ref ListItem, type, whsCode, priceList ))
                        return Content(HttpStatusCode.OK, ListItem);
                    else
                        return Content(HttpStatusCode.NoContent, "No se recuperarón artículos");
                default:
                    return Content(HttpStatusCode.NotFound, "Tipo de dato maestro desconocido");
            }

        }

        [HttpGet]
        [Route("GetBusnessPartner")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetBusnessPartner(int type, string cardcode) {
            string msj = string.Empty;
            switch (type) {
                case 1:
                    List<Socios> ListaSocios = new List<Socios>();
                    if (DtsDao.GetInformacionSocios(ref ListaSocios, ref msj, type, cardcode))
                        return Content(HttpStatusCode.OK, ListaSocios);
                    else
                        return Content(HttpStatusCode.OK, string.IsNullOrEmpty(msj) ? "No se recuperaron socios de negocios" : msj);
                case 2:
                    List<Direcciones> ListaDirecciones = new List<Direcciones>();
                    if (DtsDao.GetInformacionSocios(ref ListaDirecciones, ref msj, type, cardcode))
                        return Content(HttpStatusCode.OK, ListaDirecciones);
                    else
                        return Content(HttpStatusCode.OK, string.IsNullOrEmpty(msj) ? $"No se recuperaró información para: {cardcode}" : msj);
                default:
                    return Content(HttpStatusCode.NotFound, "Opcion desconocida");
            }
        }

        [HttpPost]
        [Route("CreateBusnessPartner")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CreateBusnessPartner(BP socioNegocios) {
            return null;
        }

        [HttpPatch]
        [Route("UpdateBusnessPartner")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateBusnessPartner(BP socioNegocios)
        {
            return null;
        }

        [HttpGet]
        [Route("GetItems")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetItems(int type, string itemcode = "") {
            string msj = string.Empty;
            switch (type)
            {
                case 1:
                    List<ItemData> ListArticulos = new List<ItemData>();
                    if (DtsDao.GetInformacionArticulo(ref ListArticulos, ref msj, type))
                        return Content(HttpStatusCode.OK, ListArticulos);
                    else
                        return Content(HttpStatusCode.OK, msj);
                case 2:
                    List<Propiedad> TabPropiedades = new List<Propiedad>();
                    if (DtsDao.TabsArticulos(ref TabPropiedades, ref msj, itemcode, type))
                    {
                        return Content(HttpStatusCode.OK, TabPropiedades);
                    }
                    else
                        return Content(HttpStatusCode.OK, msj);
                case 3:
                    List<Inventario> TabInventario = new List<Inventario>();
                    if (DtsDao.TabsArticulos(ref TabInventario, ref msj, itemcode, type))
                        return Content(HttpStatusCode.OK, TabInventario);
                    else
                        return Content(HttpStatusCode.OK, msj);
                default:
                    return Content(HttpStatusCode.NotFound, "Opción desconocida");
            }
        }
        
        [HttpPost]
        [Route("CreateItems")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CreateItems() {
            return Ok();
        }
        
        [HttpPatch]
        [Route("UpdateItem")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateItem() {
            return Ok();
        }

        [HttpGet]
        [Route("GetDestinos")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetDestinos() {
            List<Country> ListPaises = new List<Country>();
            List<State> ListEstados = new List<State>();
            string msj = string.Empty;
            if (DtsDao.GetDestinos(ref ListPaises, ref ListEstados, ref msj))
                return Content(HttpStatusCode.OK, new { country = ListPaises, state = ListEstados });
            else
                return Content(HttpStatusCode.OK, msj);
        }
    }
}
