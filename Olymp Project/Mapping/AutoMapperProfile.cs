using AutoMapper;
using Olymp_Project.Helpers;

namespace Olymp_Project.Mapping
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
            CreateMap<Kind, KindResponseDto>()
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Name));

            // Animal mapping
            // Moving only ids to arrays and converting dates to ISO-8601
            CreateMap<Animal, AnimalResponseDto>()
                .ForMember(d => d.VisitedLocations, o => o.MapFrom(s => s.VisitedLocations.Select(t => t.Id)))
                .ForMember(d => d.AnimalTypes, o => o.MapFrom(s => s.Kinds.Select(t => t.Id)))
                .ForMember(d => d.ChippingDateTime, o => o.MapFrom(s => s.ChippingDateTime.ToISO8601()))
                .ForMember(d => d.DeathDateTime, o => o.MapFrom(
                    s => s.DeathDateTime != null ? s.DeathDateTime.Value.ToISO8601() : null));

            CreateMap<long, Kind>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s));

            CreateMap<PostAnimalDto, Animal>()
                .ForMember(d => d.Kinds, o => o.MapFrom(s => s.AnimalTypes!.Select(id => new Kind { Id = id })));

            // Visited Location mapping
            CreateMap<VisitedLocation, VisitedLocationResponseDto>()
                .ForMember(d => d.DateTimeOfVisitLocationPoint, o => o.MapFrom(s => s.VisitDateTime.ToISO8601()))
                .ForMember(d => d.LocationPointId, o => o.MapFrom(s => s.LocationId));
        }
    }
}
