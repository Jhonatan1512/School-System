using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class HoraLectiva
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TimeSpan HoraInicio { get; set; } 
        public TimeSpan HoraFin { get; set; }    
        public bool EsProductiva { get; set; }
        public int Orden { get; set; }
        public string Turno { get; set; } = "Mañana";
    }
}
