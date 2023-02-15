using AutoMapper;

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

            // Animal mapping
            // Перенос необходимых идентификаторов в массивы.
            CreateMap<Animal, GetAnimalDto>()
                .ForMember(d => d.VisitedLocations, o => o.MapFrom(s => s.VisitedLocations.Select(t => t.Id)))
                .ForMember(d => d.AnimalTypes, o => o.MapFrom(s => s.Types.Select(t => t.Id)));
        }
    }
}
