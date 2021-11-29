# Planetary Docs
---
title: Planetary Docs
uid: 3d1c342d-d7ab-4548-b20e-784aa7a425ba
description: This is the readme for the Sample Program Planetary Docs to champion .NET Entity Framework Core for CosmosDB/SQL
ms.author: @JeremyLikness
ms.date: 04/22/2022
no-loc: [astronomy], [Entity Framework], [.NET], [Core], [Sample], [Example]
---
__NEW__ upgraded to .NET 6 and EF Core 6!

[![.NET 6 Builds](https://github.com/JeremyLikness/PlanetaryDocs/actions/workflows/dotnet.yml/badge.svg)](https://github.com/JeremyLikness/PlanetaryDocs/actions/workflows/dotnet.yml)

[![.NET 6 Tests](https://github.com/JeremyLikness/PlanetaryDocs/actions/workflows/tests.yml/badge.svg)](https://github.com/JeremyLikness/PlanetaryDocs/actions/workflows/tests.yml)

Welcome to Planetary Docs! This repository is intended to showcase a full
application that supports Create, Read, Update, and Delete operations (CRUD)
using Blazor (Server), Entity Framework Core and Azure Cosmos DB.

Please read our [Code of Conduct](./CODE_OF_CONDUCT.md) for participating in
discussions and contributions. If you are interested in contributing, please
read our [Guide for Contributors](./CONTRIBUTING.md).

This project is licensed under MIT. See the [license file](./LICENSE) for more information.

> **Important Security Notice** This app is meant for demo purposes only. As implemented, it
is not a production-ready app. More specifically, there are no users or roles defined and
access is _not_ secured by a login. That means anyone with the URL can modify your 
document database. This issue is being tracked at [#2](https://github.com/JeremyLikness/PlanetaryDocs/issues/2).

## New: EF Core 6

This project has been updated to use EF Core 6. This simplified the code a bit:

- Removed converter for tags as EF Core 6 directly supports collections of primtives
- Removed `HasMany` configuration as EF Core 6 recognizes implicit ownership of complex types for the Azure Cosmos DB provider
- Added code to migrate from the old model that serialized tags as a single JSON string to the new model that stores first class string arrays

> **Note:** If you are running the project for the first time, set `EnableMigrations` to `false` in the `appsettings.json` file. Leave it as is and make sure the id to check is valid
if you are upgrading from 5.0. The first time you run the app, it will detect the old format and use the new `FromRawSql` capabilities to load the old format and save it
to the new format.

## Quickstart

Here's how to get started in a few easy steps.

### Clone this repo

Using your preferred tools, clone the repository. The `git` commmand looks like this:

```bash
git clone https://github.com/JeremyLikness/PlanetaryDocs
```

### Create an Azure Cosmos DB instance

To run this demo, you will need to create an Azure Cosmos DB account. You can read
[Create an Azure Cosmos DB account](https://docs.microsoft.com/azure/cosmos-db/create-cosmosdb-resources-portal#create-an-azure-cosmos-db-account) to learn how. Be sure to check out the option
for a [free account](https://docs.microsoft.com/azure/cosmos-db/optimize-dev-test#azure-cosmos-db-free-tier)! Choose the SQL API.

> No Azure account? No worries! You can also run this project using the [Azure Cosmos DB emulator](https://docs.microsoft.com/azure/cosmos-db/local-emulator).

### Clone the ASP.NET Core docs repository

This is what was used for testing.

```bash
git clone https://github.com/dotnet/AspNetCore.Docs
```

### Initialize the database

Navigate to the `PlanetaryDocsLoader` project.

Update `Program.cs` with:

- The path to the root of the ASPNetCore.Docs repository
- The Azure Cosmos DB endpoint
- The Azure Cosmos DB key

The endpoint is the `URI` and the key is the `Primary Key` on the **keys** pane of your Azure 
Cosmos DB account in the [Azure Portal](https://portal.azure.com/).

Run the application (`dotnet run` from the command line). You should see status
as it parses documents, loads them to the database and then runs tests. This step
may take several minutes.

### Configure and run the Blazor app

In the `PlanetaryDocs` Blazor Server project, either update the `CosmosSettings`
in the `appsettings.json` file, or create a new section in `appsettings.Development.json`
and add you access key and endpoint. Run the app. You should be ready to go!

## Project Details

The following features were integrated into this project.

`PlanetaryDocsLoader` parses the docs repository and inserts the 
documents into the database. It includes tests to verify the
functionality is working.

`PlanetaryDocs.Domain` hosts the domain classes, validation logic,
and signature (interface) for data access.

`PlanetaryDocs.DataAccess` contains the EF Core `DbContext` 
and an implementation of the data access service.

- `DocsContext`
    - Has model-building code that shows how to map ownership
    - Uses value converters with JSON serialization to support primitives collection and nested
complex types
    - Demonstrates use of partition keys, including how to define them for the
model and how to specify them in queries
    - Provides an example of specifying the container by entity
    - Shows how to turn off the discriminator
    - Stores two entity types (aliases and tags) in the same container
    - Uses a "shadow property" to track partition keys on aliases and tags
    - Hooks into the `SavingChanges` event to automate the generation of audit snapshots
- `DocumentService`
    - Shows various strategies for C.R.U.D. operations
    - Programmatically synchronizes related entities
    - Demonstrates how to handle updates with concurrency to disconnected entities
    - Uses the new `IDbContextFactory<T>` implementation to manage context instances

`PlanetaryDocs` is a Blazor Server app.

- Examples of JavaScript interop in the `TitleService`, `HistoryService`, and `MultiLineEditService`.
- Uses keyboard handlers to allow keyboard-based navigation and input on the edit page
- Shows a generic autocomplete component with debounce built-in
- `HtmlPreview` uses a phantom `textarea` to render an HTML preview
- `MarkDig` is used to transform markdown into HTML
- The `MultiLineEdit` component shows a workaround using JavaScript interop for limitations with fields that have large input values
- The `Editor` component supports concurrency. If you open a document twice in separate tabs and edit in both, the second will notify that changes were made and provide the option to reset or overwrite

Your feedback is valuable! Reach me online at [@JeremyLikness](https://twitter.com/JeremyLikness). 


