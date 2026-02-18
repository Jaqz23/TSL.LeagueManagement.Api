using TSL.Core.Application.Dtos.Common;

namespace TSL.Core.Application.Interfaces.Services
{
    public interface IGenericService<TDto, TCreateDto,  TUpdateDto>
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        // Obtiene todos los registros
        Task<BaseResponseDto<List<TDto>>> GetAllAsync();

        // Obtiene un registro por su ID
        Task<BaseResponseDto<TDto>> GetByIdAsync(int id);

        // Verifica si existe un registro con el ID especificado
        Task<BaseResponseDto<bool>> ExistsAsync(int id);

        // Crea un nuevo registro
        Task<BaseResponseDto<TDto>> CreateAsync(TCreateDto createDto);

        // Actualiza un registro existente
        Task<BaseResponseDto<TDto>> UpdateAsync(int id, TUpdateDto updateDto);

        // Elimina un registro por su ID
        Task<BaseResponseDto<int>> DeleteAsync(int id);
    }
}
