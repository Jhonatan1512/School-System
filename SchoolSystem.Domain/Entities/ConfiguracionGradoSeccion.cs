using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int VacantesOcupadas { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = new byte[0];
        public Grado? Grado { get; set; } 
        public Seccion? Seccion { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }
    }
}
