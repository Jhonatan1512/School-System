using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CronogramaMatriculaController : ControllerBase
    {
        private readonly ICronogramaMatriculaService _cronulaService;
        public CronogramaMatriculaController(ICronogramaMatriculaService cronogramaMatriculaService)
        {
            _cronulaService = cronogramaMatriculaService;
        }
         
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CrearCronograma(CronogramaMatriculaDto dto)
        {
            try
            {
                var result = await _cronulaService.CreateAsync(dto);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reult = await _cronulaService.GetAllAsync();
                return Ok(reult);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("actualizar/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Actualizar(int id, ActualizarCronogramaDto dto)
        {
            try
            {
                await _cronulaService.ActualizarCronogramaAsync(id, dto);
                return Ok(new { message = "Datos actualizados" });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
