# Planetary Docs

Welcome to Planetary Docs! This repository is intended to showcase a full
application that supports Create, Read, Update, and Delete operations (CRUD)
using Blazor (Server), Entity Framework Core and Azure Cosmos DB.

Please read our [Code of Conduct](./CODE_OF_CONDUCT.md) for participating in
discussions and contributions. If you are interested in contributing, please
read our [Guide for Contributors](./CONTRIBUTING.md).

This project is licensed under MIT. See the [license file](./LICENSE) for more information.

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