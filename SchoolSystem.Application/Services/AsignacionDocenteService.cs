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
    public class AsignacionDocenteService : IAsignacionDocenteService
    {
        private readonly IAsignacionDocenteRepository _asignacionDocenteRepository;
        public AsignacionDocenteService(IAsignacionDocenteRepository asignacionDocenteRepository)
        {
            _asignacionDocenteRepository = asignacionDocenteRepository;
        }

        public async Task<AsignacionDocenteDto> AsignarCursoAsync(AsignacionDocenteCreateDto dto)
        {
            bool existe = await _asignacionDocenteRepository.ExisteAsignacionAsync(
                dto.DocenteId, dto.CursoId, dto.SeccionId, dto.PeriodoAcademicoId);

            if (existe) throw new Exception("El docente ya se encuentra asignado a este curso en esta sección en el periodo actual");

            var nuevaAsignacion = new AsignacionDocente
            {
                DocenteId = dto.DocenteId,
                CursoId = dto.CursoId,
                GradoId = dto.GradoId,
                SeccionId = dto.SeccionId,
                PeriodoAcademicoId = dto.PeriodoAcademicoId,
            };

            var creada = await _asignacionDocenteRepository.CrearAsignacionAsync(nuevaAsignacion);

            return new AsignacionDocenteDto
            {
                Id = creada.Id,
                DocenteId = creada.DocenteId,
                CursoId = creada.CursoId,
                GradoId= creada.GradoId,
                SeccionId = creada.SeccionId,
                PeriodoAcademicoId = creada.PeriodoAcademicoId
            };
        }
    }
}
