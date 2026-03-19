using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Partido;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Enums;

namespace TSL.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PartidosController : BaseApiController
    {
        private readonly IPartidoService _partidoService;

        public PartidosController(IPartidoService partidoService)
        {
            _partidoService = partidoService;
        }

        #region GET Endpoints

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id) 
        {
            var result = await _partidoService.GetByIdWithDetallesAsync(id);

            return HandleServiceResponse(result);
        }


        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>),StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? temporadaId,
            [FromQuery] int? equipoId,
            [FromQuery] string? estado)
        {
            List<PartidoDto> partidos;

            // Filtrar por Temporada
            if (temporadaId.HasValue) 
            {
                var resultTemporada = await _partidoService.GetByTemporadaAsync(temporadaId.Value);

                if (!resultTemporada.Success)
                    return HandleServiceResponse(resultTemporada);

                partidos = resultTemporada.Data ?? new List<PartidoDto>();
            }

            // Filtrar por Estado
            else if (!string.IsNullOrWhiteSpace(estado)) 
            {
                if (!Enum.TryParse<EstadoPartido>(estado, ignoreCase: true, out var estadoEnum)) 
                {
                    var errorResponse = new BaseResponseDto<List<PartidoDto>>
                    {
                        Success = false,
                        Message = "Estado inválido",
                        Errors = new List<string> { $"El estado '{estado}' no es válido. Estados válidos: Programado, Jugado" }
                    };
                    return HandleServiceResponse(errorResponse);
                }

                var resultEstado = await _partidoService.GetByEstadoAsync(estadoEnum);

                if (!resultEstado.Success)
                    return HandleServiceResponse(resultEstado);
                
                partidos = resultEstado.Data ?? new List<PartidoDto>();
                
            }

            // Sin filtros - traer todos
            else 
            {
                var resultAll = await _partidoService.GetAllAsync();

                if (!resultAll.Success)
                    return HandleServiceResponse(resultAll);

                partidos = resultAll.Data ?? new List<PartidoDto>();

            }

            // Filtro de equipo si se especifica 
            if (equipoId.HasValue) 
            {
                partidos = partidos
                    .Where(p => p.EquipoLocalId == equipoId.Value || p.EquipoVisitanteId == equipoId.Value)
                    .ToList();
            }

            // Filtro de estado si ya se filtro por temporada
            if (temporadaId.HasValue && !string.IsNullOrWhiteSpace(estado)) 
            {
                if (Enum.TryParse<EstadoPartido>(estado, ignoreCase: true, out var estadoEnumFiltro)) 
                {
                    partidos = partidos
                    .Where(p => p.Estado == estadoEnumFiltro)
                    .ToList();
                }
            }

            var response = new BaseResponseDto<List<PartidoDto>>
            {
                Success = true,
                Data = partidos,
                Message = partidos.Any()
                ? $"Se encontraron {partidos.Count} partido(s)"
                : "No se encontraron partidos"
            };

            return HandleServiceResponse(response);

        }


        [HttpGet("temporada/{temporadaId}/jornada/{numeroJornada}")]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByJornada(int temporadaId, int numeroJornada) 
        {
            var result = await _partidoService.GetByJornadaAsync(temporadaId, numeroJornada);

            return HandleServiceResponse(result);
        }


        [HttpGet("proximos")]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProximosPartidos([FromQuery] int cantidad = 5) 
        {
            var result = await _partidoService.GetProximosPartidosAsync(cantidad);

            return HandleServiceResponse(result);
        }


        [HttpGet("resultados")]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<PartidoDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUltimosResultados([FromQuery] int cantidad = 5) 
        {
            var result = await _partidoService.GetUltimosResultadosAsync(cantidad);

            return HandleServiceResponse(result);
        }

        #endregion


        #region POST Endpoints

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreatePartidoDto dto) 
        {
            var result = await _partidoService.CreateAsync(dto);

            return HandleServiceResponse(result, nameof(GetById), new { id = result.Data?.Id });
        }

        #endregion


        #region PUT Endpoints

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePartidoDto dto) 
        {
            var result = await _partidoService.UpdateAsync(id, dto);

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
            var result = await _partidoService.DeleteAsync(id);

            return HandleServiceResponse(result);
        }

        #endregion


        #region PATCH Endpoints

        [HttpPatch("{id}/resultado")]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<PartidoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarResultado(int id, [FromBody] RegistrarResultadoDto dto) 
        {
            var result = await _partidoService.RegistrarResultadoAsync(id, dto);

            return HandleServiceResponse(result);
        }

        #endregion



    }
}
