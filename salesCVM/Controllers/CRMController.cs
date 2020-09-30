using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace salesCVM.Controllers
{
    [Authorize]
    [RoutePrefix("salesCVM/CRM")]
    public class CRMController : ApiController
    {
        public CRMController() { 

        }
    }
}
