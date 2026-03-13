using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Temporada;
using TSL.Core.Application.Interfaces.Services;

namespace TSL.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class TemporadasController : BaseApiController
    {
        private readonly ITemporadaService _temporadasService;

        public TemporadasController(ITemporadaService temporadaService)
        {
            _temporadasService = temporadaService;
        }


        #region GET Endpoints

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseDto<List<TemporadaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<TemporadaDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] bool? estado)
        {
            List<TemporadaDto> temporadas;

            if (estado.HasValue)
            {
                if (estado.Value)
                {
                    var resultActivas = await _temporadasService.GetTemporadasActivasAsync();

                    if (!resultActivas.Success)
                        return HandleServiceResponse(resultActivas);

                    temporadas = resultActivas.Data ?? new List<TemporadaDto>();

                }
                else
                {
                    var resultAll = await _temporadasService.GetAllAsync();

                    if (!resultAll.Success)
                        return HandleServiceResponse(resultAll);

                    temporadas = resultAll.Data?
                        .Where(t => !t.Estado)
                        .ToList() ?? new List<TemporadaDto>();
                }
            }
            else
            {
                var resultAll = await _temporadasService.GetAllAsync();

                if (!resultAll.Success)
                    return HandleServiceResponse(resultAll);

                temporadas = resultAll.Data ?? new List<TemporadaDto>();

            }

            var response = new BaseResponseDto<List<TemporadaDto>>
            {
                Success = true,
                Data = temporadas,
                Message = temporadas.Any()
                    ? $"Se encontraron {temporadas.Count} temporada(s)"
                    : "No se encontraron temporadas"
            };

            return HandleServiceResponse(response);

        }


        [HttpGet("liga/{ligaId}")]
        [ProducesResponseType(typeof(BaseResponseDto<List<TemporadaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<TemporadaDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<List<TemporadaDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByLiga(int ligaId, [FromQuery] bool? estado)
        {
            var result = await _temporadasService.GetByLigaIdAsync(ligaId);

            if (!result.Success)
                return HandleServiceResponse(result);

            var temporadas = result.Data ?? new List<TemporadaDto>();

            if (estado.HasValue)
            {
                temporadas = temporadas
                    .Where(t => t.Estado == estado.Value)
                    .ToList();
            }

            var response = new BaseResponseDto<List<TemporadaDto>>
            {
                Success = true,
                Data = temporadas,
                Message = temporadas.Any()
            ? $"Se encontraron {temporadas.Count} temporada(s) para la liga"
            : "No se encontraron temporadas para esta liga"
            };

            return HandleServiceResponse(response);

        }


        [HttpGet("liga/{ligaId}/activa")]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTemporadaActivaPorLiga(int ligaId)
        {
            var result = await _temporadasService.GetTemporadaActivaByLigaAsync(ligaId);

            return HandleServiceResponse(result);

        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _temporadasService.GetByIdAsync(id);

            return HandleServiceResponse(result);
        }


        #endregion


        #region POST Endpoints

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateTemporadaDto dto)
        {
            var result = await _temporadasService.CreateAsync(dto);

            return HandleServiceResponse(result, nameof(GetById), new { id = result.Data?.Id });
        }

        #endregion


        #region PUT Endpoints

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTemporadaDto dto) 
        {
            var result = await _temporadasService.UpdateAsync(id, dto);

            return HandleServiceResponse(result);

        }

        #endregion


        #region DELETE Endpoints

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<int>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<int>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<int>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id) 
        {
            var result = await _temporadasService.DeleteAsync(id);

            return HandleServiceResponse(result);
        }

        #endregion


        #region PATCH Endpoints

        [HttpPatch("{id}/finalizar")]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<TemporadaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Finalizar(int id) 
        {
            var result = await _temporadasService.FinalizarAsync(id);

            return HandleServiceResponse(result);
        }

        #endregion

    }
}
