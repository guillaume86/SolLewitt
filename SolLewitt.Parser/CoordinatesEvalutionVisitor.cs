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

        private static CoordinatesPointExpression GetEquidistantPoint(params CoordinatesPointExpression[] points)
        {
            return points.Aggregate((p1,p2) => p1 + p2) / (double)points.Length;
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

        protected override SLExpression VisitDefinedByTwoPointsPoint(DefinedByTwoPointsPointExpression definedByTwoPointsPointExpression)
        {
            var point1 = (CoordinatesPointExpression)Visit(definedByTwoPointsPointExpression.Point1);
            var point2 = (CoordinatesPointExpression)Visit(definedByTwoPointsPointExpression.Point2);

            if (point1.X != point2.X || point1.Y != point2.Y)
            {
                throw new InvalidOperationException("Points should have the same coordinates.");
            }

            return point1;
        }

        protected override SLExpression VisitSideLengthAndPositionSquare(SideLengthAndPositionSquareExpression expression)
        {
            var sidePositionAxis = (TwoPointsLineExpression)Visit(expression.SidePosition);
            var axisLength = GetDistance(
                (CoordinatesPointExpression)sidePositionAxis.Point1, 
                (CoordinatesPointExpression)sidePositionAxis.Point2);

            var lines = expression.Lines.Select(Visit).Cast<TwoPointsLineExpression>().ToArray();
            var totalLinesLength = lines.Select(l => GetDistance((CoordinatesPointExpression)l.Point1, (CoordinatesPointExpression)l.Point2)).Sum();
            var factor = GetFactor(expression.LinesLengthToSideLengthFactor);
            var sideLength = factor * totalLinesLength;

            var sideAxisPoint1 = (CoordinatesPointExpression)sidePositionAxis.Point1;
            var sideAxisPoint2 = (CoordinatesPointExpression)sidePositionAxis.Point2;
            var sideCenter = GetEquidistantPoint(sideAxisPoint1, sideAxisPoint2);

            var axisPoint1RelativePositionToCenterOfSide = sideAxisPoint1 - sideCenter;
            var axisPoint2RelativePositionToCenterOfSide = sideAxisPoint2 - sideCenter;

            var squareSideToAxisLengthFactor = sideLength / axisLength;

            var squareCorner1 = axisPoint1RelativePositionToCenterOfSide * squareSideToAxisLengthFactor + sideCenter;
            var squareCorner2 = axisPoint2RelativePositionToCenterOfSide * squareSideToAxisLengthFactor + sideCenter;

            var otherSideDirection = axisPoint1RelativePositionToCenterOfSide * squareSideToAxisLengthFactor * 2;
            otherSideDirection = new CoordinatesPointExpression
            {
                X = otherSideDirection.Y,
                Y = -otherSideDirection.X,
            };

            if (expression.SideDirection == "right" || expression.SideDirection == "bottom")
            {
                otherSideDirection = otherSideDirection * -1;
            }

            var squareCorner3 = squareCorner1 + otherSideDirection;
            var squareCorner4 = squareCorner2 + otherSideDirection;

            return new PolygonFromPointsExpression
            {
                Points = new[] 
                {
                    squareCorner1,
                    squareCorner2,
                    squareCorner4,
                    squareCorner3,
                }
            };

            //return new PolygonFromLinesExpression
            //{
            //    Lines = lines//.Take(1)
            //        .Concat(new[]
            //        {
            //            sidePositionAxis,
                    
            //        }).ToArray(),
            //};
        }

        private double GetFactor(string factorStr)
        {
            if (factorStr == "tenth")
            {
                return 1 / 10d;
            }
            throw new NotImplementedException(factorStr);
        }

        private double GetDistance(CoordinatesPointExpression point1, CoordinatesPointExpression point2)
        {
            return Math.Sqrt(
                Math.Pow(point2.X - point1.X, 2) + 
                Math.Pow(point2.Y - point1.Y, 2));
        }
    }
}
