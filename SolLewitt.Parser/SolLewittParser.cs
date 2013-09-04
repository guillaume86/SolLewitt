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
                case "LinesIntersectionPolygon": return GetLinesIntersectionPolygonExpression(parseTreeNode);
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
                case "SideLengthAndPositionSquare": return GetSideLengthAndPositionSquareExpression(parseTreeNode);
                case "DefinedByTwoPointPoint": return GetDefinedByTwoPointPointExpression(parseTreeNode);
                case "AxisOfAndEquidistantFromTwoPointsLine": return GetAxisOfAndEquidistantFromTwoPointsLineExpression(parseTreeNode);
                case "PointDefinition": return GetPointDefinitionExpression(parseTreeNode);
                default: throw new NotImplementedException(parseTreeNode.Term.Name);
            }
        }

        private SLExpression GetPointDefinitionExpression(ParseTreeNode parseTreeNode)
        {
            return (PointExpression)GetExpression(parseTreeNode.ChildNodes[2]);
        }

        private SLExpression GetAxisOfAndEquidistantFromTwoPointsLineExpression(ParseTreeNode parseTreeNode)
        {
            var twoPointsList = parseTreeNode.ChildNodes[4];

            return new TwoPointsLineExpression
            {
                Point1 = (PointExpression)GetExpression(twoPointsList.ChildNodes[0]),
                Point2 = (PointExpression)GetExpression(twoPointsList.ChildNodes[1]),
            };
        }

        private SLExpression GetDefinedByTwoPointPointExpression(ParseTreeNode parseTreeNode)
        {
            var twoPointsList = parseTreeNode.ChildNodes[1];

            return new DefinedByTwoPointsPointExpression
            {
                Point1 = (PointExpression)GetExpression(twoPointsList.ChildNodes[0]),
                Point2 = (PointExpression)GetExpression(twoPointsList.ChildNodes[1]),
            };
        }

        private SLExpression GetSideLengthAndPositionSquareExpression(ParseTreeNode parseTreeNode)
        {
            var sideDirectionAndLengthDescription = parseTreeNode.ChildNodes[0];
            var sidePositionDescription = parseTreeNode.ChildNodes[2];
            var sidePositionLine = sidePositionDescription.ChildNodes[2];

            var sideDirection = sidePositionDescription.ChildNodes[0].ChildNodes[0];
            var fraction = sideDirectionAndLengthDescription.ChildNodes[3];

            return new SideLengthAndPositionSquareExpression
            {
                SideDirection = sideDirection.Term.Name,
                LinesLengthToSideLengthFactor = fraction.Term.Name,
                SidePosition = (TwoPointsLineExpression)GetExpression(sidePositionLine),
                Lines = GetExpressions(parseTreeNode.ChildNodes[1].ChildNodes).Cast<LineExpression>().ToArray(),
            };
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
            var lineNode = parseTreeNode.ChildNodes[1];
            var extremity = parseTreeNode.ChildNodes[0].Term.Name;

            var isVerb = parseTreeNode.ChildNodes[1].Term.Name == "ExtremityVerb";
            if (isVerb)
            {
                var verb = parseTreeNode.ChildNodes[1].ChildNodes[0].Term.Name;
                if (verb != "starts" && verb != "ends")
                {
                    throw new InvalidOperationException();
                }

                lineNode = parseTreeNode.ChildNodes[0];
                extremity = parseTreeNode.ChildNodes[1].ChildNodes[0].Term.Name == "starts"
                    ? "start"
                    : "end";
            }

            if (extremity != "start" && extremity != "end")
            {
                throw new InvalidOperationException();
            }

            return new LineExtremityPointExpression
            {
                Extremity = extremity,
                Line = (LineExpression)GetExpression(lineNode),
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
            return new CoordinatesPointExpression { X = 1 / 2d, Y = 1 / 2d };
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
        private SLExpression GetLinesIntersectionPolygonExpression(ParseTreeNode parseTreeNode)
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