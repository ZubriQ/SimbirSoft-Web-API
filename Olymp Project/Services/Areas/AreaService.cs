using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Olymp_Project.Helpers;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.Areas
{
    public class AreaService : IAreaService
    {
        private readonly ChipizationDbContext _db;

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

            if (await _db.Areas.AnyAsync(a => a.Name == request.Name))
            {
                return new ServiceResponse<Area>(HttpStatusCode.Conflict);
            }

            Area area = ToArea(request);

            if (PointsAreCollinear(area.Points)) 
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PointsAreCollinear");

            if (PolygonHasOverlappingEdges(area.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonHasOverlappingEdges");

            if (PolygonIntersectsExistingPolygon(area.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonIntersectsExistingPolygon");

            if (PolygonHasDuplicatePoints(area.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonHasDuplicatePoints");

            if (PolygonSharesPointsWithExistingPolygon(area.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonSharesPointsWithExistingPolygon");

            var existingAreas = _db.Areas.AsEnumerable();
            if (existingAreas.Any(a => ArePolygonsEqual(a.Points, area.Points)))
                return new ServiceResponse<Area>(HttpStatusCode.Conflict, errorMessage: "ArePolygonsEqual");

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

        public static Area ToArea(AreaRequestDto request)
        {
            var npgsqlPoints = request.AreaPoints!
                .Select(AreaPointHelper.ToNpgsqlPoint)
                .ToArray();
            var npgsqlPolygon = new NpgsqlPolygon(npgsqlPoints);

            return new Area
            {
                Name = request.Name!,
                Points = npgsqlPolygon
            };
        }

        private bool PointsAreCollinear(NpgsqlPolygon polygon)
        {
            if (polygon.Count < 3)
            {
                return true;
            }

            NpgsqlPoint firstPoint = polygon[0];
            NpgsqlPoint secondPoint = polygon[1];

            for (int i = 2; i < polygon.Count; i++)
            {
                NpgsqlPoint thirdPoint = polygon[i];

                // Calculate the cross product for consecutive points
                double crossProduct = (secondPoint.X - firstPoint.X) * (thirdPoint.Y - firstPoint.Y) -
                                      (secondPoint.Y - firstPoint.Y) * (thirdPoint.X - firstPoint.X);

                // If the cross product is not zero, points are not collinear
                if (Math.Abs(crossProduct) > 1e-9)
                {
                    return false;
                }

                firstPoint = secondPoint;
                secondPoint = thirdPoint;
            }

            return true;
        }

        private bool PolygonHasOverlappingEdges(NpgsqlPolygon polygon)
        {
            LineString? polygonBoundary = ToPolygonGeometry(polygon).Boundary as LineString;
            GeometryFactory geometryFactory = new GeometryFactory();
            for (int i = 0; i < polygonBoundary!.NumPoints - 1; i++)
            {
                LineString lineSegment1 = geometryFactory.CreateLineString(new Coordinate[] { polygonBoundary.GetCoordinateN(i), polygonBoundary.GetCoordinateN(i + 1) });
                for (int j = i + 1; j < polygonBoundary.NumPoints - 1; j++)
                {
                    LineString lineSegment2 = geometryFactory.CreateLineString(new Coordinate[] { polygonBoundary.GetCoordinateN(j), polygonBoundary.GetCoordinateN(j + 1) });
                    if (lineSegment1.Intersects(lineSegment2) && !lineSegment1.Touches(lineSegment2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool PolygonIntersectsExistingPolygon(NpgsqlPolygon newPolygon, long? excludedAreaId = null)
        {
            Polygon newPolygonGeometry = ToPolygonGeometry(newPolygon);
            foreach (var existingArea in _db.Areas)
            {
                if (excludedAreaId.HasValue && existingArea.Id == excludedAreaId.Value)
                {
                    continue;
                }

                Polygon existingPolygonGeometry = ToPolygonGeometry(existingArea.Points);
                if (newPolygonGeometry.Relate(existingPolygonGeometry, "T********"))
                {
                    return true;
                }
            }
            return false;
        }

        private Polygon ToPolygonGeometry(NpgsqlPolygon npgsqlPolygon)
        {
            var coordinates = new Coordinate[npgsqlPolygon.Count + 1];
            for (int i = 0; i < npgsqlPolygon.Count; i++)
            {
                coordinates[i] = new Coordinate(npgsqlPolygon[i].X, npgsqlPolygon[i].Y);
            }
            coordinates[npgsqlPolygon.Count] = coordinates[0]; 

            var geometryFactory = new GeometryFactory();
            return geometryFactory.CreatePolygon(coordinates);
        }

        private bool PolygonHasDuplicatePoints(NpgsqlPolygon polygon)
        {
            var pointSet = new HashSet<NpgsqlPoint>();
            foreach (var point in polygon)
            {
                if (pointSet.Contains(point))
                {
                    return true;
                }
                pointSet.Add(point);
            }
            return false;
        }

        private bool PolygonSharesPointsWithExistingPolygon(NpgsqlPolygon newPolygon, long? excludedAreaId = null)
        {
            Polygon currentPolygon = ToPolygonGeometry(newPolygon);
            var existingAreas = _db.Areas.AsEnumerable();

            foreach (var existingArea in existingAreas)
            {
                if (excludedAreaId.HasValue && existingArea.Id == excludedAreaId.Value)
                {
                    continue;
                }

                Polygon existingPolygon = ToPolygonGeometry(existingArea.Points);
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

            for (int i = 0; i < polygon1.Count; i++)
            {
                bool foundMatch = true;
                for (int j = 0; j < polygon1.Count; j++)
                {
                    if (polygon1[(i + j) % polygon1.Count] != polygon2[j])
                    {
                        foundMatch = false;
                        break;
                    }
                }

                if (foundMatch)
                {
                    return true;
                }
            }

            // Check for reversed order
            for (int i = 0; i < polygon1.Count; i++)
            {
                bool foundMatch = true;
                for (int j = 0; j < polygon1.Count; j++)
                {
                    if (polygon1[(i + j) % polygon1.Count] != polygon2[polygon2.Count - 1 - j])
                    {
                        foundMatch = false;
                        break;
                    }
                }

                if (foundMatch)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Update

        public async Task<IServiceResponse<Area>> UpdateAreaByIdAsync(long? areaId, AreaRequestDto request)
        {
            if (!IdValidator.IsValid(areaId) || !AreaRequestValidator.IsValid(request))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            if (await _db.Areas.FindAsync(areaId) is not Area existingArea)
            {
                return new ServiceResponse<Area>(HttpStatusCode.NotFound);
            }

            Area updatedArea = ToArea(request);

            var existingAreas = _db.Areas.AsEnumerable();
            if (existingAreas.Any(a => ArePolygonsEqual(a.Points, updatedArea.Points) && a.Id != areaId!.Value))
            {
                return new ServiceResponse<Area>(HttpStatusCode.Conflict);
            }

            if (PolygonSharesPointsWithExistingPolygon(updatedArea.Points, areaId))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            if (PointsAreCollinear(updatedArea.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PointsAreCollinear");

            if (PolygonHasOverlappingEdges(updatedArea.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonHasOverlappingEdges");

            if (PolygonIntersectsExistingPolygon(updatedArea.Points, areaId))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonIntersectsExistingPolygon");

            if (PolygonHasDuplicatePoints(updatedArea.Points))
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest, errorMessage: "PolygonHasDuplicatePoints");

            if (await _db.Areas.AnyAsync(a => a.Name == request.Name))
                return new ServiceResponse<Area>(HttpStatusCode.Conflict, errorMessage: "Name already exists");

            // Update the area
            existingArea.Name = updatedArea.Name;
            existingArea.Points = updatedArea.Points;

            try
            {
                await _db.SaveChangesAsync();
                return new ServiceResponse<Area>(HttpStatusCode.OK, existingArea);
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
