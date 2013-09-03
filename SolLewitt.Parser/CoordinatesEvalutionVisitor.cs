using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolLewitt.Parser
{
    public class CoordinatesEvalutionVisitor : SLExpressionVisitor
    {
        private bool LeavePolygonAsLines;

        public CoordinatesEvalutionVisitor(bool leavePolygonAsLines = false)
        {
            this.LeavePolygonAsLines = leavePolygonAsLines;
        }

        protected override SLExpression VisitEquidistantPoint(EquidistantPointExpression equidistantPointExpression)
        {
            var points = equidistantPointExpression.Points.Select(Visit).Cast<CoordinatesPointExpression>().ToArray();
            return GetEquidistantPoint(points);
        }

        private static SLExpression GetEquidistantPoint(params CoordinatesPointExpression[] points)
        {
            return new CoordinatesPointExpression
            {
                X = points.Sum(p => p.X) / (double)points.Length,
                Y = points.Sum(p => p.Y) / (double)points.Length,
            };
        }

        protected override SLExpression VisitLineExtremityPoint(LineExtremityPointExpression lineExtremityPointExpression)
        {
            var line = (TwoPointsLineExpression)Visit(lineExtremityPointExpression.Line);
            return lineExtremityPointExpression.Extremity == "start"
                ? line.Point1
                : line.Point2;
        }

        protected override SLExpression VisitHalfwayPoint(HalfwayPointExpression halfwayPointExpression)
        {
            var line = (TwoPointsLineExpression)Visit(halfwayPointExpression.Line);
            return GetEquidistantPoint((CoordinatesPointExpression)line.Point1, (CoordinatesPointExpression)line.Point2);
        }

        protected override SLExpression VisitIdentifiedLine(IdentifiedLineExpression identifiedLineExpression)
        {
            return base.Visit(identifiedLineExpression.Line);
        }

        protected override SLExpression VisitLineReference(LineReferenceExpression lineReferenceExpression)
        {
            throw new NotSupportedException("LineReferenceExpression must be replaced before this step.");
        }

        protected override SLExpression VisitLinesIntersectionPoint(LinesIntersectionPointExpression linesIntersectionPointExpression)
        {
            var line1 = (TwoPointsLineExpression)Visit(linesIntersectionPointExpression.Line1);
            var line2 = (TwoPointsLineExpression)Visit(linesIntersectionPointExpression.Line2);

            return GetIntersectionPoint(line1, line2);
        }

        private static CoordinatesPointExpression GetIntersectionPoint(TwoPointsLineExpression line1, TwoPointsLineExpression line2)
        {
            var l1p1 = (CoordinatesPointExpression)line1.Point1;
            var l1p2 = (CoordinatesPointExpression)line1.Point2;
            var l2p1 = (CoordinatesPointExpression)line2.Point1;
            var l2p2 = (CoordinatesPointExpression)line2.Point2;

            // y = ax + c
            var a1 = (l1p1.Y - l1p2.Y) / (l1p1.X - l1p2.X);
            var c1 = l1p1.Y - l1p1.X * a1;
            // Ax + By = C
            var A1 = -a1;
            var B1 = 1;
            var C1 = c1;

            var a2 = (l2p1.Y - l2p2.Y) / (l2p1.X - l2p2.X);
            var c2 = l2p1.Y - l2p1.X * a2;
            var A2 = -a2;
            var B2 = 1;
            var C2 = c2;

            var delta = A1 * B2 - A2 * B1;
            if (delta == 0)
            {
                throw new ArgumentException("Can't compute intersection of parallel lines.");
            }

            var x = (B2 * C1 - B1 * C2) / delta;
            var y = (A1 * C2 - A2 * C1) / delta;

            return new CoordinatesPointExpression
            {
                X = x,
                Y = y,
            };
        }

        protected override SLExpression VisitPolygonFromLines(PolygonFromLinesExpression polygonFromLinesExpression)
        {
            if (LeavePolygonAsLines)
            {
                return polygonFromLinesExpression;
            }

            var points = new List<PointExpression>();
            var lines = polygonFromLinesExpression.Lines.Select(Visit).Cast<TwoPointsLineExpression>().ToArray();
            lines.Aggregate(lines.Last(), (l1, l2) =>
            {
                points.Add(GetIntersectionPoint(l1, l2));
                return l2;
            });

            return new PolygonFromPointsExpression
            {
                Points = points.ToArray(),
            };
        }
    }
}
