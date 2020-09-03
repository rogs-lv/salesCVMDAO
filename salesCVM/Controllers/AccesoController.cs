using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using salesCVM.DAO.DAO;
using salesCVM.Models;
using salesCVM.Token;

namespace salesCVM.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("salesCMV/Acceso")]
    public class AccesoController : ApiController
    {
        LoginDAO dao;
        SAPConnectDAO sapDao;
        public AccesoController() {
            dao = new LoginDAO();
            sapDao = new SAPConnectDAO();
        }
        [HttpPost]
        [Route("authenticate")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult Authenticate(UserLogin usuario)
        {
            if (usuario == null) 
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            User userData = new User();
            if (dao.Login(usuario, ref userData))
            {
                if (userData != null)
                {
                    string toke = TokenGenerator.GenerateTokenJwt(userData);
                    userData.Token = toke;
                    return Ok(userData);
                }
                else
                    return Content(HttpStatusCode.NotFound, "El usuario o contraseña no son validos");
            }
            else
                return Unauthorized();
        }

        [HttpGet]
        [Route("ping")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult Ping() {
            string msjSap = string.Empty;
            bool statusPing = sapDao.PingConexionSAP(msjSap);
            if (statusPing && msjSap.Length == 0)
                return Ok("Conexión a SAP exitosa");
            else
                return Content(HttpStatusCode.ServiceUnavailable, msjSap);
        }
    }
}
