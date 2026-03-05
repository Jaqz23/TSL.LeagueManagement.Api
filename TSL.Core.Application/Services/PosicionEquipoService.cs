using AutoMapper;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.TablaPosicion;
using TSL.Core.Application.Interfaces;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Services
{
    public class PosicionEquipoService : IPosicionEquipoService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public PosicionEquipoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Consultas

        public async Task<BaseResponseDto<PosicionEquipoDto>> GetByEquipoAndTablaAsync(int equipoId, int tablaPosicionId)
        {
            try
            {
                var equipo = await _unitOfWork.EquipoRepository.GetByIdAsync(equipoId);

                if (equipo == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Equipo no encontrado",
                        $"No existe un equipo con ID {equipoId}"
                    );
                }

                var tablaExists = await _unitOfWork.TablaPosicionRepository.ExistsAsync(t => t.Id == tablaPosicionId);

                if (!tablaExists)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones con ID {tablaPosicionId}"
                    );
                }

                // Buscar la posicion del equipo en la tabla
                var posicion = await _unitOfWork.PosicionEquipoRepository.GetByEquipoAndTablaAsync(equipoId, tablaPosicionId);

                if (posicion == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Posición no encontrada",
                        $"No existe una posición para el equipo '{equipo.Nombre}' en esta tabla"
                    );
                }

                var dto = _mapper.Map<PosicionEquipoDto>(posicion);

                return BaseResponseDto<PosicionEquipoDto>.SuccessResponse(
                    dto,
                    $"Posición del equipo '{equipo.Nombre}' obtenida correctamente"
                );


            }
            catch (Exception ex) 
            {
                return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                    "Error al obtener la posición del equipo",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<PosicionEquipoDto>> GetPosicionActualEquipoAsync(int equipoId, int temporadaId)
        {
            try
            {
                var equipo = await _unitOfWork.EquipoRepository.GetByIdAsync(equipoId);

                if (equipo == null) 
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Equipo no encontrado",
                        $"No existe un equipo con ID {equipoId}"
                    );
                }

                var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(temporadaId);

                if (temporada == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }

                // Obtener la tabla de la temporada
                var tabla = await _unitOfWork.TablaPosicionRepository.GetByTemporadaIdAsync(temporadaId);

                if (tabla == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones para la temporada '{temporada.Nombre}'"
                    );
                }

                // Buscar la posicion del equipo
                var posicion = await _unitOfWork.PosicionEquipoRepository.GetByEquipoAndTablaAsync(equipoId, tabla.Id);

                if (posicion == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Posición no encontrada",
                        $"El equipo '{equipo.Nombre}' no tiene posición en la temporada '{temporada.Nombre}'"
                    );
                }

                var dto = _mapper.Map<PosicionEquipoDto>(posicion);
                
                return BaseResponseDto<PosicionEquipoDto>.SuccessResponse(
                    dto, 
                    $"Posición actual del equipo '{equipo.Nombre}' en la temporada '{temporada.Nombre}'");

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                    "Error al obtener la posición actual del equipo",
                    ex.Message
                );

            }
        }

        public async Task<BaseResponseDto<List<PosicionEquipoDto>>> GetHistorialPosicionesEquipoAsync(int equipoId)
        {
            try
            {
                var equipo = await _unitOfWork.EquipoRepository.GetByIdAsync(equipoId);

                if (equipo == null)
                {
                    return BaseResponseDto<List<PosicionEquipoDto>>.ErrorResponse(
                        "Equipo no encontrado",
                        $"No existe un equipo con ID {equipoId}"
                    );
                }

                // Obtener todas las posiciones del equipo
                var posiciones = await _unitOfWork.PosicionEquipoRepository.GetHistorialByEquipoAsync(equipoId);

                var dtos = _mapper.Map<List<PosicionEquipoDto>>(posiciones);

                return BaseResponseDto<List<PosicionEquipoDto>>.SuccessResponse(
                    dtos,
                    $"Se obtuvieron {dtos.Count} posiciones del historial del equipo '{equipo.Nombre}'"
                );


            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<PosicionEquipoDto>>.ErrorResponse(
                    "Error al obtener el historial de posiciones del equipo",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> ExistePosicionAsync(int equipoId, int tablaPosicionId)
        {
            try
            {
                var equipoExists = await _unitOfWork.EquipoRepository.ExistsAsync(e => e.Id == equipoId);

                if (!equipoExists)
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Equipo no encontrado",
                        $"No existe un equipo con ID {equipoId}"
                    );
                }

                var tablaExists = await _unitOfWork.TablaPosicionRepository.ExistsAsync(t => t.Id == tablaPosicionId);

                if (!tablaExists)
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones con ID {tablaPosicionId}"
                    );
                }

                // Verificar si existe la posicion
                var existe = await _unitOfWork.PosicionEquipoRepository.ExistePosicionAsync(equipoId, tablaPosicionId);

                return BaseResponseDto<bool>.SuccessResponse(
                    existe,
                    existe
                        ? "El equipo ya tiene una posición en esta tabla"
                        : "El equipo no tiene posición en esta tabla"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al verificar la existencia de la posición",
                    ex.Message
                );
            }
        }

        #endregion


        #region Operacion de Creacion

        public async Task<BaseResponseDto<PosicionEquipoDto>> CrearPosicionAsync(int equipoId, int tablaPosicionId)
        {
            try
            {
                var equipo = await _unitOfWork.EquipoRepository.GetByIdAsync(equipoId);

                if (equipo == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Equipo no encontrado",
                        $"No existe un equipo con ID {equipoId}"
                    );
                }

                var tabla = await _unitOfWork.TablaPosicionRepository.GetByIdAsync(tablaPosicionId);

                if (tabla == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones con ID {tablaPosicionId}"
                    );
                }

                var existePosicion = await _unitOfWork.PosicionEquipoRepository.ExistePosicionAsync(equipoId, tablaPosicionId);

                if (existePosicion)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Posición ya existe",
                        $"El equipo '{equipo.Nombre}' ya tiene una posición en esta tabla"
                    );
                }

                // Crear posicion inicial con estadisticas en 0
                var nuevaPosicion = new PosicionEquipo
                {
                    EquipoId = equipoId,
                    TablaPosicionId = tablaPosicionId,
                    PartidosJugados = 0,
                    Victorias = 0,
                    Empates = 0,
                    Derrotas = 0,
                    GolesAFavor = 0,
                    GolesEnContra = 0,
                    Puntos = 0
                };

                await _unitOfWork.PosicionEquipoRepository.AddAsync(nuevaPosicion);
                await _unitOfWork.SaveChangesAsync();

                var posicionCreada = await _unitOfWork.PosicionEquipoRepository.GetByEquipoAndTablaAsync(equipoId, tablaPosicionId);

                if (posicionCreada == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Error al crear la posición",
                        "La posición se creó pero no se pudo recuperar de la base de datos"
                    );
                }

                var dto = _mapper.Map<PosicionEquipoDto>(posicionCreada);

                return BaseResponseDto<PosicionEquipoDto>.SuccessResponse(
                    dto,
                    $"Posición inicial creada para el equipo '{equipo.Nombre}'"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                    "Error al crear la posición del equipo",
                    ex.Message
                );
            }
        }

        #endregion


        #region Actualizar Estadisticas

        public async Task<BaseResponseDto<PosicionEquipoDto>> ActualizarEstadisticasAsync(
            int equipoId, 
            int tablaPosicionId, 
            int golesFavor, 
            int golesContra, 
            string resultado)
        {
            try
            {
                var equipo = await _unitOfWork.EquipoRepository.GetByIdAsync(equipoId);

                if (equipo == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Equipo no encontrado",
                        $"No existe un equipo con ID {equipoId}"
                    );
                }

                var tablaExists = await _unitOfWork.TablaPosicionRepository.ExistsAsync(t => t.Id == tablaPosicionId);

                if (!tablaExists)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones con ID {tablaPosicionId}"
                    );
                }

                if (golesFavor < 0 || golesContra < 0) 
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Goles inválidos",
                        "Los goles no pueden ser negativos"
                    );
                }

                resultado = resultado.ToLower().Trim();

                if (resultado != "victoria" && resultado != "empate" && resultado != "derrota") 
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Resultado inválido", 
                        "El resultado debe ser 'victoria', 'empate' o 'derrota'"
                    );
                }

                // Buscar posicion del equipo
                var posicion = await _unitOfWork.PosicionEquipoRepository.GetByEquipoAndTablaAsync(equipoId, tablaPosicionId);

                // Si no existe, crear posicion automaticamente
                if (posicion == null) 
                {
                    var resultadoCrear = await CrearPosicionAsync(equipoId, tablaPosicionId);

                    if (!resultadoCrear.Success)
                    {
                        return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                            "Error al crear posición",
                            resultadoCrear.Errors ?? new List<string> { resultadoCrear.Message }
                        );
                    }

                    // Recargar posicion recien creada
                    posicion = await _unitOfWork.PosicionEquipoRepository.GetByEquipoAndTablaAsync(equipoId, tablaPosicionId);

                    if (posicion == null)
                    {
                        return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                            "Error al recuperar posición",
                            "La posición se creó pero no se pudo recuperar"
                        );
                    }

                }

                posicion.PartidosJugados++;

                posicion.GolesAFavor += golesFavor;
                posicion.GolesEnContra += golesContra;

                // Actualizar victorias/empates/derrotas y puntos segun resultado
                switch (resultado) 
                {
                    case "victoria":
                        posicion.Victorias++;
                        posicion.Puntos += 3;
                        break;

                    case "empate":
                        posicion.Empates++;
                        posicion.Puntos += 1;
                        break;

                    case "derrota":
                        posicion.Derrotas++;
                        // 0 puntos
                        break;
                }

                _unitOfWork.PosicionEquipoRepository.Update(posicion);
                await _unitOfWork.SaveChangesAsync();

                var posicionActualizada = await _unitOfWork.PosicionEquipoRepository.GetByEquipoAndTablaAsync(equipoId, tablaPosicionId);

                if (posicionActualizada == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Error al recargar posición",
                        "La posición se actualizó pero no se pudo recuperar"
                    );
                }

                var dto = _mapper.Map<PosicionEquipoDto>( posicionActualizada );

                return BaseResponseDto<PosicionEquipoDto>.SuccessResponse(
                    dto,
                    $"Estadísticas actualizadas para '{equipo.Nombre}': {resultado} ({golesFavor}-{golesContra})"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                    "Error al actualizar estadísticas del equipo",
                    ex.Message
                );
            }
        }

        #endregion


        #region Operaciones de Mantenimiento

        public async Task<BaseResponseDto<PosicionEquipoDto>> ReiniciarEstadisticasAsync(int posicionEquipoId)
        {
            try
            {
                var posicion = await _unitOfWork.PosicionEquipoRepository.GetByIdAsync(posicionEquipoId);

                if (posicion == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Posición no encontrada",
                        $"No existe una posición con ID {posicionEquipoId}"
                    );
                }

                // Resetear todas las estadisticas a 0
                posicion.PartidosJugados = 0;
                posicion.Victorias = 0;
                posicion.Empates = 0;
                posicion.Derrotas = 0;
                posicion.GolesAFavor = 0;
                posicion.GolesEnContra = 0;
                posicion.Puntos = 0;

                _unitOfWork.PosicionEquipoRepository.Update(posicion);
                await _unitOfWork.SaveChangesAsync();

                var posicionActualizada = await _unitOfWork.PosicionEquipoRepository
                    .GetByEquipoAndTablaAsync(posicion.EquipoId, posicion.TablaPosicionId);

                if (posicionActualizada == null)
                {
                    return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                        "Error al recargar posición",
                        "La posición se reinició pero no se pudo recuperar"
                    );
                }

                var dto = _mapper.Map<PosicionEquipoDto>( posicionActualizada );

                return BaseResponseDto<PosicionEquipoDto>.SuccessResponse(
                    dto,
                    "Estadísticas reiniciadas correctamente"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<PosicionEquipoDto>.ErrorResponse(
                    "Error al reiniciar las estadísticas",
                    ex.Message
                );
            }
        }


        public async Task<BaseResponseDto<bool>> EliminarTodasPosicionesAsync(int tablaPosicionId)
        {
            try
            {
                var tabla = await _unitOfWork.TablaPosicionRepository.GetByIdAsync(tablaPosicionId);

                if (tabla == null)
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones con ID {tablaPosicionId}"
                    );
                }

                // Eliminar posiciones de la tabla
                await _unitOfWork.PosicionEquipoRepository.RemoveAllByTablaAsync(tablaPosicionId);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponseDto<bool>.SuccessResponse(
                    true,
                    "Todas las posiciones fueron eliminadas correctamente"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al eliminar las posiciones",
                       ex.Message
                );
            }

        }


        #endregion

    }
}
