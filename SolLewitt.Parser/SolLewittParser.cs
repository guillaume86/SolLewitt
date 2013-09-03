using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace SolLewitt.Parser
{
    public class SolLewittParser
    {
        private readonly SolLewittGrammar Grammar;
        private readonly Irony.Parsing.Parser Parser;

        public SolLewittParser()
        {
            Grammar = new SolLewittGrammar();
            Parser = new Irony.Parsing.Parser(Grammar);
        }

        public ParseTree Parse(string instructions)
        {
            return Parser.Parse(instructions);
        }

        public SLExpression GetExpression(ParseTree parseTree)
        {
            return GetExpression(parseTree.Root);
        }

        public SLExpression GetReducedExpression(ParseTree parseTree)
        {
            var fullExp = GetExpression(parseTree);
            
            var refVisitor = new ReferenceSubstitutionVisitor();
            var noRefExp = refVisitor.Visit(fullExp);

            var evalVisitor = new CoordinatesEvalutionVisitor();
            var coordExp = evalVisitor.Visit(noRefExp);

            return coordExp;
        }

        private SLExpression GetExpression(ParseTreeNode parseTreeNode)
        {
            switch(parseTreeNode.Term.Name)
            {
                case "Drawing": return GetDrawingExpression(parseTreeNode);
                case "Polygon": return GetPolygonExpression(parseTreeNode);
                case "LineDefinition": return GetLineDefinitionExpression(parseTreeNode);
                case "TwoPointsLine": return GetTwoPointsLineExpression(parseTreeNode);
                case "HalfwayPoint": return GetHalfwayPointExpression(parseTreeNode);
                case "center of the wall": return GetCenterOfWallExpression();
                case "Corner": return GetCornerExpression(parseTreeNode);
                case "Side": return GetSideExpression(parseTreeNode);
                case "LineExtremityPoint": return GetLineExtremityExpression(parseTreeNode);
                case "ReferencedLine": return GetReferencedLineExpression(parseTreeNode);
                case "LinesIntersectionPoint": return GetLinesIntersectionPointExpression(parseTreeNode);
                case "EquidistantPoint": return GetEquidistantPointExpression(parseTreeNode);
                default: throw new NotImplementedException(parseTreeNode.Term.Name);
            }
        }

        private SLExpression GetEquidistantPointExpression(ParseTreeNode parseTreeNode)
        {
            return new EquidistantPointExpression
            {
                Points = GetExpressions(parseTreeNode.ChildNodes[0].ChildNodes).Cast<PointExpression>().ToArray(),
            };
        }

        private SLExpression GetLinesIntersectionPointExpression(ParseTreeNode parseTreeNode)
        {
            return new LinesIntersectionPointExpression
            {
                Line1 = (LineExpression)GetExpression(parseTreeNode.ChildNodes[0]),
                Line2 = (LineExpression)GetExpression(parseTreeNode.ChildNodes[1]),
            };
        }

        private SLExpression GetReferencedLineExpression(ParseTreeNode parseTreeNode)
        {
            return new LineReferenceExpression
            {
                Id = parseTreeNode.ChildNodes[0].ChildNodes[0].Term.Name,
            };
        }

        private SLExpression GetLineExtremityExpression(ParseTreeNode parseTreeNode)
        {
            return new LineExtremityPointExpression
            {
                Extremity = parseTreeNode.ChildNodes[0].Term.Name,
                Line = (LineExpression)GetExpression(parseTreeNode.ChildNodes[1]),
            };
        }

        private SLExpression GetSideExpression(ParseTreeNode parseTreeNode)
        {
            var direction = parseTreeNode.ChildNodes[0].Term.Name;
            if (!new[] { "left", "right", "TopDirection", "BottomDirection" }.Contains(direction))
            {
                throw new NotImplementedException(direction);
            }

            return new TwoPointsLineExpression
            {
                Point1 = new CoordinatesPointExpression
                {
                    X = direction == "right" ? 1 : 0,
                    Y = direction == "BottomDirection" ? 1 : 0,
                },
                Point2 = new CoordinatesPointExpression
                {
                    X = direction == "left" ? 0 : 1,
                    Y = direction == "TopDirection" ? 0 : 1,
                },
            };
        }

        private SLExpression GetCornerExpression(ParseTreeNode parseTreeNode)
        {
            return new CoordinatesPointExpression
            {
                X = parseTreeNode.ChildNodes[1].Term.Name == "left" ? 0 : 1,
                Y = parseTreeNode.ChildNodes[0].Term.Name == "TopDirection" ? 0 : 1,
            };
        }

        private static CoordinatesPointExpression GetCenterOfWallExpression()
        {
            return new CoordinatesPointExpression { X = 1 / 2, Y = 1 / 2 };
        }

        private SLExpression GetHalfwayPointExpression(ParseTreeNode parseTreeNode)
        {
            return new HalfwayPointExpression
            {
                Line = (LineExpression)GetExpression(parseTreeNode.ChildNodes[0]),
            };
        }

        private SLExpression GetTwoPointsLineExpression(ParseTreeNode parseTreeNode)
        {
            return new TwoPointsLineExpression
            {
                Point1 = (PointExpression)GetExpression(parseTreeNode.ChildNodes[0]),
                Point2 = (PointExpression)GetExpression(parseTreeNode.ChildNodes[1]),
            };
        }

        private SLExpression GetLineDefinitionExpression(ParseTreeNode parseTreeNode)
        {
            return new IdentifiedLineExpression
            {
                Id = parseTreeNode.ChildNodes[0].ChildNodes[0].Token.Text,
                Line = (LineExpression)GetExpression(parseTreeNode.ChildNodes[1]),
            };
        }

        // PolygonDescription + LineDefinitionList
        private SLExpression GetPolygonExpression(ParseTreeNode parseTreeNode)
        {
            return new PolygonFromLinesExpression
            {
                Lines = GetExpressions(parseTreeNode.ChildNodes[1].ChildNodes).Cast<LineExpression>().ToArray(),
            };
        }

        private SLExpression[] GetExpressions(IEnumerable<ParseTreeNode> parseTreeNodes)
        {
            return parseTreeNodes.Select(GetExpression).ToArray();
        }

        // PolygonList
        private SLExpression GetDrawingExpression(ParseTreeNode parseTreeNode)
        {
            return new DrawingExpression
            {
                Polygons = GetExpressions(parseTreeNode.ChildNodes).Cast<PolygonExpression>().ToArray(),
            };
        }
    }
}