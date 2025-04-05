using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using PesajeCamiones.Models;



namespace PesajeCamiones.Clases
{
    public class FotoPesaje
    {
      
            private string cadena = WebConfigurationManager.ConnectionStrings["ConexionBD"].ConnectionString;

            public string GrabarImagenPesaje(int idPesaje, List<string> imagenes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(cadena))
                    {
                        con.Open();
                        foreach (string nombre in imagenes)
                        {
                            string sql = "INSERT INTO FotoPesaje (idPesaje, ImagenVehiculo) VALUES (@idPesaje, @nombreArchivo)";
                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@idPesaje", idPesaje);
                                cmd.Parameters.AddWithValue("@nombreArchivo", nombre);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    return "Se grabaron " + imagenes.Count + " imágenes.";
                }
                catch (Exception ex)
                {
                    return "Error al grabar imágenes: " + ex.Message;
                }
            }

        public string ActualizarImagenPesaje(string nombreAntiguo, string nombreNuevo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cadena))
                {
                    con.Open();
                    string sql = "UPDATE FotoPesaje SET ImagenVehiculo = @nuevoNombre WHERE ImagenVehiculo = @nombreAntiguo";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@nuevoNombre", nombreNuevo);
                        cmd.Parameters.AddWithValue("@nombreAntiguo", nombreAntiguo);
                        int filas = cmd.ExecuteNonQuery();
                        return filas > 0 ? "Imagen actualizada en la base de datos." : "No se encontró la imagen para actualizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error al actualizar la imagen: " + ex.Message;
            }
        }

        public string EliminarImagenBD(string nombreArchivo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cadena))
                {
                    con.Open();
                    string sql = "DELETE FROM FotoPesaje WHERE ImagenVehiculo = @nombreArchivo";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@nombreArchivo", nombreArchivo);
                        int filas = cmd.ExecuteNonQuery();
                        return "Se eliminaron " + filas + " registros de la base de datos.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error al eliminar imagen de la base de datos: " + ex.Message;
            }
        }


    }
    }



