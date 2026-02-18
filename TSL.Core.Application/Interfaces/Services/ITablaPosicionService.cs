using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.TablaPosicion;

namespace TSL.Core.Application.Interfaces.Services
{
    // NO hereda de IGenericService porque la tabla se crea y actualiza automaticamente con los resultados de los partidos (no es CRUD manual)
    public interface ITablaPosicionService
    {
        // Obtiene la tabla con todas las posiciones ordenadas
        // Ordenamiento segun criterios deportivos:
        // 1. Puntos DESC
        // 2. Diferencia de goles DESC
        // 3. Goles a favor DESC
        // 4. Nombre alfabético ASC
        Task<BaseResponseDto<TablaPosicionDto>> GetByTemporadaIdWithPosicionesAsync(int temporadaId);

        // Crea la tabla de posiciones para una temporada nueva
        // Se ejecuta automaticamente al crear una temporada
        Task<BaseResponseDto<TablaPosicionDto>> CrearTablaAsync(int temporadaId);

        // Recalcula completamente la tabla de posiciones desde cero
        // Para corregir inconsistencias o después de modificaciones masivas
        // Proceso:
        // 1. Elimina todas las posiciones actuales
        // 2. Recorre todos los partidos jugados de la temporada
        // 3. Recalcula estadísticas de cada equipo
        // 4. Ordena según criterios deportivos
        Task<BaseResponseDto<TablaPosicionDto>> RecalcularTablaAsync(int temporadaId);

        // Actualiza la tabla después de registrar un resultado
        Task<BaseResponseDto<TablaPosicionDto>> ActualizarTablaPorPartidoAsync(int partidoId);

        // Verifica si existe una tabla de posiciones para la temporada
        Task<BaseResponseDto<bool>> ExisteTablaAsync(int temporadaId);

    }
}
