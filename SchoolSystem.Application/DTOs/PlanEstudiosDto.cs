using SchoolSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class CrearPlanEstudioDto
    {
        public List<int> CursosId { get; set; } = [];
        public TipoJornada Jornada { get; set; }
        public int HorasSemanales { get; set; }
        public int HorasMaximasPorDia { get; set; }
        public int DuracionBloque { get; set; }
    }

    public class ActualizarPlanEstudioDto
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public TipoJornada Jornada { get; set; }
        public int HorasSemanales { get; set; }
        public int HorasMaximasPorDia { get; set; }
        public int DuracionBloque { get; set; } 
    }
    public class PlanEstudiosDto
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public string NombreCurso { get; set; } = string.Empty;
        public TipoJornada Jornada { get; set; }
        public string NombreJornada => Jornada.ToString();
        public int HorasSemanales { get; set; }
        public int HorasMaximasPorDia { get; set; }
        public int DuracionBloque { get; set; }
        public string NombrePeriodo {  get; set; } = string.Empty;
        public int PeriodoId { get; set; }
    }
}
