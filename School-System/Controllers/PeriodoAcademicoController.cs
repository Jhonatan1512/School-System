using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PeriodoAcademicoController : ControllerBase
    {
        private readonly IPeriodoAcademicoService _periodoAcademicoService;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepo;
        public PeriodoAcademicoController(IPeriodoAcademicoService periodoAcademicoService, IPeriodoAcademicoRepository periodoAcademico)
        {
            _periodoAcademicoService = periodoAcademicoService;
            _periodoAcademicoRepo = periodoAcademico;
        }

        //POST :api/periodoAcademico
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearPeriodo([FromBody] PeriodoAcademicoDto dto)
        {
            var nuevoPeriodo = await _periodoAcademicoService.CrearPeriodoAcademicoAsync(dto);
            return Ok(new
            {
                mensjae = "Periodo académico creado",
                id = nuevoPeriodo.Id,
                nombre = nuevoPeriodo.Nombre,
                estado = nuevoPeriodo.EstadoActivo ? "Activo" : "Cerrado",
            });
        }

        //GET :api/periodoAcademico
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerPeriodos()
        {
            var periodos = await _periodoAcademicoRepo.ObtenerPeriodoAcademicoActivo();
            return Ok(periodos);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarPeriodo(int id, [FromBody] PeriodoAcademicoDto dto)
        {
            if (dto == null) return BadRequest("Los datos son nulos.");

            dto.Id = id;

            var periodoActualizar = await _periodoAcademicoService.ActualizarPeriodo(dto);

            if (periodoActualizar == null)
            {
                return NotFound(new { mensaje = $"No se encontró el periodo con ID {id}" });
            }

            return Ok(new
            {
                mensaje = "Periodo Actualizado",
                nombre = periodoActualizar.Nombre,
                estado = periodoActualizar.EstadoActivo
            });
        }
    }
}
