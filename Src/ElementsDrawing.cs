using Edytor_graficzny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Edytor_graficzny.Src
{
    static class ElementsDrawing
    {
        static public Path UpdateGEMS(GraphicElementModel gem)
        {
            Path path = new Path();
            path.Stroke = Brushes.Black;
            path.Fill = new SolidColorBrush(gem.ElementColor);
            path.StrokeThickness = 2;

            switch (gem.ElementType)
            {
                case "StartStop":
                    Rect myRect = new Rect(gem.ElementStartingLocation.X * Variables.scale, gem.ElementStartingLocation.Y * Variables.scale, gem.ElementWidth * Variables.scale, gem.ElementHeight * Variables.scale);

                    path.Data = new RectangleGeometry(myRect, Variables.cornerRoundness * Variables.scale * gem.ElementHeight, Variables.cornerRoundness * Variables.scale * gem.ElementHeight);
                    break;

                case "InputOutput":
                    PathFigure myPathFigure = new PathFigure();
                    myPathFigure.StartPoint = new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 7)) * Variables.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * Variables.scale);
                    myPathFigure.Segments.Add(
                        new LineSegment(
                            new Point(Convert.ToInt32(gem.ElementStartingLocation.X + gem.ElementWidth) * Variables.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * Variables.scale),
                            true /* IsStroked */ ));
                    myPathFigure.Segments.Add(
                        new LineSegment(
                            new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 7 * 6)) * Variables.scale, Convert.ToInt32(gem.ElementStartingLocation.Y + gem.ElementHeight) * Variables.scale),
                            true /* IsStroked */ ));
                    myPathFigure.Segments.Add(
                        new LineSegment(
                            new Point(Convert.ToInt32(gem.ElementStartingLocation.X) * Variables.scale, Convert.ToInt32(gem.ElementStartingLocation.Y + gem.ElementHeight) * Variables.scale),
                            true /* IsStroked */ ));
                    myPathFigure.Segments.Add(
                        new LineSegment(
                            new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 7)) * Variables.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * Variables.scale),
                            true /* IsStroked */ ));

                    PathGeometry myPathGeometry = new PathGeometry();
                    myPathGeometry.Figures.Add(myPathFigure);

                    path.Data = myPathGeometry;
                    break;

                case "Process":
                    Rect myRect2 = new Rect(gem.ElementStartingLocation.X * Variables.scale, gem.ElementStartingLocation.Y * Variables.scale, gem.ElementWidth * Variables.scale, gem.ElementHeight * Variables.scale);

                    path.Data = new RectangleGeometry(myRect2);
                    break;

                case "Decision":
                    PathFigure myPathFigure2 = new PathFigure();
                    myPathFigure2.StartPoint = new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 2)) * Variables.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * Variables.scale);
                    myPathFigure2.Segments.Add(
                        new LineSegment(
                            new Point(Convert.ToInt32(gem.ElementStartingLocation.X + gem.ElementWidth) * Variables.scale, Convert.ToInt32((gem.ElementStartingLocation.Y * 2) + gem.ElementHeight) * Variables.scale / 2),
                            true /* IsStroked */ ));
                    myPathFigure2.Segments.Add(
                        new LineSegment(
                            new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 2)) * Variables.scale, Convert.ToInt32(gem.ElementStartingLocation.Y + gem.ElementHeight) * Variables.scale),
                            true /* IsStroked */ ));
                    myPathFigure2.Segments.Add(
                        new LineSegment(
                            new Point(Convert.ToInt32(gem.ElementStartingLocation.X) * Variables.scale, Convert.ToInt32((gem.ElementStartingLocation.Y * 2) + gem.ElementHeight) * Variables.scale / 2),
                            true /* IsStroked */ ));
                    myPathFigure2.Segments.Add(
                        new LineSegment(
                            new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 2)) * Variables.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * Variables.scale),
                            true /* IsStroked */ ));

                    PathGeometry myPathGeometry2 = new PathGeometry();
                    myPathGeometry2.Figures.Add(myPathFigure2);

                    path.Data = myPathGeometry2;
                    break;
            }
            return path;
        }

        static public TextBlock UpdateTXT(GraphicElementModel gem)
        {
            TextBlock textBlock = new TextBlock();

            textBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
            textBlock.TextAlignment = TextAlignment.Justify;
            textBlock.Text = gem.ElementName;
            int marginX, marginY;
            switch (gem.ElementType)
            {
                case "StartStop":
                    marginX = 5;
                    marginY = 1;
                    break;
                case "InputOutput":
                    marginX = Convert.ToInt32(gem.ElementWidth * Variables.scale / 6);
                    marginY = 1;
                    break;
                case "Process":
                    marginX = 2;
                    marginY = 1;
                    break;
                case "Decision":
                    marginX = Convert.ToInt32(gem.ElementWidth * Variables.scale / 4);
                    marginY = Convert.ToInt32(gem.ElementHeight * Variables.scale / 4);
                    break;
                default:
                    marginX = 5;
                    marginY = 1;
                    break;
            }
            if (Variables.isDynamicFontActive)
            {
                try
                {
                    double maxHeight = gem.ElementHeight * Variables.scale - (marginY * 2);
                    Size maxSize = new Size(gem.ElementWidth * Variables.scale - (marginX * 2), maxHeight * 2);


                    for (int i = 50; i > 8; i--)
                    {
                        textBlock.FontSize = i;
                        textBlock.Measure(maxSize);
                        if (textBlock.DesiredSize.Height < maxHeight) break;
                    }
                    textBlock.Width = gem.ElementWidth * Variables.scale - (marginX * 2);
                    textBlock.Height = gem.ElementHeight * Variables.scale - (marginY * 2);
                }
                catch
                {
                    textBlock.FontSize = 6;
                }
            }
            else textBlock.FontSize = Variables.fontSize;

            Canvas.SetLeft(textBlock, gem.ElementStartingLocation.X * Variables.scale + marginX);
            Canvas.SetTop(textBlock, gem.ElementStartingLocation.Y * Variables.scale + marginY);

            return textBlock;
        }
        static public Path UpdateArrow(ArrowsModel arrowsModel)
        {
            Path path = new Path();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 2;

            PathFigure myPathFigureLine = new PathFigure();
            myPathFigureLine.StartPoint = new Point(arrowsModel.points[0].X * Variables.scale, arrowsModel.points[0].Y * Variables.scale);

            foreach (var _point in arrowsModel.points)
            {
                if (_point != arrowsModel.points[0])
                {
                    myPathFigureLine.Segments.Add(new LineSegment(new Point(_point.X * Variables.scale, _point.Y * Variables.scale), true));
                }
            }
            PathGeometry myPathGeometryLine = new PathGeometry();
            myPathGeometryLine.Figures.Add(myPathFigureLine);
            if (arrowsModel.arrowType == "DashedLine") path.StrokeDashArray = new DoubleCollection() { 2, 2 };
            path.Data = myPathGeometryLine;

            return path;
        }

        static public Path UpdateArrowHead(ArrowsModel arrowsModel)
        {
            Path path = new Path();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 2;

            PathFigure myPathFigureLineTip = new PathFigure();
            myPathFigureLineTip.StartPoint = new Point(arrowsModel.points.Last().X * Variables.scale, arrowsModel.points.Last().Y * Variables.scale);

            Point absCoordinates = new Point(arrowsModel.points.Last().X - arrowsModel.points[arrowsModel.points.Count() - 2].X, arrowsModel.points.Last().Y - arrowsModel.points[arrowsModel.points.Count() - 2].Y);
            double angle = Math.Atan2(absCoordinates.Y, absCoordinates.X) * 180.0 / Math.PI;
            Point arrowHeadLeft = new Point((arrowsModel.points.Last().X + (Math.Cos((angle + 135) * Math.PI / 180) / 8)) * Variables.scale, (arrowsModel.points.Last().Y + (Math.Sin((angle + 135) * Math.PI / 180) / 8)) * Variables.scale);
            Point arrowHeadRight = new Point((arrowsModel.points.Last().X + (Math.Cos((angle + 225) * Math.PI / 180) / 8)) * Variables.scale, (arrowsModel.points.Last().Y + (Math.Sin((angle + 225) * Math.PI / 180) / 8)) * Variables.scale);

            myPathFigureLineTip.Segments.Add(new LineSegment(arrowHeadLeft, true));
            myPathFigureLineTip.Segments.Add(new LineSegment(new Point(arrowsModel.points.Last().X * Variables.scale, arrowsModel.points.Last().Y * Variables.scale), true));
            myPathFigureLineTip.Segments.Add(new LineSegment(arrowHeadRight, true));
            myPathFigureLineTip.Segments.Add(new LineSegment(new Point(arrowsModel.points.Last().X * Variables.scale, arrowsModel.points.Last().Y * Variables.scale), true));

            PathGeometry myPathGeometryLineTip = new PathGeometry();
            myPathGeometryLineTip.Figures.Add(myPathFigureLineTip);
            path.Data = myPathGeometryLineTip;

            return path;
        }

        static public Path UpdateNewArrow(List<Point> arrowPoints, bool dashed)
        {
            Path path = new Path();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 2;

            PathFigure myPathFigureLine = new PathFigure();
            myPathFigureLine.StartPoint = new Point(arrowPoints[0].X * Variables.scale, arrowPoints[0].Y * Variables.scale);

            foreach (var _point in arrowPoints)
            {
                if (_point != arrowPoints[0])
                {
                    myPathFigureLine.Segments.Add(new LineSegment(new Point(_point.X * Variables.scale, _point.Y * Variables.scale), true));
                }
            }
            PathGeometry myPathGeometryLine = new PathGeometry();
            myPathGeometryLine.Figures.Add(myPathFigureLine);

            if (dashed) path.StrokeDashArray = new DoubleCollection() { 2, 2 };
            path.Data = myPathGeometryLine;

            return path;
        }

        static public Rectangle DrawingHelpers(Point pt, GraphicElementModel rG)
        {
            Rectangle hitChecker = new Rectangle();
            hitChecker.Width = rG.ElementWidth * Variables.scale;
            hitChecker.Height = rG.ElementHeight * Variables.scale;
            hitChecker.Fill = Brushes.Black;
            hitChecker.Opacity = 0.2;
            Canvas.SetLeft(hitChecker, rG.ElementStartingLocation.X * Variables.scale);
            Canvas.SetTop(hitChecker, rG.ElementStartingLocation.Y * Variables.scale);
            return hitChecker;
        }

        static public List<Rectangle> SelectedElement(Point point, double width, double height)
        {
            List<Rectangle> smallRectangles = new List<Rectangle>();
            for (int i = 0; i < 9; i++)
            {
                if (i != 4)
                {
                    smallRectangles.Add(new Rectangle());
                    smallRectangles.Last().Width = 6;
                    smallRectangles.Last().Height = 6;
                    smallRectangles.Last().Stroke = Brushes.Black;
                    smallRectangles.Last().StrokeThickness = 1;
                    smallRectangles.Last().Fill = Brushes.White;
                    Canvas.SetLeft(smallRectangles.Last(), (point.X + width / 2 * (i % 3)) * Variables.scale - 3);
                    Canvas.SetTop(smallRectangles.Last(), (point.Y + height / 2 * Convert.ToInt32(i / 3)) * Variables.scale - 3);
                }
            }
            return smallRectangles;
        }
    }
}
