using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biblioteca.Models
{
    public class Prestamo
    {
        public int Id { get; set; }
        public DateTime FechaPrestamo { get; set; }
        [Required(ErrorMessage = "El campo Fecha de Devolución es requerido")]
        public DateTime FechaDevolucion { get; set; }
        [Required(ErrorMessage = "El campo Cédula es requerido")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo Cédula solo debe contener números")]
        [Display(Name = "Cédula")]
        public int Cedula { get; set; }
        [Required(ErrorMessage = "El campo ISBN es requerido")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo ISBN solo debe contener números")]
        public int ISBN { get; set; }
    }
}