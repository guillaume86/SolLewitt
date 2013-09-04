SolLewitt
=========

A parser for Sol Lewitt style procedural drawing instructions like explained on http://cmuems.com/2013/a/golan/09/02/melanie-nailed-it/

Next step would be to reverse the process and generate instructions from images. Using something like http://rogeralsing.com/2008/12/07/genetic-programming-evolution-of-mona-lisa/
to turn image into polygons. A very inneficient plain english image encoding algorythm :).

## Tested Samples:

You can preview the resulting SVG online on http://scriptdraw.com/

### Sample 1

    A quadrangle which is formed and enclosed by four lines, 
    the first of which is drawn from a point halfway between a point halfway between a point halfway between the center of the wall and the upper left corner and the midpoint of the left side and the upper left corner to a point halfway between the midpoint of the top side and the upper right corner, 
    the second line from a point halfway between the start of the first line and a point halfway between the midpoint of the top side and the upper left corner to a point halfway between a point halfway between the center of the wall and the lower left corner and the midpoint of the bottom side, 
    the third line from a point halfway between a point halfway between the start of the first line and the end of the second line and a point halfway between the midpoint of the left side and the lower left corner to a point which is on an axis between the lower left corner to a point halfway between the midpoint of the right side and the upper right corner where a line drawn from the center of the wall to a point halfway between the midpoint of the right side and the lower right corner would cross that axis, 
    the fourth line from a point equidistant from the end of the third line, the end of the second line and a point halfway between a point halfway between the center of the wall and the midpoint of the bottom side and a point halfway between the midpoint of the bottom side and the lower right corner to a point halfway between the start of the second line and a point where a line would cross the first line if it were drawn from the midpoint of the right side to a point halfway between the midpoint of the top side and the upper left corner.

returns

    <svg viewBox="0 0 1 1">
      <rect x="0" y="0" width="1" height="1" fill="none" stroke="blue" stroke-width="0.001" />
      <polygon fill="green" points="0.245126353790614,0.0841456077015644 0.132352941176471,0.102941176470588 0.217225609756098,0.569740853658537 0.422045329532953,0.537958483348335" />
    </svg>

### Sample 2:

    A quadrangle which is formed and enclosed by four lines, 
    the first of which is drawn from a point halfway between a point halfway between the center of the wall and a point halfway between the upper left corner and the midpoint of the left side and the upper left corner to a point halfway between the midpoint of the top side and the upper right corner, 
    the second line from a point halfway between the start of the first line and a point halfway between the midpoint of the top side and the upper left corner to a point halfway between a point halfway between the center of the wall and the lower left corner and the midpoint of the bottom side, 
    the third line from a point halfway between a point halfway between the start of the first line and the end of the second line and a point halfway between the midpoint of the left side and the lower left corner to a point which is on an axis between the lower left corner to a point halfway between the midpoint of the right side and the upper right corner where a line drawn from the center of the wall to a point halfway between the midpoint of the right side and the lower right corner would cross that axis, 
    the fourth line from a point equidistant from the end of the third line, the end of the second line and a point halfway between a point halfway between the center of the wall and the midpoint of the bottom side and a point halfway between the midpoint of the bottom side and the lower right corner to a point halfway between the start of the second line and a point where a line would cross the first line if it were drawn from the midpoint of the right side to a point halfway between the midpoint of the top side and the upper left corner.

returns 

    <svg viewBox="0 0 1 1">
      <rect x="0" y="0" width="1" height="1" fill="none" stroke="blue" stroke-width="0.001" />
      <polygon fill="green" points="0.21937069813176,0.04421910848902 0.128571428571429,0.0517857142857143 0.216642228739003,0.558192815249267 0.417601695874881,0.532206677257558" />
    </svg>

### Sample 3:

    A square, each side of which is equal to a tenth of the total length of three lines, 
    the first of which is drawn from a point halfway between a point halfway between the center of the wall and a point halfway between the center of the wall and the upper left corner and the midpoint of the left side to a point halfway between the center of the wall and a point halfway between the center of the wall and the midpoint of the bottom side; 
    the second line is drawn from a point halfway between a point halfway between the start of the first line and a point halfway between the center of the wall and the upper right corner and the midpoint of the top side to the start of the first line;
    the third line is drawn from a point halfway between a point equidistant from the end of the first line, the start of the second line and a point halfway between a point halfway between the center of the wall and the midpoint of the right side and a point halfway between the center of the wall and the upper right corner and the midpoint of the top side to the point where the first line starts and the second line ends;
    the right side is located on the axis of, and equidistant from two points, the first of which is located at a point halfway between a point halfway between the center of the wall and the midpoint of the right side and the lower right corner to a point halfway between a point halfway between the midpoint of the top side and the upper right corner and the start of the third line.

returns 

    <svg viewBox="0 0 1 1">
      <rect x="0" y="0" width="1" height="1" fill="none" stroke="blue" stroke-width="0.001" />
      <polygon fill="green" points="0.680249335053517,0.378692964407731 0.653083998279816,0.282765368925602 0.557156402797687,0.309930705699302 0.584321739571388,0.405858301181431" />
    </svg>

