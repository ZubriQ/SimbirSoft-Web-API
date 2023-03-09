using Olymp_Project.Models;
using Olymp_Project.Responses;

namespace Olymp_Project.Helpers.Validators
{
    public static class AnimalValidator
    {
        public static bool IsExistingAnimalValid(Animal animal, long locationId)
        {
            if (animal.LifeStatus == "DEAD")
            {
                return false;
            }
            if (animal.ChippingLocationId == locationId && animal.VisitedLocations.Count == 0)
            {
                return false;
            }

            var lastVisitedLocation = animal.VisitedLocations
                .OrderByDescending(l => l.VisitDateTime)
                .FirstOrDefault();
            if (lastVisitedLocation is not null && lastVisitedLocation.LocationId == locationId)
            {
                return false;
            }

            return true;
        }

        public static bool IsAdjacentLocationsValid(
            Animal animal, long locationPointId, long visitedLocationId)
        {
            var locations = animal.VisitedLocations.OrderBy(al => al.VisitDateTime).ToList();
            VisitedLocation? previousLocation = null, nextLocation = null;

            int index = locations.FindIndex(l => l.Id == visitedLocationId);
            if (index > 0)
            {
                previousLocation = locations[index - 1];
            }
            if (index + 1 < locations.Count)
            {
                nextLocation = locations[index + 1];
            }

            if (nextLocation is not null && nextLocation.LocationId == locationPointId)
            {
                return false;
            }
            if (previousLocation is not null && previousLocation.LocationId == locationPointId)
            {
                return false;
            }
            return true;
        }

        public static bool IsVisitedLocationValid(
            Animal animal, VisitedLocation visitedLocation, long locationPointId)
        {
            if (visitedLocation.LocationId == locationPointId)
            {
                return false;
            }

            if (visitedLocation == animal.VisitedLocations.Last() &&
                animal.ChippingLocationId == locationPointId)
            {
                return false;
            }

            return true;
        }
    }
}
