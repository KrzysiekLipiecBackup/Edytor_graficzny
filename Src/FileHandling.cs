using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public void OpenFile()
        {
            string[] test = File.ReadAllLines(tempTemplate);
            foreach (var _string in test)
            {
                System.Diagnostics.Debug.WriteLine(_string);
            }
        }
        
        public void SaveFile()
        {
            
            
            string[] startText = new string[] {"\\documentclass{article}", "\\usepackage{tikz}", "\\begin{document}", "\\begin{tikzpicture}"};
            string[] endText = new string[] {"\\end{tikzpicture}", "\\end{document}"};
            List<string> fullTextList = new List<string>();
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
