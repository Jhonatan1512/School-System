using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class AsignacionDocente
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public Docente? Docente { get; set; }
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
        public int SeccionId { get; set; } 
        public Seccion? Seccion { get; set; }
        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }
        public ICollection<Horario> Horarios { get; set; } = new List<Horario>(); 
    }
}
