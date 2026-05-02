using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class CronogramaMatricula
    {
        public int Id { get; set; }
        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }
        public int GradoId { get; set; }
        public Grado? Grado { get; set; } 
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public bool EstadoActivo { get; set; } = true;
    }
}
