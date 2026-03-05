using AutoMapper;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Partido;
using TSL.Core.Application.Interfaces;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Entities;
using TSL.Core.Domain.Enums;

namespace TSL.Core.Application.Services
{
    public class PartidoService : GenericService<Partido, PartidoDto, CreatePartidoDto, UpdatePartidoDto> , IPartidoService
    {

        private readonly ITablaPosicionService _tablaPosicionService;

        public PartidoService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            ITablaPosicionService tablaPosicionService) 
            : base(unitOfWork, mapper)
        {
            _tablaPosicionService = tablaPosicionService ?? throw new ArgumentNullException(nameof(tablaPosicionService));
        }

        protected override IGenericRepository<Partido> GetRepositoryAsync()
        {
            return _unitOfWork.PartidoRepository;
        }

        #region Override - Validaciones del GenericService

        protected override async Task<BaseResponseDto<bool>> ValidateCreateAsync(CreatePartidoDto createDto)
        {
            // Validar que la temporada existe y esta activa

            var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(createDto.TemporadaId);

            if(temporada == null) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Temporada no encontrda", 
                    $"No existe una temporda con ID {createDto.TemporadaId}"
                );
            }

            if (!temporada.Estado) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Temporada inactica", 
                    "Solo se pueden crear partidos con temporadas activas"
                );
            }


            // Validar que los equipos sean diferentes
            if(createDto.EquipoLocalId == createDto.EquipoVisitanteId) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Equipos invalidos", 
                    "Un equipo no puede jugar contra si mismo"
                );
            }


            // Validar que ambos equipos existan
            var equipoLocal = await _unitOfWork.EquipoRepository.GetByIdAsync(createDto.EquipoLocalId);
            var equipoVisitante = await _unitOfWork.EquipoRepository.GetByIdAsync(createDto.EquipoVisitanteId);

            if (equipoLocal == null) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Equipo local no encontrado",
                    $"No existe un equipo con ID {createDto.EquipoLocalId}"
                );
            }

            if(equipoVisitante == null) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Equipo visitante no encontrado",
                    $"No existe un equipo con ID {createDto.EquipoVisitanteId}"
                );
            }

            // Validar que el equipo local no tenga otro partido en la misma jornada
            var localTienePartido = await _unitOfWork.PartidoRepository
                .EquipoTienePartidoEnJornadaAsync(
                    createDto.EquipoLocalId, 
                    createDto.Jornada,
                    createDto.TemporadaId
                );

            if (localTienePartido) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Conflicto de jornada",
                    $"El equipo '{equipoLocal.Nombre}' ya tiene un partido programado en la jornada {createDto.Jornada}"
                );
            }


            // Validar que el equipo visitante no tenga otro partido en la misma jornada
            var visitanteTienePartido = await _unitOfWork.PartidoRepository
                .EquipoTienePartidoEnJornadaAsync(
                    createDto.EquipoVisitanteId,
                    createDto.Jornada,
                    createDto.TemporadaId
                );

            if (visitanteTienePartido)
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Conflicto de jornada",
                    $"El equipo '{equipoVisitante.Nombre}' ya tiene un partido programado en la jornada {createDto.Jornada}"
                );
            }


            // Validar que la fecha este dentro del rango de la temporada
            if (!temporada.FechaEstaEnRango(createDto.Fecha)) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Fecha fuera de rango", 
                    $"La fecha del partido debe estar entre {temporada.FechaInicio:dd/MM/yyyy} y {temporada.FechaFin:dd/MM/yyyy}"
                );
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa");

        }


        protected override async Task<BaseResponseDto<bool>> ValidateUpdateAsync(int id, UpdatePartidoDto updateDto)
        {
            // Obtener el partido actual
            var partido = await _unitOfWork.PartidoRepository.GetByIdAsync(id);

            if(partido == null) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Partido no encontrado",
                    $"No existe un partido con ID {id}"
                );
            }

            // No permitir cambios si el partido ya fue jugado (excepto correcciones menores)
            if(partido.Estado == EstadoPartido.Jugado) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Partido ya jugado",
                    "No se puede modificar un partido que ya ha sido jugado. Use RegistrarResultado para actualizar el marcador."
                );
            }

            // Si se cambia la fecha, validar que este en el rango
            if (updateDto.Fecha.HasValue)
            {
                var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(partido.TemporadaId);

                if (temporada != null && !temporada.FechaEstaEnRango(updateDto.Fecha.Value))
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Fecha fuera de rango",
                        $"La fecha del partido debe estar entre {temporada.FechaInicio:dd/MM/yyyy} y {temporada.FechaFin:dd/MM/yyyy}"
                    );
                }
            }

            // Si se cambia la jornada, validar conflictos
            if (updateDto.Jornada.HasValue && updateDto.Jornada.Value != partido.Jornada) 
            {
                // validar equipo local
                var localTienePartido = await _unitOfWork.PartidoRepository
                    .EquipoTienePartidoEnJornadaAsync(
                        partido.EquipoLocalId, 
                        updateDto.Jornada.Value, 
                        partido.TemporadaId,
                        id // excluir el partido actual
                    );

                if (localTienePartido) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Conflicto de jornada",
                        $"El equipo local ya tiene un partido en la jornada {updateDto.Jornada.Value}"
                    );
                }


                // Validar equipo visitante
                var visitanteTienePartido = await _unitOfWork.PartidoRepository
                    .EquipoTienePartidoEnJornadaAsync(
                        partido.EquipoVisitanteId,
                        updateDto.Jornada.Value,
                        partido.TemporadaId,
                        id // Excluir el partido actual
                    );

                if (visitanteTienePartido)
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Conflicto de jornada",
                        $"El equipo visitante ya tiene un partido en la jornada {updateDto.Jornada.Value}"
                    );
                }


            }

            return BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa");

        }


        protected override Task<BaseResponseDto<bool>> ValidateDeleteAsync(int id, Partido entity)
        {
            // No se puede eliminar un partido jugado porque afecta la tabla de posiciones
            if(entity.Estado == EstadoPartido.Jugado) 
            {
                return Task.FromResult(BaseResponseDto<bool>.ErrorResponse(
                    "No se puede eliminar",
                    "No se puede eliminar un partido que ya ha sido jugado porque afecta la tabla de posiciones"
                ));
            }

            return Task.FromResult(BaseResponseDto<bool>.SuccessResponse(
                true, 
                "El partido puede ser eliminado"
            ));

        }


        #endregion


        #region Consultas Especializadas (Simples)

        public async Task<BaseResponseDto<PartidoDto>> GetByIdWithDetallesAsync(int id)
        {
            try
            {
                var partido = await _unitOfWork.PartidoRepository.GetByIdWithDetallesAsync(id);

                if(partido == null) 
                {
                    return BaseResponseDto<PartidoDto>.ErrorResponse(
                        "Patido no encontrado", 
                        $"No existe un partido con ID {id}"
                    );
                }

                var dto = _mapper.Map<PartidoDto>( partido );

                return BaseResponseDto<PartidoDto>.SuccessResponse(
                    dto,
                    "Partido obtenido correctamente con todos sus detalles"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<PartidoDto>.ErrorResponse(
                    "Error al obtener el partido con detalles",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<List<PartidoDto>>> GetByTemporadaAsync(int temporadaId)
        {
            try
            {
                var temporadaExists = await _unitOfWork.TemporadaRepository.ExistsAsync(t => t.Id == temporadaId);

                if (!temporadaExists) 
                {
                    return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }

                var partidos = await _unitOfWork.PartidoRepository.GetByTemporadaAsync(temporadaId);
                var dtos = _mapper.Map<List<PartidoDto>>( partidos );

                return BaseResponseDto<List<PartidoDto>>.SuccessResponse(
                    dtos,
                    $"Se obtuvieron {dtos.Count} partidos de la temporada"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                    "Error al obtener los partidos de la temporada",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<List<PartidoDto>>> GetByJornadaAsync(int temporadaId, int jornada)
        {
            try
            {
                var temporadaExists = await _unitOfWork.TemporadaRepository.ExistsAsync(t => t.Id == temporadaId);

                if (!temporadaExists) 
                {
                    return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }

                if(jornada <= 0) 
                {
                    return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                        "Jornada inválida",
                        "La jornada debe ser un número mayor a 0"
                    );
                }

                var partidos = await _unitOfWork.PartidoRepository.GetByJornadaAsync(temporadaId, jornada);
                var dtos = _mapper.Map<List<PartidoDto>>(partidos);

                return BaseResponseDto<List<PartidoDto>>.SuccessResponse(
                     dtos,
                     $"Se obtuvieron {dtos.Count} partidos de la jornada {jornada}"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                    "Error al obtener los partidos de la jornada", 
                    ex.Message
                );
            }
        }

        #endregion


        #region Consultas Especializadas (Filtros y Ordenamiento)
        public async Task<BaseResponseDto<List<PartidoDto>>> GetByEstadoAsync(EstadoPartido estado)
        {
            try
            {
                var partidos = await _unitOfWork.PartidoRepository.GetByEstadoAsync(estado);
                var dtos = _mapper.Map<List<PartidoDto>>(partidos);

                var estadoTexto = estado switch
                {
                    EstadoPartido.Programado => "programados",
                    EstadoPartido.EnCurso => "en curso",
                    EstadoPartido.Jugado => "jugados",
                    _ => "con estado desconocido"
                };

                return BaseResponseDto<List<PartidoDto>>.SuccessResponse(
                    dtos, 
                    $"Se obtuvieron {dtos.Count} partidos {estadoTexto}"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                    "Error al obtener los partidos por estado",
                    ex.Message
                );

            }
        }

        public async Task<BaseResponseDto<List<PartidoDto>>> GetProximosPartidosAsync(int cantidad = 5)
        {
            try
            {
                if (cantidad <= 0) 
                {
                    return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                        "Cantidad inválida",
                        "La cantidad debe ser un número mayor a 0"
                    );
                }

                // Limitar cantidad maxima 
                if (cantidad > 50) 
                {
                    cantidad = 50;
                }

                var partidos = await _unitOfWork.PartidoRepository.GetProximosPartidosAsync(cantidad);
                var dtos = _mapper.Map<List<PartidoDto>>(partidos);

                return BaseResponseDto<List<PartidoDto>>.SuccessResponse(
                    dtos,
                    $"Se obtuvieron los próximos {dtos.Count} partidos programados"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                    "Error al obtener los próximos partidos",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<List<PartidoDto>>> GetUltimosResultadosAsync(int cantidad = 5)
        {
            try
            {
                if (cantidad <= 0) 
                {
                    return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                        "Cantidad inválida",
                        "La cantidad debe ser un número mayor a 0"
                    );
                }

                if (cantidad > 50)
                {
                    cantidad = 50;
                }

                var partidos = await _unitOfWork.PartidoRepository.GetUltimosResultadosAsync(cantidad);
                var dtos = _mapper.Map<List<PartidoDto>>(partidos);

                return BaseResponseDto<List<PartidoDto>>.SuccessResponse(
                    dtos,
                    $"Se obtuvieron los últimos {dtos.Count} resultados"
                );
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<PartidoDto>>.ErrorResponse(
                    "Error al obtener los últimos resultados",
                    ex.Message
                );
            }
        }

        #endregion


        #region Consultas Especializadas (Registrar Resultado)

        public async Task<BaseResponseDto<PartidoDto>> RegistrarResultadoAsync(int id, RegistrarResultadoDto dto)
        {
            try
            {
                var partido = await _unitOfWork.PartidoRepository.GetByIdWithDetallesAsync(id);

                if (partido == null)
                {
                    return BaseResponseDto<PartidoDto>.ErrorResponse(
                        "Partido no encontrado",
                        $"No existe un partido con ID {id}"
                    );
                }

                if(partido.Estado == EstadoPartido.Jugado) 
                {
                    return BaseResponseDto<PartidoDto>.ErrorResponse(
                        "Partido ya jugado",
                        "Este partido ya tiene un resultado registrado. No se puede modificar nuevamente."
                    );
                }

                if (dto.GolesLocal < 0 || dto.GolesVisitante < 0)
                {
                    return BaseResponseDto<PartidoDto>.ErrorResponse(
                        "Goles inválidos",
                        "Los goles no pueden ser negativos"
                    );
                }

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    if (!partido.PuedeRegistrarResultado()) 
                    {
                        await _unitOfWork.RollbackTransactionAsync();

                        return BaseResponseDto<PartidoDto>.ErrorResponse(
                            "Error al registrar resultado",
                            "El partido no está en un estado válido para registrar resultados"
                        );
                    }

                    partido.RegistrarResultado(dto.GolesLocal, dto.GolesVisitante);

                    _unitOfWork.PartidoRepository.Update(partido);
                    await _unitOfWork.SaveChangesAsync();

                    // Actualizar la tabla de posiciones
                    var resultadoTabla = await _tablaPosicionService.ActualizarTablaPorPartidoAsync(partido.Id);

                    if (!resultadoTabla.Success) 
                    {
                        // Si falla la actualizacion de la tabla, revertir todo
                        await _unitOfWork.RollbackTransactionAsync();

                        return BaseResponseDto<PartidoDto>.ErrorResponse(
                            "Error al actualizar tabla de posiciones",
                            resultadoTabla.Errors ?? new List<string> { resultadoTabla.Message }
                        );
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    // Mapear y retornar el partido actualizado
                    var partidoDto = _mapper.Map<PartidoDto>(partido);

                    return BaseResponseDto<PartidoDto>.SuccessResponse(
                        partidoDto,
                        $"Resultado registrado correctamente: {partido.EquipoLocal.Nombre} {dto.GolesLocal} - {dto.GolesVisitante} {partido.EquipoVisitante.Nombre}"
                    );

                }
                catch (Exception innerEx) 
                {
                    // Si algo falla dentro de la transaccion, revertir
                    await _unitOfWork.RollbackTransactionAsync();

                    return BaseResponseDto<PartidoDto>.ErrorResponse(
                        "Error en la transacción",
                        $"Ocurrió un error al registrar el resultado: {innerEx.Message}"
                    );
                }

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<PartidoDto>.ErrorResponse(
                    "Error al registrar el resultado",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> EquipoTienePartidoEnJornadaAsync(
            int equipoId, 
            int jornada, 
            int temporadaId, 
            int? excludePartidoId = null)
        {
            try
            {
                var equipo = await _unitOfWork.EquipoRepository.GetByIdAsync( equipoId );

                if (equipo == null) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Equipo no encontrado", 
                        $"No existe un equipo cin ID {equipoId}"
                    );
                }

                var temportadaExists = await _unitOfWork.TemporadaRepository.ExistsAsync(t => t.Id == temporadaId);

                if (!temportadaExists) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }

                if (jornada <= 0) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Jornada inválida",
                        "La jornada debe ser un número mayor a 0"
                    );
                }

                // Verificar si el equipo tiene partido en esa jornada
                var tienePartido = await _unitOfWork.PartidoRepository.EquipoTienePartidoEnJornadaAsync(
                    equipoId, 
                    jornada, 
                    temporadaId, 
                    excludePartidoId
                );

                return BaseResponseDto<bool>.SuccessResponse(
                    tienePartido, 
                    tienePartido 
                        ? $"El equipo '{equipo.Nombre}' ya tiene un partido en la jornada {jornada}"
                        : $"El equipo '{equipo.Nombre}' está disponible en la jornada {jornada}"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al verificar disponibilidad del equipo",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> ValidarEquiposDiferentesAsync(int equipoLocalId, int equipoVisitanteId)
        {
            try
            {
                var SonDiferentes = equipoLocalId != equipoVisitanteId;

                if (!SonDiferentes) 
                {
                    return BaseResponseDto<bool>.SuccessResponse(
                        false,
                        "Un equipo no puede jugar contra sí mismo"
                    );
                }

                var equipoLocal = await _unitOfWork.EquipoRepository.GetByIdAsync(equipoLocalId);
                var equipoVisitante = await _unitOfWork.EquipoRepository.GetByIdAsync(equipoVisitanteId);

                if (equipoLocal == null) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Equipo local no encontrado",
                        $"No existe un equipo con ID {equipoLocalId}"
                    );
                }

                if (equipoVisitante == null) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Equipo visitante no encontrado",
                        $"No existe un equipo con ID {equipoVisitanteId}"
                    );
                }

                return BaseResponseDto<bool>.SuccessResponse(
                    true,
                    $"Los equipos '{equipoLocal.Nombre}' y '{equipoVisitante.Nombre}' son diferentes y válidos"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al validar los equipos",
                    ex.Message
                );
            }
        }

        #endregion

    }
}