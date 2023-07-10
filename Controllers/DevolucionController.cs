using Biblioteca.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteca.Controllers
{
    public class DevolucionController : Controller
    {
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        private static List<Libro> librosPrestados = new List<Libro>();
        // GET: Devolucion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscarLibrosPrestado(FormCollection form)
        {
            string cedula = form["Cedula"];
            List<Libro> librosPrestados = new List<Libro>();

            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                string sentencia = "SELECT L.ISBN, L.Titulo, L.CasaEditorial, L.NumeroEdicion, L.NombreAutor, L.CopiasDisponibles, P.FechaDevolucion " +
                   "FROM Libro l JOIN Prestamo p ON l.ISBN = p.LibroId JOIN Persona pe ON p.PersonaId = pe.Cedula " +
                   "WHERE pe.Cedula = @Cedula AND p.FechaEntrega IS NULL";

                SqlCommand cmd = new SqlCommand(sentencia, oconexion);
                cmd.Parameters.AddWithValue("@Cedula", cedula);

                oconexion.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Libro libro = new Libro
                    {
                        ISBN = reader["ISBN"].ToString(),
                        Titulo = reader["Titulo"].ToString(),
                        CasaEditorial = reader["CasaEditorial"].ToString(),
                        NumeroEdicion = Convert.ToInt32(reader["NumeroEdicion"]),
                        NombreAutor = reader["NombreAutor"].ToString(),
                        CantidadCopiasDisponibles = Convert.ToInt32(reader["CopiasDisponibles"]),
                        FechaDevolucion = Convert.ToDateTime(reader["FechaDevolucion"])
                    };

                    librosPrestados.Add(libro);
                }

                oconexion.Close();
            }
            ViewBag.Cedula = cedula;

            // Validar retraso en la entrega
            foreach (var libro in librosPrestados)
            {
                if (libro.FechaDevolucion < DateTime.Today)
                {
                    ViewBag.Advertencia = "¡Atención! Hay una entrega tardía para el libro " + libro.Titulo;
                    break; // Si hay un retraso, no es necesario verificar los demás libros
                }
            }

            return View("~/Views/Home/Devolucion.cshtml", librosPrestados);
        }

        [HttpPost]
        public ActionResult ProcesarDevolucion(List<string> librosDevolver, string cedula)
        {
            foreach (string isbn in librosDevolver)
            {
                // Actualizar la cantidad de copias disponibles para el libro devuelto
                using (SqlConnection oconexion = new SqlConnection(conexion))
                {
                    string sentencia = "UPDATE Libro SET CopiasDisponibles = CopiasDisponibles + 1 WHERE ISBN = @ISBN";
                    SqlCommand cmd = new SqlCommand(sentencia, oconexion);
                    cmd.Parameters.AddWithValue("@ISBN", isbn);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    oconexion.Close();
                }

                // Registrar la fecha de devolución para el libro devuelto
                using (SqlConnection oconexion = new SqlConnection(conexion))
                {
                    string sentencia = "UPDATE Prestamo SET FechaEntrega = @FechaEntrega WHERE LibroId = @ISBN AND PersonaId = @Cedula";
                    SqlCommand cmd = new SqlCommand(sentencia, oconexion);
                    cmd.Parameters.AddWithValue("@FechaEntrega", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ISBN", isbn);
                    cmd.Parameters.AddWithValue("@Cedula", cedula);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    oconexion.Close();
                }
            }

            return RedirectToAction("Index", "Home"); // Redirige a la página principal después de procesar la devolución
        }



    }
}