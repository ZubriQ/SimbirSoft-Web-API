using SimbirSoft_Web_API.Helpers.Validators;
using SimbirSoft_Web_API.Mapping;
using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Areas
{
    public class AreaService : IAreaService
    {
        private readonly ChipizationDbContext _db;
        private readonly AreaMapper _mapper = new();
        private AreaValidator _areaValidator = new();

        public AreaService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Get

        public async Task<IResponse<Area>> GetAreaByIdAsync(long? areaId)
        {
            if (!IdValidator.IsValid(areaId))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            if (await _db.Areas.FindAsync(areaId) is not Area area)
            {
                return new ServiceResponse<Area>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Area>(HttpStatusCode.OK, area);
        }

        #endregion

        #region Insert

        public async Task<IResponse<Area>> InsertAreaAsync(AreaRequestDto request)
        {
            if (!AreaRequestValidator.IsValid(request))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            if (await _db.Areas.AnyAsync(a => a.Name == request.Name))
            {
                return new ServiceResponse<Area>(HttpStatusCode.Conflict);
            }

            var area = _mapper.ToArea(request);

            var validationResponse = ValidateAreaAsync(area);
            if (validationResponse is not HttpStatusCode.OK)
            {
                return new ServiceResponse<Area>(validationResponse);
            }

            return await AddAreaToDatabaseAsync(area);
        }

        private HttpStatusCode ValidateAreaAsync(Area area, long? areaId = null)
        {
            _areaValidator.SetExistingAreas(_db.Areas.AsEnumerable());

            if (!_areaValidator.IsGeometryValid(area, areaId))
            {
                return HttpStatusCode.BadRequest;
            }

            if (_areaValidator.IsPointsAlreadyExist(area, areaId))
            {
                return HttpStatusCode.Conflict;
            }

            return HttpStatusCode.OK;
        }

        private async Task<ServiceResponse<Area>> AddAreaToDatabaseAsync(Area area)
        {
            try
            {
                _db.Areas.Add(area);
                await _db.SaveChangesAsync();
                return new ServiceResponse<Area>(HttpStatusCode.Created, area);
            }
            catch (Exception)
            {
                return new ServiceResponse<Area>();
            }
        }

        #endregion

        #region Update

        public async Task<IResponse<Area>> UpdateAreaByIdAsync(long? areaId, AreaRequestDto request)
        {
            if (!IdValidator.IsValid(areaId) || !AreaRequestValidator.IsValid(request))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }
            
            if (await _db.Areas.FindAsync(areaId) is not Area areaToUpdate)
            {
                return new ServiceResponse<Area>(HttpStatusCode.NotFound);
            }

            if (await _db.Areas.AnyAsync(a => a.Name == request.Name && a.Id != areaToUpdate.Id))
            {
                return new ServiceResponse<Area>(HttpStatusCode.Conflict);
            }

            var area = _mapper.ToArea(request);

            var validationResponse = ValidateAreaAsync(area, areaId);
            if (validationResponse is not HttpStatusCode.OK)
            {
                return new ServiceResponse<Area>(validationResponse);
            }

            return await UpdateAreaInDatabaseAsync(areaToUpdate, area);
        }

        private async Task<ServiceResponse<Area>> UpdateAreaInDatabaseAsync(Area areaToUpdate, Area area)
        {
            try
            {
                areaToUpdate.Name = area.Name;
                areaToUpdate.Points = area.Points;
                await _db.SaveChangesAsync();
                
                return new ServiceResponse<Area>(HttpStatusCode.OK, areaToUpdate);
            }
            catch (Exception)
            {
                return new ServiceResponse<Area>();
            }
        }

        #endregion

        #region Delete

        public async Task<HttpStatusCode> RemoveAreaByIdAsync(long? areaId)
        {
            if (!IdValidator.IsValid(areaId))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await _db.Areas.FindAsync(areaId!.Value) is not Area existingArea)
            {
                return HttpStatusCode.NotFound;
            }

            return await RemoveAreaFromDatabaseAsync(existingArea);
        }

        private async Task<HttpStatusCode> RemoveAreaFromDatabaseAsync(Area area)
        {
            try
            {
                _db.Areas.Remove(area);
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
