using System;
using System.Collections.Generic;
using System.Text;

namespace Edytor_graficzny.Models
{
    class GraphicElementModel
    {
        public int id;
        public double startX = 0;
        public double startY = 0;
        public double width = 0;
        public double height = 0;
        public string name;
        //private string color;
        //private string stroke;
        
        public GraphicElementModel(int id) 
        {
            this.id = id;
        }
    }
}
