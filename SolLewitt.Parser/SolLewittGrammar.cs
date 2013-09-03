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


            // 2. Non-terminals
            var LineDefinitionVariants = new NonTerminal("LineDefinitionVariants");
            var LineDefinition = new NonTerminal("LineDefinition");
            var LineDefinitionList = new NonTerminal("LineDefinitionList");
            var PolygonDescription = new NonTerminal("PolygonDescription");
            var Polygon = new NonTerminal("Polygon");
            var Nth = new NonTerminal("Nth");
            var TopDirection = new NonTerminal("TopDirection");
            var BottomDirection = new NonTerminal("BottomDirection");
            var HorizontalDirection = new NonTerminal("HorizontalDirection");
            var VerticalDirection = new NonTerminal("VerticalDirection");
            var Direction = new NonTerminal("Direction");
            var PointList = new NonTerminal("PointList");
            var EquidistantPoint = new NonTerminal("EquidistantPoint");
            var LinesIntersectionPoint = new NonTerminal("LinesIntersectionPoint");
            var TwoPointsLine = new NonTerminal("TwoPointsLine");
            var TwoPointsLineVariants = new NonTerminal("TwoPointsLineVariants");
            var Line = new NonTerminal("Line");
            var ReferencedLine = new NonTerminal("ReferencedLine");
            var Side = new NonTerminal("Side");
            var HalfwayPoint = new NonTerminal("HalfwayPoint");
            var Point = new NonTerminal("Point");
            var Corner = new NonTerminal("Corner");
            var Drawing = new NonTerminal("Drawing");
            var Extremity = new NonTerminal("Extremity");
            var LineExtremityPoint = new NonTerminal("LineExtremityPoint");

            // 3. BNF rules
            TopDirection.Rule = (top | upper);
            BottomDirection.Rule = (bottom | lower);
            HorizontalDirection.Rule = (left | right);
            VerticalDirection.Rule = (TopDirection | BottomDirection);
            Direction.Rule = HorizontalDirection | VerticalDirection;

            Side.Rule = Direction + side;

            TwoPointsLine.Rule = (from + Point + to + Point)
                | (Point + and + Point)
                | (Point + to + Point);

            TwoPointsLineVariants.Rule = TwoPointsLine
                | (aLineDrawn + TwoPointsLine) 
                | (anAxisBetween + TwoPointsLine);

            Nth.Rule = ToTerm("first") | "second" | "third" | "fourth";

            ReferencedLine.Rule = Nth + line;

            Extremity.Rule = (start | end);
            LineExtremityPoint.Rule = Extremity + of + Line;

            Line.Rule = Side 
                | ReferencedLine
                | TwoPointsLineVariants
                | the + Line;

            HalfwayPoint.Rule =  halfwayBetween + Line
                | midpointOf + Line;

            LinesIntersectionPoint.Rule = 
                aPoint + whichIsOn + Line + where + Line + wouldCrossThatAxis
                | aPoint + whereALineWouldCross + Line + ifItWereDrawn + Line;

            PointList.Rule = MakeListRule(PointList, comma | and, Point);

            EquidistantPoint.Rule = equidistantFrom + PointList;

            Point.Rule = HalfwayPoint 
                | Corner 
                | LineExtremityPoint
                | LinesIntersectionPoint
                | EquidistantPoint
                | center
                | the + Point
                | aPoint + Point;

            Corner.Rule = VerticalDirection + HorizontalDirection + "corner";

            PolygonDescription.Rule = ToTerm("a") + "quadrangle" + "which is formed and enclosed by" + "four" + "lines";

            LineDefinitionVariants.Rule = (line | ofWhichIsDrawn);
            LineDefinition.Rule = the + Nth + LineDefinitionVariants + Line;

            LineDefinitionList.Rule = MakeListRule(LineDefinitionList, comma, LineDefinition);

            Polygon.Rule = PolygonDescription + comma + LineDefinitionList + dot;

            Drawing.Rule = MakeStarRule(Drawing, Polygon);

            this.Root = Drawing;

            // 4. Operators precedence

            // 5. Punctuation and transient terms
            MarkPunctuation(from, to, halfwayBetween, midpointOf, the, of, aPoint, and, equidistantFrom, side, 
                line, ofWhichIsDrawn, comma, dot, LineDefinitionVariants, whichIsOn, where, whereALineWouldCross,
                aLineDrawn, anAxisBetween, wouldCrossThatAxis, ifItWereDrawn);

            MarkTransient(HorizontalDirection, VerticalDirection, Point, Line, Extremity, TwoPointsLineVariants, Direction);

            this.LanguageFlags = LanguageFlags.NewLineBeforeEOF; 
        }
    }
}
