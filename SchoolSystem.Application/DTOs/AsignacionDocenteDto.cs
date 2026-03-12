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

    }
    public class AsignacionDocenteCreateDto 
    {
        public int DocenteId { get; set; }
        public int CursoId { get; set; }
        public int GradoId { get; set; }
        public int SeccionId { get; set; }
        public int PeriodoAcademicoId { get; set; }
    }   
}
