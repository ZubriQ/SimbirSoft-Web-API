using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.Locations
{
    public class LocationService : ILocationService
    {
        private readonly ChipizationDbContext _db;

        public LocationService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<IServiceResponse<Location>> GetLocationAsync(long? id)
        {
            if (!IdValidator.IsValid(id))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }

            // TODO: Test it.
            if (await _db.Locations.FindAsync(id) is not Location location)
            {
                return new ServiceResponse<Location>(HttpStatusCode.NotFound);
            }
            return new ServiceResponse<Location>(HttpStatusCode.OK, location);
        }

        public async Task<IServiceResponse<Location>> InsertLocationAsync(LocationRequestDto location)
        {
            if (!LocationDtoValidator.IsValid(location))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }

            bool coordinatesAlreadyExist = await _db.Locations.AnyAsync(
                l => (l.Latitude == location.Latitude) && (l.Longitude == location.Longitude));
            if (coordinatesAlreadyExist)
            {
                return new ServiceResponse<Location>(HttpStatusCode.Conflict);
            }

            try
            {
                return await AddLocation(location);
            }
            catch (Exception)
            {
                return new ServiceResponse<Location>();
            }
        }

        private async Task<IServiceResponse<Location>> AddLocation(LocationRequestDto location)
        {
            var newLocation = new Location()
            {
                Latitude = location.Latitude!.Value,
                Longitude = location.Longitude!.Value
            };
            await _db.Locations.AddAsync(newLocation);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Location>(HttpStatusCode.Created, newLocation);
        }

        public async Task<IServiceResponse<Location>> UpdateLocationAsync(
            long? id, LocationRequestDto request)
        {
            if (!IdValidator.IsValid(id) || !LocationDtoValidator.IsValid(request))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }

            if (await _db.Locations.FirstOrDefaultAsync(l => l.Id == id) is not Location location)
            {
                return new ServiceResponse<Location>(HttpStatusCode.NotFound);
            }

            var exists = await _db.Locations.AnyAsync(l => l.Latitude == request.Latitude
                                                        && l.Longitude == request.Longitude);
            if (exists)
            {
                return new ServiceResponse<Location>(HttpStatusCode.Conflict);
            }

            try
            {
                return await UpdateLocation(location, request);
            }
            catch (Exception)
            {
                return new ServiceResponse<Location>();
            }
        }

        private async Task<IServiceResponse<Location>> UpdateLocation(
            Location location, LocationRequestDto newData)
        {
            AssignNewData(location, newData);
            _db.Entry(location).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return new ServiceResponse<Location>(HttpStatusCode.OK, location);
        }

        private void AssignNewData(Location destination, LocationRequestDto source)
        {
            destination.Latitude = source.Latitude!.Value;
            destination.Longitude = source.Longitude!.Value;
        }

        public async Task<HttpStatusCode> RemoveLocationAsync(long? id)
        {
            if (!IdValidator.IsValid(id))
            {
                return HttpStatusCode.BadRequest;
            }
            
            if (await _db.Locations.Include(l => l.Animals).FirstOrDefaultAsync(l => l.Id == id)
                is not Location location)
            {
                return HttpStatusCode.NotFound;
            }
            if (location.Animals.Any())
            {
                return HttpStatusCode.BadRequest;
            }

            bool visitedLocations = _db.VisitedLocations.Any(vl => vl.LocationId == id);
            if (visitedLocations)
            {
                return HttpStatusCode.BadRequest;
            }

            try
            {
                return await RemoveLocation(location);
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private async Task<HttpStatusCode> RemoveLocation(Location location)
        {
            _db.Locations.Remove(location);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }
    }
}
