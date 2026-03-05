using AutoMapper;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Temporada;
using TSL.Core.Application.Interfaces;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Services
{
    public class TemporadaService : GenericService<Temporada, TemporadaDto, CreateTemporadaDto, UpdateTemporadaDto>, ITemporadaService
    {
        private readonly ITablaPosicionService _tablaPosicionService;

        public TemporadaService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ITablaPosicionService tablaPosicionService)
            : base(unitOfWork, mapper)
        {
            _tablaPosicionService = tablaPosicionService ?? throw new ArgumentNullException(nameof(tablaPosicionService));
        }


        // Retorna el repositorio específico de Temporada desde el UnitOfWork
        protected override IGenericRepository<Temporada> GetRepositoryAsync() 
        {
            return _unitOfWork.TemporadaRepository;
        }

        #region Override - Validaciones del GenericService

        protected override async Task<BaseResponseDto<bool>> ValidateCreateAsync(CreateTemporadaDto createDto) 
        {
            var liga = await _unitOfWork.LigaRepository.GetByIdAsync(createDto.LigaId);

            if (liga == null) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Liga no encontrada",
                    $"No existe una liga con ID {createDto.LigaId}"
                );
            }

            if (!liga.Estado) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Liga inactiva",
                    "Solo se pueden crear temporadas en ligas activas"
                );
            }

            if (createDto.FechaInicio >= createDto.FechaFin) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Fechas inválidas",
                    "La fecha de inicio debe ser anterior a la fecha de fin"
                );
            }

            var existeOtraActiva = await _unitOfWork.TemporadaRepository
                .ExisteOtraTemporadaActivaAsync(createDto.LigaId);

            if (existeOtraActiva) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Temporada activa existente",
                    $"Ya existe una temporada activa en la liga '{liga.Nombre}'. Solo puede haber una temporada activa por liga."
                );
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa");

        }

        protected override async Task<BaseResponseDto<bool>> ValidateUpdateAsync(int id, UpdateTemporadaDto updateDto) 
        {
            var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(id);

            if (temporada == null)
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Temporada no encontrada",
                    $"No existe una temporada con ID {id}"
                );
            }

            if (updateDto.FechaInicio >= updateDto.FechaFin)
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Fechas inválidas",
                    "La fecha de inicio debe ser anterior a la fecha de fin"
                );
            }

            // Si se esta activando, validar que no exista otra activa
            if (updateDto.Estado && !temporada.Estado) // Cambio de inactiva a activa
            {
                var existeOtraActiva = await _unitOfWork.TemporadaRepository
                    .ExisteOtraTemporadaActivaAsync(temporada.LigaId, id);

                if (existeOtraActiva) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                       "Temporada activa existente",
                       "Ya existe otra temporada activa en esta liga"
                    );
                }
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa");

        }

        protected override async Task<BaseResponseDto<bool>> ValidateDeleteAsync(int id, Temporada entity) 
        {
            var tienePartidos = await _unitOfWork.TemporadaRepository.TienePartidosAsync(id);

            if (tienePartidos)
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "No se puede eliminar",
                    "La temporada tiene partidos registrados. Debe eliminarlos primero."
                );
            }

            // Metodo de negocio de la entidad como validación adicional

            if (!entity.PuedeSerEliminada())
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "No se puede eliminar",
                    "La temporada no puede ser eliminada"
                );
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "La temporada puede ser eliminada");

        }

        #endregion

        #region Override - CreateAsync con Logica Adicional

        public override async Task<BaseResponseDto<TemporadaDto>> CreateAsync(CreateTemporadaDto createDto) 
        {
            try
            {
                var validationResult = await ValidateCreateAsync(createDto);
                if (!validationResult.Success) 
                {
                    return BaseResponseDto<TemporadaDto>.ErrorResponse(
                        validationResult.Message,
                        validationResult.Errors ?? new List<string>()
                    );
                }

                // Iniciar transaccion para crear temporada + tabla
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Crear la temporada
                    var temporada = _mapper.Map<Temporada>(createDto);
                    await _unitOfWork.TemporadaRepository.AddAsync(temporada);
                    await _unitOfWork.SaveChangesAsync();

                    // Crear automaticamente la tabla de posiciones
                    var resultTabla = await _tablaPosicionService.CrearTablaAsync(temporada.Id);

                    if (!resultTabla.Success) 
                    {
                        // Si falla la creacion de la tabla, revertir todo
                        await _unitOfWork.RollbackTransactionAsync();

                        return BaseResponseDto<TemporadaDto>.ErrorResponse(
                            "Error al crear la tabla de posiciones",
                            resultTabla.Errors ?? new List<string>()
                        );
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    var dto = _mapper.Map<TemporadaDto>(temporada);

                    return BaseResponseDto<TemporadaDto>.SuccessResponse(
                        dto,
                        "Temporada creada correctamente con su tabla de posiciones"
                    );

                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TemporadaDto>.ErrorResponse(
                    "Error al crear la temporada",
                    ex.Message
                );
            }
        }


        #endregion

        #region Consultas Especializadas

        public async Task<BaseResponseDto<TemporadaDto>> GetByIdWithDetallesAsync(int id)
        {
            try
            {
                var temporada =  await _unitOfWork.TemporadaRepository.GetByIdWithDetallesAsync(id);

                if (temporada == null) 
                {
                    return BaseResponseDto<TemporadaDto>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {id}"
                    );
                }

                var dto = _mapper.Map<TemporadaDto>(temporada);

                return BaseResponseDto<TemporadaDto>.SuccessResponse(
                    dto,
                    "Temporada obtenida correctamente con todos sus detalles"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TemporadaDto>.ErrorResponse(
                    "Error al obtener la temporada con detalles",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<TemporadaDto>> GetTemporadaActivaByLigaAsync(int ligaId)
        {
            try
            {
                var temporada = await _unitOfWork.TemporadaRepository.GetTemporadaActivaByLigaAsync(ligaId);

                if (temporada == null) 
                {
                    return BaseResponseDto<TemporadaDto>.ErrorResponse(
                        "No hay temporada activa",
                        $"No existe una temporada activa para la liga con ID {ligaId}"
                    );
                }

                var dto = _mapper.Map<TemporadaDto>(temporada);
                return BaseResponseDto<TemporadaDto>.SuccessResponse(
                    dto,
                    "Temporada activa obtenida correctamente"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TemporadaDto>.ErrorResponse(
                    "Error al obtener la temporada activa",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<List<TemporadaDto>>> GetByLigaIdAsync(int ligaId)
        {
            try
            {
                var temporadas = await _unitOfWork.TemporadaRepository.GetByLigaIdAsync(ligaId);
                var dtos = _mapper.Map<List<TemporadaDto>>(temporadas);

                return BaseResponseDto<List<TemporadaDto>>.SuccessResponse(
                    dtos,
                    $"Se obtuvieron {dtos.Count} temporadas de la liga"
                );
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<TemporadaDto>>.ErrorResponse(
                    "Error al obtener las temporadas de la liga",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<List<TemporadaDto>>> GetTemporadasActivasAsync()
        {
            try
            {
                var temporadas = await _unitOfWork.TemporadaRepository.GetTemporadasActivasAsync();
                var dtos = _mapper.Map<List<TemporadaDto>>(temporadas);

                return BaseResponseDto<List<TemporadaDto>>.SuccessResponse(
                    dtos, 
                    $"Se obtuvieron {dtos.Count} temporadas activas"
                );
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<TemporadaDto>>.ErrorResponse(
                    "Error al obtener las temporadas activas",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> ExisteOtraTemporadaActivaAsync(int ligaId, int? excludeId = null)
        {
            try
            {
                var existe = await _unitOfWork.TemporadaRepository.ExisteOtraTemporadaActivaAsync(ligaId, excludeId);

                return BaseResponseDto<bool>.SuccessResponse(
                    existe,
                    existe
                        ? "Existe otra temporada activa en esta liga"
                        : "No existe otra temporada activa en esta liga"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al verificar temporadas activas",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> TienePartidosAsync(int temporadaId)
        {
            try
            {
                var tienePartidos = await _unitOfWork.TemporadaRepository.TienePartidosAsync(temporadaId);

                return BaseResponseDto<bool>.SuccessResponse(
                    tienePartidos, 
                    tienePartidos 
                    ? "La temporada tiene partidos registrados" 
                    : "La temporada no tiene partidos registrados"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al verificar partidos de la temporada",
                    ex.Message
                );
            }
        }

        #endregion

        #region Operaciones de estado

        public async Task<BaseResponseDto<bool>> FinalizarAsync(int id)
        {
            try 
            {
                var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(id);

                if(temporada == null) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {id}"
                    );
                }

                if (!temporada.Estado) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Temporada ya finalizada",
                        "La temporada ya se encuentra finalizada"
                    );
                }

                temporada.Finalizar();

                // Guardar cambios
                _unitOfWork.TemporadaRepository.Update(temporada);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponseDto<bool>.SuccessResponse(
                    true,
                    $"La temporada '{temporada.Nombre}' ha sido finalizada correctamente"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al finalizar la temporada",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> CrearTablaPosicionAsync(int temporadaId)
        {
            try
            {
                var temporada = await _unitOfWork.TemporadaRepository.GetByIdAsync(temporadaId);

                if (temporada == null) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Temporada no encontrada",
                        $"No existe una temporada con ID {temporadaId}"
                    );
                }

                var resultado = await _tablaPosicionService.CrearTablaAsync(temporadaId);

                if (!resultado.Success) 
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        resultado.Message,
                        resultado.Errors ?? new List<string>()
                    );

                }

                return BaseResponseDto<bool>.SuccessResponse(
                    true,
                    "Tabla de posiciones creada correctamente"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al crear la tabla de posiciones",
                    ex.Message
                );
            }
        }

        #endregion


    }
}
