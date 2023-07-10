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
    public class SocioController : Controller
    {
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        // GET: Socio
        public ActionResult Index()
        {
            return View();
        }

        // Acción para procesar el formulario y registrar un nuevo socio
        [HttpPost]
        public ActionResult RegistrarSocio(Socio socio)
        {
            if (ModelState.IsValid)
            {
                // Validar los datos ingresados
                using (SqlConnection oconexion = new SqlConnection(conexion))
                {
                    DateTime fechaHoy = DateTime.Today;
                    socio.FechaRegistro = fechaHoy;
                    socio.Activo = true;
                    string sentencia;
                    sentencia = " Insert Into Persona (Cedula, Nombre, Apellidos, FechaRegistro, Activo)" +
                               " Values (@Cedula, @Nombre, @Apellidos, @FechaRegistro, @Activo)";
                    SqlCommand cmd = new SqlCommand(sentencia, oconexion);
                    
                    cmd.CommandText = sentencia;
                    cmd.Parameters.AddWithValue("@Cedula", socio.Cedula);
                    cmd.Parameters.AddWithValue("@Nombre", socio.Nombre);
                    cmd.Parameters.AddWithValue("@Apellidos", socio.Apellidos);
                    cmd.Parameters.AddWithValue("@FechaRegistro", socio.FechaRegistro);
                    cmd.Parameters.AddWithValue("@Activo", socio.Activo);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    oconexion.Close();

                    return RedirectToAction("Index", "Home");

                }
            }
            return View("~/Views/Home/Socio.cshtml", socio);
        }
    }
}