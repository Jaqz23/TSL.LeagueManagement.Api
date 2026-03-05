using AutoMapper;
using TSL.Core.Application.Dtos.Common;
using TSL.Core.Application.Dtos.Equipo;
using TSL.Core.Application.Interfaces;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Services
{
    public class EquipoService : GenericService<Equipo, EquipoDto, CreateEquipoDto, UpdateEquipoDto> , IEquipoService
    {
        public EquipoService(IUnitOfWork unitOfWork , IMapper mapper) 
            : base(unitOfWork, mapper)
        {
            
        }

        protected override IGenericRepository<Equipo> GetRepositoryAsync() 
        {
            return _unitOfWork.EquipoRepository;
        }

        #region Override - Validaciones del GenericService

        protected override async Task<BaseResponseDto<bool>> ValidateCreateAsync(CreateEquipoDto createDto)
        {
            var existe = await _unitOfWork.EquipoRepository.ExisteNombreAsync(createDto.Nombre);

            if (existe) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Nombre duplicado", 
                    $"Ya existe un equipo con el nombre '{createDto.Nombre}'"
                );
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa");

        }

        protected override async Task<BaseResponseDto<bool>> ValidateUpdateAsync(int id, UpdateEquipoDto updateDto)
        {
            var existe = await _unitOfWork.EquipoRepository.ExisteNombreAsync(updateDto.Nombre, id);
            
            if (existe)
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Nombre duplicado",
                    $"Ya existe otro equipo con el nombre '{updateDto.Nombre}'"
                );
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "Validación exitosa");

        }



        protected override async Task<BaseResponseDto<bool>> ValidateDeleteAsync(int id, Equipo entity)
        {
            // Validar que no tenga partidos asociados
            var tienePartidos = await _unitOfWork.EquipoRepository.TienePartidosAsync(id);

            if (tienePartidos) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "No se puede eliminar",
                    "El equipo tiene partidos asociados. No se puede eliminar."
                );
            }


            if (!entity.PuedeSerEliminado()) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "No se puede eliminar",
                    "El equipo no puede ser eliminado"
                );
            }

            return BaseResponseDto<bool>.SuccessResponse(true, "El equipo puede ser eliminado");

        }

        #endregion


        #region Consultas Especializadas

        public async Task<BaseResponseDto<EquipoDto>> GetByIdWithPartidosAsync(int id)
        {
            try
            {
                var equipo = await _unitOfWork.EquipoRepository.GetByIdWithPartidosAsync(id);

                if(equipo == null) 
                {
                    return BaseResponseDto<EquipoDto>.ErrorResponse(
                        "Equipo no encontrado",
                        $"No existe un equipo con ID {id}"
                    );
                }

                var dto = _mapper.Map<EquipoDto>(equipo);

                return BaseResponseDto<EquipoDto>.SuccessResponse(
                    dto,
                    "Equipo obtenido correctamente con su historial de partidos"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<EquipoDto>.ErrorResponse(
                    "Error al obtener el equipo con partidos",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<EquipoDto>> GetByIdWithPosicionesAsync(int id)
        {
            try
            {
                var equipo = await _unitOfWork.EquipoRepository.GetByIdWithPosicionesAsync(id);

                if(equipo == null) 
                {
                    return BaseResponseDto<EquipoDto>.ErrorResponse(
                        "Equipo no encontrado",
                        $"No existe un equipo con ID {id}"
                    );
                }

                var dto = _mapper.Map<EquipoDto>(equipo);

                return BaseResponseDto<EquipoDto>.SuccessResponse(
                    dto,
                    "Equipo obtenido correctamente con su historial de posiciones"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<EquipoDto>.ErrorResponse(
                    "Error al obtener el equipo con posiciones",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<List<EquipoDto>>> GetByCiudadAsync(string ciudad)
        {
            try
            {
                // Validar que la ciudad no esté vacía
                if (string.IsNullOrWhiteSpace(ciudad))
                {
                    return BaseResponseDto<List<EquipoDto>>.ErrorResponse(
                        "Ciudad inválida",
                        "Debe proporcionar un nombre de ciudad válido"
                    );
                }

                var equipos = await _unitOfWork.EquipoRepository.GetByCiudadAsync(ciudad);
                var dtos = _mapper.Map<List<EquipoDto>>(equipos);

                return BaseResponseDto<List<EquipoDto>>.SuccessResponse(
                    dtos,
                    $"Se encontraron {dtos.Count} equipos en {ciudad}"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<List<EquipoDto>>.ErrorResponse(
                    "Error al obtener equipos por ciudad",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> ExisteNombreAsync(string nombre, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return BaseResponseDto<bool>.ErrorResponse(
                        "Nombre inválido",
                        "Debe proporcionar un nombre válido"
                    );
                }

                var existe = await _unitOfWork.EquipoRepository.ExisteNombreAsync(nombre, excludeId);

                return BaseResponseDto<bool>.SuccessResponse(
                    existe, 
                    existe 
                    ? $"El nombre '{nombre}' ya esta en uso" 
                    : $"El nombre '{nombre}' está disponible"
                );


            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al verificar la existencia del nombre",
                    ex.Message
                );
            }
        }

        public async Task<BaseResponseDto<bool>> TienePartidosAsync(int equipoId)
        {
            try
            {
                var tienePartidos = await _unitOfWork.EquipoRepository.TienePartidosAsync(equipoId);

                return BaseResponseDto<bool>.SuccessResponse(
                    tienePartidos, 
                    tienePartidos 
                    ? "El equipo tiene patidos asociados" 
                    : "El equipo no tiene patidos asociados"
                );

            }
            catch (Exception ex) 
            {
                return BaseResponseDto<bool>.ErrorResponse(
                    "Error al verificar partidos del equipo", 
                    ex.Message
                );
            }
        }

        #endregion

    }
}
