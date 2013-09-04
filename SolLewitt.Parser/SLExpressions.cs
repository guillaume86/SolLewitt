using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolLewitt.Parser
{
    public abstract class SLExpression
    {

    }

    public class DrawingExpression : SLExpression
    {
        public PolygonExpression[] Polygons { get; set; }
    }

    public class PolygonExpression : SLExpression
    {

    }

    public class SideLengthAndPositionSquareExpression : PolygonExpression
    {
        public string LinesLengthToSideLengthFactor { get; set; }
        public string SideDirection { get; set; }
        public LineExpression[] Lines { get; set; }
        public TwoPointsLineExpression SidePosition { get; set; }
    }

    public class PolygonFromPointsExpression : PolygonExpression
    {
        public PointExpression[] Points { get; set; }
    }

    public class PolygonFromLinesExpression : PolygonExpression
    {
        public LineExpression[] Lines { get; set; }
    }

    public abstract class LineExpression : SLExpression
    {

    }

    public class LineReferenceExpression : LineExpression
    {
        public string Id { get; set; }
    }

    public class LineExtremityPointExpression : PointExpression
    {
        public string Extremity { get; set; }
        public LineExpression Line { get; set; }
    }

    public class LinesIntersectionPointExpression : PointExpression
    {
        public LineExpression Line1 { get; set; }
        public LineExpression Line2 { get; set; }
    }

    public class DefinedByTwoPointsPointExpression : PointExpression
    {
        public PointExpression Point1 { get; set; }
        public PointExpression Point2 { get; set; }
    }

    public class TwoPointsLineExpression : LineExpression
    {
        public PointExpression Point1 { get; set; }
        public PointExpression Point2 { get; set; }
    }

    public class IdentifiedLineExpression : LineExpression
    {
        public string Id { get; set; }
        public LineExpression Line { get; set; }
    }

    public abstract class PointExpression : SLExpression
    {

    }

    public class EquidistantPointExpression : PointExpression
    {
        public PointExpression[] Points { get; set; }
    }

    public class HalfwayPointExpression : PointExpression
    {
        public LineExpression Line;
    }

    public class CoordinatesPointExpression : PointExpression
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static CoordinatesPointExpression operator +(CoordinatesPointExpression point1, CoordinatesPointExpression point2)
        {
            return new CoordinatesPointExpression
            {
                X = point1.X + point2.X,
                Y = point1.Y + point2.Y,
            };
        }

        public static CoordinatesPointExpression operator -(CoordinatesPointExpression point1, CoordinatesPointExpression point2)
        {
            return new CoordinatesPointExpression
            {
                X = point1.X - point2.X,
                Y = point1.Y - point2.Y,
            };
        }

        public static CoordinatesPointExpression operator *(CoordinatesPointExpression point1, double factor)
        {
            return new CoordinatesPointExpression
            {
                X = point1.X * factor,
                Y = point1.Y * factor,
            };
        }

        public static CoordinatesPointExpression operator /(CoordinatesPointExpression point1, double factor)
        {
            return new CoordinatesPointExpression
            {
                X = point1.X / factor,
                Y = point1.Y / factor,
            };
        }
    }
}
