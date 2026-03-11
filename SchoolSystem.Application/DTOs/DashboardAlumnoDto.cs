using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class DashboardAlumnoDto
    {
        public int cursoId {  get; set; }
        public string NombreCurso { get; set; } = string.Empty;
        public string NombreDocente { get; set; } = string.Empty;
        public List<Competencia> Competencias { get; set; } = new List<Competencia>();
    }
    public class CompetenciasDto
    {
        public int Id { get; set; }
        public string Nombre {  get; set; } = string.Empty;
    }
}
