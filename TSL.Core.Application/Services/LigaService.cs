using AutoMapper;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Liga;
using TSL.Core.Application.Interfaces;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Services
{
    public class LigaService : GenericService<Liga, LigaDto, CreateLigaDto, UpdateLigaDto> , ILigaService
    {
        public LigaService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper) 
        {
        }

        // Retorna el repositorio especifico de Liga desde el UnitOfWork
        protected override IGenericRepository<Liga> GetRepositoryAsync() 
        {
            return _unitOfWork.LigaRepository;
        }

        #region Override - Validaciones del GenericService


        protected override async Task<BaseResponseDto<bool>> ValidateCreateAsync(CreateLigaDto createDto)
        {
            var existe = await _unitOfWork.LigaRepository.ExisteNombreAsync(createDto.Nombre);

            if (existe)
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Nombre duplicado",
                    $"Ya existe una liga con el nombre '{createDto.Nombre}'"
                );
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "Validacion exitosa");
        }

        protected override async Task<BaseResponseDto<bool>> ValidateUpdateAsync(int id, UpdateLigaDto updateDto)
        {
            var existe = await _unitOfWork.LigaRepository.ExisteNombreAsync(updateDto.Nombre, id);

            if (existe) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Nombre duplicado",
                    $"Ya existe otra liga con el nombre '{updateDto.Nombre}'"
                );
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa");
        }


        protected override Task<BaseResponseDto<bool>> ValidateDeleteAsync(int id, Liga entity)
        {
            if (!entity.PuedeSerEliminada()) 
            {
                return Task.FromResult(BaseResponseDto<bool>.ErrorResponse(
                    "No se puede eliminar", 
                    "La liga tiene temporadas asociadas. Debe eliminarlas primero."
                ));
            }

            return Task.FromResult(BaseResponseDto<bool>.SuccessResponse(
                true, 
                "La liga puede ser eliminada"
            ));
        }

        #endregion


        #region Consultas Especializadas

        public async Task<BaseResponseDto<LigaDto>> GetByIdWithTemporadasAsync(int id)
        {
            try
            {
                var liga = await _unitOfWork.LigaRepository.GetByIdWithTemporadasAsync(id);

                if (liga == null) 
                {
                    return BaseResponseDto<LigaDto>.ErrorResponse(
                        "Liga no encontrada",
                        $"No existe una liga con ID {id}"
                    );
                }

                var dto = _mapper.Map<LigaDto>(liga);

                return BaseResponseDto<LigaDto>.SuccessResponse(
                    dto, 
                    "Liga obtenida correctamente con sus temporadas"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<LigaDto>.ErrorResponse(
                    "Error al obtener la liga con temporadas",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<List<LigaDto>>> GetLigasActivasAsync()
        {
            try
            {

                var ligas = await _unitOfWork.LigaRepository.GetLigasActivasAsync();
                var dtos = _mapper.Map<List<LigaDto>>(ligas);

                return BaseResponseDto<List<LigaDto>>.SuccessResponse(
                    dtos, 
                    $"Se obtuvieron {dtos.Count} ligas activas"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<LigaDto>>.ErrorResponse(
                    "Error al obtener las ligas activas",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<List<LigaDto>>> GetLigasConTemporadasActivasAsync()
        {
            try
            {
                var ligas = await _unitOfWork.LigaRepository.GetLigasConTemporadasActivasAsync();
                var dtos = _mapper.Map<List<LigaDto>>(ligas);

                return BaseResponseDto<List<LigaDto>>.SuccessResponse(
                    dtos, 
                    $"Se obtuvieron {dtos.Count} ligas con temporadas activas"
                );
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<LigaDto>>.ErrorResponse(
                    "Error al obtener las ligas con temporadas activas",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> ExisteNombreAsync(string nombre, int? excludeId = null)
        {
            try
            {
                var existe = await _unitOfWork.LigaRepository.ExisteNombreAsync(nombre, excludeId);

                return BaseResponseDto<bool>.SuccessResponse(
                    existe, 
                    existe 
                    ? $"El nombre '{nombre}' ya está en uso"  
                    : $"El nombre '{nombre}' está disponible");
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al verificar la existencia del nombre",
                    ex.Message
                );
            }
        }

        #endregion


        // Operaciones de Estado

        public async Task<BaseResponseDto<bool>> ActivarAsync(int id)
        {
            try
            {

                // Obtener la liga
                var liga = await _unitOfWork.LigaRepository.GetByIdAsync(id);

                if (liga == null)
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Liga no encontrada",
                        $"No existe una liga con ID {id}"
                    );
                }

                // Verificar si ya esta activa
                if (liga.Estado)
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Liga activa",
                        "La liga ya se encuentra activa"
                    );
                }

                liga.Activar();

                // Guardar cambios
                _unitOfWork.LigaRepository.Update(liga);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponseDto<bool>.SuccessResponse(
                    true,
                    $"La liga '{liga.Nombre}' ha sido activada correctamente"
                );


            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al activar la liga",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> DesactivarAsync(int id)
        {
            try
            {

                // Obtener la liga
                var liga = await _unitOfWork.LigaRepository.GetByIdAsync(id);

                if (liga == null)
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Liga no encontrada",
                        $"No existe una liga con ID {id}"
                    );
                }

                // Verificar si ya esta inactiva
                if (!liga.Estado)
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Liga inactiva",
                        "La liga ya se encuentra inactiva"
                    );
                }

                liga.Desactivar();

                // Guardar cambios
                _unitOfWork.LigaRepository.Update(liga);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponseDto<bool>.SuccessResponse(
                    true,
                    $"La liga '{liga.Nombre}' ha sido desactivada correctamente"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al desactivar la liga",
                    ex.Message
                );
            }
        }

    }
}
