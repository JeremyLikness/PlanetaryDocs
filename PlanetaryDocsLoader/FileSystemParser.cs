using System;
using System.Collections.Generic;
using System.IO;

namespace PlanetaryDocsLoader
{
    public static class FileSystemParser
    {
        public static HashSet<string> FindCandidateFiles(string docsPath)
        {
            var dirsToVisit = new Stack<string>();
            var filesToParse = new HashSet<string>();

            Console.WriteLine("Recursing folders...");
            dirsToVisit.Push(docsPath);

            while (dirsToVisit.Count > 0)
            {
                var dir = dirsToVisit.Pop();
                Console.Write($":");

                foreach (var subDirectory in Directory.EnumerateDirectories(dir))
                {
                    Console.Write("#");
                    dirsToVisit.Push(subDirectory);
                }

                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    if (Path.GetExtension(file) == ".md")
                    {
                        Console.Write("!");
                        filesToParse.Add(file);
                    }
                    else
                    {
                        Console.Write("-");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Found {filesToParse.Count} candidate documents.");

            return filesToParse;
        }
    }
}
