using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PesajeCamiones.Clases
{
    public class clsUpload
    {

        
        public string Datos { get; set; } 
        public string Proceso { get; set; } 
        public HttpRequestMessage request { get; set; }
        private List<string> Archivos;

        public async Task<HttpResponseMessage> GrabarArchivo(bool Actualizar)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                return request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "No se envió un archivo válido.");
            }

            string root = HttpContext.Current.Server.MapPath("~/Archivos");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await request.Content.ReadAsMultipartAsync(provider);
                if (provider.FileData.Count > 0)
                {
                    Archivos = new List<string>();

                    foreach (MultipartFileData file in provider.FileData)
                    {
                        string fileName = file.Headers.ContentDisposition.FileName.Trim('"');
                        fileName = Path.GetFileName(fileName);

                        string fullPath = Path.Combine(root, fileName);

                        if (File.Exists(fullPath))
                        {
                            string nombreAReemplazar = Datos; 

                            string fullPaths = Path.Combine(root, nombreAReemplazar);

                            if (File.Exists(fullPaths))
                            {
                                File.Delete(fullPaths);
                                File.Move(file.LocalFileName, fullPaths);
                                return request.CreateResponse(System.Net.HttpStatusCode.OK, "Se actualizó la imagen.");
                            }
                            else
                            {
                                File.Delete(file.LocalFileName); 
                                return request.CreateErrorResponse(System.Net.HttpStatusCode.NotFound, "El archivo que se desea actualizar no existe.");
                            }
                        }
                        else
                        {
                            if (!Actualizar)
                            {
                                Archivos.Add(fileName);
                                File.Move(file.LocalFileName, fullPath);
                            }
                            else
                            {
                                return request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "El archivo no existe, no se puede actualizar.");
                            }
                        }
                    }

                    string respuesta = ProcesarBD();
                    return request.CreateResponse(System.Net.HttpStatusCode.OK, "Se cargaron los archivos. " + respuesta);
                }

                return request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "No se enviaron archivos.");
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public HttpResponseMessage DescargarArchivo(string nombreArchivo)
        {
            try
            {
                string ruta = HttpContext.Current.Server.MapPath("~/Archivos");
                string archivo = Path.Combine(ruta, nombreArchivo);

                if (File.Exists(archivo))
                {
                    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    var stream = new FileStream(archivo, FileMode.Open);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = nombreArchivo
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    return response;
                }
                else
                {
                    return request.CreateErrorResponse(System.Net.HttpStatusCode.NotFound, "Archivo no encontrado.");
                }
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private string ProcesarBD()
        {
            switch (Proceso.ToUpper())
            {
                case "PESAJE":
                case "FOTOPESAJE":
                    FotoPesaje servicio = new FotoPesaje();
                    return servicio.GrabarImagenPesaje(Convert.ToInt32(Datos), Archivos);
                default:
                    return "Proceso no definido en la base de datos.";
            }
        }

        public HttpResponseMessage EliminarArchivo(string nombreArchivo)
        {
            try
            {
                string ruta = HttpContext.Current.Server.MapPath("~/Archivos");
                string archivo = Path.Combine(ruta, nombreArchivo);

               
                if (File.Exists(archivo))
                {
                    File.Delete(archivo);
                }

             
                FotoPesaje servicio = new FotoPesaje();
                string resultadoBD = servicio.EliminarImagenBD(nombreArchivo);

                return request.CreateResponse(System.Net.HttpStatusCode.OK, "Imagen eliminada. " + resultadoBD);
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
