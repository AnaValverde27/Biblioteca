using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biblioteca.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo ISBN es requerido")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "El campo Título es requerido")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El campo Casa Editorial es requerido")]
        [Display(Name = "Casa Editorial")]
        public string CasaEditorial { get; set; }

        [Required(ErrorMessage = "El campo Número de Edición es requerido")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo Número de Edición solo debe contener números")]
        [Display(Name = "Número de Edición")]
        public int NumeroEdicion { get; set; }

        [Required(ErrorMessage = "El campo Nombre de Autor es requerido")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El campo Nombre de Autor solo debe contener letras y espacios")]
        [Display(Name = "Nombre de Autor")]
        public string NombreAutor { get; set; }

        [Required(ErrorMessage = "El campo Cantidad de Copias Disponibles es requerido")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo Cantidad de Copias Disponibles solo debe contener números")]
        [Display(Name = "Cantidad de Copias Disponibles")]
        public int CantidadCopiasDisponibles { get; set; }

        public DateTime FechaDevolucion { get; set; }
    }

}