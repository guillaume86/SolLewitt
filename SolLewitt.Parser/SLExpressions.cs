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

    public class CoordinatesPointExpression : PointExpression
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class HalfwayPointExpression : PointExpression
    {
        public LineExpression Line;
    }
}
