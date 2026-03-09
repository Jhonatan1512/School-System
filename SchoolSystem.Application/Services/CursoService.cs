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
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly IGradoRepository _gradoRepository;

        public CursoService(ICursoRepository cursoRepository, IGradoRepository gradoRepository)
        {
            _cursoRepository = cursoRepository;
            _gradoRepository = gradoRepository;
        }

        public async Task<bool> ActualizarCursoCompetenciaAsync(int id, CursoCompetenciaDto dto)
        {
            var cursoExiste = await _cursoRepository.ObtenerPorIdAsync(id);
            if (cursoExiste == null) return false;

            cursoExiste.Nombre = dto.Nombre;
            cursoExiste.GradoId = dto.GradoId;

            var idsDelDto = dto.Competencias.Select(c => c.Id).ToList();
            var CompetenciaEliminar = cursoExiste.Competencias.Where(c => !idsDelDto.Contains(c.Id)).ToList();

            foreach (var competencia in CompetenciaEliminar)
            {
                cursoExiste.Competencias.Remove(competencia);
            }

            foreach (var competenciaDto in dto.Competencias)
            {
                if(competenciaDto.Id == 0)
                {
                    cursoExiste.Competencias.Add(new Competencia { Nombre = competenciaDto.Nombre });
                } else
                {
                    var competenciaExiste = cursoExiste.Competencias.FirstOrDefault(c => c.Id == competenciaDto.Id);
                    if(competenciaExiste != null)
                    {
                        competenciaExiste.Nombre = competenciaDto.Nombre;
                    }
                }
            }

            await _cursoRepository.ActualizarCursoAsync(cursoExiste);
            return true;
        }

        public async Task<Curso> CrearCursoCompetenciaAsync(CursoCompetenciaDto dto)
        {
            var gradoExiste = await _gradoRepository.ObtenerPorId(dto.GradoId);
            if(gradoExiste == null)
            {
                throw new Exception("El grado especificado no existe");
            }

            var nuevoCurso = new Curso
            {
                Nombre = dto.Nombre,
                GradoId = dto.GradoId,
                Competencias = dto.Competencias.Select(c => new Competencia
                {
                    Nombre = c.Nombre
                }).ToList()
            };

            await _cursoRepository.AgregarCursoAsync(nuevoCurso);
            return nuevoCurso;
        }

        public async Task<IEnumerable<CursoCompetenciaDto>> ObtenerTodosAsync()
        {
            var cursos = await _cursoRepository.ObtenerCursosAsync();

            var cursosDto = cursos.Select(curso => new CursoCompetenciaDto
            {
                Id = curso.Id,
                Nombre = curso.Nombre,
                GradoId = curso.GradoId,
                Competencias = curso.Competencias.Select(comp => new CrearCompetenciaDto
                {
                    //Id = comp.Id,
                    Nombre = comp.Nombre,
                }).ToList()
            }).ToList();

            return cursosDto;
        }
    }
}
