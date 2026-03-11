using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class GetMatriculadosSeccionDto
    {
        public int MatriculaId { get; set; }
        public int AlumnoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni {  get; set; } = string.Empty ;
        public string Correo {  get; set; } = string.Empty ;
        public string Aula {  get; set; } = string.Empty ;
        public string Estado {  get; set; } = string.Empty;
    }
}
