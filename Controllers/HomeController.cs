using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteca.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Socio()
        {
            ViewBag.Message = "Your socio page.";

            return View();
        }

        public ActionResult Libro()
        {
            ViewBag.Message = "Your libro page.";

            return View();
        }
        public ActionResult Prestamo()
        {
            ViewBag.Message = "Your prestamo page.";

            return View();
        }

        public ActionResult Devolucion()
        {
            ViewBag.Message = "Your application devolucion page.";

            return View();
        }
    }
}