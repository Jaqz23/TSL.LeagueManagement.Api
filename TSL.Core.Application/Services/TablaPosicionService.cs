using AutoMapper;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.TablaPosicion;
using TSL.Core.Application.Interfaces;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Entities;
using TSL.Core.Domain.Enums;

namespace TSL.Core.Application.Services
{
    public class TablaPosicionService : ITablaPosicionService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        private readonly IPosicionEquipoService _posicionEquipoService;

        public TablaPosicionService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IPosicionEquipoService posicionEquipoService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _posicionEquipoService = posicionEquipoService ?? throw new ArgumentNullException(nameof(posicionEquipoService));
        }


        #region Consultas

        public async Task<BaseResponseDto<TablaPosicionDto>> GetByTemporadaIdWithPosicionesAsync(int temporadaId)
        {
            try 
            {
                var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(temporadaId);

                if (temporada == null) 
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }

                // Obtener la tabla de posiciones con todas sus posiciones y relaciones
                var tabla = await _unitOfWork.TablaPosicionRepository.GetByTemporadaIdWithPosicionesAsync(temporadaId);

                if (tabla == null) 
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones para la temporada '{temporada.Nombre}'. " +
                        "La tabla se crea automáticamente al crear la temporada."
                    );
                }

                var dto = _mapper.Map<TablaPosicionDto>(tabla);
                // Asignar nUmeros de posicion (1, 2, 3, etc.)
                // Las posiciones ya vienen ordenadas del repositorio
                int posicion = 1;
                foreach(var posicionEquipo in dto.Posiciones) 
                {
                    posicionEquipo.Posicion = posicion++;
                }

                return BaseResponseDto<TablaPosicionDto>.SuccessResponse(
                    dto,
                    $"Tabla de posiciones obtenida correctamente con {dto.Posiciones.Count} equipos"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                    "Error al obtener la tabla de posiciones",
                    ex.Message
                );

            }
        }

        public async Task<BaseResponseDto<bool>> ExisteTablaAsync(int temporadaId)
        {
            try
            {
                var temporadaExists = await _unitOfWork.TemporadaRepository.ExistsAsync(t => t.Id == temporadaId);

                if (!temporadaExists) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }

                var existe = await _unitOfWork.TablaPosicionRepository.ExisteParaTemporadaAsync(temporadaId);

                return BaseResponseDto<bool>.SuccessResponse(
                    existe,
                    existe
                        ? "La tabla de posiciones existe para esta temporada"
                        : "No existe tabla de posiciones para esta temporada"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                   "Error al verificar la existencia de la tabla",
                   ex.Message
               );
            }
        }

        #endregion


        #region Operaciones de Creacion

        public async Task<BaseResponseDto<TablaPosicionDto>> CrearTablaAsync(int temporadaId)
        {
            try
            {
                var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(temporadaId);

                if (temporada == null)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }


                var tablaExiste = await _unitOfWork.TablaPosicionRepository.ExisteParaTemporadaAsync(temporadaId);

                if (tablaExiste)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Tabla ya existe",
                        $"Ya existe una tabla de posiciones para la temporada '{temporada.Nombre}'"
                    );
                }

                // Crear la tabla vacia
                var nuevaTabla = new TablaPosicion
                {
                    TemporadaId = temporadaId
                };

                await _unitOfWork.TablaPosicionRepository.AddAsync(nuevaTabla);
                await _unitOfWork.SaveChangesAsync();

                // Recargar con relaciones para mapear correctamente
                var tablaCreada = await _unitOfWork.TablaPosicionRepository.GetByTemporadaIdWithPosicionesAsync(temporadaId);

                if (tablaCreada == null) 
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Error al crear la tabla",
                        "La tabla se creó pero no se pudo recuperar de la base de datos"
                    );
                }

                // Mapear y retornar
                var dto = _mapper.Map<TablaPosicionDto>(tablaCreada);

                return BaseResponseDto<TablaPosicionDto>.SuccessResponse(
                    dto,
                    $"Tabla de posiciones creada correctamente para la temporada '{temporada.Nombre}'"
                );


            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                    "Error al crear la tabla de posiciones",
                    ex.Message
                );
            }
        }

        #endregion 


        #region Actualizacion Incremental

        public async Task<BaseResponseDto<TablaPosicionDto>> ActualizarTablaPorPartidoAsync(int partidoId)
        {
            try
            {
                var partido = await _unitOfWork.PartidoRepository.GetByIdWithDetallesAsync(partidoId);

                if (partido == null) 
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Partido no encontrado",
                        $"No existe un partido con ID {partidoId}"
                    );
                }

                if (partido.Estado != EstadoPartido.Jugado) 
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Partido no jugado", 
                        "Solo se puede actualizar la tabla con partidos que ya han sido jugados"
                    );
                }

                // Validar que los goles no sean null
                if (!partido.GolesLocal.HasValue || !partido.GolesVisitante.HasValue)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Datos de partido incompletos",
                        "El partido no tiene goles registrados. Verifique que el resultado se haya registrado correctamente."
                    );
                }

                // Obtener la tabla de posiciones
                var tabla = await _unitOfWork.TablaPosicionRepository.GetByTemporadaIdWithPosicionesAsync(partido.TemporadaId);
                
                if (tabla == null) 
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones para la temporada con ID {partido.TemporadaId}"
                    );
                }

                // Determinar el resultado del partido
                string resultadoLocal;
                string resultadoVisitante;

                if (partido.GolesLocal > partido.GolesVisitante) 
                {
                    // victoria del equipo local
                    resultadoLocal = "victoria";
                    resultadoVisitante = "derrota";
                }
                else if (partido.GolesLocal < partido.GolesVisitante) 
                {
                    // victoria del equipo visitante
                    resultadoLocal = "derrota";
                    resultadoVisitante = "victoria";
                }
                else 
                {
                    // empate
                    resultadoLocal = "empate";
                    resultadoVisitante = "empate";
                }

                // Actualizar posicion del equipo local
                var resultadoActualizacionLocal = await _posicionEquipoService.ActualizarEstadisticasAsync(
                    equipoId: partido.EquipoLocalId,
                    tablaPosicionId: tabla.Id,
                    golesFavor: partido.GolesLocal.Value,
                    golesContra: partido.GolesVisitante.Value,
                    resultado: resultadoLocal
                );

                if (!resultadoActualizacionLocal.Success)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Error al actualizar equipo local",
                        resultadoActualizacionLocal.Errors ?? new List<string> { resultadoActualizacionLocal.Message }
                    );
                }

                // Actualizar posicion del equipo visitante
                var resultadoActualizacionVisitante = await _posicionEquipoService.ActualizarEstadisticasAsync(
                    equipoId: partido.EquipoVisitanteId,
                    tablaPosicionId: tabla.Id,
                    golesFavor: partido.GolesVisitante.Value,
                    golesContra: partido.GolesLocal.Value,   
                    resultado: resultadoVisitante
                );


                if (!resultadoActualizacionVisitante.Success)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Error al actualizar equipo visitante",
                        resultadoActualizacionVisitante.Errors ?? new List<string> { resultadoActualizacionVisitante.Message }
                    );
                }

                // Recargar la tabla con posiciones actualizadas
                var tablaActualizada = await _unitOfWork.TablaPosicionRepository.GetByTemporadaIdWithPosicionesAsync(partido.TemporadaId);

                if (tablaActualizada == null) 
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Error al recargar la tabla",
                        "La tabla se actualizó pero no se pudo recuperar"
                    );
                }

                // Mapear, asignar posiciones y retornar
                var dto = _mapper.Map<TablaPosicionDto>( tablaActualizada );

                // Asignar números de posicion
                int posicion = 1;

                foreach(var posicionEquipo in dto.Posiciones) 
                {
                    posicionEquipo.Posicion = posicion++;
                }

                return BaseResponseDto<TablaPosicionDto>.SuccessResponse(
                    dto,
                    $"Tabla actualizada correctamente. Resultado: {partido.EquipoLocal.Nombre} {partido.GolesLocal.Value} - {partido.GolesVisitante.Value} {partido.EquipoVisitante.Nombre}"
                );


            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                    "Error al actualizar la tabla de posiciones",
                    ex.Message
                );
            }
        }

        #endregion


        #region Recalcular Completo

        public async Task<BaseResponseDto<TablaPosicionDto>> RecalcularTablaAsync(int temporadaId)
        {
            try
            {
                var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(temporadaId);

                if (temporada == null)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }


                var tabla = await _unitOfWork.TablaPosicionRepository.GetByTemporadaIdWithPosicionesAsync(temporadaId);

                if (tabla == null)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Tabla de posiciones no encontrada",
                        $"No existe una tabla de posiciones para la temporada '{temporada.Nombre}'"
                    );
                }

                // Eliminar todas las posiciones actuales (resetear)
                var resultadoEliminar = await _posicionEquipoService.EliminarTodasPosicionesAsync(tabla.Id);

                if (!resultadoEliminar.Success)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Error al resetear la tabla",
                        resultadoEliminar.Errors ?? new List<string> { resultadoEliminar.Message }
                    );
                }

                // Obtener TODOS los partidos JUGADOS de la temporada
                var partidos = await _unitOfWork.PartidoRepository.GetPartidosJugadosAsync(temporadaId);

                if (partidos == null || !partidos.Any()) 
                {
                    // No hay partidos jugados, tabla queda vacia (valido para temporadas nuevas)
                    var tablaVacia = await _unitOfWork.TablaPosicionRepository.GetByTemporadaIdWithPosicionesAsync(temporadaId);

                    var dtoVacia = _mapper.Map<TablaPosicionDto>(tablaVacia);

                    return BaseResponseDto<TablaPosicionDto>.SuccessResponse(
                        dtoVacia,
                        $"Tabla recalculada correctamente. No hay partidos jugados en la temporada '{temporada.Nombre}'"
                    );

                }

                // Procesar cada partido y actualizar estadisticas
                int partidosProcesados = 0;
                int errores = 0;
                List<string> mensajesError = new List<string>();

                foreach (var partido in partidos) 
                {
                    if(!partido.GolesLocal.HasValue || !partido.GolesVisitante.HasValue) 
                    {
                        errores++;
                        mensajesError.Add($"Partido ID {partido.Id}: Goles no registrados");
                        continue; // Saltar este partido
                    }

                    string resultadoLocal;
                    string resultadoVisitante;

                    if (partido.GolesLocal.Value > partido.GolesVisitante.Value)
                    {
                        resultadoLocal = "victoria";
                        resultadoVisitante = "derrota";
                    }
                    else if (partido.GolesLocal.Value < partido.GolesVisitante.Value)
                    {
                        resultadoLocal = "derrota";
                        resultadoVisitante = "victoria";
                    }
                    else
                    {
                        resultadoLocal = "empate";
                        resultadoVisitante = "empate";
                    }

                    // Actualizar equipo local
                    var resultadoLocal_Update = await _posicionEquipoService.ActualizarEstadisticasAsync(
                        equipoId: partido.EquipoLocalId,
                        tablaPosicionId: tabla.Id,
                        golesFavor: partido.GolesLocal.Value,
                        golesContra: partido.GolesVisitante.Value,
                        resultado: resultadoLocal
                    );

                    if (!resultadoLocal_Update.Success)
                    {
                        errores++;
                        mensajesError.Add($"Partido ID {partido.Id} - Equipo Local: {resultadoLocal_Update.Message}");
                        continue; // Saltar a siguiente partido
                    }

                    // Actualizar equipo visitante
                    var resultadoVisitante_Update = await _posicionEquipoService.ActualizarEstadisticasAsync(
                        equipoId: partido.EquipoVisitanteId,
                        tablaPosicionId: tabla.Id,
                        golesFavor: partido.GolesVisitante.Value,
                        golesContra: partido.GolesLocal.Value,
                        resultado: resultadoVisitante
                    );

                    if (!resultadoVisitante_Update.Success) 
                    {
                        errores++;
                        mensajesError.Add($"Partido ID {partido.Id} - Equipo Visitante: {resultadoVisitante_Update.Message}");
                        continue;
                    }

                    partidosProcesados++;

                }

                // Recargar tabla recalculada
                var tablaRecalculada = await _unitOfWork.TablaPosicionRepository.GetByTemporadaIdWithPosicionesAsync(temporadaId);

                if (tablaRecalculada == null)
                {
                    return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                        "Error al recargar la tabla",
                        "La tabla se recalculó pero no se pudo recuperar"
                    );
                }

                var dto = _mapper.Map<TablaPosicionDto>(tablaRecalculada);

                int posicion = 1;
                foreach (var posicionEquipo in dto.Posiciones)
                {
                    posicionEquipo.Posicion = posicion++;
                }

                // Mensaje de resultado
                string mensaje = $"Tabla recalculada correctamente. {partidosProcesados} partidos procesados";

                if (errores > 0) 
                {
                    mensaje += $". {errores} partidos con errores (ignorados)";
                }

                return new BaseResponseDto<TablaPosicionDto>
                {
                    Success = true,
                    Data = dto,
                    Message = mensaje,
                    Errors = errores > 0 ? mensajesError : null
                };

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TablaPosicionDto>.ErrorResponse(
                    "Error al recalcular la tabla de posiciones",
                    ex.Message
                );
            }
        }

        #endregion

    }
}
