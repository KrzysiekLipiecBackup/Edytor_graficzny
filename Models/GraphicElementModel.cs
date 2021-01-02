using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Edytor_graficzny.Models
{
    class GraphicElementModel   //0;   ellipse;    Start;    0.0;    3;    1;    255,120,120;    1
    {
        public int ElementId { get; set; }
        public string ElementType { get; set; }
        public string ElementName { get; set; }
        public Point ElementStartingLocation { get; set; }
        public double ElementWidth { get; set; }
        public double ElementHeight { get; set; }
        public Color ElementColor { get; set; }
        public double ElementStroke { get; set; }


        public GraphicElementModel(int id = 0, string type = "", string name = "", Point location = default, double width = 0, double height = 0, Color color = default, double stroke = 1)
        {
            ElementId = id;
            ElementType = type;
            ElementName = name;
            ElementStartingLocation = location;
            ElementWidth = width;
            ElementHeight = height;
            ElementColor = color;
            ElementStroke = stroke;
        }

        public GraphicElementModel(int id = 0, string type = "", string name = "", double width = 0, double height = 0, Color color = default)
        {
            ElementId = id;
            ElementType = type;
            ElementName = name;
            ElementStartingLocation = new Point(0,0);
            ElementWidth = width;
            ElementHeight = height;
            ElementColor = color;
            ElementStroke = 1;
        }
    }
}
