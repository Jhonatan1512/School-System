using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class Matricula
    { 
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public Alumno? Alumno { get; set; }
        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }
        public int GradoId { get; set; }
        public Grado? Grado { get; set; }
        public int SeccionId {  get; set; }
        public Seccion? Seccion { get; set; }
        public ICollection<DetalleMatricula> DetallesMatriculas { get; set; } = new List<DetalleMatricula>();
    }
}
