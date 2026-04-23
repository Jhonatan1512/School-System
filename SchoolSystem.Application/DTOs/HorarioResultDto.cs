using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class HorarioResultDto
    {
        public bool Exito {  get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int HorasAsignadas { get; set; }
        public List<string> Advertencias { get; set; } = [];
    }

    public class HorarioSeccionDto
    {
        public int HoraLectivaId { get; set; }
        public string Bloque { get; set; } = string.Empty; 
        public string RangoHora { get; set; } = string.Empty;
        public bool EsProductiva { get; set; }
        public string GradoSeccion { get; set; } = string.Empty;
        public string Lunes { get; set; } = "-";
        public string Martes { get; set; } = "-";
        public string Miercoles { get; set; } = "-";
        public string Jueves { get; set; } = "-";
        public string Viernes { get; set; } = "-";
    }
}
