﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CollectStringLiteral
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                Regex regex = null;
                var inputs = new List<string>();

                for (int i=0; i<args.Count(); ++i)
                {
                    if (args[i] == "-r")
                    {
                        regex = new Regex(args[i + 1]);
                        ++i;
                    }
                    else
                    {
                        inputs.Add(args[i]);
                    }
                }

                if (inputs.Count < 1)
                {
                    Console.WriteLine($"Search the C# code line containing the string literal.");
                    Console.WriteLine($"");
                    Console.WriteLine($"Usage: {Environment.GetCommandLineArgs()[0]} [-r Regex] .sln or .csproj files");
                    Console.WriteLine($"-r Regex      Regular expressions filter.");
                    Console.WriteLine($"");
                    throw new ArgumentException("need input file.");
                }

                var analyzer = new Analyser(regex);
                foreach (var input in inputs)
                {
                    analyzer.AnalyseAsync(input, CancellationToken.None).Wait(CancellationToken.None);
                }

                return 0;
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.Error.Write(e.Message);
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.Message);
                return 1;
            }
        }
    }
}
