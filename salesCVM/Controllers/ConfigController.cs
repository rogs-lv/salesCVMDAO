using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using salesCVM.DAO.DAO;
using salesCVM.Models;


namespace salesCVM.Controllers
{
    [Authorize]
    [RoutePrefix("salesCVM/Config")]
    public class ConfigController : ApiController
    {
        ConfiguracionDAO    configDao;
        SAPConnectDAO       sapDao;
        public ConfigController() {
            configDao = new ConfiguracionDAO();
            sapDao = new SAPConnectDAO();
        }

        [HttpGet]
        [Route("GetConfiguration")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetConfigurationUser(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("El usuario es requerido");

            Configuracion.Menu menuOpciones = new Configuracion.Menu();
            Configuracion.Adicional confOpciones = new Configuracion.Adicional();
            if (configDao.ConfiguracionOpciones(ref menuOpciones, ref confOpciones, code))
            {
                if (menuOpciones == null)
                    return Content(HttpStatusCode.NotFound, "No se pudo descargar los modulos para el menu");
                if (confOpciones == null)
                    return Content(HttpStatusCode.NotFound, "No se pudo descargar las configuraciones adicionales");

                return Ok(new { menuOpciones, confOpciones });
            }
            else
                return BadRequest("No se pudo acceder a las configuraciones del portal");
        }

        [HttpPost]
        [Route("SaveConfig")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult SaveConfiguration(int codeConn, SAP dataSAP) {
            string msjSave = string.Empty;

            if (dataSAP == null)
                return BadRequest("Los datos son obligatorios");

            if (sapDao.GuardarConexionSAP(ref msjSave, dataSAP, codeConn))
                return Ok("Conexión guardada");
            else
                return Content(HttpStatusCode.InternalServerError, msjSave);
        }

        [HttpGet]
        [Route("GetConfigConnection")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetConnectionSAP(int codeConn) {
            SAP modelSAP = new SAP();

            if (codeConn <= 0)
                return BadRequest("La conexión que quiere consultar es incorrecta");

            if (sapDao.RecuperarConexionSAP(ref modelSAP, codeConn))
            {
               
                modelSAP.DbPassword = "";
                modelSAP.Password = "";
                return Ok(modelSAP);
            }
            else
                return InternalServerError();
        }
    }
}
