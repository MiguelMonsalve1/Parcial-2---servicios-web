using PesajeCamiones.Clases;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PesajeCamiones.Controllers
{
    [RoutePrefix("api/FotoPesaje")]
    public class FotoPesajeController : ApiController
    {
        [HttpPost]
        [Route("SubirImagen")]
        public async Task<HttpResponseMessage> SubirImagen(HttpRequestMessage request, int idPesaje)
        {
            clsUpload upload = new clsUpload();
            upload.Datos = idPesaje.ToString();
            upload.Proceso = "FOTOPESAJE"; 
            upload.request = request;
            return await upload.GrabarArchivo(false);
        }

        [HttpGet]
        [Route("ConsultarImagen")]
        public HttpResponseMessage ConsultarImagen(string nombreImagen)
        {
            clsUpload upload = new clsUpload();
            return upload.DescargarArchivo(nombreImagen);
        }

        [HttpPut]
        [Route("ActualizarImagen")]
        public async Task<HttpResponseMessage> ActualizarImagen(HttpRequestMessage request, [FromUri] string nombreImagen)
        {
            clsUpload upload = new clsUpload();
            upload.request = request;
            upload.Datos = nombreImagen;
            return await upload.GrabarArchivo(true);
        }


        [HttpDelete]
        [Route("EliminarImagen")]
        public HttpResponseMessage EliminarImagen(string nombreImagen)
        {
            clsUpload upload = new clsUpload();
            upload.request = Request;
            return upload.EliminarArchivo(nombreImagen);
        }
    }
}
