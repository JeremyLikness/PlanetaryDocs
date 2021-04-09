using Markdig;
using PlanetaryDocs.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PlanetaryDocsLoader
{
    public static class MarkdownParser
    {
        public static List<Document> ParseFiles(
            IEnumerable<string> filesToParse)
        {
            var docsList = new List<Document>();

            foreach (var file in filesToParse)
            {
                Console.WriteLine("===");
                var fileName = file.Split(Path.DirectorySeparatorChar)[^1];
                Console.WriteLine($"DOC:\t{fileName}");
                var lines = File.ReadAllLines(file);
                Console.WriteLine($"LINES:\t{lines.Length}");
                var doc = new Document();
                bool metaStart = false,
                    metaEnd = false,
                    titleFound = false,
                    aliasFound = false,
                    descriptionFound = false,
                    dateFound = false,
                    uidFound = false;

                var markdown = new List<string>();

                for (var idx = 0; idx < lines.Length; idx += 1)
                {
                    var line = lines[idx];

                    if (metaStart == false)
                    {
                        if (line.StartsWith("---"))
                        {
                            metaStart = true;
                        }
                        continue;
                    }

                    if (metaEnd == false)
                    {
                        if (line.StartsWith("---"))
                        {
                            metaEnd = true;
                            continue;
                        }
                        else
                        {
                            var metadata = line.Split(":");
                            var key = metadata[0].Trim().ToLowerInvariant();
                            switch (key)
                            {
                                case "title":
                                    titleFound = true;
                                    doc.Title = metadata[1].Trim();
                                    Console.WriteLine($"TITLE:\t{doc.Title}");
                                    break;

                                case "uid":
                                    uidFound = true;
                                    doc.Uid = metadata[1].Trim().Replace('/','_');
                                    Console.WriteLine($"UID:\t{doc.Uid}");
                                    break;

                                case "description":
                                    descriptionFound = true;
                                    doc.Description = metadata[1].Trim();
                                    break;

                                case "ms.author":
                                    aliasFound = true;
                                    doc.AuthorAlias = metadata[1].Trim();
                                    Console.WriteLine($"AUTHOR:\t{doc.AuthorAlias}");
                                    break;

                                case "ms.date":
                                    dateFound = true;
                                    doc.PublishDate = DateTime.ParseExact(
                                        metadata[1].Trim(),
                                        "M/d/yyyy",
                                        CultureInfo.InvariantCulture);
                                    Console.WriteLine($"PUB DATE:\t{doc.PublishDate}");
                                    break;

                                case "no-loc":
                                    var tags = metadata[1].Trim()
                                        .Replace("[", string.Empty)
                                        .Replace("]", string.Empty)
                                        .Replace("\"", string.Empty)
                                        .Split(",");
                                    foreach (var tag in tags.Select(
                                        t => t.Trim())
                                        .Where(t => !string.IsNullOrWhiteSpace(t))
                                        .Distinct())
                                    {
                                        doc.Tags.Add(tag);
                                    }
                                    var tagList = string.Join(", ", doc.Tags);
                                    Console.WriteLine($"TAGS:\t{tagList}");
                                    break;

                                case "default":
                                    continue;
                            }

                            continue;
                        }
                    }

                    markdown.Add(line);
                }

                var valid = titleFound && aliasFound && descriptionFound && dateFound
                    && uidFound;

                if (valid)
                {
                    Console.WriteLine("VALID");
                    doc.Markdown = string.Join(Environment.NewLine, markdown);
                    doc.Html = Markdown.ToHtml(doc.Markdown);
                    docsList.Add(doc);
                    if (doc.Title.Contains("Tutorial")) // hack
                    {
                        doc.Title = $"Tutorial: {doc.Description}";
                        if (doc.Title.Length > 60)
                        {
                            doc.Title = $"{doc.Title.Substring(0, 59)}...";   
                        }
                    }
                }
                else
                {
                    Console.WriteLine("INVALID");
                    continue;
                }
            }

            Console.WriteLine($"Processed {docsList.Count} documents.");
            return docsList;
        }
    }
}
