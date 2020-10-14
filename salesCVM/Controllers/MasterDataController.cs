using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using salesCVM.Models;
using salesCVM.DAO.DAO;
using System.Web.Http.Cors;

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
                case 4:
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
        public IHttpActionResult CreateItems([FromBody]ItemSAP documento, string usuario) {
            MensajesObj msj = new MensajesObj();
            if (string.IsNullOrEmpty(documento.Header.ItemCode))
                return Content(HttpStatusCode.BadRequest, "Debe ingresar datos para el artículo");

            if (DtsDao.CrearArticuloSAP(ref msj, documento, usuario))
                return Content(HttpStatusCode.OK, msj.Code);
            else
                return Content(HttpStatusCode.BadRequest, msj.Mensaje);
        }
        
        [HttpPatch]
        [Route("UpdateItem")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateItem([FromBody] ItemSAP documento, string usuario) {
            MensajesObj msj = new MensajesObj();
            if (string.IsNullOrEmpty(documento.Header.ItemCode))
                return Content(HttpStatusCode.BadRequest, "Debe especcificar el artículo que quiere actualizar");

            if (DtsDao.UpdateArticuloSAP(ref msj, documento, usuario))
                return Content(HttpStatusCode.OK, msj.Code);
            else
                return Content(HttpStatusCode.BadRequest, msj.Mensaje);
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

        [HttpPost]
        [Route("CreateBusnessPartner")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CreateBusnessPartner([FromBody] BP document, string usuario)
        {
            MensajesObj msj = new MensajesObj();
            if (string.IsNullOrEmpty(document.Header.CardCode))
                return Content(HttpStatusCode.BadRequest, "Debe ingresar datos para el socio de negocios");

            if (DtsDao.CrearSocioSAP(ref msj, document, usuario))
                return Content(HttpStatusCode.OK, msj.Code);
            else
                return Content(HttpStatusCode.BadRequest, msj.Mensaje);

        }

        [HttpPatch]
        [Route("UpdateBusnessPartner")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateBusnessPartner([FromBody] BP document, string usuario)
        {
            if (string.IsNullOrEmpty(document.Header.CardCode))
                return Content(HttpStatusCode.BadRequest, "Debe especificar el socio de negocios que quiere actualizar");

            MensajesObj msj = new MensajesObj();
            if (DtsDao.UpdateSocioSAP(ref msj, document, usuario))
                return Content(HttpStatusCode.OK, msj.Code);
            else
                return Content(HttpStatusCode.BadRequest, msj.Mensaje);
        }

        [HttpGet]
        [Route("GetPrecios")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetPrecios(int type, string itemcode = "", int listnum = 0) {
            string msj = string.Empty;
            switch (type)
            {
                case 1:
                    List<PriceList> ListaPrecios = new List<PriceList>();
                    if (DtsDao.GetPrecios(ref ListaPrecios, ref msj, type, itemcode, listnum))
                        return Content(HttpStatusCode.OK, ListaPrecios);
                    else
                        return Content(HttpStatusCode.NotFound, "No se recuperaron listas de precios");
                case 2:
                    List<PrecioArticulo> ListaPreArt = new List<PrecioArticulo>();
                    if (DtsDao.GetPrecios(ref ListaPreArt, ref msj, type, itemcode, listnum))
                        return Content(HttpStatusCode.OK, ListaPreArt);
                    else
                        return Content(HttpStatusCode.NotFound, "No se recuperaró precio para el artículo");
                default:
                    return Content(HttpStatusCode.NotFound, "Tipo desconocido");
            }
        }

        [HttpGet]
        [Route("GetOpciones")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetOpciones(int type) {
            List<UoM> UnidadMedidoa = new List<UoM>();
            List<GrupoArticulos> GrpArt = new List<GrupoArticulos>();
            string msj = string.Empty;
            if (DtsDao.GetOpciones(ref UnidadMedidoa, ref GrpArt, ref msj, type))
                return Content(HttpStatusCode.OK, new { Grupo = GrpArt, Unidad = UnidadMedidoa });
            else
                return Content(HttpStatusCode.NotFound, string.IsNullOrEmpty(msj) ? "No se recuperaron opciones" : msj);
        }

        [HttpGet]
        [Route("GetEmpleadosVentas")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetEmpleadosVentas(int type) {
            List<Vendedor> ListVendedor = new List<Vendedor>();
            string msj = string.Empty;
            if (DtsDao.GetEmpleadosVts(ref ListVendedor, ref msj, type))
                return Content(HttpStatusCode.OK, ListVendedor);
            else
                return Content(HttpStatusCode.NotFound, string.IsNullOrEmpty(msj) ? "No se recuperaron vendedores" : msj);
        }
    }
}
