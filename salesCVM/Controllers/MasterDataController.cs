using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using salesCVM.Models;
using salesCVM.DAO.DAO;
using System.Web.Http.Cors;
using System.Security.Permissions;
using Microsoft.Ajax.Utilities;
using System.IO;

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

                    if (DtsDao.GetDatosMaestros(ref ListBp, type, "", ""))
                        return Content(HttpStatusCode.OK, ListBp);
                    else
                        return Content(HttpStatusCode.NoContent, "No se recuperarón socios de negocios");
                case 4:
                    List<Item> ListItem = new List<Item>();
                    if (DtsDao.GetDatosMaestros(ref ListItem, type, whsCode, priceList))
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
                case 5:
                    List<Contacto> ListaContactos = new List<Contacto>();
                    if (DtsDao.GetInformacionSocios(ref ListaContactos, ref msj, type, cardcode))
                        return Content(HttpStatusCode.OK, ListaContactos);
                    else
                        return Content(HttpStatusCode.OK, ListaContactos);
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
        public IHttpActionResult CreateItems([FromBody] ItemSAP documento, string usuario) {
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
            
            bool existe = false;
            if (DtsDao.Existe(ref existe, document.Header.LicTradNum, "1"))
            {
                if (!existe) //No existe RFC
                {
                    if (DtsDao.CrearSocioSAP(ref msj, document, usuario))
                        return Content(HttpStatusCode.OK, msj.Code);
                    else
                        return Content(HttpStatusCode.BadRequest, msj.Mensaje);
                }
                else { //Existe RFC
                    return Content(HttpStatusCode.BadRequest, $"El RFC {document.Header.LicTradNum} ya existe");
                }
            } else
                return Content(HttpStatusCode.BadRequest, "Se generó un error al verificar que el dato exista");
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

        [HttpGet]
        [Route("GetDocumentNumbering")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetDocumentNumbering(string type, string subtype) {
            string msj = string.Empty;
            DocumentNumbering docNumb = new DocumentNumbering();
            if (DtsDao.GetDocumentsNumbering(ref docNumb, ref msj, type, subtype))
                return Content(HttpStatusCode.OK, docNumb);
            else
                return Content(HttpStatusCode.BadRequest, string.IsNullOrEmpty(msj) ? "Error al recuperar la serie del documento" : msj);
        }

        [HttpGet]
        [Route("GetExiste")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetExiste(string type, string value) {
            bool existe = false;
            if(DtsDao.Existe(ref existe, value, type))
                return Content(HttpStatusCode.OK, existe);
            else
                return Content(HttpStatusCode.BadRequest, "Se generó un error al verificar que el dato exista");
        }

        [HttpGet]
        [Route("GetPersonaContacto")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetPersonaContacto(string cardcode, int type)
        {
            List<ContactPerson> ListContactos = new List<ContactPerson>();
            List<DireccionEntrega> ListDirEntrega = new List<DireccionEntrega>();
            string msj = string.Empty;
            if (DtsDao.GetContactos(ref ListContactos, ref ListDirEntrega, ref msj, type, cardcode))
                return Content(HttpStatusCode.OK, new { contactos = ListContactos , direcciones = ListDirEntrega});
            else
                return Content(HttpStatusCode.NotFound, string.IsNullOrEmpty(msj) ? "No se recuperaron contactos" : msj);
        }
        [HttpGet]
        [Route("GetImage")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetImage(string PictureName) {
            if(string.IsNullOrEmpty(PictureName))
                return Content(HttpStatusCode.OK, "");

            string image64 = "";
            if (DtsDao.GetImageFromFolder(PictureName, ref image64))
                return Content(HttpStatusCode.OK, image64);
            else
                return Content(HttpStatusCode.OK, "");
        }
        [HttpGet]
        [Route("GetPagos")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetPagos(int accion, string cardcode) {
            List<FormaPago> ListFPagos = new List<FormaPago>();
            List<MetodoPago> ListMPagos = new List<MetodoPago>();
            string msj = string.Empty;
            if (DtsDao.GetPagos(ref ListFPagos, ref ListMPagos, ref msj, accion, cardcode))
                return Content(HttpStatusCode.OK, new { FormaPago = ListFPagos, MetodoPago = ListMPagos });
            else
                return Content(HttpStatusCode.NotFound, string.IsNullOrEmpty(msj) ? "No se recuperaron pagos" : msj);
        }

    }
}
