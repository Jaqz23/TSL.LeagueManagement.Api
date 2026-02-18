using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Temporada;

namespace TSL.Core.Application.Interfaces.Services
{
    public interface ITemporadaService : IGenericService<TemporadaDto, CreateTemporadaDto, UpdateTemporadaDto>
    {
        #region Consultas Especializadas

        // Obtiene una temporada con todos sus datos relacionados
        Task<BaseResponseDto<TemporadaDto>> GetByIdWithDetallesAsync(int id);

        // Obtiene la temporada activa de una liga específica
        Task<BaseResponseDto<TemporadaDto>> GetTemporadaActivaByLigaAsync(int ligaId);

        // Obtiene todas las temporadas de una liga ordenadas por fecha (para ver el historial completo de una competición)
        Task<BaseResponseDto<List<TemporadaDto>>> GetByLigaIdAsync(int ligaId);

        // Obtiene todas las temporadas activas del sistema
        Task<BaseResponseDto<List<TemporadaDto>>> GetTemporadasActivasAsync();

        #endregion

        #region Validaciones

        // Verifica si existe otra temporada activa en la misma liga
        //Validación critica: Una liga no puede tener dos temporadas activas simultaneas
        Task<BaseResponseDto<bool>> ExisteOtraTemporadaActivaAsync(int ligaId, int? excludeId = null);

        // Verifica si la temporada tiene partidos registrados
        Task<BaseResponseDto<bool>> TienePartidosAsync(int temporadaId);

        #endregion

        #region Operaciones de Estado

        // Finaliza una temporada cambiando su estado a inactiva
        Task<BaseResponseDto<bool>> FinalizarAsync(int id);

        // Crea automaticamente la tabla de posiciones al crear la temporada
        Task<BaseResponseDto<bool>> CrearTablaPosicionAsync(int temporadaId);

        #endregion

    }
}
