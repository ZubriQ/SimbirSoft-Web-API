using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Olymp_Project.Dtos.Area;
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
            if (!AreaValidator.IsValid(request))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            // TODO: (400)
            //Все точки лежат на одной прямой.
            //Границы новой зоны пересекаются между собой.
            //Границы новой зоны пересекают границы уже существующей зоны.
            //Граница новой зоны находятся внутри границ существующей зоны.
            //Границы существующей зоны находятся внутри границ новой зоны.
            //Новая зона имеет дубликаты точек.
            //Новая зона состоит из части точек существующей зоны и находится на площади существующей зоны.
            // TODO: (409)
            //Зона, состоящая из таких точек, уже существует. (При этом важен порядок, в котором указаны точки,
            //    но не важна начальная точка).
            //Зона с таким name уже существует.

            #region Validation

            Area area = ToArea(request);

            // Все точки лежат на одной прямой.
            if (PointsAreCollinear(area.Points) || 
                PolygonHasOverlappingEdges(area.Points) ||
                PolygonIntersectsExistingPolygon(area.Points))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            //if (PointsAreCollinear(area.Points) || PolygonHasOverlappingEdges(area.Points)
            //    || PolygonIntersectsExistingPolygon(area.Points) || PolygonIsInsideExistingPolygon(area.Points)
            //    || ExistingPolygonIsInsidePolygon(area.Points) || AreaHasDuplicatePoints(area.Points))
            //{
            //    return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            //}

            //if (await AreaNameExistsAsync(area.Name) || await AreaWithSamePointsExistsAsync(area.Points))
            //{
            //    return new ServiceResponse<Area>(HttpStatusCode.Conflict);
            //}

            #endregion

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

        //private async Task<bool> AreaNameExistsAsync(string name)
        //{
        //    return await _db.Areas.AnyAsync(a => a.Name == name);
        //}

        //private async Task<bool> AreaWithSamePointsExistsAsync(NpgsqlPolygon polygon)
        //{
        //    return await _db.Areas.AnyAsync(a => a.Points.Equals(polygon));
        //}

        //private bool AreaHasDuplicatePoints(NpgsqlPolygon polygon)
        //{
        //    return Enumerable.Range(0, polygon.Count)
        //        .GroupBy(i => polygon[i])
        //        .Any(g => g.Count() > 1);
        //}

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
            LineString polygonBoundary = ToPolygonGeometry(polygon).Boundary as LineString;
            GeometryFactory geometryFactory = new GeometryFactory();
            for (int i = 0; i < polygonBoundary.NumPoints - 1; i++)
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

        private bool PolygonIntersectsExistingPolygon(NpgsqlPolygon newPolygon)
        {
            Polygon newPolygonGeometry = ToPolygonGeometry(newPolygon);
            foreach (var existingArea in _db.Areas)
            {
                Polygon existingPolygonGeometry = ToPolygonGeometry(existingArea.Points);
                // Check if the polygons have an intersection that is not only at their boundaries
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
            coordinates[npgsqlPolygon.Count] = coordinates[0]; // Close the polygon

            var geometryFactory = new GeometryFactory();
            return geometryFactory.CreatePolygon(coordinates);
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

        //private bool PolygonIsInsideExistingPolygon(NpgsqlPolygon newPolygon)
        //{
        //    Polygon newPolygonGeometry = ToPolygonGeometry(newPolygon);
        //    foreach (var existingArea in _db.Areas)
        //    {
        //        Polygon existingPolygonGeometry = ToPolygonGeometry(existingArea.Points);
        //        if (newPolygonGeometry.Within(existingPolygonGeometry))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}



        //private bool ExistingPolygonIsInsidePolygon(NpgsqlPolygon newPolygon)
        //{
        //    Polygon newPolygonGeometry = ToPolygonGeometry(newPolygon);
        //    foreach (var existingArea in _db.Areas)
        //    {
        //        Polygon existingPolygonGeometry = ToPolygonGeometry(existingArea.Points);
        //        if (existingPolygonGeometry.Within(newPolygonGeometry))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        #endregion

        #region Update

        public async Task<IServiceResponse<Area>> UpdateAreaByIdAsync(long? areaId, AreaRequestDto request)
        {
            if (!IdValidator.IsValid(areaId) || !AreaValidator.IsValid(request))
            {
                return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            }

            if (await _db.Areas.FindAsync(areaId) is not Area existingArea)
            {
                return new ServiceResponse<Area>(HttpStatusCode.NotFound);
            }

            Area updatedArea = ToArea(request);

            // Validate the updated area
            //NpgsqlPolygon updatedPolygon = updatedArea.Points;
            //if (PointsAreCollinear(updatedPolygon) ||
            //    PolygonHasOverlappingEdges(updatedPolygon) ||
            //    PolygonIntersectsExistingPolygon(updatedPolygon)) //||
            //    //PolygonHasDuplicatePoints(updatedPolygon) || // You need to create this method
            //    //NewPolygonBoundaryInsideExistingPolygonBoundary(updatedPolygon) || // You need to create this method
            //    //ExistingPolygonBoundaryInsideNewPolygonBoundary(updatedPolygon) || // You need to create this method
            //    //NewPolygonSharesPointsWithExistingAndIsInside(updatedPolygon, existingArea.Points)) // You need to create this method
            //{
            //    return new ServiceResponse<Area>(HttpStatusCode.BadRequest);
            //}

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
            // TODO: ADMIN

            if (await _db.Areas.FindAsync(areaId!.Value) is not Area existingArea)
            {
                return HttpStatusCode.NotFound;
            }

            try
            {
                _db.Areas.Remove(existingArea);
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
