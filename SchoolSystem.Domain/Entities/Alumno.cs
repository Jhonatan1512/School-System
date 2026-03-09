using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Domain.Entities
{
    public class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni {  get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public AlumnoEnum Estado { get; set; } = AlumnoEnum.Activo;
        public string UsuarioId { get; set; }
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    }
}
