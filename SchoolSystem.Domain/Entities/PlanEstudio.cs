using SchoolSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class PlanEstudio
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
        public TipoJornada Jornada { get; set; }
        public int HorasSemanales { get; set; } 
        public int HorasMaximasPorDia { get; set; }
        public int DuracionBloque { get; set; } 
        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }
        public ICollection<AsignacionDocente> Asignaciones { get; set; } = [];
    }
}
