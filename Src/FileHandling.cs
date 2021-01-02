using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Edytor_graficzny.Models;
using Microsoft.Win32;

namespace Edytor_graficzny.Src
{
    class FileHandling
    {
        private string fileName = @"D:\temp\test.tex";
        private string tempTemplate = @"D:\Programowanie\Visual Studio C# WPF\Edytor graficzny\Res\Text\StartText.txt";
        public List<GraphicElementModel> gems = new List<GraphicElementModel>();
        public List<GraphicElementModel> gemsDeclarations = new List<GraphicElementModel>();
        public List<ArrowsModel> arrows = new List<ArrowsModel>();
        public double scale = 25;
        public double gridItemsPerCM = 2;

        public FileHandling() { }
        
        public void NewFile()
        {
            try
            {   
                if (File.Exists(fileName))  File.Delete(fileName);

                // WRITE
                using (FileStream fs = File.Create(fileName))
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes("New Text File");
                    fs.Write(title, 0, title.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes("TEst");
                    fs.Write(author, 0, author.Length);
                }

                // READ    
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }

                using (FileStream fs = File.Create(fileName))
                {
                    using (StreamReader sr = File.OpenText(tempTemplate))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            byte[] line = new UTF8Encoding(true).GetBytes(s);
                            fs.Write(line, 0, line.Length);
                            byte[] newLine = new UTF8Encoding(true).GetBytes("\n");
                            fs.Write(newLine, 0, newLine.Length);
                        }
                    }
                }  

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }
        public void OpenFile(List<GraphicElementModel> graphicElementsModel)
        {
            string[] test = File.ReadAllLines(tempTemplate);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                test = File.ReadAllLines(openFileDialog.FileName);
            }
            gems.Clear();
            gemsDeclarations.Clear();
            arrows.Clear();

            string type = "", name;
            double width, height;
            Color color = default;
            Point point = new Point(0, 0);
            Regex regex = new Regex("[^0-9-,]");
            int[] tabPos = new int[] { 0, 0, 0, 0, 0 };
            List<string> nodeNames = new List<string>();

            foreach (var _string in test)
            {
                if (_string.Contains("\\tikzstyle"))
                {
                    _string.Trim();
                    string[] parts = _string.Split("{}=,".ToCharArray());

                    if (_string.Contains("rectangle"))
                    {
                        if (_string.Contains("rounded corners"))
                        {
                            tabPos = new int[] { 6, 8, 16, 17, 18 };
                            type = "Start/Stop";
                        }
                        else
                        {
                            tabPos = new int[] { 5, 7, 15, 16, 17 };
                            type = "Process";
                        }
                    }
                    else if (_string.Contains("trapezium"))
                    {
                        tabPos = new int[] { 9, 11, 19, 20, 21 };
                        type = "Input/Output";
                    }

                    else if (_string.Contains("diamond"))
                    {
                        tabPos = new int[] { 5, 7, 15, 16, 17 };
                        type = "Decision";
                    }


                    name = parts[1];
                    width = Convert.ToDouble(regex.Replace(parts[tabPos[0]].Replace('.', ','), ""));
                    height = Convert.ToDouble(regex.Replace(parts[tabPos[1]].Replace('.', ','), ""));
                    color.A = 255;
                    color.R = Convert.ToByte(regex.Replace(parts[tabPos[2]], ""));
                    color.G = Convert.ToByte(regex.Replace(parts[tabPos[3]], ""));
                    color.B = Convert.ToByte(regex.Replace(parts[tabPos[4]], ""));

                    gemsDeclarations.Add(new GraphicElementModel(gemsDeclarations.Count(), type, name, width * gridItemsPerCM, height * gridItemsPerCM, color));
                }
                else if (_string.Contains("\\node"))
                {
                    foreach (var _gem in gemsDeclarations)
                    {
                        if (_string.Contains(_gem.ElementName))
                        {
                            type = _gem.ElementType;
                            string[] parts = _string.Split("()[=,{}]".ToCharArray());
                            nodeNames.Add(parts[1]);

                            point.X = ((Convert.ToDouble(regex.Replace(parts[5].Replace('.', ','), "")) * gridItemsPerCM) - _gem.ElementWidth / 2);
                            point.Y = ((Convert.ToDouble(regex.Replace(parts[7].Replace('.', ','), "")) * -gridItemsPerCM) - _gem.ElementHeight / 2);
                            name = parts[9];

                            gems.Add(new GraphicElementModel(gems.Count(), type, name, point, _gem.ElementWidth, _gem.ElementHeight, _gem.ElementColor, 1));
                        }
                    }
                }
                else if (_string.Contains("\\draw"))
                {
                    int iteratrionNumber = 0;
                    int gemNumb = 0;
                    bool firstPointInElement = false;

                    List<Point> arrowPoints = new List<Point>();
                    List<Point> correctArrowPoints = new List<Point>();
                    Point pointFirst;
                    Point pointNext;

                    string[] parts = _string.Split("(),".ToCharArray());
                    for (int i = 1; i < parts.Count(); i++)     //  (parts[1] = Element0)  OR  (parts[1] = 4cm, parts[2] = -3.5cm)
                    {
                        bool pointConnected = false;
                        for (int j = 0; j < nodeNames.Count(); j++) //      nodeNames[0] = Element0, nodeNames[1] = Element1
                        { 
                            if (parts[i] == nodeNames[j])
                            {
                                arrowPoints.Add(new Point(gems[j].ElementStartingLocation.X + gems[j].ElementWidth / 2, gems[j].ElementStartingLocation.Y + gems[j].ElementHeight / 2));
                                if (!firstPointInElement) gemNumb = j;
                                if (iteratrionNumber == 0) firstPointInElement = true;

                                if (iteratrionNumber == 1)
                                {
                                    if(firstPointInElement)
                                    {
                                        Point absCoordinates = new Point(arrowPoints.Last().X - arrowPoints[arrowPoints.Count() - 2].X, arrowPoints.Last().Y - arrowPoints[arrowPoints.Count() - 2].Y);
                                        double angle = Math.Atan2(absCoordinates.Y, absCoordinates.X) * 180.0 / Math.PI;

                                        if (angle >= -45 && angle < 45) pointFirst = new Point(gems[gemNumb].ElementStartingLocation.X + gems[gemNumb].ElementWidth, gems[gemNumb].ElementStartingLocation.Y + gems[gemNumb].ElementHeight / 2);
                                        else if (angle >= 45 && angle < 135) pointFirst = new Point(gems[gemNumb].ElementStartingLocation.X + gems[gemNumb].ElementWidth / 2, gems[gemNumb].ElementStartingLocation.Y + gems[gemNumb].ElementHeight);
                                        else if (angle >= 135 || angle < -135) pointFirst = new Point(gems[gemNumb].ElementStartingLocation.X, gems[gemNumb].ElementStartingLocation.Y + gems[gemNumb].ElementHeight / 2);
                                        else if (angle >= -135 && angle < -45) pointFirst = new Point(gems[gemNumb].ElementStartingLocation.X + gems[gemNumb].ElementWidth / 2, gems[gemNumb].ElementStartingLocation.Y);
                                    }
                                    else 
                                    {
                                        pointFirst.X = Convert.ToDouble(regex.Replace(parts[i-3].Replace('.', ','), "")) * gridItemsPerCM;
                                        pointFirst.Y = Convert.ToDouble(regex.Replace(parts[i -2].Replace('.', ','), "")) * -gridItemsPerCM;
                                        
                                    }
                                    correctArrowPoints.Add(pointFirst);
                                }



                                if (iteratrionNumber > 0)
                                {
                                    Point absCoordinates = new Point(arrowPoints.Last().X - arrowPoints[arrowPoints.Count() - 2].X, arrowPoints.Last().Y - arrowPoints[arrowPoints.Count() - 2].Y);
                                    double angle = Math.Atan2(absCoordinates.Y, absCoordinates.X) * 180.0 / Math.PI;

                                    if (angle >= -45 && angle < 45) pointNext = new Point(gems[j].ElementStartingLocation.X, gems[j].ElementStartingLocation.Y + gems[j].ElementHeight / 2);
                                    else if (angle >= 45 && angle < 135) pointNext = new Point(gems[j].ElementStartingLocation.X + gems[j].ElementWidth / 2, gems[j].ElementStartingLocation.Y);
                                    else if (angle >= 135 || angle < -135) pointNext = new Point(gems[j].ElementStartingLocation.X + gems[j].ElementWidth, gems[j].ElementStartingLocation.Y + gems[j].ElementHeight / 2);
                                    else if (angle >= -135 && angle < -45) pointNext = new Point(gems[j].ElementStartingLocation.X + gems[j].ElementWidth / 2, gems[j].ElementStartingLocation.Y + gems[j].ElementHeight);
                                    
                                    correctArrowPoints.Add(pointNext);
                                }

                                pointConnected = true;
                                i++;
                                iteratrionNumber++;
                                break;
                            }
                        }
                        if (!pointConnected)
                        {
                            pointNext.X = Convert.ToDouble(regex.Replace(parts[i].Replace('.', ','), "")) * gridItemsPerCM;
                            pointNext.Y = Convert.ToDouble(regex.Replace(parts[i+1].Replace('.', ','), "")) * -gridItemsPerCM;
                            arrowPoints.Add(pointNext);

                            if (iteratrionNumber == 1)
                            {
                                if (firstPointInElement)
                                {
                                    Point absCoordinates = new Point(arrowPoints.Last().X - arrowPoints[arrowPoints.Count() - 2].X, arrowPoints.Last().Y - arrowPoints[arrowPoints.Count() - 2].Y);
                                    double angle = Math.Atan2(absCoordinates.Y, absCoordinates.X) * 180.0 / Math.PI;

                                    if (angle >= -45 && angle < 45) pointFirst = new Point(gems[gemNumb].ElementStartingLocation.X + gems[gemNumb].ElementWidth, gems[gemNumb].ElementStartingLocation.Y + gems[gemNumb].ElementHeight / 2);
                                    else if (angle >= 45 && angle < 135) pointFirst = new Point(gems[gemNumb].ElementStartingLocation.X + gems[gemNumb].ElementWidth / 2, gems[gemNumb].ElementStartingLocation.Y + gems[gemNumb].ElementHeight);
                                    else if (angle >= 135 || angle < -135) pointFirst = new Point(gems[gemNumb].ElementStartingLocation.X, gems[gemNumb].ElementStartingLocation.Y + gems[gemNumb].ElementHeight / 2);
                                    else if (angle >= -135 && angle < -45) pointFirst = new Point(gems[gemNumb].ElementStartingLocation.X + gems[gemNumb].ElementWidth / 2, gems[gemNumb].ElementStartingLocation.Y);
                                    correctArrowPoints.Add(pointFirst);
                                }
                                else
                                {
                                    pointFirst.X = Convert.ToDouble(regex.Replace(parts[i].Replace('.', ','), "")) * gridItemsPerCM;
                                    pointFirst.Y = Convert.ToDouble(regex.Replace(parts[i + 1].Replace('.', ','), "")) * -gridItemsPerCM;

                                }
                                
                            }
                            correctArrowPoints.Add(pointNext);
                            i += 2;
                            iteratrionNumber++;
                        }
                    }

                    arrows.Add(new ArrowsModel(correctArrowPoints, "Arrow"));
                }
            }
        }

        public void SaveFile()
        {
            List<string> fullTextList = new List<string>();
            string[] startText = new string[] { "\\documentclass{article}", "\\usepackage{tikz}", "\\usetikzlibrary{shapes.geometric, arrows}", "" };
            string[] middleText = new string[] { "", "\\begin{document}", "\\begin{tikzpicture}", "" };
            string[] endText = new string[] { "", "\\end{tikzpicture}", "\\end{document}" };

            fullTextList.AddRange(startText);

            #region \\tikzstyle{startstop} = [rectangle, rounded corners, minimum width = 3cm, minimum height = 1cm, text centered, draw = black, fill = red!30]");
            
            List<string> partialDeclarations = new List<string>();
            List<string> declarations = new List<string>();
            List<int> objectToDeclarationDependency = new List<int>();
            foreach (GraphicElementModel gem in gems)
            {
                string tempDeclarationEnd = "", tempDeclaration = "";

                string widthConverted = Convert.ToString(gem.ElementWidth / gridItemsPerCM);
                widthConverted = widthConverted.Replace(',', '.');
                string heightConverted = Convert.ToString(gem.ElementHeight / gridItemsPerCM);
                heightConverted = heightConverted.Replace(',', '.');
                switch (gem.ElementType)
                {
                    case "Start/Stop":
                        tempDeclarationEnd = "} = [rectangle, rounded corners, minimum width = ";
                        break;
                    case "Input/Output":
                        tempDeclarationEnd = "} = [trapezium, trapezium left angle = 70, trapezium right angle = 110, minimum width = ";
                        break;
                    case "Process":
                        tempDeclarationEnd = "} = [rectangle, minimum width = ";
                        break;
                    case "Decision":
                        tempDeclarationEnd = "} = [diamond, minimum width = ";
                        break;
                }
                tempDeclarationEnd += widthConverted + "cm, minimum height = "
                            + heightConverted + "cm, text centered, draw = black, fill = {rgb,255:red,"
                            + Convert.ToString(gem.ElementColor.R) + "; green,"
                            + Convert.ToString(gem.ElementColor.G) + "; blue,"
                            + Convert.ToString(gem.ElementColor.B) + "}]";

                if (!partialDeclarations.Contains(tempDeclarationEnd))
                {
                    switch (gem.ElementType)
                    {
                        case "Start/Stop":
                            tempDeclaration = "\\tikzstyle{Start/Stop";
                            break;
                        case "Input/Output":
                            tempDeclaration = "\\tikzstyle{Input/Output";
                            break;
                        case "Process":
                            tempDeclaration = "\\tikzstyle{Process";
                            break;
                        case "Decision":
                            tempDeclaration = "\\tikzstyle{Decision";
                            break;
                    }

                    tempDeclaration += partialDeclarations.Count() + tempDeclarationEnd;
                    partialDeclarations.Add(tempDeclarationEnd);
                    declarations.Add(tempDeclaration);
                    objectToDeclarationDependency.Add(partialDeclarations.Count() - 1); 
                }
                else
                {
                    for (int i = 0; i < partialDeclarations.Count(); i++)
                    {
                        if (partialDeclarations[i] == tempDeclarationEnd)
                        {
                            objectToDeclarationDependency.Add(i);
                        }
                    }
                }
            }
            fullTextList.AddRange(declarations);
            #endregion

            fullTextList.AddRange(middleText);

            #region \node(start0)[startStop0, xshift = 1cm, yshift = -1cm] { Start};
            for (int i = 0; i < gems.Count(); i++)
            {
                string locationX = Convert.ToString((gems[i].ElementStartingLocation.X + (gems[i].ElementWidth / 2)) / gridItemsPerCM);
                locationX = locationX.Replace(',', '.');
                string locationY = Convert.ToString((gems[i].ElementStartingLocation.Y + (gems[i].ElementHeight / 2)) / gridItemsPerCM * -1);
                locationY = locationY.Replace(',', '.');

                fullTextList.Add("\\node (Element"
                    + Convert.ToString(gems[i].ElementId) + ") ["
                    + gems[i].ElementType + Convert.ToString(objectToDeclarationDependency[i]) + ", xshift=" 
                    + locationX + "cm, yshift=" 
                    + locationY + "cm] {" 
                    + gems[i].ElementName + "};");
            }
            #endregion

            fullTextList.Add("");

            #region \draw [->] (2cm,-1.5cm) -- (2cm,-2.5cm);
            foreach (ArrowsModel _arrow in arrows)
            {
                string textList = "\\draw [->] (";
                foreach (Point _point in _arrow.points)
                {
                    bool pointConnected = false;
                    string elementConnected = "";
                    foreach (GraphicElementModel gem in gems)
                    {
                        if (_point.X >= gem.ElementStartingLocation.X && _point.X <= gem.ElementStartingLocation.X + gem.ElementWidth && _point.Y >= gem.ElementStartingLocation.Y && _point.Y <= gem.ElementStartingLocation.Y + gem.ElementHeight)
                        {
                            pointConnected = true;
                            elementConnected = "Element" + gem.ElementId;
                        }
                    }

                    if (pointConnected) textList += elementConnected + ")";
                    else
                    {
                        string locationX = Convert.ToString(_point.X / gridItemsPerCM).Replace(',', '.');
                        string locationY = Convert.ToString(_point.Y / gridItemsPerCM * -1).Replace(',', '.');

                        textList += locationX + "cm, "
                                  + locationY + "cm)";
                    }

                    if (_point != _arrow.points.Last())
                    {
                        textList += " -- (";
                    }
                }
                fullTextList.Add(textList + ";");
            }
            #endregion

            fullTextList.AddRange(endText);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveFileDialog.FileName, fullTextList);
            }
        }
    }
}
