using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AsignacionDocenteController : ControllerBase
    {
        private readonly IAsignacionDocenteService _asignacionDocenteService;
        public AsignacionDocenteController(IAsignacionDocenteService asignacionDocenteService)
        {
            _asignacionDocenteService = asignacionDocenteService;
        }

        [HttpPost]
        public async Task<IActionResult> AsignarCurso([FromBody] AsignacionDocenteCreateDto dto)
        {
            try
            {
                var resultado = await _asignacionDocenteService.AsignarCursoAsync(dto);
                return Ok(new
                {
                    mensaje = "Docente asignado al curso",
                    data = resultado
                });
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }
    }
}
