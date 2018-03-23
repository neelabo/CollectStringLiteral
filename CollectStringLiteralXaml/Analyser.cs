using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CollectStringLiteralXaml
{
    class Analyser
    {
        private Regex _regex;

        public Analyser(Regex regex)
        {
            _regex = regex;
        }

        public void Analyse(string path)
        {
            var directory = new DirectoryInfo(path);
            if (directory.Exists)
            {
                foreach(var file in directory.GetFiles("*.xaml", SearchOption.AllDirectories))
                {
                    AnalyseXaml(file.FullName);
                }
            }
            else
            {
                AnalyseXaml(path);
            }
        }

        public void AnalyseXaml(string path)
        {
            var lines = new List<int>();

            using (var reader = new XmlTextReader(path))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (IsMatch(reader.Value))
                                    {
                                        lines.Add(reader.LineNumber);
                                    }
                                }
                            }
                            break;

                        case XmlNodeType.Text:
                            if (IsMatch(reader.Value))
                            {
                                lines.Add(reader.LineNumber);
                            }
                            break;
                    }
                }
            }

            lines = lines.Distinct().ToList();

            using (StreamReader reader = new StreamReader(path))
            {
                int count = 0;
                while (reader.EndOfStream == false)
                {
                    count++;
                    var record = reader.ReadLine();
                    if (lines.Contains(count))
                    {
                        Console.WriteLine($"{path}({count}):{record}");
                    }
                }
            }
        }


        private bool IsMatch(string text)
        {
            return _regex == null || _regex.IsMatch(text);
        }
    }

}
