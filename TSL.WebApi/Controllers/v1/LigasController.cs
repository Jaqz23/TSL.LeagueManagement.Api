using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Liga;
using TSL.Core.Application.Interfaces.Services;

namespace TSL.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class LigasController : BaseApiController
    {
        private readonly ILigaService _ligaService;

        public LigasController(ILigaService ligaService)
        {
            _ligaService = ligaService;
        }

        #region GET Endpoints

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id) 
        {
            var result = await _ligaService.GetByIdAsync(id);

            return HandleServiceResponse(result);
        }


        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseDto<List<LigaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<LigaDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool? estado,
            [FromQuery] string? search)
        {
            List<LigaDto> ligas;

            // Filtrar por estado (activas o inactivas)
            if (estado.HasValue) 
            {
                if (estado.Value) 
                {
                    var resultActivas = await _ligaService.GetLigasActivasAsync();

                    if (!resultActivas.Success)
                        return HandleServiceResponse(resultActivas);

                    ligas = resultActivas.Data ?? new List<LigaDto>();

                }
                else 
                {
                    var resultAll = await _ligaService.GetAllAsync();

                    if (!resultAll.Success)
                        return HandleServiceResponse(resultAll);

                    ligas = resultAll.Data?
                        .Where(l => !l.Estado)
                        .ToList() ?? new List<LigaDto>();
                }
            }

            else if (!string.IsNullOrWhiteSpace(search)) 
            {
                var resultAll = await _ligaService.GetAllAsync();

                if (!resultAll.Success)
                    return HandleServiceResponse(resultAll);

                ligas = resultAll.Data?
                    .Where(l => l.Nombre.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList() ?? new List<LigaDto>();

            }

            else 
            {
                var resultAll = await _ligaService.GetAllAsync();

                if(!resultAll.Success)
                    return HandleServiceResponse(resultAll);

                ligas = resultAll.Data ?? new List<LigaDto>();
            }

            var response = new BaseResponseDto<List<LigaDto>>
            {
                Success = true,
                Data = ligas,
                Message = ligas.Any()
                ? $"Se encontraron {ligas.Count} liga(s)"
                : "No se encontraron ligas"
            };

            return HandleServiceResponse(response);

        }


        #endregion


        #region POST Endpoints

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>),StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateLigaDto dto) 
        {
            var result = await _ligaService.CreateAsync(dto);

            return HandleServiceResponse(result, nameof(GetById), new {id = result.Data?.Id});
        }

        #endregion


        #region PUT Endpoints

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLigaDto dto) 
        {
            var result = await _ligaService.UpdateAsync(id, dto);

            return HandleServiceResponse(result);
        }

        #endregion


        #region PATCH Endpoints

        [HttpPatch("{id}/activar")]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Activar(int id) 
        {
            var result = await _ligaService.ActivarAsync(id);

            return HandleServiceResponse(result);
        }


        [HttpPatch("{id}/desactivar")]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<LigaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Desactivar(int id)
        {
            var result = await _ligaService.DesactivarAsync(id);

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
            var result = await _ligaService.DeleteAsync(id);

            return HandleServiceResponse(result);
        }

        #endregion

    }
}
