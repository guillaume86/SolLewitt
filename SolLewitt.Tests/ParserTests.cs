using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SolLewitt.Parser;

namespace SolLewitt.Tests
{
    public class ParserTests
    {
        private string[] Samples = new [] 
        {
            @"A quadrangle which is formed and enclosed by four lines, 
            the first of which is drawn from a point halfway between a point halfway between a point halfway between the center of the wall and the upper left corner and the midpoint of the left side and the upper left corner to a point halfway between the midpoint of the top side and the upper right corner, 
            the second line from a point halfway between the start of the first line and a point halfway between the midpoint of the top side and the upper left corner to a point halfway between a point halfway between the center of the wall and the lower left corner and the midpoint of the bottom side, 
            the third line from a point halfway between a point halfway between the start of the first line and the end of the second line and a point halfway between the midpoint of the left side and the lower left corner to a point which is on an axis between the lower left corner to a point halfway between the midpoint of the right side and the upper right corner where a line drawn from the center of the wall to a point halfway between the midpoint of the right side and the lower right corner would cross that axis, 
            the fourth line from a point equidistant from the end of the third line, the end of the second line and a point halfway between a point halfway between the center of the wall and the midpoint of the bottom side and a point halfway between the midpoint of the bottom side and the lower right corner to a point halfway between the start of the second line and a point where a line would cross the first line if it were drawn from the midpoint of the right side to a point halfway between the midpoint of the top side and the upper left corner."
            ,
            @"A quadrangle which is formed and enclosed by four lines, 
            the first of which is drawn from a point halfway between a point halfway between the center of the wall and a point halfway between the upper left corner and the midpoint of the left side and the upper left corner to a point halfway between the midpoint of the top side and the upper right corner, 
            the second line from a point halfway between the start of the first line and a point halfway between the midpoint of the top side and the upper left corner to a point halfway between a point halfway between the center of the wall and the lower left corner and the midpoint of the bottom side, 
            the third line from a point halfway between a point halfway between the start of the first line and the end of the second line and a point halfway between the midpoint of the left side and the lower left corner to a point which is on an axis between the lower left corner to a point halfway between the midpoint of the right side and the upper right corner where a line drawn from the center of the wall to a point halfway between the midpoint of the right side and the lower right corner would cross that axis, 
            the fourth line from a point equidistant from the end of the third line, the end of the second line and a point halfway between a point halfway between the center of the wall and the midpoint of the bottom side and a point halfway between the midpoint of the bottom side and the lower right corner to a point halfway between the start of the second line and a point where a line would cross the first line if it were drawn from the midpoint of the right side to a point halfway between the midpoint of the top side and the upper left corner."
            ,
            @"A square, each side of which is equal to a tenth of the total length of three lines, 
            the first of which is drawn from a point halfway between a point halfway between the center of the wall and a point halfway between the center of the wall and the upper left corner and the midpoint of the left side to a point halfway between the center of the wall and a point halfway between the center of the wall and the midpoint of the bottom side; 
            the second line is drawn from a point halfway between a point halfway between the start of the first line and a point halfway between the center of the wall and the upper right corner and the midpoint of the top side to the start of the first line;
            the third line is drawn from a point halfway between a point equidistant from the end of the first line, the start of the second line and a point halfway between a point halfway between the center of the wall and the midpoint of the right side and a point halfway between the center of the wall and the upper right corner and the midpoint of the top side to the point where the first line starts and the second line ends;
            the right side is located on the axis of, and equidistant from two points, the first of which is located at a point halfway between a point halfway between the center of the wall and the midpoint of the right side and the lower right corner to a point halfway between a point halfway between the midpoint of the top side and the upper right corner and the start of the third line.",
        };

        [Test, TestCaseSource("Samples")]
        public void ShouldParseSample(string sample)
        {
            var parser = new SolLewittParser();
            var parseTree = parser.Parse(sample);

            parseTree.ParserMessages.ForEach(pm => Console.WriteLine(pm.Message));
            Assert.That(parseTree.HasErrors(), Is.False);
        }

        [Test, TestCaseSource("Samples")]
        public void ShouldReduceExpression(string sample)
        {
            var parser = new SolLewittParser();
            var parseTree = parser.Parse(sample);

            var exp = parser.GetReducedExpression(parseTree);

            Dump(exp);
        }

        [Test, TestCaseSource("Samples")]
        public void ShouldConvertToSvg(string sample)
        {
            var parser = new SolLewittParser();
            var parseTree = parser.Parse(sample);
            var exp = parser.GetReducedExpression(parseTree);

            var svgConverter = new SvgConverter();
            var svg = svgConverter.Convert(exp);

            Dump(svg.ToString());
        }

        public void Dump(object obj)
        {
            if (obj is string)
            {
                Console.WriteLine(obj);
                return;
            }
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}
