using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using Edytor_graficzny.Models;
using Microsoft.Win32;

namespace Edytor_graficzny.Src
{
    class FileHandling
    {
        private string fileName = @"D:\temp\test.tex";
        private string tempTemplate = @"D:\Programowanie\Visual Studio C# WPF\Edytor graficzny\Res\Text\StartText.txt";

        public FileHandling()
        {

        }
        
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
            List<string> elementsFromOpenedFile = new List<string>();
            bool isElement = false;
            foreach (var _string in test)
            {
                if (_string == "\\end{tikzpicture}") isElement = false;
                if (isElement)
                {
                    elementsFromOpenedFile.Add(_string);
                    if (_string.Contains("\\draw"))
                    {
                        int size = 0;
                        if (graphicElementsModel != null)
                        {
                            size = graphicElementsModel.Count;
                        }
                        GraphicElementModel gem = new GraphicElementModel(size);

                        //TODO: inne typy
                        if (_string.Contains("ellipse"))
                        {
                            gem.name = "ellipse";

                            string table = "";
                            double numbe = 0;
                            List<double> numbersFromOneLine = new List<double>();
                            for (int i = 0; i < _string.Length; i++)
                            {
                                if (char.IsDigit(_string[i]) || _string[i] == '.')
                                {
                                    table += _string[i];
                                    if (!char.IsDigit(_string[i+1]) && _string[i+1] != '.')
                                    {
                                        //bool isParsable = Int32.TryParse(table, out numbe);
                                        bool isParsable = double.TryParse(table, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out numbe);
                                        if (!isParsable)
                                        {
                                            Console.WriteLine("Could not be parsed.");
                                        }
                                        numbersFromOneLine.Add(numbe);
                                        table = "";
                                    }
                                }
                            }
                            gem.startX = numbersFromOneLine[0];
                            gem.startY = numbersFromOneLine[1];
                            gem.width = numbersFromOneLine[2];
                            gem.height = numbersFromOneLine[3];
                            graphicElementsModel.Add(gem);
                        }
                    }
                }
                if(_string == "\\begin{tikzpicture}") isElement = true;
            }

            foreach (var _string in elementsFromOpenedFile)
            {
                System.Diagnostics.Debug.Write(_string);
                System.Diagnostics.Debug.Write("\n");
            }

            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //if (openFileDialog.ShowDialog() == true)
            //    MainWindow.currentTool.Text = File.ReadAllText(openFileDialog.FileName);
        }
        
        public void SaveFile()
        {

            List<string> fullTextList = new List<string>();
            string[] startText = new string[] {"\\documentclass{article}", "\\usepackage{tikz}", "\\begin{document}", "\\begin{tikzpicture}"};
            string[] endText = new string[] {"\\end{tikzpicture}", "\\end{document}"};
            
            fullTextList.AddRange(startText);
            fullTextList.Add("\\draw (2,2) ellipse (7 and 5);");
            fullTextList.AddRange(endText);


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveFileDialog.FileName, fullTextList);
            }
        }
    }
}
