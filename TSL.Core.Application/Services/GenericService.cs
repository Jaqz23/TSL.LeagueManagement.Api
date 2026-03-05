using AutoMapper;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Interfaces;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Common;

namespace TSL.Core.Application.Services
{
    public class GenericService<TEntity, TDto,TCreateDto, TUpdateDto> : IGenericService<TDto, TCreateDto,TUpdateDto>
        where TEntity : BaseEntity
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public GenericService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public virtual async Task<BaseResponseDto<List<TDto>>> GetAllAsync()
        {
            try
            {
                var entities = await GetRepositoryAsync().GetAllAsync();
                var dtos = _mapper.Map<List<TDto>>(entities);

                return BaseResponseDto<List<TDto>>.SuccessResponse(
                    dtos, 
                    $"Se obtuvieron {dtos.Count} registros correctamente"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<TDto>>.ErrorResponse(
                    "Error al obtener los registros", 
                    ex.Message
                );
            }
        }

        public virtual async Task<BaseResponseDto<TDto>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await GetRepositoryAsync().GetByIdAsync(id);

                if (entity == null)
                {
                    return BaseResponseDto<TDto>.ErrorResponse(
                        "Registro no encontrado",
                        $"No existe un registro con ID {id}"
                    );
                }

                var dto = _mapper.Map<TDto>(entity);

                return BaseResponseDto<TDto>.SuccessResponse(
                    dto,
                    "Registro obtenido correctamente"
                );
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TDto>.ErrorResponse(
                   "Error al obtener el registro",
                    ex.Message
                );
            }
        }

        public virtual async Task<BaseResponseDto<bool>> ExistsAsync(int id)
        {
            try
            {
                var exists = await GetRepositoryAsync().ExistsAsync(e => e.Id == id);

                return BaseResponseDto<bool>.SuccessResponse(
                    exists,
                    exists ? "El registro existe" : "El registro no existe"
                );
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al verificar la existencia del registro",
                     ex.Message
                );
            }
        }

        public virtual async Task<BaseResponseDto<TDto>> CreateAsync(TCreateDto createDto)
        {
            try
            {
                var validationResult = await ValidateCreateAsync(createDto);
                if (!validationResult.Success)
                {
                    return BaseResponseDto<TDto>.ErrorResponse(
                        validationResult.Message,
                        validationResult.Errors ?? new List<string>()
                    );
                }

                // Mapear DTO a entidad
                var entity = _mapper.Map<TEntity>(createDto);

                // Agregar al repositorio
                await GetRepositoryAsync().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                // Mapear entidad creada a DTO
                var dto = _mapper.Map<TDto>(entity);

                return BaseResponseDto<TDto>.SuccessResponse(
                    dto,
                    "Registro creado correctamente"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TDto>.ErrorResponse(
                    "Error al crear el registro", 
                    ex.Message
                );
            }
        }

        public virtual async Task<BaseResponseDto<TDto>> UpdateAsync(int id, TUpdateDto updateDto)
        {
            try
            {
                var entity = await GetRepositoryAsync().GetByIdAsync(id);
                if (entity == null)
                {
                    return BaseResponseDto<TDto>.ErrorResponse(
                        "Registro no encontrado",
                        $"No existe un registro con ID {id}"
                    );
                }

                var validationResult = await ValidateUpdateAsync(id, updateDto);
                if (!validationResult.Success)
                {
                    return BaseResponseDto<TDto>.ErrorResponse(
                        validationResult.Message,
                        validationResult.Errors ?? new List<string>()
                    );
                }

                // Mapear cambios del DTO a la entidad existente
                _mapper.Map(updateDto, entity);

                // Actualizar
                GetRepositoryAsync().Update(entity);
                await _unitOfWork.SaveChangesAsync();

                // Mapear entidad actualizada a DTO
                var dto = _mapper.Map<TDto>(entity);

                return BaseResponseDto<TDto>.SuccessResponse(
                    dto,
                    "Registro actualizado correctamente"
                );
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<TDto>.ErrorResponse(
                    "Error al actualizar el registro",
                    ex.Message
                );
            }
        }

        public virtual async Task<BaseResponseDto<int>> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetRepositoryAsync().GetByIdAsync(id);
                if (entity == null)
                {
                    return BaseResponseDto<int>.ErrorResponse(
                        "Registro no encontrado",
                        $"No existe un registro con ID {id}"
                    );
                }

                var validationResult = await ValidateDeleteAsync(id, entity);
                if (!validationResult.Success)
                {
                    return BaseResponseDto<int>.ErrorResponse(
                        validationResult.Message,
                        validationResult.Errors ?? new List<string>()
                    );
                }

                // Eliminar
                GetRepositoryAsync().Remove(entity);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponseDto<int>.SuccessResponse(
                    id,
                    "Registro eliminado correctamente"
                );
            }
            catch (Exception ex) 
            {
                return BaseResponseDto<int>.ErrorResponse(
                    "Error al eliminar el registro",
                    ex.Message
                );
            }
        }


        #region Metodos Virtuales para Validacion (Template Method Pattern)

        // Por defecto, no hay validaciones adicionales

        protected virtual Task<BaseResponseDto<bool>> ValidateCreateAsync(TCreateDto createDto) 
        {
            return Task.FromResult(BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa"));
        }

        protected virtual Task<BaseResponseDto<bool>> ValidateUpdateAsync(int id, TUpdateDto updateDto)
        {
            return Task.FromResult(BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa"));
        }

        protected virtual Task<BaseResponseDto<bool>> ValidateDeleteAsync(int id, TEntity entity)
        {
            
            return Task.FromResult(BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa"));
        }

        #endregion


        // Obtiene el repositorio especifico para la entidad
        // Los servicios hijos deben hacer override de este metodo
        protected virtual IGenericRepository<TEntity> GetRepositoryAsync() 
        {
            throw new NotImplementedException
            (

            $"El servicio debe implementar GetRepositoryAsync() para retornar el repositorio específico"

            );
        }

   
    }
}
