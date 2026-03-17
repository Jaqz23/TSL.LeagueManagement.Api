using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.TablaPosicion;
using TSL.Core.Application.Interfaces.Services;

namespace TSL.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PosicionesEquipoController : BaseApiController
    {
        private readonly IPosicionEquipoService _posicionEquipoService;

        public PosicionesEquipoController(IPosicionEquipoService posicionEquipoService)
        {
            _posicionEquipoService = posicionEquipoService;
        }

        #region GET Endpoints 

        // Obtiene la posicion de un equipo en una tabla especifica

        [HttpGet("equipo/{equipoId}/tabla/{tablaId}")]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEquipoYTabla(int equipoId, int tablaId) 
        {
            var result = await _posicionEquipoService.GetByEquipoAndTablaAsync(equipoId, tablaId);

            return HandleServiceResponse(result);
        }

        // Obtiene la posicion actual de un equipo en una temporada

        [HttpGet("equipo/{equipoId}/temporada/{temporadaId}")]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPosicionActual(int equipoId, int temporadaId) 
        {
            var result = await _posicionEquipoService.GetPosicionActualEquipoAsync(equipoId, temporadaId);

            return HandleServiceResponse(result);
        }


        [HttpGet("equipo/{equipoId}/historial")]
        [ProducesResponseType(typeof(BaseResponseDto<List<PosicionEquipoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<PosicionEquipoDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<List<PosicionEquipoDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHistorialEquipo(int equipoId) 
        {
            var result = await _posicionEquipoService.GetHistorialPosicionesEquipoAsync(equipoId);

            return HandleServiceResponse(result);
        }


        [HttpGet("equipo/{equipoId}/tabla/{tablaId}/existe")]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExistePosicion(int equipoId, int tablaId) 
        {
            var result = await _posicionEquipoService.ExistePosicionAsync(equipoId, tablaId);

            return HandleServiceResponse(result);
        }

        #endregion


        #region POST Endpoints

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearPosicion([FromBody] CrearPosicionDto dto) 
        {
            var result = await _posicionEquipoService.CrearPosicionAsync(dto.EquipoId, dto.TablaPosicionId);

            return HandleServiceResponse(result, nameof(GetByEquipoYTabla), 
                new {equipoId = dto.EquipoId, tablaId = dto.TablaPosicionId});
        }

        #endregion


        #region PATCH Endpoints

        [HttpPatch("{id}/reiniciar")]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<PosicionEquipoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReiniciarEstadisticas (int id) 
        {
            var result = await _posicionEquipoService.ReiniciarEstadisticasAsync(id);

            return HandleServiceResponse(result);
        }

        #endregion


        #region PATCH Endpoints

        [HttpDelete("tabla/{tablaId}/eliminar-todas")]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarTodasPosiciones(int tablaId) 
        {
            var result = await _posicionEquipoService.EliminarTodasPosicionesAsync(tablaId);

            return HandleServiceResponse(result);
        }

        #endregion

    }
}
