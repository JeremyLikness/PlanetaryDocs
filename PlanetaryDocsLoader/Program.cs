using PlanetaryDocs.Domain;
using PlanetaryDocsLoader;
using System;
using System.Collections.Generic;
using System.IO;

const string DocsPath = @"C:\path\to\aspnetcore.docs";
const string EndPoint = "<your endpoint>";
const string AccessKey = "<your key>";
bool testsOnly = false; // set to true to re-run tests without rebuilding db

if (!Directory.Exists(DocsPath))
{
    Console.WriteLine($"Invalid path to docs: {DocsPath}");
    return;
}

List<Document> docsList = null;

if (!testsOnly)
{
    var filesToParse = FileSystemParser.FindCandidateFiles(DocsPath);
    docsList = MarkdownParser.ParseFiles(filesToParse);
}

await CosmosLoader.LoadDocumentsAsync(docsList, EndPoint, AccessKey);