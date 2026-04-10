using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class GradoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int TotalSecciones { get; set; } 
        public int TotalAlumnos {  get; set; }
    } 

    public class GradoActualizacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty ;
    }

    public class GradoDetalleDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public List<CursoSimple> Cursos { get; set; } = [];
    }
    public class CursoSimple
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    } 
}
