using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolLewitt.Parser
{
    public class SLExpressionVisitor
    {
        public SLExpressionVisitor()
        {

        }

        public virtual SLExpression Visit(SLExpression expression)
        {
            var expType = expression.GetType().Name;
            switch(expType)
            {
                case "DrawingExpression": return VisitDrawing((DrawingExpression)expression);
                case "PolygonFromLinesExpression": return VisitPolygonFromLines((PolygonFromLinesExpression)expression);
                case "PolygonFromPointsExpression": return VisitPolygonFromPoints((PolygonFromPointsExpression)expression);
                case "IdentifiedLineExpression": return VisitIdentifiedLine((IdentifiedLineExpression)expression);
                case "TwoPointsLineExpression": return VisitTwoPointsLine((TwoPointsLineExpression)expression);
                case "HalfwayPointExpression": return VisitHalfwayPoint((HalfwayPointExpression)expression);
                case "CoordinatesPointExpression": return VisitCoordinatesPoint((CoordinatesPointExpression)expression);
                case "LineExtremityPointExpression": return VisitLineExtremityPoint((LineExtremityPointExpression)expression);
                case "LineReferenceExpression": return VisitLineReference((LineReferenceExpression)expression);
                case "LinesIntersectionPointExpression": return VisitLinesIntersectionPoint((LinesIntersectionPointExpression)expression);
                case "EquidistantPointExpression": return VisitEquidistantPoint((EquidistantPointExpression)expression);
                default: throw new NotImplementedException(expType);
            }
        }

        protected virtual SLExpression VisitPolygonFromPoints(PolygonFromPointsExpression polygonFromPointsExpression)
        {
            var points = polygonFromPointsExpression.Points.Select(Visit).ToArray();
            if (HasChanged(points, polygonFromPointsExpression.Points))
            {
                return new PolygonFromPointsExpression
                {
                    Points = points.Cast<PointExpression>().ToArray(),
                };
            }
            return polygonFromPointsExpression;
        }

        protected virtual SLExpression VisitEquidistantPoint(EquidistantPointExpression equidistantPointExpression)
        {
            var points = equidistantPointExpression.Points.Select(Visit).ToArray();
            if (HasChanged(points, equidistantPointExpression.Points))
            {
                return new EquidistantPointExpression
                {
                    Points = points.Cast<PointExpression>().ToArray(),
                };
            }
            return equidistantPointExpression;
        }

        protected virtual SLExpression VisitLinesIntersectionPoint(LinesIntersectionPointExpression linesIntersectionPointExpression)
        {
            var line1 = Visit(linesIntersectionPointExpression.Line1);
            var line2 = Visit(linesIntersectionPointExpression.Line2);
            if (line1 != linesIntersectionPointExpression.Line1 || line2 != linesIntersectionPointExpression.Line2)
            {
                return new LinesIntersectionPointExpression
                {
                    Line1 = (LineExpression)line1,
                    Line2 = (LineExpression)line2
                };
            }
            return linesIntersectionPointExpression;
        }

        protected virtual SLExpression VisitLineReference(LineReferenceExpression lineReferenceExpression)
        {
            return lineReferenceExpression;
        }

        protected virtual SLExpression VisitLineExtremityPoint(LineExtremityPointExpression lineExtremityPointExpression)
        {
            var line = Visit(lineExtremityPointExpression.Line);
            if (line != lineExtremityPointExpression.Line)
            {
                return new LineExtremityPointExpression
                {
                    Extremity = lineExtremityPointExpression.Extremity,
                    Line = (LineExpression)line,
                };
            }
            return lineExtremityPointExpression;
        }

        protected virtual SLExpression VisitCoordinatesPoint(CoordinatesPointExpression coordinatesPointExpression)
        {
            return coordinatesPointExpression;
        }

        protected virtual SLExpression VisitHalfwayPoint(HalfwayPointExpression halfwayPointExpression)
        {
            var line = Visit(halfwayPointExpression.Line);
            if (line != halfwayPointExpression.Line)
            {
                return new HalfwayPointExpression
                {
                    Line = (LineExpression)line,
                };
            }
            return halfwayPointExpression;
        }

        protected virtual SLExpression VisitTwoPointsLine(TwoPointsLineExpression twoPointsLineExpression)
        {
            var point1 = Visit(twoPointsLineExpression.Point1);
            var point2 = Visit(twoPointsLineExpression.Point2);
            if (point1 != twoPointsLineExpression.Point1 || point2 != twoPointsLineExpression.Point2)
            {
                return new TwoPointsLineExpression
                {
                    Point1 = (PointExpression)point1,
                    Point2 = (PointExpression)point2
                };
            }
            return twoPointsLineExpression;
        }

        protected virtual SLExpression VisitIdentifiedLine(IdentifiedLineExpression identifiedLineExpression)
        {
            var line = Visit(identifiedLineExpression.Line);
            if (line != identifiedLineExpression.Line)
            {
                return new IdentifiedLineExpression
                {
                    Id = identifiedLineExpression.Id,
                    Line = (LineExpression)line,
                };
            }
            return identifiedLineExpression;
        }

        protected virtual SLExpression VisitPolygonFromLines(PolygonFromLinesExpression polygonFromLinesExpression)
        {
            var lines = polygonFromLinesExpression.Lines.Select(Visit).ToArray();
            if (HasChanged(lines, polygonFromLinesExpression.Lines))
            {
                return new PolygonFromLinesExpression
                {
                    Lines = lines.Cast<LineExpression>().ToArray(),
                };
            }
            return polygonFromLinesExpression;
        }

        protected virtual SLExpression VisitDrawing(DrawingExpression expression)
        {
            var polygons = expression.Polygons.Select(Visit).ToArray();
            if (HasChanged(expression.Polygons, polygons))
            {
                return new DrawingExpression
                {
                    Polygons = polygons.Cast<PolygonExpression>().ToArray(),
                };
            }
            return expression;
        }

        protected bool HasChanged(SLExpression[] list1, SLExpression[] list2)
        {
            return list1.Length != list2.Length
                || list1.Zip(list2, (l1, l2) => l1 != l2).Any();
        }
    }
}
