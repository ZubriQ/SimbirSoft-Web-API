using SimbirSoft_Web_API.Helpers.Validators;
using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Locations
{
    public class LocationService : ILocationService
    {
        private readonly ChipizationDbContext _db;

        public LocationService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Get by id

        public async Task<IResponse<Location>> GetLocationByIdAsync(long? locationId)
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

        #region Get by coordinates

        public async Task<IResponse<Location>> GetLocationByCoordinatesAsync(LocationRequestDto request)
        {
            if (!LocationPointValidator.IsValid(request))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }

            if (await _db.Locations.FirstOrDefaultAsync(
                l => l.Latitude == request.Latitude && l.Longitude == request.Longitude) is not Location location)
            {
                return new ServiceResponse<Location>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Location>(HttpStatusCode.OK, location);
        }

        #endregion

        #region Insert

        public async Task<IResponse<Location>> InsertLocationAsync(LocationRequestDto request)
        {
            if (!LocationPointValidator.IsValid(request))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }
            if (await CoordinatesAlreadyExist(request))
            {
                return new ServiceResponse<Location>(HttpStatusCode.Conflict);
            }

            return await AddLocationToDatabaseAsync(request);
        }

        private async Task<bool> CoordinatesAlreadyExist(LocationRequestDto request)
        {
            return await _db.Locations
                .AnyAsync(l => (l.Latitude == request.Latitude) && (l.Longitude == request.Longitude));
        }

        private async Task<IResponse<Location>> AddLocationToDatabaseAsync(LocationRequestDto request)
        {
            try
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
            catch (Exception)
            {
                return new ServiceResponse<Location>();
            }
        }

        #endregion

        #region Update

        public async Task<IResponse<Location>> UpdateLocationAsync(
            long? locationId, LocationRequestDto request)
        {
            if (!IdValidator.IsValid(locationId) || !LocationPointValidator.IsValid(request))
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

            if (await IsLocationInUse(locationId!.Value))
            {
                return new ServiceResponse<Location>(HttpStatusCode.BadRequest);
            }

            return await UpdateLocationInDatabaseAsync(location, request);

        }

        private async Task<bool> LocationAlreadyExist(LocationRequestDto request)
        {
            return await _db.Locations.AnyAsync(l => l.Latitude == request.Latitude
                                                  && l.Longitude == request.Longitude);
        }

        private async Task<bool> IsLocationInUse(long locationId)
        {
            return await _db.Animals.AnyAsync(a => a.ChippingLocationId == locationId) ||
                   await _db.VisitedLocations.AnyAsync(vl => vl.LocationId == locationId);
        }

        private async Task<IResponse<Location>> UpdateLocationInDatabaseAsync(
            Location location, LocationRequestDto newData)
        {
            try
            {
                location.Latitude = newData.Latitude!.Value;
                location.Longitude = newData.Longitude!.Value;
                _db.Entry(location).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return new ServiceResponse<Location>(HttpStatusCode.OK, location);
            }
            catch (Exception)
            {
                return new ServiceResponse<Location>();
            }
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

            return await RemoveLocationFromDatabaseAsync(location!);
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

        private async Task<HttpStatusCode> RemoveLocationFromDatabaseAsync(Location location)
        {
            try
            {
                _db.Locations.Remove(location);
                await _db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        #endregion
    }
}
