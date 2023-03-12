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

        #region Get by id

        public async Task<IServiceResponse<Location>> GetLocationByIdAsync(long? locationId)
        {
            if (!IdValidator.IsValid(locationId))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }
            if (await _db.Locations.FindAsync(locationId) is not Location location)
            {
                return new ServiceResponse<Location>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Location>(HttpStatusCode.OK, location);
        }

        #endregion

        #region Insert

        public async Task<IServiceResponse<Location>> InsertLocationAsync(LocationRequestDto request)
        {
            if (!LocationDtoValidator.IsValid(request))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }
            if (await CoordinatesAlreadyExist(request))
            {
                return new ServiceResponse<Location>(HttpStatusCode.Conflict);
            }

            try
            {
                return await AddLocation(request);
            }
            catch (Exception)
            {
                return new ServiceResponse<Location>();
            }
        }

        private async Task<bool> CoordinatesAlreadyExist(LocationRequestDto request)
        {
            return await _db.Locations
                .AnyAsync(l => (l.Latitude == request.Latitude) && (l.Longitude == request.Longitude));
        }

        private async Task<IServiceResponse<Location>> AddLocation(LocationRequestDto request)
        {
            var newLocation = new Location()
            {
                Latitude = request.Latitude!.Value,
                Longitude = request.Longitude!.Value
            };
            await _db.Locations.AddAsync(newLocation);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Location>(HttpStatusCode.Created, newLocation);
        }

        #endregion

        #region Update

        public async Task<IServiceResponse<Location>> UpdateLocationAsync(
            long? locationId, LocationRequestDto request)
        {
            if (!IdValidator.IsValid(locationId) || !LocationDtoValidator.IsValid(request))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }

            if (await _db.Locations.FirstOrDefaultAsync(l => l.Id == locationId) is not Location location)
            {
                return new ServiceResponse<Location>(HttpStatusCode.NotFound);
            }

            if (await LocationAlreadyExist(request))
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

        private async Task<bool> LocationAlreadyExist(LocationRequestDto request)
        {
            return await _db.Locations.AnyAsync(l => l.Latitude == request.Latitude
                                                  && l.Longitude == request.Longitude);
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

        #endregion

        #region Remove

        public async Task<HttpStatusCode> RemoveLocationAsync(long? locationId)
        {
            (var statusCode, var location) = await ValidateRemoveRequest(locationId);
            if (statusCode is not HttpStatusCode.OK)
            {
                return statusCode;
            }

            try
            {
                return await RemoveLocation(location!);
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private async Task<(HttpStatusCode, Location?)> ValidateRemoveRequest(long? locationId)
        {
            if (!IdValidator.IsValid(locationId))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            if (await GetLocationByIdWithAnimalsAsync(locationId!.Value) is not Location location)
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (LocationHasAnimalsOrVisitedLocationWithId(location, locationId!.Value))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            return (HttpStatusCode.OK, location);
        }

        private async Task<Location?> GetLocationByIdWithAnimalsAsync(long id)
        {
            return await _db.Locations
                .Include(l => l.Animals)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
        
        private bool LocationHasAnimalsOrVisitedLocationWithId(Location location, long locationId)
        {
            bool visitedLocations = _db.VisitedLocations.Any(vl => vl.LocationId == locationId);
            if (visitedLocations || location.Animals.Any())
            {
                return true;
            }

            return false;
        }

        private async Task<HttpStatusCode> RemoveLocation(Location location)
        {
            _db.Locations.Remove(location);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        #endregion
    }
}
