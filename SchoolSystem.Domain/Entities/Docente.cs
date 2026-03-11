using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class Docente
    {
        public int Id { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni {  get; set; } = string.Empty;
        public bool EsActivo { get; set; } = true;
        public string UsuarioId { get; set; } 
        public ICollection<AsignacionDocente> Asignaciones { get; set; } = new List<AsignacionDocente>(); 

    }
}
