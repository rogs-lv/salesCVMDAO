using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using salesCVM.Models;
using salesCVM.DAO;
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
    }
}
