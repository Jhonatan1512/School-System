using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class AsignacionDocenteDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public int CursoId { get; set; }
        public int SeccionId { get; set; }
        public int GradoId { get; set; } 
        public int PeriodoAcademicoId { get; set; }
        public int HorasAsignadas { get; set; }

    } 
    public class AsignacionDocenteCreateDto  
    {
        public int DocenteId { get; set; }
        public List<CursoAsignadoDto> CursosIds { get; set; } = [];
        public int GradoId { get; set; }
        public int SeccionId { get; set; }
        public int PeriodoAcademicoId { get; set; }
    }

    public class CursoAsignadoDto
    {
        public int CursoId { get; set; }
        public int HorasAsignadas { get; set; } 
    }

    public class GetAsignación
    {
        public int Id { get; set; }
        public string NombreDocente { get; set; } = string.Empty;
        public string Dni {  get; set; } = string.Empty ;
        public string NombreCurso { get; set; } = string.Empty;
        public string NombreAula { get; set; } = string.Empty;
        public int HorasAsignadas { get; set; }
        public string NombrePeriodo {  get; set; } = string.Empty;
        public string Estado {  get; set; } = string.Empty;
    }
}
