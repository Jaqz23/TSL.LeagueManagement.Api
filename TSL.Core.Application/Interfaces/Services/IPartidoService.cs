using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Partido;
using TSL.Core.Domain.Enums;

namespace TSL.Core.Application.Interfaces.Services
{
    public interface IPartidoService : IGenericService<PartidoDto, CreatePartidoDto, UpdatePartidoDto>
    {
        #region Consultas Especializadas

        // Obtiene un partido con todos sus detalles
        // Incluye: Temporada, Liga, Equipo Local, Equipo Visitante
        Task<BaseResponseDto<PartidoDto>> GetByIdWithDetallesAsync(int id);

        // Obtiene todos los partidos de una temporada específica
        // Ordenados por jornada y fecha
        Task<BaseResponseDto<List<PartidoDto>>> GetByTemporadaAsync(int temporadaId);

        // Obtiene los partidos de una jornada específica
        Task<BaseResponseDto<List<PartidoDto>>> GetByJornadaAsync(int temporadaId, int jornada);

        // Obtiene partidos filtrados por estado
        Task<BaseResponseDto<List<PartidoDto>>> GetByEstadoAsync(EstadoPartido estado);

        // Obtiene los proximos partidos programados
        Task<BaseResponseDto<List<PartidoDto>>> GetProximosPartidosAsync(int cantidad = 5);

        // Obtiene los ultimos resultados de partidos jugados
        Task<BaseResponseDto<List<PartidoDto>>> GetUltimosResultadosAsync(int cantidad = 5);

        #endregion

        #region Operaciones de Resultados

        // Registra el resultado de un partido y actualiza la tabla de posiciones
        // Se Cambia el estado a "Jugado" y recalcula posiciones
        Task<BaseResponseDto<PartidoDto>> RegistrarResultadoAsync(int id, RegistrarResultadoDto dto);

        #endregion

        #region Validaciones

        // Verifica si un equipo ya tiene partido en una jornada, ya que un equipo no puede tener dos partidos en la misma jornada
        Task<BaseResponseDto<bool>> EquipoTienePartidoEnJornadaAsync(
            int equipoId,
            int jornada,
            int temporadaId,
            int? excludePartidoId = null);

        // Valida que los equipos del partido sean diferentes
        Task<BaseResponseDto<bool>> ValidarEquiposDiferentesAsync(int equipoLocalId, int equipoVisitanteId);

        #endregion


    }
}
