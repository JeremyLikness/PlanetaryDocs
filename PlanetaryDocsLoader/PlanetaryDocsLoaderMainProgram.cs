// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using PlanetaryDocs.Domain;
using PlanetaryDocsLoader;

// path to repository
const string DocsPath = @"..\..\..\..";// @"C:\path\to\aspnetcore.docs";

// Azure Cosmos DB endpoint
string EndPoint = new System.Text.RegularExpressions.Regex("\\\\").Replace(Environment.GetEnvironmentVariable("COSMOS_ENDPOINT"), "/"); // "https://<youraccount>.documents.azure.com:443/";

// Secret key for Azure Cosmos DB
string AccessKey = Environment.GetEnvironmentVariable("COSMOS_ACCOUNTKEY");// "<yourkey>";

// set to true to re-run tests without rebuilding db
var testsOnly = false;

if (!testsOnly && !Directory.Exists(DocsPath))
{
    Console.WriteLine($"Invalid path to docs: {DocsPath}");
    return;
}

List<Document> docsList = null;

if (!testsOnly)
{
    var filesToParse = FileSystemParser.FindCandidateFiles(Path.GetFullPath(DocsPath));
    docsList = MarkdownParser.ParseFiles(filesToParse);
}

await CosmosLoader.LoadDocumentsAsync(docsList, EndPoint, AccessKey);
