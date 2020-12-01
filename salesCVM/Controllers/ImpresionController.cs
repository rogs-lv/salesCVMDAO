using salesCVM.DAO.DAO;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace salesCVM.Controllers
{
    [Authorize]
    [RoutePrefix("salesCVM/Impresion")]
    public class ImpresionController : ApiController
    {
        private readonly ImpresionDAO impDAO;
        public ImpresionController()
        {
            impDAO = new ImpresionDAO();
        }

        [HttpGet]
        [Route("PrintGenerate")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage PrintGenerate(int idDoc, int type) {
            if (idDoc <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "El número de documento no es valido");
            if (type <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "El tipo de documento no es valido");

            Stream printStream = impDAO.GetFormatPrint(idDoc, "", type);
            if (printStream != null)
            {
                int contentLenght;
                byte[] buff = new byte[2048];
                string LocalDirectory = Path.Combine(Path.GetTempPath(), "Recibo.pdf");
                using (FileStream fs = new FileStream(LocalDirectory, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (printStream)
                    {
                        contentLenght = printStream.Read(buff, 0, 2048);
                        while (contentLenght != 0)
                        {
                            fs.Write(buff, 0, contentLenght);
                            contentLenght = printStream.Read(buff, 0, 2048);
                        }
                    }
                }
                FileStream fileStrm = new FileStream(LocalDirectory, FileMode.Open, FileAccess.Read);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(fileStrm);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                return result;
            }
            else {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error al generar la impresión");
            }
        }
    }
}
