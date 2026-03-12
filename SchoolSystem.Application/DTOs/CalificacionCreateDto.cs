using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.DTOs
{
    public class CalificacionCreateDto
    {
        //public int DocenteId { get; set; }
        public string Nota {  get; set; } = string.Empty;
        public int TimestreId { get; set; }
        public int CompetenciaId { get; set; }
        public int DetalleMatriculaId { get; set; }
    }
    public class CalificacionDto
    {
        public int Id { get; set; }
        public string Nota { get; set; } = string.Empty;
        public int TimestreId { get; set; }
        public int CompetenciaId { get; set; }
        public int DetalleMatriculaId { get; set; }
    }
}
