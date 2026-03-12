using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class Trimestre
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EstadoActivo { get; set; } = false;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCierre { get; set;} 
        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }
}
