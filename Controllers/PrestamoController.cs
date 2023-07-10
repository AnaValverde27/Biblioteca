using Biblioteca.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteca.Controllers
{
    public class PrestamoController : Controller
    {
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        private static List<Socio> listPersona = new List<Socio>();
        // GET: Prestamo
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult RegistrarPrestamo(Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                string cedula = prestamo.Cedula.ToString();
                string ISBN = prestamo.ISBN.ToString();

                // Validar fecha de devolución
                DateTime fechaDevolucion = prestamo.FechaDevolucion;
                DateTime fechaMaxima = DateTime.Today.AddMonths(1);
                if (fechaDevolucion > fechaMaxima)
                {
                    ModelState.AddModelError("FechaDevolucion", "La fecha de devolución no puede ser mayor a un mes");
                    return View("~/Views/Home/Prestamo.cshtml", prestamo); // Mostrar la vista del formulario nuevamente con el mensaje de error
                }

                // Validar si la persona existe
                bool personaExiste = VerificarPersonaExistente(cedula);

                if (!personaExiste)
                {
                    ModelState.AddModelError("Cedula", "La persona no existe");
                    return View("~/Views/Home/Prestamo.cshtml", prestamo); // Mostrar la vista del formulario nuevamente con el mensaje de error
                }

                // Validar si el libro existe
                bool libroExiste = VerificarLibroExistente(ISBN);

                if (!libroExiste)
                {
                    ModelState.AddModelError("ISBN", "El libro no existe");
                    return View("~/Views/Home/Prestamo.cshtml", prestamo); // Mostrar la vista del formulario nuevamente con el mensaje de error
                }

                // Obtener la cantidad disponible de copias del libro
                int copiasDisponibles = ObtenerCantidadCopiasDisponibles(ISBN);
                if (copiasDisponibles <= 0)
                {
                    ModelState.AddModelError("ISBN", "No hay copias disponibles de este libro");
                    return View("~/Views/Home/Prestamo.cshtml", prestamo); // Mostrar la vista del formulario nuevamente con el mensaje de error
                }

                // Validar cantidad máxima de libros por persona
                int librosEnPoder = ObtenerCantidadLibrosEnPoder(cedula);
                if (librosEnPoder >= 3)
                {
                    // Guardar el mensaje de error en TempData
                    ViewBag.ErrorMessage = "La persona ya tiene 3 libros en su poder";

                    // Redirigir a la vista del formulario nuevamente
                    return View("~/Views/Home/Prestamo.cshtml");
                }

                using (SqlConnection oconexion = new SqlConnection(conexion))
                {
                    DateTime fechaHoy = DateTime.Today;
                    string sentencia;
                    sentencia = " Insert Into Prestamo (FechaPrestamo, FechaDevolucion, PersonaId, LibroId)" +
                               " Values (@FechaPrestamo, @FechaDevolucion, @PersonaId, @LibroId)";
                    SqlCommand cmd = new SqlCommand(sentencia, oconexion);

                    cmd.CommandText = sentencia;
                    cmd.Parameters.AddWithValue("@FechaPrestamo", fechaHoy);
                    cmd.Parameters.AddWithValue("@FechaDevolucion", prestamo.FechaDevolucion);
                    cmd.Parameters.AddWithValue("@PersonaId", cedula);
                    cmd.Parameters.AddWithValue("@LibroId", ISBN);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    // Restar 1 al campo CopiasDisponibles del libro
                    string restarCopiasSentencia = "UPDATE Libro SET CopiasDisponibles = CopiasDisponibles - 1 WHERE ISBN = @ISBN";
                    SqlCommand restarCopiasCmd = new SqlCommand(restarCopiasSentencia, oconexion);
                    restarCopiasCmd.Parameters.AddWithValue("@ISBN", ISBN);
                    restarCopiasCmd.ExecuteNonQuery();

                    oconexion.Close();
                }

                return RedirectToAction("Index", "Home");

            }
            return View("~/Views/Home/Prestamo.cshtml", prestamo);
        }

        private bool VerificarPersonaExistente(string cedula)
        {
            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                string sentencia = "SELECT COUNT(*) FROM Persona WHERE Cedula = @Cedula";
                SqlCommand cmd = new SqlCommand(sentencia, oconexion);
                cmd.Parameters.AddWithValue("@Cedula", cedula);

                oconexion.Open();

                int count = (int)cmd.ExecuteScalar();

                oconexion.Close();

                return count > 0; // Retorna true si se encuentra al menos un registro con la cédula proporcionada
            }
        }

        private bool VerificarLibroExistente(string ISBN)
        {
            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                string sentencia = "SELECT COUNT(*) FROM Libro WHERE ISBN = @ISBN";
                SqlCommand cmd = new SqlCommand(sentencia, oconexion);
                cmd.Parameters.AddWithValue("@ISBN", ISBN);

                oconexion.Open();

                int count = (int)cmd.ExecuteScalar();

                oconexion.Close();

                return count > 0; // Retorna true si se encuentra al menos un registro con la cédula proporcionada
            }
        }

        private int ObtenerCantidadLibrosEnPoder(string cedula)
        {
            int cantidadLibros = 0;

            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                string sentencia = "SELECT COUNT(*) FROM Prestamo WHERE PersonaId = @PersonaId AND FechaEntrega IS NULL";
                SqlCommand cmd = new SqlCommand(sentencia, oconexion);
                cmd.Parameters.AddWithValue("@PersonaId", cedula);
                oconexion.Open();

                // Ejecutar el comando y obtener el resultado
                cantidadLibros = (int)cmd.ExecuteScalar();

                oconexion.Close();
            }

            return cantidadLibros;
        }

        public int ObtenerCantidadCopiasDisponibles(string ISBN)
        {
            int cantidadCopias = 0;

            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                string sentencia = "SELECT CopiasDisponibles FROM Libro WHERE ISBN = @ISBN";
                SqlCommand cmd = new SqlCommand(sentencia, oconexion);
                cmd.Parameters.AddWithValue("@ISBN", ISBN);

                oconexion.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cantidadCopias = Convert.ToInt32(reader["CopiasDisponibles"]);
                }

                reader.Close();
                oconexion.Close();
            }

            return cantidadCopias;
        }


    }
}