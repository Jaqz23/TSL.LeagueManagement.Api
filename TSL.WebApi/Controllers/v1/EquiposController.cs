using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Equipo;
using TSL.Core.Application.Interfaces.Services;

namespace TSL.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class EquiposController : BaseApiController
    {
        private readonly IEquipoService _equipoService;

        public EquiposController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        #region GET Endpoints

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponseDto<List<EquipoDto>>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponseDto<List<EquipoDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? ciudad,
            [FromQuery] string? search)
        {
            List<EquipoDto> equipos;

            if (!string.IsNullOrWhiteSpace(ciudad)) 
            {
                var resultCiudad = await _equipoService.GetByCiudadAsync(ciudad);

                if (!resultCiudad.Success)
                    return HandleServiceResponse(resultCiudad);

                equipos = resultCiudad.Data ?? new List<EquipoDto>();
            }

            else if (!string.IsNullOrWhiteSpace(search)) 
            {
                var resultAll = await _equipoService.GetAllAsync();

                if (!resultAll.Success)
                    return HandleServiceResponse(resultAll);

                equipos = resultAll.Data?
                    .Where(e => e.Nombre.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList() ?? new List<EquipoDto>();

            }
            else 
            {
                var resultAll = await _equipoService.GetAllAsync();

                if (!resultAll.Success)
                    return HandleServiceResponse(resultAll);

                equipos = resultAll.Data ?? new List<EquipoDto>();

            }

            var response = new BaseResponseDto<List<EquipoDto>>
            {
                Success = true,
                Data = equipos,
                Message = equipos.Any()
                    ? $"Se encontraron {equipos.Count} equipo(s)"
                    : "No se encontraron equipos"
            };

            return HandleServiceResponse(response);

        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id) 
        {
            var result = await _equipoService.GetByIdAsync(id);
            return HandleServiceResponse(result);
        }

        #endregion


        #region POST Endpoints

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>),StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateEquipoDto dto) 
        {
            var result = await _equipoService.CreateAsync(dto);

            return HandleServiceResponse(result, nameof(GetById), new { id = result.Data?.Id });
        }

        #endregion


        #region PUT Endpoints

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<EquipoDto>),StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipoDto dto) 
        {
            var result = await _equipoService.UpdateAsync(id, dto);

            return HandleServiceResponse(result);
        }

        #endregion


        #region DELETE Endpoints

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResponseDto<int>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseDto<int>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseDto<int>),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseDto<int>),StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id) 
        {
            var result = await _equipoService.DeleteAsync(id);

            return HandleServiceResponse(result);
        }

        #endregion

    }
}
