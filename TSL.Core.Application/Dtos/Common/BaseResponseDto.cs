
namespace TSL.Core.Application.Dtos.Common
{
    // Respuesta generica para operaciones de la API

    public class BaseResponseDto<T>
    {
        // Indica si la operacion fue exitosa
        public bool Success { get; set; }

        // Mensaje descriptivo de la operacion
        public string Message { get; set; } = string.Empty;

        // Datos de la respuesta
        public T? Data { get; set; }

        // Lista de errores (si los hay)
        public List<string>? Errors { get; set; }


        #region Factory Methods

        // Crea una respuesta exitosa
        public static BaseResponseDto<T> SuccessResponse(T data, string message = "Operación exitosa") 
        {
            return new BaseResponseDto<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        // Crea una respuesta de error
        public static BaseResponseDto<T> ErrorResponse(string message, List<string>? errors = null) 
        {
            return new BaseResponseDto<T> 
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        // Crea una respuesta de error con un solo mensaje
        public static BaseResponseDto<T> ErrorResponse(string message, string error)
        {
            return new BaseResponseDto<T>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error }
            };
        }

        #endregion
    }
}
