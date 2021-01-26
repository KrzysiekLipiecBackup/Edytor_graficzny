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
        private string tempTemplate = @"D:\Programowanie\Visual Studio C# WPF\Edytor graficzny\Res\Text\StartText.txt";
        public List<GraphicElementModel> gems = new List<GraphicElementModel>();
        public List<GraphicElementModel> gemsDeclarations = new List<GraphicElementModel>();
        public List<ArrowsModel> arrows = new List<ArrowsModel>();

        public Color[] toolColor = new Color[4];
        public Color[] customButtonsColors = new Color[12];

        public FileHandling()
        {
            for(int i = 0; i < 12; i++)
            {
                customButtonsColors[i].A = 0;
                customButtonsColors[i].R = 195;
                customButtonsColors[i].G = 195;
                customButtonsColors[i].B = 195;
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
                            type = "StartStop";
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
                        type = "InputOutput";
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

                    gemsDeclarations.Add(new GraphicElementModel(gemsDeclarations.Count(), type, name, width * Variables.gridItemsPerCM, height * Variables.gridItemsPerCM, color));
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

                            point.X = ((Convert.ToDouble(regex.Replace(parts[5].Replace('.', ','), "")) * Variables.gridItemsPerCM) - _gem.ElementWidth / 2);
                            point.Y = ((Convert.ToDouble(regex.Replace(parts[7].Replace('.', ','), "")) * -Variables.gridItemsPerCM) - _gem.ElementHeight / 2);
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
                    bool dashed = false;
                    string arrowType = "SolidLine";

                    List<Point> arrowPoints = new List<Point>();
                    List<Point> correctArrowPoints = new List<Point>();
                    Point pointFirst;
                    Point pointNext;

                    string[] parts = _string.Split("(),".ToCharArray());
                    if (parts[1].Contains("dashed")) dashed = true;
                    for (int i = 1; i < parts.Count(); i++)     //  (parts[1] = Element0)  OR  (parts[1] = 4cm, parts[2] = -3.5cm)
                    {
                        if (dashed)
                        {
                            i++;
                            dashed = false;
                            arrowType = "DashedLine";
                        }
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
                                    if (firstPointInElement)
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
                                        pointFirst.X = Convert.ToDouble(regex.Replace(parts[i - 3].Replace('.', ','), "")) * Variables.gridItemsPerCM;
                                        pointFirst.Y = Convert.ToDouble(regex.Replace(parts[i - 2].Replace('.', ','), "")) * -Variables.gridItemsPerCM;

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
                            pointNext.X = Convert.ToDouble(regex.Replace(parts[i].Replace('.', ','), "")) * Variables.gridItemsPerCM;
                            pointNext.Y = Convert.ToDouble(regex.Replace(parts[i + 1].Replace('.', ','), "")) * -Variables.gridItemsPerCM;
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
                                    pointFirst.X = Convert.ToDouble(regex.Replace(parts[i].Replace('.', ','), "")) * Variables.gridItemsPerCM;
                                    pointFirst.Y = Convert.ToDouble(regex.Replace(parts[i + 1].Replace('.', ','), "")) * -Variables.gridItemsPerCM;

                                }

                            }
                            correctArrowPoints.Add(pointNext);
                            i += 2;
                            iteratrionNumber++;
                        }
                    }

                    arrows.Add(new ArrowsModel(correctArrowPoints, arrowType));
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

                string widthConverted = Convert.ToString(gem.ElementWidth / Variables.gridItemsPerCM);
                widthConverted = widthConverted.Replace(',', '.');
                string heightConverted = Convert.ToString(gem.ElementHeight / Variables.gridItemsPerCM);
                heightConverted = heightConverted.Replace(',', '.');
                switch (gem.ElementType)
                {
                    case "StartStop":
                        tempDeclarationEnd = "} = [rectangle, rounded corners, minimum width = ";
                        break;
                    case "InputOutput":
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
                        case "StartStop":
                            tempDeclaration = "\\tikzstyle{StartStop";
                            break;
                        case "InputOutput":
                            tempDeclaration = "\\tikzstyle{InputOutput";
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
                string locationX = Convert.ToString((gems[i].ElementStartingLocation.X + (gems[i].ElementWidth / 2)) / Variables.gridItemsPerCM);
                locationX = locationX.Replace(',', '.');
                string locationY = Convert.ToString((gems[i].ElementStartingLocation.Y + (gems[i].ElementHeight / 2)) / Variables.gridItemsPerCM * -1);
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
                string textList = "\\draw [->";
                if (_arrow.arrowType == "DashedLine") textList += ", dashed] (";
                else textList += "] (";
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
                        string locationX = Convert.ToString(_point.X / Variables.gridItemsPerCM).Replace(',', '.');
                        string locationY = Convert.ToString(_point.Y / Variables.gridItemsPerCM * -1).Replace(',', '.');

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

        public void OpenConfiguration()
        {
            if (File.Exists("Config.txt"))
            {
                TextReader textReader = new StreamReader("Config.txt");

                Variables.width = Convert.ToDouble(textReader.ReadLine());
                Variables.height = Convert.ToDouble(textReader.ReadLine());
                Variables.scale = Convert.ToDouble(textReader.ReadLine());
                Variables.gridItemsPerCM = Convert.ToDouble(textReader.ReadLine());

                for (int i = 0; i < 12; i++)
                {
                    customButtonsColors[i].A = 255;
                    customButtonsColors[i].R = Convert.ToByte(textReader.ReadLine());
                    customButtonsColors[i].G = Convert.ToByte(textReader.ReadLine());
                    customButtonsColors[i].B = Convert.ToByte(textReader.ReadLine());
                }
                for (int i = 0; i < 4; i++)
                {
                    toolColor[i].A = 255;
                    toolColor[i].R = Convert.ToByte(textReader.ReadLine());
                    toolColor[i].G = Convert.ToByte(textReader.ReadLine());
                    toolColor[i].B = Convert.ToByte(textReader.ReadLine());
                }

                textReader.Close();
            }
            else
            {
                toolColor[0] = customButtonsColors[8] = Color.FromArgb(255, 255, 179, 178);
                toolColor[1] = customButtonsColors[9] = Color.FromArgb(255, 175, 179, 255);
                toolColor[2] = customButtonsColors[10] = Color.FromArgb(255, 255, 216, 176);
                toolColor[3] = customButtonsColors[11] = Color.FromArgb(255, 179, 255, 175);
            }
        }

        public void SaveConfiguration()
        {
            TextWriter textWriter = new StreamWriter("Config.txt");

            textWriter.WriteLine(Variables.width);
            textWriter.WriteLine(Variables.height);
            textWriter.WriteLine(Variables.scale);
            textWriter.WriteLine(Variables.gridItemsPerCM);

            for(int i = 0; i<12;i++)
            {
                textWriter.WriteLine(customButtonsColors[i].R);
                textWriter.WriteLine(customButtonsColors[i].G);
                textWriter.WriteLine(customButtonsColors[i].B);
            }

            for (int i = 0; i < 4; i++)
            {
                textWriter.WriteLine(toolColor[i].R);
                textWriter.WriteLine(toolColor[i].G);
                textWriter.WriteLine(toolColor[i].B);
            }

            textWriter.Close();
        }
    }
}