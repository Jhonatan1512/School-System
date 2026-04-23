using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace SchoolSystem.Application.Services
{
    public class HorarioService : IHorarioService
    {
        private readonly IHorarioRepository _horarioRepository;
        private readonly IAsignacionDocenteRepository _asignacionDocenteRepository;
        private readonly IHoraLectivaRepository _horaLectivaRepository;

        public HorarioService(
            IHorarioRepository horarioRepository,
            IAsignacionDocenteRepository asignacionDocenteRepository,
            IHoraLectivaRepository horaLectivaRepository)
        {
            _horarioRepository = horarioRepository;
            _asignacionDocenteRepository = asignacionDocenteRepository;
            _horaLectivaRepository = horaLectivaRepository;
        }


        public async Task<HorarioResultDto> GenerarHorarioAsync(int periodoId)
        {
            var result = new HorarioResultDto();

            var asignaciones = await _asignacionDocenteRepository.ObtenerPorPeriodoAsync(periodoId);

            var todosLosBloques = await _horaLectivaRepository.GetAllAsync();
            var bloquesProductivos = todosLosBloques
                                        .Where(h => h.EsProductiva)
                                        .OrderBy(h => h.Orden)
                                        .ToList();

            var dias = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };

            var asignacionesOrdenadas = asignaciones
                .OrderBy(a => (int)a.Curso!.Prioridad)
                .ThenBy(a => a.GradoId)
                .ThenBy(a => a.SeccionId)
                .ToList();

            await _horarioRepository.LimpiarHorariosPeriodoAsync(periodoId);

            var nuevosHorariosMemoria = new List<Horario>();

            foreach (var asig in asignacionesOrdenadas)
            {
                int horasRestantes = asig.Curso!.HorasSemanales;
                int duracionBloque = asig.Curso.DuracionBloque;

                foreach (var dia in dias)
                {
                    if (horasRestantes <= 0) break;

                    int horasAsignadasHoy = 0;

                    foreach (var bloque in bloquesProductivos)
                    {
                        if (horasRestantes <= 0 || (horasAsignadasHoy + duracionBloque) > asig.Curso.HorasMaximasPorDia)
                            break;

                        if (await PuedeAsignarBloqueCompleto(dia, bloque, duracionBloque, asig, nuevosHorariosMemoria, todosLosBloques, periodoId))
                        {
                            for (int i = 0; i < duracionBloque; i++)
                            {
                                var bloqueActual = todosLosBloques.First(b => b.Orden == bloque.Orden + i);

                                nuevosHorariosMemoria.Add(new Horario
                                {
                                    AsignacionDocenteId = asig.Id,
                                    HoraLectivaId = bloqueActual.Id,
                                    DiaSemana = dia,
                                    AsignacionDocente = asig 
                                });
                            }
                            horasAsignadasHoy += duracionBloque;
                            horasRestantes -= duracionBloque;
                        }
                    }
                }

                if (horasRestantes > 0)
                {
                    result.Advertencias.Add($"Incompleto: {asig.Curso.Nombre} en {asig.Grado?.Nombre} - {asig.Seccion?.Nombre} (Faltaron {horasRestantes}h)");
                }
            }

            await _horarioRepository.InsertarRangoAsync(nuevosHorariosMemoria);

            result.Exito = true;
            result.HorasAsignadas = nuevosHorariosMemoria.Count;
            result.Mensaje = "Proceso de generación automática finalizado para todos los grados y secciones.";
            return result;
        }

        private async Task<bool> PuedeAsignarBloqueCompleto(string dia, HoraLectiva bloqueInicio, int duracion, AsignacionDocente asig, List<Horario> memoria, IEnumerable<HoraLectiva> todosLosBloques, int periodoId)
        {
            for (int i = 0; i < duracion; i++)
            {
                var bloqueActual = todosLosBloques.FirstOrDefault(b => b.Orden == bloqueInicio.Orden + i);

                if (bloqueActual == null || !bloqueActual.EsProductiva) return false;

                bool docenteOcupado = await _horarioRepository.ExisteCruce(asig.DocenteId, dia, bloqueActual.Id, periodoId)
                    || memoria.Any(m => m.DiaSemana == dia && m.HoraLectivaId == bloqueActual.Id && m.AsignacionDocente?.DocenteId == asig.DocenteId);

                if (docenteOcupado) return false;

                bool salonOcupado = await _horarioRepository.ExisteCruceSeccion(asig.SeccionId, dia, bloqueActual.Id, periodoId)
                    || memoria.Any(m => m.DiaSemana == dia && m.HoraLectivaId == bloqueActual.Id &&
                                  m.AsignacionDocente?.SeccionId == asig.SeccionId &&
                                  m.AsignacionDocente?.GradoId == asig.GradoId);

                if (salonOcupado) return false;
            }
            return true;
        }


        public async Task<List<HorarioSeccionDto>> ObtenerHorariosPorGradoSeccion(int gradoId, int seccionId, int periodoId)
        {
            var bloques = (await _horaLectivaRepository.GetAllAsync()).OrderBy(b => b.Orden).ToList();

            var horarios = await _horarioRepository.ObtenerPorGradoSeccionPeriodo(gradoId, seccionId, periodoId);

            var asig = horarios.FirstOrDefault()?.AsignacionDocente;
            string tituloCabecera = asig != null ? $"{asig.Grado?.Nombre} - {asig.Seccion?.Nombre}" : "Sin Horario Asignado";

            var tablaDto = new List<HorarioSeccionDto>();

            foreach (var bloque in bloques)
            {
                var fila = new HorarioSeccionDto
                {
                    HoraLectivaId = bloque.Id,
                    Bloque = bloque.Nombre,
                    RangoHora = $"{bloque.HoraInicio:hh\\:mm} - {bloque.HoraFin:hh\\:mm}",
                    EsProductiva = bloque.EsProductiva,
                    GradoSeccion = tituloCabecera
                };

                if (!bloque.EsProductiva)
                {
                    fila.Lunes = fila.Martes = fila.Miercoles = fila.Jueves = fila.Viernes = "RECREO";
                }
                else
                {
                    fila.Lunes = FormatearCelda(horarios, "Lunes", bloque.Id);
                    fila.Martes = FormatearCelda(horarios, "Martes", bloque.Id);
                    fila.Miercoles = FormatearCelda(horarios, "Miércoles", bloque.Id);
                    fila.Jueves = FormatearCelda(horarios, "Jueves", bloque.Id);
                    fila.Viernes = FormatearCelda(horarios, "Viernes", bloque.Id);
                }

                tablaDto.Add(fila);
            }

            return tablaDto;
        }

        private string FormatearCelda(IEnumerable<Horario> horarios, string dia, int bloqueId)
        {
            var h = horarios.FirstOrDefault(x => x.DiaSemana == dia && x.HoraLectivaId == bloqueId);
            if (h == null) return "-";

            return $"{h.AsignacionDocente?.Curso?.Nombre}\n({h.AsignacionDocente?.Docente?.Nombres})";
        }
    }
}