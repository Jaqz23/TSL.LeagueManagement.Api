using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Liga;

namespace TSL.Core.Application.Interfaces.Services
{
    public interface ILigaService : IGenericService<LigaDto, CreateLigaDto, UpdateLigaDto>
    {
        // Obtiene una liga con sus temporadas incluidas
        Task<BaseResponseDto<LigaDto>> GetByIdWithTemporadasAsync(int id);

        // Obtiene solo las ligas que están activas en el sistema
        Task<BaseResponseDto<List<LigaDto>>> GetLigasActivasAsync();

        // Obtiene ligas activas con sus temporadas activas incluidas (para mostrar competiciones en curso)
        Task<BaseResponseDto<List<LigaDto>>> GetLigasConTemporadasActivasAsync();

        // Verifica si el nombre de liga ya existe en el sistema
        Task<BaseResponseDto<bool>> ExisteNombreAsync(string nombre, int? excludeId = null);

        #region Operaciones de Estado

        // Activa una liga permitiendo registrar nuevas temporadas
        // Regla de negocio: Solo ligas activas pueden crear temporadas
        Task<BaseResponseDto<bool>> ActivarAsync(int id);

        // Desactiva una liga impidiendo crear nuevas temporadas
        Task<BaseResponseDto<bool>> DesactivarAsync(int id);

        #endregion

    }
}
