using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class Horario
    {
        public int Id { get; set; }
        public int AsignacionDocenteId {  get; set; }
        public AsignacionDocente? AsignacionDocente { get; set; }
        public string DiaSemana { get; set; } = string.Empty;
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin {  get; set; }
    }
}
