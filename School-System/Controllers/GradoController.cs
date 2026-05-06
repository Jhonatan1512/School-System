using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class GradoController : ControllerBase
    {
        private readonly IGradoRepository _gradoRepository;
        private readonly IGradoSevice _gradoSevice;
        public GradoController(IGradoRepository gradoRepository, IGradoSevice gradoSevice) 
        {
            _gradoRepository = gradoRepository;
            _gradoSevice = gradoSevice; 
        }

        //GET :api/grado
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Obtenergrado(int id)
        {
            var grado = await _gradoSevice.GetGradoDetalle(id);
            if (grado == null) return NotFound(new {mensaje = "Grado no entontrado"});
            return Ok(grado);
        }

        //POST :api/grado
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearGrado(Grado grado)
        {
            if (grado == null || string.IsNullOrEmpty(grado.Nombre))
            {
                return BadRequest(new { mensaje = "El nombre del grado es obligatorio" });
            }

            var nuevoGrado = new Grado
            {
                Nombre = grado.Nombre,
            };

            await _gradoRepository.CrearGrado(nuevoGrado);
            return Ok(new
            {
                mesaje = "Grado creado",
                Id = nuevoGrado.Id,
                Nombre = nuevoGrado.Nombre,
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var grados = await _gradoSevice.GetAllAsync();
            return Ok(grados);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var gradoExiste = await _gradoRepository.ObtenerPorId(id);
                if(gradoExiste is null)
                {
                    return NotFound("Grado no encontrado");
                }

                await _gradoRepository.EliminarGrado(id);
                return Ok(new { mensaje = "Registro de grado eliminado" });

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Grado grado)
        {
            if (grado == null || string.IsNullOrEmpty(grado.Nombre))
            {
                return BadRequest(new { mensaje = "El nombre del grado es obligatorio" });
            }

            try
            {
                var gradoExiste = await _gradoRepository.ObtenerPorId(id);

                if (gradoExiste == null)
                    return NotFound(new { mensaje = "Grado no encontrado" });

                gradoExiste.Nombre = grado.Nombre;

                await _gradoRepository.ActualizarGradoAsync(gradoExiste);
                return Ok(new { mensaje = "Datos del grado actualizados" });

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
