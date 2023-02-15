using System.Net;

namespace Olymp_Project.Services.Locations
{
    public class LocationService : ILocationService
    {
        private readonly ChipizationDbContext _db;

        public LocationService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<Location?> GetLocationAsync(long id)
        {
            return await _db.Locations.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<(HttpStatusCode, Location?)> AddLocationAsync(Location location)
        {
            try
            {
                bool exists = await _db.Locations.AnyAsync(l => l.Latitude == location.Latitude
                                                                && l.Longitude == location.Longitude);
                if (exists)
                {
                    return (HttpStatusCode.Conflict, null);
                }

                await _db.Locations.AddAsync(location);
                await _db.SaveChangesAsync();
                return (HttpStatusCode.Created, location);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<(HttpStatusCode, Location?)> UpdateLocationAsync(long id, Location location)
        {
            try
            {
                var locationToUpdate = await _db.Locations.FirstOrDefaultAsync(l => l.Id == id);
                if (locationToUpdate is null)
                {
                    return (HttpStatusCode.NotFound, null);
                }

                var exists = await _db.Locations.AnyAsync(l => l.Latitude == location.Latitude
                                                               && l.Longitude == location.Longitude);
                if (exists)
                {
                    return (HttpStatusCode.Conflict, null);
                }

                UpdateLocation(locationToUpdate, location);
                _db.Entry(locationToUpdate).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return (HttpStatusCode.OK, locationToUpdate);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        private void UpdateLocation(Location destination, Location source)
        {
            destination.Latitude = source.Latitude;
            destination.Longitude = source.Longitude;
        }

        public async Task<HttpStatusCode> DeleteLocationAsync(long id)
        {
            try
            {
                var location = await _db.Locations.Include(l => l.Animals)
                                                  .FirstOrDefaultAsync(l => l.Id == id);
                if (location is null)
                {
                    return HttpStatusCode.NotFound;
                }
                if (location.Animals.Any())
                {
                    return HttpStatusCode.BadRequest;
                }

                _db.Locations.Remove(location);
                await _db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
