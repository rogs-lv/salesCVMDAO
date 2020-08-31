using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using salesCVM.Models;
using salesCVM.Token;
using salesCVM.Models;
using salesCVM.DAO.DAO;

namespace salesCVM.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("salesCMV/Acceso")]
    public class AccesoController : ApiController
    {
        LoginDAO dao;
        public AccesoController() {
            dao = new LoginDAO();
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
    }
}
