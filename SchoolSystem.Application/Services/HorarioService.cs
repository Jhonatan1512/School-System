using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var bloquesProductivos = todosLosBloques.Where(h => h.EsProductiva).OrderBy(h => h.Orden).ToList();

            var dias = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };

            var bloquesPrimeras6 = bloquesProductivos.Take(6).ToList();
            var bloquesRestantes = bloquesProductivos.Skip(6).ToList();

            var asignacionesPriorizadas = asignaciones
                .OrderByDescending(a => a.PlanEstudio!.DuracionBloque)
                .ThenByDescending(a => a.PlanEstudio!.HorasSemanales)
                .ThenBy(a => (int)a.PlanEstudio!.Curso!.Prioridad)
                .ThenBy(a => a.GradoId)
                .ThenBy(a => a.SeccionId)
                .ToList();

            await _horarioRepository.LimpiarHorariosPeriodoAsync(periodoId);

            var nuevosHorariosMemoria = new List<Horario>();
            var horasAsignadasDocenteSemanales = new Dictionary<int, int>();
            var advertencias = new List<string>();

            await ProcesarAsignacionesAsync(asignacionesPriorizadas, dias, bloquesPrimeras6, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 2);
            await ProcesarAsignacionesAsync(asignacionesPriorizadas, dias, bloquesProductivos, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 2);
            await ProcesarAsignacionesAsync(asignacionesPriorizadas, dias, bloquesRestantes, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 1);
            await ProcesarAsignacionesAsync(asignacionesPriorizadas, dias, bloquesProductivos, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 1);

            foreach (var asig in asignaciones)
            {
                int horasYaAsignadas = nuevosHorariosMemoria.Count(m => m.AsignacionDocenteId == asig.Id);
                int faltan = asig.PlanEstudio!.HorasSemanales - horasYaAsignadas;

                if (faltan > 0)
                {
                    string alerta = $"Incompleto: {asig.PlanEstudio.Curso!.Nombre} en {asig.Grado?.Nombre} - {asig.Seccion?.Nombre} (Faltaron {faltan}h). Posibles cruces o falta de disponibilidad.";
                    if (!advertencias.Contains(alerta)) advertencias.Add(alerta);
                }
            }

            await _horarioRepository.InsertarRangoAsync(nuevosHorariosMemoria);

            result.Exito = true;
            result.HorasAsignadas = nuevosHorariosMemoria.Count;
            result.Advertencias = advertencias.Distinct().ToList();
            result.Mensaje = "Proceso finalizado: Zonificación por tamaño de bloque y reglas de tutoría aplicadas.";

            return result;
        }

        private async Task ProcesarAsignacionesAsync(
            List<AsignacionDocente> listaAsignaciones,
            string[] dias,
            List<HoraLectiva> bloquesEstrategicos,
            List<HoraLectiva> todosLosBloques,
            List<Horario> nuevosHorariosMemoria,
            Dictionary<int, int> horasAsignadasDocenteSemanales,
            List<string> advertencias,
            int periodoId,
            int tamanoRonda)
        {
            var random = new Random();

            foreach (var asig in listaAsignaciones)
            {
                if (asig.PlanEstudio!.DuracionBloque < tamanoRonda) continue;

                // --- LÓGICA DE TUTORÍA ---
                bool esTutoria = asig.PlanEstudio.Curso!.Nombre.Contains("Tutoría", StringComparison.OrdinalIgnoreCase);
                string? diaObligatorio = null;

                if (esTutoria)
                {
                    // Asignar el día obligatorio dependiendo del nombre del grado
                    // NOTA: Ajusta el "1", "2", etc., si en tu BD se llaman "Primero", "Segundo", etc.
                    string nombreGrado = asig.Grado?.Nombre ?? "";
                    if (nombreGrado.Contains("1") || nombreGrado.Contains("Primero", StringComparison.OrdinalIgnoreCase)) diaObligatorio = "Lunes";
                    else if (nombreGrado.Contains("2") || nombreGrado.Contains("Segundo", StringComparison.OrdinalIgnoreCase)) diaObligatorio = "Martes";
                    else if (nombreGrado.Contains("3") || nombreGrado.Contains("Tercero", StringComparison.OrdinalIgnoreCase)) diaObligatorio = "Miércoles";
                    else if (nombreGrado.Contains("4") || nombreGrado.Contains("Cuarto", StringComparison.OrdinalIgnoreCase)) diaObligatorio = "Jueves";
                    else if (nombreGrado.Contains("5") || nombreGrado.Contains("Quinto", StringComparison.OrdinalIgnoreCase)) diaObligatorio = "Viernes";
                }
                // -------------------------

                int docenteId = asig.DocenteId;
                if (!horasAsignadasDocenteSemanales.ContainsKey(docenteId))
                    horasAsignadasDocenteSemanales[docenteId] = 0;

                int limiteHorasDocente = asig.Docente?.MaxHorasLectivas ?? 40;

                // Si es Tutoría y tiene día obligatorio, solo iteramos sobre ese día
                var diasAProcesar = esTutoria && diaObligatorio != null
                    ? new List<string> { diaObligatorio }
                    : dias.OrderBy(dia =>
                        nuevosHorariosMemoria.Count(m =>
                            m.DiaSemana == dia &&
                            (m.AsignacionDocente?.DocenteId == docenteId || m.AsignacionDocente?.SeccionId == asig.SeccionId)
                        )
                    ).ThenBy(_ => random.Next()).ToList();

                foreach (var dia in diasAProcesar)
                {
                    int horasYaAsignadas = nuevosHorariosMemoria.Count(m => m.AsignacionDocenteId == asig.Id);
                    int horasRestantes = asig.PlanEstudio.HorasSemanales - horasYaAsignadas;

                    if (horasRestantes < tamanoRonda) break;

                    int horasDeEsteCursoHoy = nuevosHorariosMemoria.Count(m =>
                        m.DiaSemana == dia &&
                        m.AsignacionDocenteId == asig.Id);

                    if ((horasDeEsteCursoHoy + tamanoRonda) > asig.PlanEstudio.HorasMaximasPorDia)
                        continue;

                    // Si es Tutoría, invertimos el orden de los bloques para empezar desde el final (últimas horas)
                    var bloquesAIntentar = esTutoria
                        ? bloquesEstrategicos.OrderByDescending(b => b.Orden).ToList()
                        : bloquesEstrategicos;

                    foreach (var bloque in bloquesAIntentar)
                    {
                        if (horasAsignadasDocenteSemanales[docenteId] + tamanoRonda > limiteHorasDocente)
                        {
                            string alerta = $"Límite Docente: {asig.Docente?.Nombres} excede su carga semanal intentando asignar {asig.PlanEstudio.Curso!.Nombre}.";
                            if (!advertencias.Contains(alerta)) advertencias.Add(alerta);
                            break;
                        }

                        // Verificamos si en este bloque hay disponibilidad
                        if (await PuedeAsignarBloqueCompleto(dia, bloque, tamanoRonda, asig, nuevosHorariosMemoria, todosLosBloques, periodoId))
                        {
                            for (int i = 0; i < tamanoRonda; i++)
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

                            horasAsignadasDocenteSemanales[docenteId] += tamanoRonda;
                            break; // Logramos asignar las horas en este día, pasamos al siguiente si aún faltan
                        }

                        // Si es tutoría y no se pudo asignar en la última hora, cortamos el bucle
                        // para que no intente meterlo en la mañana
                        if (esTutoria) break;
                    }
                }
            }
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
            Console.WriteLine($"HORARIOS ENCONTRADOS: {horarios.Count}");

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

            return $"{h.AsignacionDocente?.PlanEstudio?.Curso?.Nombre}\n({h.AsignacionDocente?.Docente?.Nombres})";
        }
    }
}