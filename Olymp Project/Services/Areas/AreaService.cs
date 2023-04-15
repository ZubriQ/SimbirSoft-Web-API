using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Mapping;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.Areas
{
    public class AreaService : IAreaService
    {
        private readonly ChipizationDbContext _db;
        private readonly AreaMapper _mapper = new();

        public AreaService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Get

        public async Task<IServiceResponse<Area>> GetAreaByIdAsync(long? areaId)
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

        public async Task<IServiceResponse<Area>> InsertAreaAsync(AreaRequestDto request)
        {
            if (!AreaRequestValidator.IsValid(request))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            var area = _mapper.ToArea(request);

            if (await IsAlreadyExists(area))
            {
                return new ServiceResponse<Area>(HttpStatusCode.Conflict);
            }

            if (!IsCreateAreaGeometryValid(area))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            return await AddAreaToDatabase(area);
        }

        private async Task<bool> IsAlreadyExists(Area area)
        {
            if (await _db.Areas.AnyAsync(a => a.Name == area.Name))
            {
                return true;
            }

            var existingAreas = _db.Areas.AsEnumerable();
            if (existingAreas.Any(a => ArePolygonsEqual(a.Points, area.Points)))
            {
                return true;
            }

            return false;
        }

        private bool IsCreateAreaGeometryValid(Area area)
        {
            if (PointsAreCollinear(area.Points) ||
                PolygonHasOverlappingEdges(area.Points) ||
                PolygonIntersectsExistingPolygon(area.Points) ||
                PolygonHasDuplicatePoints(area.Points) ||
                PolygonSharesPointsWithExistingPolygon(area.Points))
            {
                return false;
            }

            return true;
        }

        private async Task<ServiceResponse<Area>> AddAreaToDatabase(Area area)
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

        #region Geometry Validation

        private bool PointsAreCollinear(NpgsqlPolygon polygon)
        {
            int pointCount = polygon.Count;
            if (pointCount < 3)
            {
                return true;
            }

            for (int i = 0; i < pointCount - 2; i++)
            {
                NpgsqlPoint firstPoint = polygon[i];
                NpgsqlPoint secondPoint = polygon[i + 1];
                NpgsqlPoint thirdPoint = polygon[i + 2];

                // If the cross product is not zero, points are not collinear
                if (!ArePointsCollinear(firstPoint, secondPoint, thirdPoint))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ArePointsCollinear(NpgsqlPoint firstPoint, NpgsqlPoint secondPoint, NpgsqlPoint thirdPoint)
        {
            // Cross product for consecutive points
            double crossProduct = (secondPoint.X - firstPoint.X) * (thirdPoint.Y - firstPoint.Y) -
                                  (secondPoint.Y - firstPoint.Y) * (thirdPoint.X - firstPoint.X);

            return Math.Abs(crossProduct) <= 1e-9;
        }

        private bool PolygonHasOverlappingEdges(NpgsqlPolygon polygon)
        {
            LineString? polygonBoundary = _mapper.ToPolygonGeometry(polygon).Boundary as LineString;
            GeometryFactory geometryFactory = new GeometryFactory();
            int numPoints = polygonBoundary!.NumPoints;

            return CheckForOverlapping(polygonBoundary, geometryFactory, numPoints);
        }

        private bool CheckForOverlapping(LineString boundary, GeometryFactory factory, int numPoints)
        {
            for (int i = 0; i < numPoints - 1; i++)
            {
                LineString lineSegment1 = CreateLineSegment(factory, boundary, i);

                for (int j = i + 1; j < numPoints - 1; j++)
                {
                    LineString lineSegment2 = CreateLineSegment(factory, boundary, j);

                    if (lineSegment1.Intersects(lineSegment2) && !lineSegment1.Touches(lineSegment2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private LineString CreateLineSegment(GeometryFactory factory, LineString polygonBoundary, int index)
        {
            Coordinate[] coordinates = new[]
            {
                polygonBoundary.GetCoordinateN(index),
                polygonBoundary.GetCoordinateN(index + 1)
            };
            return factory.CreateLineString(coordinates);
        }

        // TODO: Test this method
        private bool PolygonIntersectsExistingPolygon(NpgsqlPolygon newPolygon, long? excludedAreaId = null)
        {
            Polygon newPolygonGeometry = _mapper.ToPolygonGeometry(newPolygon);
            foreach (var existingArea in _db.Areas)
            {
                if (excludedAreaId.HasValue && existingArea.Id == excludedAreaId.Value)
                {
                    continue;
                }

                Polygon existingPolygonGeometry = _mapper.ToPolygonGeometry(existingArea.Points);
                if (newPolygonGeometry.Relate(existingPolygonGeometry, "T********"))
                {
                    return true;
                }
            }
            return false;
        }

        private bool PolygonHasDuplicatePoints(NpgsqlPolygon polygon)
        {
            var pointSet = new HashSet<NpgsqlPoint>(polygon);
            return pointSet.Count < polygon.Count;
        }

        // TODO: refactor if have time
        private bool PolygonSharesPointsWithExistingPolygon(NpgsqlPolygon newPolygon, long? excludedAreaId = null)
        {
            Polygon currentPolygon = _mapper.ToPolygonGeometry(newPolygon);
            var existingAreas = _db.Areas.AsEnumerable();

            foreach (var existingArea in existingAreas)
            {
                if (excludedAreaId.HasValue && existingArea.Id == excludedAreaId.Value)
                {
                    continue;
                }

                Polygon existingPolygon = _mapper.ToPolygonGeometry(existingArea.Points);
                if (currentPolygon.CoveredBy(existingPolygon) ||
                    existingPolygon.CoveredBy(currentPolygon))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ArePolygonsEqual(NpgsqlPolygon polygon1, NpgsqlPolygon polygon2)
        {
            if (polygon1.Count != polygon2.Count)
            {
                return false;
            }

            return PolygonsHaveMatchingVertices(polygon1, polygon2) ||
                   PolygonsHaveMatchingVertices(polygon1, ReversePolygon(polygon2));
        }

        private bool PolygonsHaveMatchingVertices(NpgsqlPolygon polygon1, NpgsqlPolygon polygon2)
        {
            for (int i = 0; i < polygon1.Count; i++)
            {
                if (VerticesMatch(polygon1, polygon2, i))
                {
                    return true;
                }
            }
            return false;
        }

        private bool VerticesMatch(NpgsqlPolygon polygon1, NpgsqlPolygon polygon2, int offset)
        {
            for (int j = 0; j < polygon1.Count; j++)
            {
                if (polygon1[(offset + j) % polygon1.Count] != polygon2[j])
                {
                    return false;
                }
            }
            return true;
        }

        private NpgsqlPolygon ReversePolygon(NpgsqlPolygon polygon)
        {
            var reversedPolygon = new NpgsqlPolygon(polygon);
            reversedPolygon.Reverse();
            return reversedPolygon;
        }

        #endregion

        #endregion

        #region Update

        // TODO: refactor
        public async Task<IServiceResponse<Area>> UpdateAreaByIdAsync(long? areaId, AreaRequestDto request)
        {
            if (!IdValidator.IsValid(areaId) || !AreaRequestValidator.IsValid(request))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            if (await _db.Areas.FindAsync(areaId) is not Area updatedArea)
            {
                return new ServiceResponse<Area>(HttpStatusCode.NotFound);
            }

            var area = _mapper.ToArea(request);

            var existingAreas = _db.Areas.AsEnumerable();
            if (existingAreas.Any(a => ArePolygonsEqual(a.Points, updatedArea.Points) && a.Id != areaId!.Value))
                return new ServiceResponse<Area>(HttpStatusCode.Conflict);

            if (await _db.Areas.AnyAsync(a => a.Name == request.Name))
                return new ServiceResponse<Area>(HttpStatusCode.Conflict, errorMessage: "Name already exists");

            if (PolygonSharesPointsWithExistingPolygon(area.Points, areaId)) // areaId
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);

            if (PointsAreCollinear(area.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PointsAreCollinear");

            if (PolygonHasOverlappingEdges(area.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonHasOverlappingEdges");

            if (PolygonIntersectsExistingPolygon(area.Points, areaId)) // areaId
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonIntersectsExistingPolygon");

            if (PolygonHasDuplicatePoints(area.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonHasDuplicatePoints");

            try
            {
                updatedArea.Name = area.Name;
                updatedArea.Points = area.Points;
                await _db.SaveChangesAsync();
                return new ServiceResponse<Area>(HttpStatusCode.OK, updatedArea);
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

            try
            {
                return await RemoveAndSaveChangesAsync(existingArea);
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private async Task<HttpStatusCode> RemoveAndSaveChangesAsync(Area area)
        {
            _db.Areas.Remove(area);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        #endregion
    }
}
