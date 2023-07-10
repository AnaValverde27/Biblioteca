using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biblioteca.Models
{
    public class Socio
    {
        [Required(ErrorMessage = "La cédula es requerida")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La cédula solo debe contener números")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "El nombre solo debe contener letras y espacios")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Los apellidos solo deben contener letras y espacios")]
        public string Apellidos { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}