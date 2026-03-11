using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class PeriodoAcademicoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EstadoActivo { get; set; } = true;
    }
}
