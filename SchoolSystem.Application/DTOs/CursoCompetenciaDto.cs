using SchoolSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class CursoCompetenciaDto  
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreAula {  get; set; } = string.Empty;
        public int GradoId { get; set; }
        public int HorasSemanales { get; set; }
        public int HorasMaximasPorDia { get; set; }
        public int HorasRestantes { get; set; }
        public int DuracionBloque {  get; set; } 
        public int Prioridad { get; set; }
        public List<CrearCompetenciaDto> Competencias { get; set; } = new List<CrearCompetenciaDto>();
    }
     
    public class CrearCursoComptenciasDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int GradoId { get; set; }
        public int Prioridad { get; set; }
        public List<CrearCompetenciaDto> Competencias { get; set; } = new List<CrearCompetenciaDto>();
        public List<CrearPlanBasicoDto> PlanEstudios { get; set; } = [];
    }

    public class CrearPlanBasicoDto
    {
        public TipoJornada Jornada { get; set; }
        public int HorasSemanales { get; set; }
        public int HorasMaximasPorDia { get; set; }
        public int DuracionBloque { get; set; }
    }


    public class CrearCompetenciaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CursoId { get; set; }
    } 

    public class CompetenciasCursoDto
    {
        public int Id { get; set; }
        public string NombreCompetencia {  get; set; } = string.Empty;
        public string NombreCurso {  get; set; } = string.Empty;
        public int CursoId { get; set; }
        public int GradoId { get; set; }
        public string NombreGrado {  get; set; } = string.Empty;
    }
}
