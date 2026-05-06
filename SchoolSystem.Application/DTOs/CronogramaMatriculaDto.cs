using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class CronogramaMatriculaDto
    {
        public int PeriodoId { get; set; }
        public int GradoId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraCierre { get; set; }
        public bool EstadoActivo { get; set; } = true;
    }

    public class ActualizarCronogramaDto
    {
        public DateTime NuevaFechaHoraInicio { get; set; }
        public DateTime NuevaFechaHoraCierre { get; set; }
    }

    public class GetCronogramaDto
    {
        public int Id { get; set; }
        public int PeriodoId { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        public int GradoId { get; set; }
        public string NombreGrado { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraCierre { get; set; }
        public bool EstadoActivo { get; set; }
    }
}
 