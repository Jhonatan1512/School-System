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

        public async Task<CalificacionDto> CreateAsync(int docenteId, CalificacionCreateDto dto)
        {
            var trimestre = await _trimestreRepository.ObtenerPorIdAsync(dto.TimestreId);
            if (trimestre == null) throw new Exception("el trimestre indicado no existe");
            if (!trimestre.EstadoActivo) throw new Exception("El periodo de registro de notas ha finalizado");

            var detalle = await _detalleMatriculaRepository.ObtenerDetallePorId(dto.DetalleMatriculaId);
            if (detalle == null) throw new Exception("El detalle de matricula no existe");

            Console.WriteLine("\n=== MODO DETECTIVE ACTIVADO ===");
            Console.WriteLine($"1. DocenteId enviado desde Postman: {docenteId}");
            Console.WriteLine($"2. CursoId sacado del Detalle: {detalle.CursoId}");
            Console.WriteLine($"3. SeccionId sacado de la Matricula: {detalle.Matricula.SeccionId}");
            Console.WriteLine($"4. PeriodoId sacado de la Matricula: {detalle.Matricula.PeriodoAcademicoId}");
            Console.WriteLine("===============================\n");

            bool esSuProfesor = await _asignacionDocenteRepository.ExisteAsignacionAsync(
                docenteId,
                detalle.CursoId,
                detalle.Matricula.SeccionId,
                detalle.Matricula.PeriodoAcademicoId);

            if (!esSuProfesor)
                throw new Exception("Acceso Denegado: no esta asignado a esta asignatura en esta sección");

            var nuevaCalificación = new Calificacion
            {
                Nota = dto.Nota,
                TrimestreId = dto.TimestreId,
                CompetenciaId = dto.CompetenciaId,
                DetalleMatriculaId = dto.DetalleMatriculaId,
            };

            var creado = await _calificacionRepository.RegistrarCalificacion(nuevaCalificación);

            return new CalificacionDto
            {
                Id = creado.Id,
                Nota = creado.Nota,
                TimestreId = creado.TrimestreId,
                DetalleMatriculaId = creado.DetalleMatriculaId,
                CompetenciaId = creado.CompetenciaId,
            };
        }
    }
}
