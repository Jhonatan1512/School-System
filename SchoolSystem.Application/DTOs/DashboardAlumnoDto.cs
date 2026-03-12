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
        public int cursoId { get; set; }
        public string NombreCurso { get; set; } = string.Empty;
        public string NombreDocente { get; set; } = string.Empty;
    }
    public class CompetenciasNotaDto
    {
        public int CompetenciaId { get; set; }
        public string NombreCompetencia { get; set; } = string.Empty;
        public string Nota {  get; set; } = string.Empty;
    }

    public class DetalleCursoAlumnoDto
    {
        public string NombreCurso { get; set; } = string.Empty;
        public string NombreDocente { get; set; } = string.Empty;
        public List<CompetenciasNotaDto> Competencias { get; set; } = new List<CompetenciasNotaDto>();
    }
}
