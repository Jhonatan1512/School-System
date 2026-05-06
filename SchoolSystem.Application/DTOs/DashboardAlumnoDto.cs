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
        public int AlumnoId { get; set; }
        public int cursoId { get; set; } 
        public string NombreCurso { get; set; } = string.Empty;
        public List<DocentesCusroDto> Docentes { get; set; } = [];
        public int gradoId { get; set; }
        public int seccionId { get; set; }
        public int matriculaId { get; set; }
        public string NombreAula { get; set; } = string.Empty; 
    }

    public class DocentesCusroDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty ;
    }
    public class CompetenciasNotaDto 
    {
        public int CompetenciaId { get; set; }
        public string NombreCompetencia { get; set; } = string.Empty;
        public int TrimestreId { get; set; }
        public string NombreTrimestre {  get; set; } = string.Empty;
        public string Nota {  get; set; } = string.Empty;
    }

    public class DetalleCursoAlumnoDto 
    {
        public string NombreCurso { get; set; } = string.Empty;
        public string NombreDocente { get; set; } = string.Empty;
        public List<CompetenciasNotaDto> Competencias { get; set; } = new List<CompetenciasNotaDto>();
    }
}
