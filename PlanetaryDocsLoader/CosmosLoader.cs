using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlanetaryDocs.DataAccess;
using PlanetaryDocs.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetaryDocsLoader
{
    public static class CosmosLoader
    {
        public static async Task LoadDocumentsAsync(
            IEnumerable<Document> docsList,
            string endPoint,
            string accessKey)
        {
            Console.WriteLine("Initializing database...");

            string alias = string.Empty,
                altAlias = string.Empty,
                tag = string.Empty, 
                text = string.Empty, 
                uid = string.Empty;

            bool testOnly = docsList == null;

            var sc = new ServiceCollection();
            sc.AddDbContextFactory<DocsContext>(
                options => options.UseCosmos(
                    endPoint, accessKey, nameof(DocsContext)));
            sc.AddSingleton<IDocumentService, DocumentService>();
            var sp = sc.BuildServiceProvider().CreateScope().ServiceProvider;
            var factory = sp.GetRequiredService<IDbContextFactory<DocsContext>>();
            var service = sp.GetRequiredService<IDocumentService>();

            using (var context = factory.CreateDbContext())
            {
                if (testOnly)
                {
                    docsList = await context.Documents.ToListAsync();
                }
                else
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                }
            }

            Console.WriteLine("Database created. Populating...");

            var current = 1;
            var total = docsList.Count();
            foreach (var doc in docsList)
            {
                Console.WriteLine($"{current++}/{total}:\tInsert {doc.Uid}");
                if (!testOnly)
                {
                    await service.InsertDocumentAsync(doc);
                }
                if (doc.Tags.Count > 0 && string.IsNullOrWhiteSpace(alias))
                {
                    alias = doc.AuthorAlias;
                    tag = doc.Tags[0];
                    text = doc.Title;
                    uid = doc.Uid;
                }
                else if (
                    string.IsNullOrWhiteSpace(altAlias) && 
                    doc.AuthorAlias != alias)
                {
                    altAlias = doc.AuthorAlias;
                }
            }

            Console.WriteLine("The way has been prepared.");
            Console.WriteLine("Running tests...");

            await TestLoadDocumentAsync(service, uid);
            await TestUpdateDocumentAsync(service, uid, alias, altAlias);
            await TestSearchTagsAsync(service, tag);
            await TestSearchAuthorsAsync(service, alias);
            await TestQueryAsync(service, tag, alias, text);

            Console.WriteLine("All done!");
        }

        private static async Task TestLoadDocumentAsync(
            IDocumentService service, string uid)
        {
            var doc = await service.LoadDocumentAsync(uid);
            Console.WriteLine("Loaded document:");
            Console.WriteLine($"Title:\t{doc.Title}");
            Console.WriteLine($"Description:\t{doc.Description}");
            Console.Write("Tags:\t");
            Console.WriteLine(string.Join(", ", doc.Tags));
        }

        private static async Task TestUpdateDocumentAsync(
            IDocumentService service, 
            string uid, 
            string author, 
            string altAuthor)
        {
            var doc = await service.LoadDocumentAsync(uid);
            var newAuthor = doc.AuthorAlias == author ?
                altAuthor : author;
            doc.AuthorAlias = newAuthor;
            doc.Title = $"(new) {doc.Title}";
            doc.Tags.RemoveAt(0);
            await service.UpdateDocumentAsync(doc);
            Console.WriteLine("Updated document.");
        }

        private static async Task TestSearchAuthorsAsync(
            IDocumentService service, string alias)
        {
            Console.WriteLine("Testing author search...");
            for (var x = 1; x < alias.Length-1; x += 1)
            {
                var search = alias.Substring(0, x);
                var results = await service.SearchAuthorsAsync(search);
                var resultText = string.Join(',', results);
                Console.WriteLine($"{search}=>{resultText}");
            }
        }

        private static async Task TestSearchTagsAsync(
            IDocumentService service, string tag)
        {
            Console.WriteLine("Testing tag search...");
            for (var x = 1; x < tag.Length - 1; x += 1)
            {
                var search = tag.Substring(0, x);
                var results = await service.SearchTagsAsync(search);
                var resultText = string.Join(',', results);
                Console.WriteLine($"{search}=>{resultText}");
            }
        }

        private static async Task TestQueryAsync(
            IDocumentService service,
            string tag,
            string alias,
            string text)
        {
            Console.WriteLine("Testing query...");
            var textParts = text.Split(' ');
            var start = new Random().Next(0, textParts.Length - 2);
            var search = $"{textParts[start]} {textParts[start + 1]}";

            Console.WriteLine($"Using tag={tag} alias={alias} text={search}");

            foreach (var useTag in new [] { true, false })
            {
                foreach (var useAlias in new[] { true, false })
                {
                    foreach (var useText in new[] { true, false })
                    {
                        Console.WriteLine($"tag={useTag} alias={useAlias} text={useText}");
                        var results = await service.QueryDocumentsAsync(
                            useText ? search : string.Empty,
                            useAlias ? alias : string.Empty,
                            useTag ? tag : string.Empty);
                        foreach (var item in results)
                        {
                            Console.WriteLine($"Found {item.Uid} by {item.AuthorAlias}: '{item.Title}'");
                        }
                    }
                }
            }
        }
    }
}
