using System;
using System.Collections.Generic;
using System.Text;

namespace Edytor_graficzny.Models
{
    class GraphicElementList
    {
        //public List<GraphicElementModel> gems = new List<GraphicElementModel>();
        public List<GraphicElementModel> gems { get; set; }

        public GraphicElementList()
        {
            gems = new List<GraphicElementModel>();
        }

        public void RequestGraphicElementList(List<GraphicElementModel> newGEMS)
        {
            gems = newGEMS;
        }

        public List<GraphicElementModel> SendGraphicElementList()
        {
            return gems;
        }
    }

}
