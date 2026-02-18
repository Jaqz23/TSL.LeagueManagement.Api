using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.TablaPosicion;

namespace TSL.Core.Application.Interfaces.Services
{
    // NO hereda de IGenericService porque las posiciones se calculan automaticamente basandose en los resultados de los partidos
    public interface IPosicionEquipoService
    {
        // Obtiene la posicion de un equipo en una tabla especifica
        Task<BaseResponseDto<PosicionEquipoDto>> GetByEquipoAndTablaAsync(int equipoId, int tablaPosicionId);

        // Obtiene la posicion actual de un equipo en una temporada
        Task<BaseResponseDto<PosicionEquipoDto>> GetPosicionActualEquipoAsync(int equipoId, int temporadaId);

        // Obtiene el historial de posiciones de un equipo en diferentes temporadas
        Task<BaseResponseDto<List<PosicionEquipoDto>>> GetHistorialPosicionesEquipoAsync(int equipoId);

        // Crea una posicion inicial para un equipo en una tabla
        Task<BaseResponseDto<PosicionEquipoDto>> CrearPosicionAsync(int equipoId, int tablaPosicionId);

        // Actualiza las estadisticas de un equipo despues de un partido
        // Incrementa: Victorias/Empates/Derrotas, Goles, Partidos Jugados, Puntos
        Task<BaseResponseDto<PosicionEquipoDto>> ActualizarEstadisticasAsync(
            int equipoId,
            int tablaPosicionId,
            int golesFavor,
            int golesContra,
            string resultado);

        // Reinicia todas las estadisticas de una posicion a cero
        Task<BaseResponseDto<PosicionEquipoDto>> ReiniciarEstadisticasAsync(int posicionEquipoId);

        // Elimina todas las posiciones de una tabla
        Task<BaseResponseDto<bool>> EliminarTodasPosicionesAsync(int tablaPosicionId);

        // Verifica si un equipo ya tiene posicion en una tabla
        Task<BaseResponseDto<bool>> ExistePosicionAsync(int equipoId, int tablaPosicionId);


    }
}
