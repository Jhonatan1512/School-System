using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HorariosController : ControllerBase
    {
        private readonly IHorarioService _horarioService;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;

        public HorariosController(IHorarioService horarioService, IPeriodoAcademicoRepository periodoAcademicoRepository)
        {
            _horarioService = horarioService;
            _periodoAcademicoRepository = periodoAcademicoRepository;
        }
          
        [HttpPost]
        public async Task<IActionResult> Generar()
        {
            var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            var result = await _horarioService.GenerarHorarioAsync(periodoActivo!.Id);
            if(!result.Exito) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("ver/{gradoId}/{seccionId}")]
        public async Task<IActionResult> VerHorario(int gradoId, int seccionId)
        {
            var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();

            var result = await _horarioService.ObtenerHorariosPorGradoSeccion(gradoId, seccionId, periodoActivo!.Id);

            return Ok(result);
        }
    }
}
