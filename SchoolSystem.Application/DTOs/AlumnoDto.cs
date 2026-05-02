using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class AlumnoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Aula {  get; set; } = string.Empty;
        public int GradoId { get; set; }
        public int SeccionId { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Estado {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; 
        public string PeriodoAcademico {  get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;  
        public int MatriculaId { get; set; }
    }
    public class ActualizarEstadoDto
    {
        public int Id { get; set; }
        public string Estado { get; set; } = string.Empty ;
    }
}
