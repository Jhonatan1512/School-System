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
    public class CalificacionService : ICalificacionService
    {
        private readonly ICalificacionRepository _calificacionRepository;
        private readonly ITrimestreRepository _trimestreRepository;
        private readonly IAsignacionDocenteRepository _asignacionDocenteRepository;
        private readonly IDetalleMatriculaRepository _detalleMatriculaRepository;
        public CalificacionService(
            ICalificacionRepository calificacionRepository,  
            ITrimestreRepository trimestreRepository,  
            IAsignacionDocenteRepository asignacionDocenteRepository,
            IDetalleMatriculaRepository detalleMatriculaRepository)
        {
            _calificacionRepository = calificacionRepository;
            _trimestreRepository = trimestreRepository;
            _asignacionDocenteRepository = asignacionDocenteRepository;
            _detalleMatriculaRepository = detalleMatriculaRepository; 
        }

        public async Task<List<CalificacionDto>> RegistroMasivo(int docenteId, List<CalificacionCreateDto> list)
        {
            var calificacionesProcesadas = new List<CalificacionDto>();

            foreach (var dto in list)
            {
                // 1. Validación de Trimestre
                var trimestre = await _trimestreRepository.ObtenerPorIdAsync(dto.TrimestreId);
                if (trimestre == null || !trimestre.EstadoActivo) continue;

                // 2. RESTRICCIÓN: Verificar si ya existe la nota para evitar duplicados
                var calificacionExistente = await _calificacionRepository.ObtenerCalificacionExistente(
                    dto.DetalleMatriculaId, dto.CompetenciaId, dto.TrimestreId);

                Calificacion resultadoDb;

                if (calificacionExistente != null)
                {
                    // SI EXISTE: Solo actualizamos la nota
                    calificacionExistente.Nota = dto.Nota;
                    // Usamos el método de actualizar de tu repositorio
                    await _calificacionRepository.ActualizarCalificacionAsync(calificacionExistente);
                    resultadoDb = calificacionExistente;
                }
                else
                {
                    // SI NO EXISTE: Creamos el nuevo registro
                    var nuevaCalificacion = new Calificacion
                    {
                        Nota = dto.Nota,
                        TrimestreId = dto.TrimestreId,
                        CompetenciaId = dto.CompetenciaId,
                        DetalleMatriculaId = dto.DetalleMatriculaId,
                    };
                    resultadoDb = await _calificacionRepository.RegistrarCalificacion(nuevaCalificacion);
                }

                calificacionesProcesadas.Add(new CalificacionDto
                {
                    Id = resultadoDb.Id,
                    Nota = resultadoDb.Nota,
                    TrimestreId = resultadoDb.TrimestreId,
                    CompetenciaId = resultadoDb.CompetenciaId,
                    DetalleMatriculaId = resultadoDb.DetalleMatriculaId,
                });
            }
            return calificacionesProcesadas;
        }
    }
}
