using AutoMapper;
using Olymp_Project.Dtos.Kind;

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
            CreateMap<Animal, GetAnimalDto>()
                .ForMember(d => d.VisitedLocations, o => o.MapFrom(s => s.VisitedLocations.Select(t => t.Id)))
                .ForMember(d => d.AnimalKinds, o => o.MapFrom(s => s.Kinds.Select(t => t.Id)));
        }
    }
}
