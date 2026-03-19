using AutoMapper;
using TSL.Core.Application.Dtos.Account;
using TSL.Core.Application.Dtos.Equipo;
using TSL.Core.Application.Dtos.Liga;
using TSL.Core.Application.Dtos.Partido;
using TSL.Core.Application.Dtos.TablaPosicion;
using TSL.Core.Application.Dtos.Temporada;
using TSL.Core.Application.Dtos.Users;
using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            #region Liga Mappings

            // Entidad -> DTO de lectura

            CreateMap<Liga, LigaDto>()
                .ForMember(dest => dest.CantidadTemporadas, 
                    opt => opt.MapFrom(src => src.Temporadas.Count));

            // CreateDto -> Entidad
            CreateMap<CreateLigaDto, Liga>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Temporadas, opt => opt.Ignore());

            // UpdateDto -> Entidad
            CreateMap<UpdateLigaDto, Liga>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Temporadas, opt => opt.Ignore());

            #endregion

            #region Temporada Mappings

            // Entidad -> DTO de lectura
            CreateMap<Temporada, TemporadaDto>()
                .ForMember(dest => dest.NombreLiga, 
                opt => opt.MapFrom(src => src.Liga.Nombre))
                .ForMember(dest => dest.CantidadPartidos, 
                opt => opt.MapFrom(src => src.Partidos.Count));

            // CreateDto -> Entidad
            CreateMap<CreateTemporadaDto, Temporada>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Liga, opt => opt.Ignore())
                .ForMember(dest => dest.Partidos, opt => opt.Ignore())
                .ForMember(dest => dest.TablaPosicion, opt => opt.Ignore());

            // UpdateDto -> Entidad
            CreateMap<UpdateTemporadaDto, Temporada>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LigaId, opt => opt.Ignore())
                .ForMember(dest => dest.Liga, opt => opt.Ignore())
                .ForMember(dest => dest.Partidos, opt => opt.Ignore())
                .ForMember(dest => dest.TablaPosicion, opt => opt.Ignore());

            #endregion

            #region Equipo Mappings

            // Entidad -> DTO de lectura
            CreateMap<Equipo, EquipoDto>()
                .ForMember(dest => dest.CantidadPartidos, 
                    opt => opt.MapFrom(src => src.PartidosLocal.Count + src.PartidosVisitante.Count));

            // CreateDto -> Entidad
            CreateMap<CreateEquipoDto, Equipo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PartidosLocal, opt => opt.Ignore())
                .ForMember(dest => dest.PartidosVisitante, opt => opt.Ignore())
                .ForMember(dest => dest.Posiciones, opt => opt.Ignore());

            // UpdateDto -> Entidad
            CreateMap<UpdateEquipoDto, Equipo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
                .ForMember(dest => dest.PartidosLocal, opt => opt.Ignore())
                .ForMember(dest => dest.PartidosVisitante, opt => opt.Ignore())
                .ForMember(dest => dest.Posiciones, opt => opt.Ignore());

            #endregion

            #region Partido Mappings

            // Entidad -> DTO de lectura
            CreateMap<Partido, PartidoDto>()
                .ForMember(dest => dest.NombreTemporada,
                opt => opt.MapFrom(src => src.Temporada.Nombre))
                .ForMember(dest => dest.NombreEquipoLocal,
                opt => opt.MapFrom(src => src.EquipoLocal.Nombre))
                .ForMember(dest => dest.EscudoEquipoLocal,
                opt => opt.MapFrom(src => src.EquipoLocal.Escudo))
                .ForMember(dest => dest.NombreEquipoVisitante,
                opt => opt.MapFrom(src => src.EquipoVisitante.Nombre))
                .ForMember(dest => dest.EscudoEquipoVisitante,
                opt => opt.MapFrom(src => src.EquipoVisitante.Escudo));

            // CreateDto -> Entidad
            CreateMap<CreatePartidoDto, Partido>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.GolesLocal, opt => opt.Ignore())
                .ForMember(dest => dest.GolesVisitante, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => Domain.Enums.EstadoPartido.Programado))
                .ForMember(dest => dest.Temporada, opt => opt.Ignore())
                .ForMember(dest => dest.EquipoLocal, opt => opt.Ignore())
                .ForMember(dest => dest.EquipoVisitante, opt => opt.Ignore());

            // UpdateDto -> Entidad
            CreateMap<UpdatePartidoDto, Partido>()
                .ForMember(dest => dest.TemporadaId, opt => opt.MapFrom(src => src.TemporadaId))
                .ForMember(dest => dest.EquipoLocalId, opt => opt.MapFrom(src => src.EquipoLocalId))
                .ForMember(dest => dest.EquipoVisitanteId, opt => opt.MapFrom(src => src.EquipoVisitanteId))
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => src.Fecha))
                .ForMember(dest => dest.Jornada, opt => opt.MapFrom(src => src.Jornada));

            // RegistrarResultadoDto -> Partido (actualización parcial)
            CreateMap<RegistrarResultadoDto, Partido>()
                .ForMember(dest => dest.GolesLocal, opt => opt.MapFrom(src => src.GolesLocal))
                .ForMember(dest => dest.GolesVisitante, opt => opt.MapFrom(src => src.GolesVisitante))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => Domain.Enums.EstadoPartido.Jugado));

            #endregion

            #region TablaPosicion Mappings

            // Entidad -> DTO de lectura
            CreateMap<TablaPosicion, TablaPosicionDto>()
                .ForMember(dest => dest.NombreTemporada, opt => opt.MapFrom(src => src.Temporada.Nombre))
                .ForMember(dest => dest.NombreLiga, opt => opt.MapFrom(src => src.Temporada.Liga.Nombre))
                .ForMember(dest => dest.Posiciones, 
                    opt => opt.MapFrom(src => src.Posiciones
                        .OrderByDescending(p => p.Puntos)
                        .ThenByDescending(p => p.GolesAFavor - p.GolesEnContra)
                        .ThenByDescending(p => p.GolesAFavor)
                        .ThenBy(p => p.Equipo.Nombre)));

            #endregion

            #region PosicionEquipo Mappings

            // Entidad -> DTO de lectura
            CreateMap<PosicionEquipo, PosicionEquipoDto>()
                .ForMember(dest => dest.NombreEquipo,
                    opt => opt.MapFrom(src => src.Equipo.Nombre))
                .ForMember(dest => dest.EscudoEquipo,
                    opt => opt.MapFrom(src => src.Equipo.Escudo))
                .ForMember(dest => dest.DiferenciaGoles,
                opt => opt.MapFrom(src => src.GolesAFavor - src.GolesEnContra))
                .ForMember(dest => dest.Posicion, opt => opt.Ignore());

            #endregion

            #region UserProfile

            CreateMap<LoginDto, AuthenticationRequest>()
                .ReverseMap()
                .ForMember(dest => dest.Error, opt => opt.Ignore())
                .ForMember(dest => dest.HasError, opt => opt.Ignore());

            CreateMap<SaveUserDto, RegisterRequest>()
                .ReverseMap()
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.Error, opt => opt.Ignore())
                .ForMember(dest => dest.HasError, opt => opt.Ignore());

            CreateMap<ForgotPasswordDto, ForgotPasswordRequest>()
                .ReverseMap()
                .ForMember(dest => dest.Error, opt => opt.Ignore())
                .ForMember(dest => dest.HasError, opt => opt.Ignore());

            CreateMap<ResetPasswordDto, ResetPasswordRequest>()
                .ReverseMap()
                .ForMember(dest => dest.Error, opt => opt.Ignore())
                .ForMember(dest => dest.HasError, opt => opt.Ignore());

            #endregion
        }
    }
}
