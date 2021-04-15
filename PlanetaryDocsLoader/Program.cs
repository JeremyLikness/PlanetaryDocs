// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using PlanetaryDocs.Domain;
using PlanetaryDocsLoader;

// path to repository
const string DocsPath = @"C:\path\to\aspnetcore.docs";

// Azure Cosmos DB endpoint
const string EndPoint = "https://<youraccount>.documents.azure.com:443/";

// Secret key for Azure Cosmos DB
const string AccessKey = "<yourkey>";

// set to true to re-run tests without rebuilding db
bool testsOnly = false;

if (testsOnly == false && !Directory.Exists(DocsPath))
{
    Console.WriteLine($"Invalid path to docs: {DocsPath}");
    return;
}

List<Document> docsList = null;

if (testsOnly == false)
{
    var filesToParse = FileSystemParser.FindCandidateFiles(DocsPath);
    docsList = MarkdownParser.ParseFiles(filesToParse);
}

await CosmosLoader.LoadDocumentsAsync(docsList, EndPoint, AccessKey);
