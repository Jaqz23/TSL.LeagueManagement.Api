using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.TablaPosicion;
using TSL.Core.Application.Interfaces.Services;

namespace TSL.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class TablaPosicionesController : BaseApiController
    {
        private readonly ITablaPosicionService _tablaPosicionService;

        public TablaPosicionesController(ITablaPosicionService tablaPosicionService)
        {
            _tablaPosicionService = tablaPosicionService;
        }

        #region GET Endpoints

        [HttpGet("temporada/{temporadaId}")]
        [ProducesResponseType(typeof(BaseResponseDto<TablaPosicionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<TablaPosicionDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<TablaPosicionDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByTemporada(int temporadaId) 
        {
            var result = await _tablaPosicionService.GetByTemporadaIdWithPosicionesAsync(temporadaId);

            return HandleServiceResponse(result);
        }


        [HttpGet("temporada/{temporadaId}/existe")]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExisteTabla(int temporadaId) 
        {
            var result = await _tablaPosicionService.ExisteTablaAsync(temporadaId);

            return HandleServiceResponse(result);
        }

        #endregion


        #region POST Endpoints

        [HttpPost("temporada/{temporadaId}/recalcular")]
        [ProducesResponseType(typeof(BaseResponseDto<TablaPosicionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<TablaPosicionDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<TablaPosicionDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecalcularTabla(int temporadaId) 
        {
            var result = await _tablaPosicionService.RecalcularTablaAsync(temporadaId);

            return HandleServiceResponse(result);
        }

        #endregion

    }
}
