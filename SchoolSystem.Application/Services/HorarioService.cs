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
        private static readonly Random _random = new Random(); 

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
            var dias = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };

            // Ronda 1: Bloques de 2h, en horas aleatorias dentro de las primeras 6
            await ProcesarRondaAsync(asignacionesPriorizadas, dias, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 2, tipoRonda: "Primeras6");

            // Ronda 2: Bloques de 2h, en cualquier hora aleatoria restante
            await ProcesarRondaAsync(asignacionesPriorizadas, dias, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 2, tipoRonda: "Todas");

            // Ronda 3: Bloques de 1h, prioridad en la hora que sobra (7ma hora)
            await ProcesarRondaAsync(asignacionesPriorizadas, dias, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 1, tipoRonda: "Restantes");

            // Ronda 4: Bloques de 1h, en cualquier espacio aleatorio
            await ProcesarRondaAsync(asignacionesPriorizadas, dias, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 1, tipoRonda: "Todas");

            // Ronda 5: Múltiples intentos de rescate con alta aleatoriedad para encajar los rebeldes
            for (int i = 0; i < 5; i++)
            {
                await ProcesarRondaAsync(asignacionesPriorizadas, dias, todosLosBloques, nuevosHorariosMemoria, horasAsignadasDocenteSemanales, advertencias, periodoId, 1, tipoRonda: "Rescate");
            }

            // Verificación final
            foreach (var asig in asignaciones)
            {
                int horasYaAsignadas = nuevosHorariosMemoria.Count(m => m.AsignacionDocenteId == asig.Id);
                int faltan = asig.PlanEstudio!.HorasSemanales - horasYaAsignadas;

                if (faltan > 0)
                {
                    var jornadaStr = asig.PlanEstudio.Jornada.ToString() ?? "JER";
                    string alerta = $"Incompleto: {asig.PlanEstudio.Curso!.Nombre} en {asig.Grado?.Nombre} {asig.Seccion?.Nombre} ({jornadaStr}). Faltaron {faltan}h.";
                    if (!advertencias.Contains(alerta)) advertencias.Add(alerta);
                }
            }

            await _horarioRepository.InsertarRangoAsync(nuevosHorariosMemoria);

            result.Exito = true;
            result.HorasAsignadas = nuevosHorariosMemoria.Count;
            result.Advertencias = advertencias.Distinct().ToList();
            result.Mensaje = "Horario generado exitosamente respetando Jornadas JER/JEC.";

            return result;
        }

        private async Task ProcesarRondaAsync(
            List<AsignacionDocente> listaAsignaciones,
            string[] dias,
            List<HoraLectiva> todosLosBloques,
            List<Horario> nuevosHorariosMemoria,
            Dictionary<int, int> horasAsignadasDocenteSemanales,
            List<string> advertencias,
            int periodoId,
            int tamanoBloqueAColocar,
            string tipoRonda)
        {
            foreach (var asig in listaAsignaciones)
            {
                if (asig.PlanEstudio!.DuracionBloque < tamanoBloqueAColocar) continue;

                var esJER = asig.PlanEstudio.Jornada.ToString().Trim().ToUpper() == "JER";

                var bloquesValidosJornada = todosLosBloques
                    .Where(b => (esJER && b.AplicaJER) || (!esJER && b.AplicaJEC))
                    .OrderBy(b => b.Orden)
                    .ToList();

                var bloquesProductivos = bloquesValidosJornada.Where(b => b.EsProductiva).ToList();

                List<HoraLectiva> bloquesEstrategicos;
                if (tipoRonda == "Primeras6")
                {
                    bloquesEstrategicos = bloquesProductivos.Take(6).OrderBy(_ => _random.Next()).ToList();
                }
                else if (tipoRonda == "Restantes")
                {
                    bloquesEstrategicos = bloquesProductivos.Skip(6).OrderBy(_ => _random.Next()).ToList();
                }
                else
                {
                    bloquesEstrategicos = bloquesProductivos.OrderBy(_ => _random.Next()).ToList();
                }

                int docenteId = asig.DocenteId;
                if (!horasAsignadasDocenteSemanales.ContainsKey(docenteId)) horasAsignadasDocenteSemanales[docenteId] = 0;

                int limiteHorasDocente = asig.Docente?.MaxHorasLectivas ?? 40;
                if (tipoRonda == "Rescate") limiteHorasDocente += 5;

                var diasAleatorios = dias.OrderBy(_ => _random.Next()).ToList();

                foreach (var dia in diasAleatorios)
                {
                    int horasYaAsignadas = nuevosHorariosMemoria.Count(m => m.AsignacionDocenteId == asig.Id);
                    int horasRestantes = asig.PlanEstudio!.HorasSemanales - horasYaAsignadas;

                    if (horasRestantes < tamanoBloqueAColocar) break;

                    int horasHoy = nuevosHorariosMemoria.Count(m => m.DiaSemana == dia && m.AsignacionDocenteId == asig.Id);

                    int limiteDiario = asig.PlanEstudio.HorasMaximasPorDia;
                    if (tipoRonda == "Rescate") limiteDiario += 2;

                    if ((horasHoy + tamanoBloqueAColocar) > limiteDiario) continue;

                    foreach (var bloque in bloquesEstrategicos)
                    {
                        if (horasAsignadasDocenteSemanales[docenteId] + tamanoBloqueAColocar > limiteHorasDocente) break;

                        if (await PuedeAsignarBloqueCompleto(dia, bloque, tamanoBloqueAColocar, asig, nuevosHorariosMemoria, bloquesValidosJornada, periodoId))
                        {
                            for (int i = 0; i < tamanoBloqueAColocar; i++)
                            {
                                var bloqueActual = bloquesValidosJornada.First(b => b.Orden == bloque.Orden + i);
                                nuevosHorariosMemoria.Add(new Horario
                                {
                                    AsignacionDocenteId = asig.Id,
                                    HoraLectivaId = bloqueActual.Id,
                                    DiaSemana = dia,
                                    AsignacionDocente = asig
                                });
                            }
                            horasAsignadasDocenteSemanales[docenteId] += tamanoBloqueAColocar;
                            break;
                        }
                    }
                }
            }
        }

        private async Task<bool> PuedeAsignarBloqueCompleto(string dia, HoraLectiva bloqueInicio, int duracion, AsignacionDocente asig, List<Horario> memoria, List<HoraLectiva> bloquesJornada, int periodoId)
        {
            for (int i = 0; i < duracion; i++)
            {
                var bloqueActual = bloquesJornada.FirstOrDefault(b => b.Orden == bloqueInicio.Orden + i);

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
            var horarios = await _horarioRepository.ObtenerPorGradoSeccionPeriodo(gradoId, seccionId, periodoId);
            var todasLasHoras = await _horaLectivaRepository.GetAllAsync();

            var jornadaEnum = horarios.FirstOrDefault()?.AsignacionDocente?.PlanEstudio?.Jornada;
            var jornadaSeccion = jornadaEnum != null ? jornadaEnum.ToString()!.Trim().ToUpper() : "JER";

            var bloquesFiltrados = todasLasHoras
                .Where(b => (jornadaSeccion == "JER" && b.AplicaJER) || (jornadaSeccion == "JEC" && b.AplicaJEC))
                .OrderBy(b => b.Orden)
                .ToList();

            var asigInfo = horarios.FirstOrDefault()?.AsignacionDocente;
            string tituloCabecera = asigInfo != null ? $"{jornadaSeccion} - {asigInfo.Grado?.Nombre} {asigInfo.Seccion?.Nombre}" : "Sin Horario Asignado";

            var tablaDto = new List<HorarioSeccionDto>();

            foreach (var bloque in bloquesFiltrados)
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
            return h == null ? "-" : $"{h.AsignacionDocente?.PlanEstudio?.Curso?.Nombre}\n({h.AsignacionDocente?.Docente?.Nombres})";
        }
    }
}