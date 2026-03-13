using Microsoft.AspNetCore.Mvc;
using TSL.Core.Application.Dtos.Common;

namespace TSL.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        #region Responses

        protected IActionResult OkResponse <T>(BaseResponseDto<T> response) 
        {
            return Ok(response);
        }

        protected IActionResult CreatedResponse<T>(string actionName, object routeValues, BaseResponseDto<T> response) 
        {
            return CreatedAtAction(actionName, routeValues, response);
        }

        protected IActionResult BadRequestResponse<T>(BaseResponseDto<T> response) 
        {
            return BadRequest(response);
        }

        protected IActionResult NotFoundResponse<T>(BaseResponseDto<T> response)
        {
            return NotFound(response);
        }

        protected IActionResult NoContentResponse()
        {
            return NoContent();
        }

        protected IActionResult InternalServerErrorResponse<T>(BaseResponseDto<T> response) 
        {
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        #endregion

        // Maneja la respuesta del servicio y retorna el codigo HTTP apropiado
        protected IActionResult HandleServiceResponse<T>(
            BaseResponseDto<T> response, 
            string? createdActionName = null,
            object? routeValues = null) 
        {
            if (!response.Success) 
            {

                bool esNotFound = EsNotFound(response.Message, response.Errors);

                if (esNotFound) 
                {
                    return NotFoundResponse(response);
                }

                if (response.Errors != null && response.Errors.Any())
                {
                    return BadRequestResponse(response);
                }

                return InternalServerErrorResponse(response);

            }

            if (!string.IsNullOrEmpty(createdActionName) && routeValues != null)
            {
                return CreatedResponse(createdActionName, routeValues, response);
            }

            if (response.Data == null)
            {
                return NoContentResponse();
            }

            // Para listas, verificar si esta vacia
            if(response.Data is System.Collections.IEnumerable enumerable) 
            {
                // Convertir a lista para contar elementos
                var enumerator = enumerable.GetEnumerator();
                if (!enumerator.MoveNext()) 
                {
                    return NoContentResponse();
                }
            }

            return OkResponse(response);

        }

        private bool EsNotFound(string message, List<string>? errors) 
        {
            var patrones = new[]
            {
                "no encontrad",   // "Liga no encontrada", "Temporada no encontrada"
                "no exist",       // "No existe un registro..."
                "no tiene",       // "La liga no tiene una temporada activa"
                "no hay"          // "No hay temporada activa"
            };

            if (!string.IsNullOrEmpty(message)) 
            {
                foreach (var patron in patrones) 
                {
                    if (message.Contains(patron, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            if (errors != null && errors.Any()) 
            {
                foreach (var error in errors) 
                {
                    foreach(var patron in patrones) 
                    {
                        if (error.Contains(patron, StringComparison.OrdinalIgnoreCase)) 
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

    }
}
