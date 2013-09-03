using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SolLewitt.Parser
{
    public class SvgConverter : SLExpressionVisitor
    {
        public SvgConverter()
        {

        }

        private List<PolygonFromLinesExpression> PolygonsFromLines;
        private List<PolygonFromPointsExpression> PolygonsFromPoints;
        public XDocument Convert(SLExpression expression)
        {
            PolygonsFromLines = new List<PolygonFromLinesExpression>();
            PolygonsFromPoints = new List<PolygonFromPointsExpression>();

            this.Visit(expression);

            var doc = new XDocument(
                new XElement("svg",
                    PolygonsFromPoints.Select(ConvertPolygon).ToArray(),
                    PolygonsFromLines.Select(ConvertPolygon).ToArray(),
                    new XAttribute("viewBox", "0 0 1 1")));
            return doc;
        }

        public XElement ConvertPolygon(PolygonFromPointsExpression polygon)
        {
            var points = polygon.Points.Cast<CoordinatesPointExpression>().ToArray();
            var pointsAttributeValue = String.Join(" ", 
                points.Select(p => String.Format(CultureInfo.InvariantCulture, "{0},{1}", p.X, p.Y)));

            return new XElement("polygon",
                new XAttribute("fill", "green"),
                new XAttribute("points", pointsAttributeValue));
        }

        public XElement ConvertPolygon(PolygonFromLinesExpression polygon)
        {
            var lines = polygon.Lines.Cast<TwoPointsLineExpression>().ToArray();

            return new XElement("g",
                new XAttribute("stroke", "green"),
                new XAttribute("stroke-width", "0.01"),
                lines
                .Select(l => new
                {
                    P1 = (CoordinatesPointExpression)l.Point1,
                    P2 = (CoordinatesPointExpression)l.Point2,
                })
                .Select(l => new XElement("line",
                    new XAttribute("x1", l.P1.X),
                    new XAttribute("y1", l.P1.Y),
                    new XAttribute("x2", l.P2.X),
                    new XAttribute("y2", l.P2.Y)))
                .ToArray());
        }

        protected override SLExpression VisitPolygonFromLines(PolygonFromLinesExpression polygonFromLinesExpression)
        {
            PolygonsFromLines.Add(polygonFromLinesExpression);
            return polygonFromLinesExpression;
        }

        protected override SLExpression VisitPolygonFromPoints(PolygonFromPointsExpression polygonFromPointsExpression)
        {
            PolygonsFromPoints.Add(polygonFromPointsExpression);
            return polygonFromPointsExpression;
        }
    }
}
