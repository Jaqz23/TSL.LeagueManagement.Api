using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Equipo;

namespace TSL.Core.Application.Interfaces.Services
{
    public interface IEquipoService : IGenericService<EquipoDto, CreateEquipoDto, UpdateEquipoDto>
    {
        #region Consultas Especializadas

        // Obtiene un equipo con todos sus partidos (local + visitante)
        Task<BaseResponseDto<EquipoDto>> GetByIdWithPartidosAsync(int id);

        // Obtiene un equipo con sus posiciones en todas las tablas
        // Para ver el rendimiento histórico del equipo
        Task<BaseResponseDto<EquipoDto>> GetByIdWithPosicionesAsync(int id);

        // Obtiene equipos filtrados por ciudad
        Task<BaseResponseDto<List<EquipoDto>>> GetByCiudadAsync(string ciudad);

        #endregion

        #region Validaciones

        // Verifica si el nombre del equipo ya existe en el sistema
        Task<BaseResponseDto<bool>> ExisteNombreAsync(string nombre, int? excludeId = null);

        // Verifica si el equipo tiene partidos asociados
        Task<BaseResponseDto<bool>> TienePartidosAsync(int equipoId);

        #endregion

    }
}
