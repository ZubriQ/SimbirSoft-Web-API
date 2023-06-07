using NetTopologySuite.Geometries;
using NpgsqlTypes;
using SimbirSoft_Web_API.Mapping;

namespace SimbirSoft_Web_API.Helpers.Validators;

public class AreaValidator
{
    private readonly AreaMapper _mapper = new();
    private IEnumerable<Area> _areas = new List<Area>();

    public void SetExistingAreas(IEnumerable<Area> existingAreas)
    {
        _areas = existingAreas;
    }

    #region Geometry validation

    public bool IsGeometryValid(Area areaToCheck, long? areaId = null)
    {
        if (IsPointsCollinear(areaToCheck.Points) ||
            PolygonHasOverlappingEdges(areaToCheck.Points) ||
            PolygonIntersectsExistingPolygon(areaToCheck.Points, areaId) ||
            PolygonHasDuplicatePoints(areaToCheck.Points) ||
            PolygonSharesPointsWithExistingPolygon(areaToCheck.Points, areaId))
        {
            return false;
        }

        return true;
    }

    private bool IsPointsCollinear(NpgsqlPolygon polygon)
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

            if (!ArePointsCollinear(firstPoint, secondPoint, thirdPoint))
            {
                return false;
            }
        }

        return true;
    }

    private bool ArePointsCollinear(NpgsqlPoint firstPoint, NpgsqlPoint secondPoint, NpgsqlPoint thirdPoint)
    {
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

    private bool PolygonIntersectsExistingPolygon(NpgsqlPolygon newPolygon, long? excludedAreaId = null)
    {
        Polygon newPolygonGeometry = _mapper.ToPolygonGeometry(newPolygon);
        foreach (var existingArea in _areas)
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

    private bool PolygonSharesPointsWithExistingPolygon(NpgsqlPolygon newPolygon, long? excludedAreaId = null)
    {
        Polygon currentPolygon = _mapper.ToPolygonGeometry(newPolygon);
        var existingAreas = _areas;

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

    #endregion

    #region Geometry has conflicts

    public bool IsPointsAlreadyExist(Area areaToValidate, long? areaId = null)
    {
        foreach (Area area in _areas)
        {
            bool isAreaEqual = ArePolygonsEqual(area.Points, areaToValidate.Points);
            bool isIdDifferent = !areaId.HasValue || area.Id != areaId.Value;

            if (isAreaEqual && isIdDifferent)
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

    private NpgsqlPolygon ReversePolygon(NpgsqlPolygon polygon)
    {
        var reversedPolygon = new NpgsqlPolygon(polygon);
        reversedPolygon.Reverse();
        return reversedPolygon;
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

    #endregion
}
