using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Entities
{
    public class Calificacion 
    {
        public int Id { get; set; }
        public string Nota { get; set; }
        public int TrimestreId { get; set; }
        public Trimestre? Trimestre { get; set; }
        public int CompetenciaId { get; set; }
        public Competencia? Competencia { get; set; }
        public int DetalleMatriculaId { get; set; }
        public DetalleMatricula? DetalleMatricula { get; set; } 
    } 
}
