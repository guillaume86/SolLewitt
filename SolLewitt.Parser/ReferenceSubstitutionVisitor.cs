using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolLewitt.Parser
{
    public class ReferenceSubstitutionVisitor : SLExpressionVisitor
    {
        private LineExpression[] PolygonLines { get; set; }

        protected override SLExpression VisitPolygonFromLines(PolygonFromLinesExpression polygonFromLinesExpression)
        {
            this.PolygonLines = polygonFromLinesExpression.Lines;
            return base.VisitPolygonFromLines(polygonFromLinesExpression);
        }

        protected override SLExpression VisitLineReference(LineReferenceExpression lineReferenceExpression)
        {
            var line = PolygonLines.Cast<IdentifiedLineExpression>()
                .First(l => l.Id == lineReferenceExpression.Id);

            return Visit(line);
        }
    }
}
