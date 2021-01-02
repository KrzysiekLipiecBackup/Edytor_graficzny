using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Edytor_graficzny.Models
{
    class ArrowsModel
    {
        public List<Point> points { get; set; }
        public string arrowType { get; set; }


        public ArrowsModel(List<Point> p, string at)
        {
            points = p;
            arrowType = at;
        }
    }

}
