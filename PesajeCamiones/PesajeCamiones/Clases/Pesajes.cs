using PesajeCamiones.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PesajeCamiones.Clases
{
    public class Pesajes
    {

        private DBExamenEntities1 dbsuper = new DBExamenEntities1();

        public PesajeCamiones.Models.Pesaje pesaje { get; set; }

        public Camion camion { get; set; }

        public clsUpload FotoPesaje { get; set; }

        public Camion ConsultarPlaca(string placa)
        {
            return dbsuper.Camion.FirstOrDefault(c => c.Placa == placa);

        }
        public string IngresarPesaje(DateTime fecha, string placa, float peso, string estacion)
        {
            try
            {
                    pesaje.FechaPesaje = fecha;
                    pesaje.PlacaCamion = placa;
                    pesaje.Peso = peso;
                    pesaje.Estacion = estacion;

                    dbsuper.Pesaje.Add(pesaje);
                    dbsuper.SaveChanges();
                    return "Se ha ingresado el pesaje éxitosamente";                                         
            }
            catch (Exception ex)
            {
                return "El error ocurrido es: " + ex.Message;
            }
        }

        public string IngresarCamion(string placa, string marca, int numeroEjes)
        {
            try
            {
             
                camion.Placa = placa;
                camion.Marca = marca;
                camion.NumeroEjes = numeroEjes;

                dbsuper.Camion.Add(camion);
                dbsuper.SaveChanges();
                return "Se ha ingresado el camion éxitosamente";
            }
            catch (Exception ex)
            {
                return "El error ocurrido es: " + ex.Message;
            }
        }

    }

    
}