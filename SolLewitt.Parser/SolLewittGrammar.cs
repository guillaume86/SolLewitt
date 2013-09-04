using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace SolLewitt.Parser
{
    [Language("SolLewittDrawing", "0.1", "Sol Lewitt style drawing description language")]
    public class SolLewittGrammar : Grammar
    {
        public SolLewittGrammar() : base (false)
        {
            // 1. Terminals
            var upper = ToTerm("upper");
            var top = ToTerm("top");
            var lower = ToTerm("lower");
            var bottom = ToTerm("bottom");
            var left = ToTerm("left");
            var right = ToTerm("right");
            var center = ToTerm("center of the wall");
            var from = ToTerm("from");
            var to = ToTerm("to");
            var and = ToTerm("and");
            var start = ToTerm("start");
            var of = ToTerm("of");
            var end = ToTerm("end");
            var the = ToTerm("the");
            var halfwayBetween = ToTerm("halfway between");
            var midpointOf = ToTerm("midpoint of");
            var aPoint = ToTerm("a point");
            var comma = ToTerm(",");
            var dotComma = ToTerm(";");
            var dot = ToTerm(".");
            var line = ToTerm("line");
            var ofWhichIsDrawn = ToTerm("of which is drawn");
            var equidistantFrom = ToTerm("equidistant from");
            var whichIsOn = ToTerm("which is on");
            var side = ToTerm("side");
            var where = ToTerm("where");
            var whereALineWouldCross = ToTerm("where a line would cross");
            var aLineDrawn = ToTerm("a line drawn");
            var anAxisBetween = ToTerm("an axis between");
            var wouldCrossThatAxis = ToTerm("would cross that axis");
            var ifItWereDrawn = ToTerm("if it were drawn");
            var isDrawn = ToTerm("is drawn");

            CommentTerminal SingleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            CommentTerminal DelimitedComment = new CommentTerminal("DelimitedComment", "/*", "*/");
            NonGrammarTerminals.Add(SingleLineComment);
            NonGrammarTerminals.Add(DelimitedComment);

            NonGrammarTerminals.Add(the);


            // 2. Non-terminals
            var LineDefinitionVariants = new NonTerminal("LineDefinitionVariants");
            var LineDefinition = new NonTerminal("LineDefinition");
            var LineDefinitionList = new NonTerminal("LineDefinitionList");
            var LinesIntersectionPolygonDescription = new NonTerminal("PolygonDescription");
            var LinesIntersectionPolygon = new NonTerminal("LinesIntersectionPolygon");
            var SideLengthAndPositionSquareDescription = new NonTerminal("SideLengthAndPositionSquareDescription");
            var SideLengthAndPositionSquareSidePosition = new NonTerminal("SideLengthAndPositionSquareSidePosition");
            var SideLengthAndPositionSquare = new NonTerminal("SideLengthAndPositionSquare");
            var Polygon = new NonTerminal("Polygon");
            var Nth = new NonTerminal("Nth");
            var TopDirection = new NonTerminal("TopDirection");
            var BottomDirection = new NonTerminal("BottomDirection");
            var HorizontalDirection = new NonTerminal("HorizontalDirection");
            var VerticalDirection = new NonTerminal("VerticalDirection");
            var Direction = new NonTerminal("Direction");
            var PointDefinition = new NonTerminal("PointDefinition");
            //var PointList = new NonTerminal("PointList");
            var EquidistantPoint = new NonTerminal("EquidistantPoint");
            var LinesIntersectionPoint = new NonTerminal("LinesIntersectionPoint");
            var TwoPointsLine = new NonTerminal("TwoPointsLine");
            var AxisOfAndEquidistantFromTwoPointsLine = new NonTerminal("AxisOfAndEquidistantFromTwoPointsLine");
            var TwoPointsLineVariants = new NonTerminal("TwoPointsLineVariants");
            var Line = new NonTerminal("Line");
            var ReferencedLine = new NonTerminal("ReferencedLine");
            var Side = new NonTerminal("Side");
            var HalfwayPoint = new NonTerminal("HalfwayPoint");
            var DefinedByTwoPointPoint = new NonTerminal("DefinedByTwoPointPoint");
            var Point = new NonTerminal("Point");
            var Corner = new NonTerminal("Corner");
            var Drawing = new NonTerminal("Drawing");
            var Extremity = new NonTerminal("Extremity");
            var LineExtremityPoint = new NonTerminal("LineExtremityPoint");

            // 3. BNF rules
            TopDirection.Rule = top | upper;
            BottomDirection.Rule = bottom | lower;
            HorizontalDirection.Rule = left | right;
            VerticalDirection.Rule = TopDirection | BottomDirection;
            Direction.Rule = HorizontalDirection | VerticalDirection;

            Side.Rule = Direction + side;

            TwoPointsLine.Rule = from + Point + to + Point
                | Point + and + Point
                | Point + to + Point;

            TwoPointsLineVariants.Rule = TwoPointsLine
                | (aLineDrawn + TwoPointsLine)
                | (anAxisBetween + TwoPointsLine);

            Nth.Rule = ToTerm("first") | "second" | "third" | "fourth";

            ReferencedLine.Rule = Nth + line;

            var ExtremityVerb = new NonTerminal("ExtremityVerb");
            ExtremityVerb.Rule = (ToTerm("starts") | "ends");
            Extremity.Rule = start | end;
            LineExtremityPoint.Rule = 
                Extremity + of + Line
                | ReferencedLine + ExtremityVerb;

            var TwoPointListSeparator = new NonTerminal("TwoPointListSeparator");
            TwoPointListSeparator.Rule = (and | to);
            var TwoPointList = new NonTerminal("TwoPointList");
            TwoPointList.Rule = Point + TwoPointListSeparator + Point;

            var ThreePointList = new NonTerminal("ThreePointList");
            ThreePointList.Rule = Point + comma + Point + and + Point;

            AxisOfAndEquidistantFromTwoPointsLine.Rule = "axis of" + comma + "and equidistant from" + "two" + "points" + comma + TwoPointList;

            Line.Rule = Side
                | ReferencedLine
                | TwoPointsLineVariants;

            HalfwayPoint.Rule = halfwayBetween + Line
                | midpointOf + Line;

            LinesIntersectionPoint.Rule =
                aPoint + whichIsOn + Line + where + Line + wouldCrossThatAxis
                | aPoint + whereALineWouldCross + Line + ifItWereDrawn + Line;

            //PointList.Rule = MakeListRule(PointList, comma | and | to, Point);

            EquidistantPoint.Rule = equidistantFrom + ThreePointList;

            PointDefinition.Rule = Nth + "of which is located at" + Point;

            DefinedByTwoPointPoint.Rule = "point where" + TwoPointList;

            Point.Rule = HalfwayPoint
                | Corner
                | LineExtremityPoint
                | LinesIntersectionPoint
                | EquidistantPoint
                | center
                | PointDefinition
                | DefinedByTwoPointPoint
                //| the + Point
                | aPoint + Point;

            Corner.Rule = VerticalDirection + HorizontalDirection + "corner";

            LineDefinitionVariants.Rule = (line | ofWhichIsDrawn | line + isDrawn);
            LineDefinition.Rule = Nth + LineDefinitionVariants + Line;

            LineDefinitionList.Rule = MakeListRule(LineDefinitionList, comma | dotComma, LineDefinition);

            // Polygon defined by lines intersection

            LinesIntersectionPolygonDescription.Rule = ToTerm("a") + "quadrangle" + "which is formed and enclosed by" + "four" + "lines";

            LinesIntersectionPolygon.Rule = LinesIntersectionPolygonDescription + comma + LineDefinitionList + dot;

            // Square defined by perimeter and Side location

            SideLengthAndPositionSquareDescription.Rule =
                ToTerm("a") + "square" + comma + "each side of which is equal to a" + "tenth" + "of the total length of" + "three" + "lines";

            SideLengthAndPositionSquareSidePosition.Rule = Side + "is located on" + AxisOfAndEquidistantFromTwoPointsLine;

            SideLengthAndPositionSquare.Rule =
                SideLengthAndPositionSquareDescription + comma +
                LineDefinitionList +
                dotComma + SideLengthAndPositionSquareSidePosition +
                dot;


            Polygon.Rule = LinesIntersectionPolygon
                | SideLengthAndPositionSquare;

            Drawing.Rule = MakeStarRule(Drawing, Polygon);

            this.Root = Drawing;

            // 4. Operators precedence

            // 5. Punctuation and transient terms
            MarkPunctuation(from, to, halfwayBetween, midpointOf, of, aPoint, and, equidistantFrom, side,
                line, ofWhichIsDrawn, comma, dot, LineDefinitionVariants, whichIsOn, where, whereALineWouldCross,
                aLineDrawn, anAxisBetween, wouldCrossThatAxis, ifItWereDrawn, dotComma, isDrawn, TwoPointListSeparator);

            MarkTransient(HorizontalDirection, VerticalDirection, Point, Line, Extremity, TwoPointsLineVariants, Direction, Polygon);

            this.LanguageFlags = LanguageFlags.NewLineBeforeEOF;
        }
    }
}
