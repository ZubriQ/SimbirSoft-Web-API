using AutoMapper;
using Olymp_Project.Dtos.Kind;
using Olymp_Project.Dtos.VisitedLocation;

namespace Olymp_Project
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Account mapping
            CreateMap<Account, AccountResponseDto>();
            CreateMap<AccountRequestDto, Account>();

            // Location mapping
            CreateMap<Location, LocationResponseDto>();
            CreateMap<LocationRequestDto, Location>();

            // Kind mapping
            CreateMap<Kind, KindResponseDto>();

            // Animal mapping
            // Перенос необходимых идентификаторов из классов в массивы.
            CreateMap<Animal, AnimalResponseDto>()
                .ForMember(d => d.VisitedLocations, o => o.MapFrom(s => s.VisitedLocations.Select(t => t.Id)))
                .ForMember(d => d.AnimalKinds, o => o.MapFrom(s => s.Kinds.Select(t => t.Id)));

            CreateMap<long, Kind>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s));

            CreateMap<PostAnimalDto, Animal>()
                .ForMember(d => d.Kinds, o => o.MapFrom(s => s.AnimalKinds.Select(id => new Kind { Id = id })));

            // Visited Location mapping
            CreateMap<VisitedLocation, VisitedLocationResponseDto>();
        }
    }
}
