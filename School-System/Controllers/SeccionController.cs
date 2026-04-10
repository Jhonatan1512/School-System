using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using static System.Collections.Specialized.BitVector32;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SeccionController : ControllerBase
    { 
        private readonly ISeccionRepository _seccionRepository;
        public SeccionController(ISeccionRepository seccionRepository)
        {
            _seccionRepository = seccionRepository;
        }

        //POST :api/seccion
        [HttpPost]
        public async Task<IActionResult> AgregarSeccion([FromBody] Seccion seccion)
        {
            if (seccion.Nombre.IsNullOrEmpty())
            {
                return BadRequest("Todos los campos son obligatorios");
            }
            var nuevaSeccion = new Seccion
            {
                Nombre = seccion.Nombre
            };

            await _seccionRepository.AgregarSeccionAsync(nuevaSeccion);
            return Ok(new {mensaje = "Sección creada", nuevaSeccion });
        }

        //GET :api/seccion
        [HttpGet]
        public async Task<IActionResult> ObetenerTodos()
        {
            var secciones = await _seccionRepository.ObtenerTodosAsync();
            return Ok(secciones);
        }

        //GET :a8pi/seccion/id
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPord(int id)
        {
            var resultado = await _seccionRepository.ObtenerPorIdAsync(id);
            if(resultado == null) return NotFound(new {mensaje = "Sección no Encontrada"});
            return Ok(resultado);
        }

        //PUT :api/seccion/id
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarDatos(int id,[FromBody] SecciónDto seccionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (seccionDto.Nombre.IsNullOrEmpty())
            {
                return BadRequest("Todos los campos son obligatorios");
            }

            var resultado = await _seccionRepository.ObtenerPorIdAsync(id);

            if (resultado != null)
            {                
                resultado.Nombre = seccionDto.Nombre;
                await _seccionRepository.ActualizarAsync(resultado);
                return Ok(new { mensaje = "Sección Actualizada" });
            }

            return NotFound(new { mensaje = "Sección no Encontrada" });            
        }
    }
}
