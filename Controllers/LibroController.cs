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
    public class LibroController : Controller
    {
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        // GET: Libro
        public ActionResult Index()
        {
            return View();
        }

        // POST: Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarLibro(Libro libro)
        {
            if (ModelState.IsValid)
            {
                // Validar los datos ingresados
                using (SqlConnection oconexion = new SqlConnection(conexion))
                {
                    oconexion.Open();

                    // Verificar si el ISBN ya existe en la base de datos
                    string consultaISBN = "SELECT COUNT(*) FROM Libro WHERE ISBN = @ISBN";
                    SqlCommand cmdVerificarISBN = new SqlCommand(consultaISBN, oconexion);
                    cmdVerificarISBN.Parameters.AddWithValue("@ISBN", libro.ISBN);

                    int countISBN = (int)cmdVerificarISBN.ExecuteScalar();

                    if (countISBN > 0)
                    {
                        // El ISBN ya existe, mostrar mensaje de error
                        ModelState.AddModelError("ISBN", "El ISBN ya existe en la base de datos");
                        return View("~/Views/Home/Libro.cshtml", libro);
                    }

                    // Verificar si el nombre del libro y la edición existen en la base de datos
                    string consultaNombreEdicion = "SELECT COUNT(*) FROM Libro WHERE Titulo = @Titulo AND NumeroEdicion = @NumeroEdicion";
                    SqlCommand cmdVerificarNombreEdicion = new SqlCommand(consultaNombreEdicion, oconexion);
                    cmdVerificarNombreEdicion.Parameters.AddWithValue("@Titulo", libro.Titulo);
                    cmdVerificarNombreEdicion.Parameters.AddWithValue("@NumeroEdicion", libro.NumeroEdicion);

                    int countNombreEdicion = (int)cmdVerificarNombreEdicion.ExecuteScalar();

                    if (countNombreEdicion > 0)
                    {
                        // El nombre del libro y la edición ya existen para otro libro, mostrar mensaje de error
                        ModelState.AddModelError("NumeroEdicion", "La edición ya existe para otro libro con el mismo nombre");
                        return View("~/Views/Home/Libro.cshtml", libro);
                    }

                    // El ISBN, el nombre del libro y la edición son válidos, insertar el nuevo libro en la base de datos
                    string sentencia = "INSERT INTO Libro (ISBN, Titulo, CasaEditorial, NumeroEdicion, NombreAutor, CopiasDisponibles)" +
                                       "VALUES (@ISBN, @Titulo, @CasaEditorial, @NumeroEdicion, @NombreAutor, @CopiasDisponibles)";
                    SqlCommand cmdInsertarLibro = new SqlCommand(sentencia, oconexion);
                    cmdInsertarLibro.Parameters.AddWithValue("@ISBN", libro.ISBN);
                    cmdInsertarLibro.Parameters.AddWithValue("@Titulo", libro.Titulo);
                    cmdInsertarLibro.Parameters.AddWithValue("@CasaEditorial", libro.CasaEditorial);
                    cmdInsertarLibro.Parameters.AddWithValue("@NumeroEdicion", libro.NumeroEdicion);
                    cmdInsertarLibro.Parameters.AddWithValue("@NombreAutor", libro.NombreAutor);
                    cmdInsertarLibro.Parameters.AddWithValue("@CopiasDisponibles", libro.CantidadCopiasDisponibles);

                    cmdInsertarLibro.ExecuteNonQuery();

                    oconexion.Close();

                    return RedirectToAction("Index", "Home");
                }
            }

            return View("~/Views/Home/Libro.cshtml", libro);
        }

    }
}