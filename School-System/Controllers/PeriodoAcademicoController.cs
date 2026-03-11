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
        public async Task<IActionResult> ObtenerPeriodos()
        {
            var periodos = await _periodoAcademicoRepo.ObtenerTodosasync();
            return Ok(periodos);
        }
    }
}
