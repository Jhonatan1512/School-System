using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services
{
    public class CompetenciaService : ICompetenciaService
    {
        private readonly ICompetenciaRepository competenciaRepository;
        private readonly ICursoRepository cursoRepository;
        private readonly IGradoRepository gradoRepository;

        public CompetenciaService(ICompetenciaRepository competenciaRepository, ICursoRepository cursoRepository, IGradoRepository gradoRepository)
        {
            this.competenciaRepository = competenciaRepository;
            this.cursoRepository = cursoRepository;
            this.gradoRepository = gradoRepository;
        }
        public async Task ActualizarCompetenciaAsync(int id, CrearCompetenciaDto dto) 
        {
            var competenciaExiste = await competenciaRepository.BuscarPorIdAsync(id);
            if (competenciaExiste == null)
            {
                throw new Exception("La comptencia no existe");
            }

            competenciaExiste.Nombre = dto.Nombre;
            await competenciaRepository.EditarCompetenciaAsync(competenciaExiste);
        }

        public async Task<Competencia> CrearCompetenciaAsync(CrearCompetenciaDto dto)
        {
            var cursoExiste = await cursoRepository.ObtenerPorIdAsync(dto.CursoId);
            if (cursoExiste is null)
            {
                throw new Exception("El curso no existe");
            }

            var nuevaCompetencia = new Competencia
            {
                Nombre = dto.Nombre,
                CursoId = dto.CursoId,
            };

            await competenciaRepository.CrearCompetenciaAsync(nuevaCompetencia);
            return nuevaCompetencia;
        }
    }
}
