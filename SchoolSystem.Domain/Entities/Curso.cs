using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int GradoId { get; set; }
        public Grado? Grado { get; set; }
         
        public PrioridadCurso Prioridad { get; set; }
        public ICollection<Competencia> Competencias { get; set; } = new List<Competencia>();  
        public ICollection<PlanEstudio> PlanEstudios { get; set;} = new List<PlanEstudio>();
    }
    
    public enum PrioridadCurso
    {
        Alta = 1,   
        Media = 2,  
        Baja = 3    
    }
}
 