using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class CursoCompetenciaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int GradoId { get; set; }
        public List<CrearCompetenciaDto> Competencias { get; set; } = new List<CrearCompetenciaDto>();
    }

    public class CrearCompetenciaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
