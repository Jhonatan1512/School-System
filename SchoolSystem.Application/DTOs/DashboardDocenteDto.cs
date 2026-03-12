using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class DashboardDocenteDto
    {
        public int CursoId { get; set; }
        public string NombreCurso { get; set; } = string.Empty;
        public string Aula {  get; set; } = string.Empty;
        public string PeriodoTrimestre {  get; set; } = string.Empty ;
        public List<CompetenciaDto> Competencias { get; set; } = new List<CompetenciaDto>();
        public List<AlumnoBasicoDto> AlumnosMatriculados = new List<AlumnoBasicoDto>();
    }

    public class CompetenciaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class AlumnoBasicoDto
    {
        public int AlumnoId { get; set; } 
        public string NombreCompelto { get; set; } = string.Empty;
    }
}
