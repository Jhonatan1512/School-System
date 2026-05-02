using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class ConfiguracionGradoSecionDto
    {
        public int GradoId { get; set; }
        public int SeccionId { get; set; }
        public int PeriodoacademicoId { get; set; }
        public int CapacidadMax { get; set; } 
    }

    public class ConfigracionUpdateDto
    {
        public int Id { get; set; }
        public int CapacidadMax { get; set; }
    }

    public class ConfiguracionDetalleDto 
    {
        public int id { get; set; }
        public string NombreGrado { get; set; } = string.Empty;
        public int GradoId { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int SeccionId { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
        public int PeriodoId { get; set; } 
        public int Capacidad { get; set; }
        public int VacantesOcupadas { get; set; }
        public int CuposDisponible => Capacidad - VacantesOcupadas;
    }
}
