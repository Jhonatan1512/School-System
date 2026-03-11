using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class DetalleMatricula
    {
        public int Id { get; set; }
        public int MatriculaId { get; set; }
        public Matricula? Matricula { get; set; }
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>(); 
    }
}
 