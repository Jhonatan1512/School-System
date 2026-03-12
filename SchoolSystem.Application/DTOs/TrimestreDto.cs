using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class TrimestreDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCierre { get; set;}
        public bool EstadoActivo { get; set; }
        public int PeriodoAcademicoId { get; set; }
    }

    public class TrimestreCreadoDto
    {
        public string Nombre { get; set; } = string.Empty ;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCierre { get; set; }
        public int PeriodoAcademicoId { get; set; }
        public bool EstadoActivo { get; set; }
    }
    public class TrimestreExtensionDto
    {
        public DateTime NuevaFechaCierre { get; set;}
    }
}
