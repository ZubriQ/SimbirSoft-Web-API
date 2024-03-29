﻿using SimbirSoft_Web_API.Helpers.Validators;
using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.VisitedLocations
{
    public class VisitedLocationService : IVisitedLocationService
    {
        private readonly ChipizationDbContext _db;

        public VisitedLocationService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Get by search parameters

        public async Task<IResponse<ICollection<VisitedLocation>>> GetVisitedLocationsAsync(
            long? animalId, DateTimeRangeQuery query, Paging paging)
        {
            if (!IdValidator.IsValid(animalId) || !PagingValidator.IsValid(paging))
            {
                return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalByIdWithVisitedLocationsAsync(animalId!.Value) is not Animal animal)
            {
                return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            return GetFilteredVisitedLocations(query, paging, animal);
        }

        private IResponse<ICollection<VisitedLocation>> GetFilteredVisitedLocations(
            DateTimeRangeQuery query, Paging paging, Animal animal)
        {
            try
            {
                var filteredVisitedLocations = GetVisitedLocationsWithFilter(animal.VisitedLocations, query);
                var paginatedVisitedLocations = PaginateVisitedLocations(filteredVisitedLocations, paging);

                return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.OK, paginatedVisitedLocations);
            }
            catch (Exception)
            {
                return new CollectionServiceResponse<VisitedLocation>();
            }
        }

        private async Task<Animal?> GetAnimalByIdWithVisitedLocationsAsync(long animalId)
        {
            return await _db.Animals
                .Include(a => a.VisitedLocations)
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        private IEnumerable<VisitedLocation> GetVisitedLocationsWithFilter(
            IEnumerable<VisitedLocation> visitedLocations, DateTimeRangeQuery query)
        {
            var endDateTime = query.EndDateTime.HasValue ? 
                new DateTime(query.EndDateTime.Value.Ticks).AddSeconds(1) : (DateTime?)null;

            return visitedLocations
                .Where(v => query.StartDateTime is null || v.VisitDateTime >= query.StartDateTime)
                .Where(v => endDateTime is null || v.VisitDateTime <= endDateTime.Value);
        }

        private List<VisitedLocation> PaginateVisitedLocations(
            IEnumerable<VisitedLocation> locations, Paging paging)
        {
            return locations
                .OrderBy(l => l.Id)
                .Skip(paging.From!.Value)
                .Take(paging.Size!.Value)
                .ToList();
        }

        #endregion

        #region Insert

        public async Task<IResponse<VisitedLocation>> InsertVisitedLocationAsync(
            long? animalId, long? locationId)
        {
            if (!IdValidator.IsValid(animalId, locationId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalByIdWithVisitedLocationsAsync(animalId!.Value) is not Animal animal ||
                await _db.Locations.FindAsync(locationId) is not Location location)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (!AnimalValidator.IsExistingAnimalValid(animal, locationId!.Value))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            return await AddVisitedLocationToDatabaseAsync(animal, location);
        }

        private async Task<IResponse<VisitedLocation>> AddVisitedLocationToDatabaseAsync(
            Animal animal, Location location)
        {
            try
            {
                VisitedLocation visitedLocation = new()
                {
                    Animal = animal,
                    Location = location,
                    VisitDateTime = DateTime.UtcNow
                };
                _db.VisitedLocations.Add(visitedLocation);
                await _db.SaveChangesAsync();

                return new ServiceResponse<VisitedLocation>(HttpStatusCode.Created, visitedLocation);
            }
            catch (Exception)
            {
                return new ServiceResponse<VisitedLocation>();
            }
        }

        #endregion

        #region Update

        public async Task<IResponse<VisitedLocation>> UpdateVisitedLocationAsync(
            long? animalId, VisitedLocationRequestDto request)
        {
            #region Request Validation

            if (!IdValidator.IsValid(animalId, request.VisitedLocationId, request.LocationId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalByIdWithVisitedLocationsAsync(animalId!.Value) is not Animal animal ||
                await _db.Locations.FirstOrDefaultAsync(l => l.Id == request.LocationId) is not Location location)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (animal.VisitedLocations.FirstOrDefault(vl => vl.Id == request.VisitedLocationId)
                is not VisitedLocation visitedLocationToUpdate ||
                !AnimalHasVisitedLocation(animal, visitedLocationToUpdate.LocationId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (!AnimalValidator.IsUpdateVisitedLocationRequestValid(animal, visitedLocationToUpdate, request))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            #endregion

            return await UpdateVisitedLocationInDatabaseAsync(visitedLocationToUpdate!, location!);
        }

        private bool AnimalHasVisitedLocation(Animal animal, long visitedLocationId)
        {
            return animal.VisitedLocations.Any(vl => vl.LocationId == visitedLocationId);
        }

        private async Task<IResponse<VisitedLocation>> UpdateVisitedLocationInDatabaseAsync(
            VisitedLocation visitedLocation, Location location)
        {
            try
            {
                visitedLocation.Location = location;
                _db.VisitedLocations.Attach(visitedLocation);
                _db.Entry(visitedLocation).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return new ServiceResponse<VisitedLocation>(HttpStatusCode.OK, visitedLocation);
            }
            catch (Exception)
            {
                return new ServiceResponse<VisitedLocation>();
            }
        }

        #endregion

        #region Remove

        public async Task<HttpStatusCode> RemoveVisitedLocationAsync(
            long? animalId, long? visitedLocationId)
        {
            if (!IdValidator.IsValid(animalId, visitedLocationId))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await GetAnimalByIdWithVisitedLocationsAsync(animalId!.Value) is not Animal animal ||
                !await VisitedLocationExists(visitedLocationId!.Value) ||
                animal.VisitedLocations
                    .FirstOrDefault(vl => vl.Id == visitedLocationId) is not VisitedLocation visitedLocationToDelete)
            {
                return HttpStatusCode.NotFound;
            }

            return await RemoveVisitedLocationFromDatabaseAsync(animal, visitedLocationToDelete);
        }

        private async Task<bool> VisitedLocationExists(long visitedLocationId)
        {
            return await _db.VisitedLocations.AnyAsync(vl => vl.Id == visitedLocationId);
        }

        private async Task<HttpStatusCode> RemoveVisitedLocationFromDatabaseAsync(
            Animal animal, VisitedLocation visitedLocationToDelete)
        {
            try
            {
                _db.VisitedLocations.Remove(visitedLocationToDelete);
                animal.VisitedLocations.Remove(visitedLocationToDelete);
                RemoveVisitedLocationIfMatchesChippingLocation(animal);
                await _db.SaveChangesAsync();

                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private void RemoveVisitedLocationIfMatchesChippingLocation(Animal animal)
        {
            var newFirstLocation = animal.VisitedLocations
                .OrderBy(vl => vl.VisitDateTime)
                .FirstOrDefault();

            if (newFirstLocation is not null &&
                newFirstLocation.LocationId == animal.ChippingLocationId)
            {
                _db.VisitedLocations.Remove(newFirstLocation);
                animal.VisitedLocations.Remove(newFirstLocation);
            }
        }

        #endregion
    }
}
