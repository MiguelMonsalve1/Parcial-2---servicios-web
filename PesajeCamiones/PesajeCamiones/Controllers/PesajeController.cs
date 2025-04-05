using PesajeCamiones.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

[RoutePrefix("api/pesajes")]
public class PesajesController : ApiController
{
    private readonly DBExamenEntities1 dbsuper = new DBExamenEntities1();

    [HttpPost]
    [Route("registrar")]
    public IHttpActionResult RegistrarPesaje([FromBody] PesajeRequest request)
    {
        if (request == null)
            return Content(HttpStatusCode.BadRequest, "Datos inválidos");

        try
        {
            using (var transaction = dbsuper.Database.BeginTransaction())
            {

                var camion = dbsuper.Camion.FirstOrDefault(c => c.Placa == request.Placa);
                if (camion == null)
                {
                    camion = new Camion
                    {
                        Placa = request.Placa,
                        Marca = request.Marca,
                        NumeroEjes = request.NumEjes
                    };
                    dbsuper.Camion.Add(camion);
                    dbsuper.SaveChanges();
                }

                var pesaje = new Pesaje
                {
                    PlacaCamion = camion.Placa,
                    FechaPesaje = request.FechaPesaje,
                    Peso = request.Peso,
                    Estacion = request.Estacion
                };

                dbsuper.Pesaje.Add(pesaje);
                dbsuper.SaveChanges();
                transaction.Commit();

                return Content(HttpStatusCode.Created, "Pesaje registrado correctamente.");
            }
        }
        catch (System.Data.Entity.Validation.DbEntityValidationException ex)
        {
            var errores = "";

            foreach (var errorEntidad in ex.EntityValidationErrors)
            {
                foreach (var error in errorEntidad.ValidationErrors)
                {
                    errores += $"Entidad: {errorEntidad.Entry.Entity.GetType().Name}, " +
                               $"Propiedad: {error.PropertyName}, " +
                               $"Error: {error.ErrorMessage}\n";
                }
            }

            return Content(HttpStatusCode.InternalServerError, "Error de validación:\n" + errores);
        }
    }

    [HttpGet]
    [Route("consultar/{placa}")]
    public IHttpActionResult ConsultarPesajes(string placa)
    {
        var resultado = dbsuper.Pesaje
        .Where(p => p.PlacaCamion == placa)
        .Select(p => new
        {
            Placa = p.PlacaCamion,
            Marca = p.Camion.Marca,
            NumeroEjes = p.Camion.NumeroEjes,
            FechaPesaje = p.FechaPesaje,
            Peso = p.Peso,
            Estacion = p.Estacion,
            Imagenes = p.FotoPesaje.Select(f => f.ImagenVehiculo).ToList()
        })
        .ToList();

        if (!resultado.Any())
            return Content(HttpStatusCode.NotFound, "No se encontraron pesajes para este camión.");

        return Ok(resultado);
    }
}


public class PesajeRequest
{
    public string Placa { get; set; }
    public string Marca { get; set; }
    public int NumEjes { get; set; }
    public DateTime FechaPesaje { get; set; }
    public float Peso { get; set; }

    public string Estacion { get; set; }

}