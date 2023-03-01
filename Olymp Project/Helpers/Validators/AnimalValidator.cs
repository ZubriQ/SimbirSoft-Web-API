namespace Olymp_Project.Controllers.Validators
{
    public static class AnimalValidator
    {
        #region Query validation

        public static bool IsQueryValid(AnimalQuery query)
        {
            return IsQueryLifeStatusValid(query.LifeStatus)
                && IsQueryGenderValid(query.Gender)
                && IsQueryChippingValid(query.ChipperId, query.ChippingLocationId);
            // TODO: ALSO CHECK THE DATETIMES.
        }

        private static bool IsQueryLifeStatusValid(string? lifeStatus)
        {
            if (lifeStatus == null)
            {
                return true;
            }
            else
            {
                return lifeStatus == "ALIVE" || lifeStatus == "DEAD";
            }
        }

        private static bool IsQueryGenderValid(string? gender)
        {
            if (gender == null)
            {
                return true;
            }
            else
            {
                return gender == "MALE" || gender == "FEMALE" || gender == "OTHER";
            }
        }

        private static bool IsQueryChippingValid(int? chipperId, long? chippingLocationId)
        {
            return chipperId > 0 || chippingLocationId > 0;
            // TODO: if (HasValue) only then check > 0?
        }

        #endregion

        #region POST request validation

        public static bool IsRequestValid(Animal animal)
        {
            return IsAnimalKindsValid(animal)
                && IsSizeValid(animal.Weight, animal.Length, animal.Height)
                && IsGenderValid(animal.Gender)
                && IsChippingValid(animal.ChipperId, animal.ChippingLocationId);
        }

        private static bool IsAnimalKindsValid(Animal animal)
        {
            if (animal.Kinds == null || animal.Kinds.Count == 0)
            {
                return false;
            }

            foreach (var kind in animal.Kinds)
            {
                if (kind == null || kind.Id <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsSizeValid(float? weight, float? length, float? height)
        {
            return weight.HasValue && weight > 0
                && length.HasValue && length > 0
                && height.HasValue && height > 0;
        }

        private static bool IsGenderValid(string? gender)
        {
            if (gender == null)
            {
                return false;
            }
            else
            {
                return gender == "MALE" || gender == "FEMALE" || gender == "OTHER";
            }
        }

        private static bool IsChippingValid(int? chipperId, long? chippingLocationId)
        {
            return chipperId.HasValue && chipperId > 0
                && chippingLocationId.HasValue && chippingLocationId > 0;
        }

        #endregion

        #region PUT request validation

        public static bool IsRequestValid(PutAnimalDto request)
        {
            return IsSizeValid(request.Weight, request.Length, request.Height)
                && IsGenderValid(request.Gender)
                && IsLifeStatusValid(request.LifeStatus)
                && IsChippingValid(request.ChipperId, request.ChippingLocationId);
        }

        private static bool IsLifeStatusValid(string? lifeStatus)
        {
            if (lifeStatus == null)
            {
                return false;
            }
            else
            {
                return lifeStatus == "ALIVE" || lifeStatus == "DEAD";
            }
        }

        #endregion
    }
}
