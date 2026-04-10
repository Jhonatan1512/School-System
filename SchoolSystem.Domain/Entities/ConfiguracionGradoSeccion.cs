using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class ConfiguracionGradoSeccion 
    {
        public int Id { get; set; }
        public int GradoId { get; set; }
        public int SeccionId { get; set; }
        public int PeriodoacademicoId { get; set; }
        public int CapacidadMax { get; set; }
        public Grado? Grado { get; set; }
        public Seccion? Seccion { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }
    }
}
