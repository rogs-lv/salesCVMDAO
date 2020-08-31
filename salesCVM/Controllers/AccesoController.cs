using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using salesCVM.Models;
using salesCVM.Token;

namespace salesCVM.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("salesCMV/Acceso")]
    public class AccesoController : ApiController
    {
        [HttpPost]
        [Route("authenticate")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult Authenticate(UserLogin usuario)
        {
            if (usuario == null) 
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            //execute query to DB
            bool isCredentialValid = (usuario.Password == "12345" && usuario.IdUser == "asdf");

            //Si el query confirm exist user
            if (isCredentialValid)
            {
                User user = new User(usuario.IdUser, usuario.Password);
                string token = TokenGenerator.GenerateTokenJwt(user);
                return Ok(token);
            }
            else 
                return Unauthorized();
        }
    }
}
