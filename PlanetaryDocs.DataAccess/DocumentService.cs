using Microsoft.EntityFrameworkCore;
using PlanetaryDocs.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetaryDocs.DataAccess
{
    public class DocumentService : IDocumentService
    {
        private readonly IDbContextFactory<DocsContext> factory;

        public DocumentService(IDbContextFactory<DocsContext> factory)
            => this.factory = factory;

        public async Task InsertDocumentAsync(Document document)
        {
            using var context = factory.CreateDbContext();

            await HandleMetaAsync(context, document);

            context.Add(document);
          
            await context.SaveChangesAsync();
        }

        public async Task<Document> LoadDocumentAsync(string uid)
        {
            using var context = factory.CreateDbContext();
            return await context.FindAsync<Document>(uid);
        }

        public async Task<List<DocumentSummary>> QueryDocumentsAsync(
            string searchText,
            string authorAlias,
            string tag)
        {
            using var context = factory.CreateDbContext();
            
            var result = new HashSet<DocumentSummary>();
            
            bool partialResults = false;

            if (!string.IsNullOrWhiteSpace(authorAlias))
            {
                partialResults = true;
                var author = await context.FindMetaAsync<Author>(authorAlias);
                foreach (var ds in author.Documents)
                {
                    result.Add(ds);
                }
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                var tagEntity = await context.FindMetaAsync<Tag>(tag);

                IEnumerable<DocumentSummary> resultSet =
                    Enumerable.Empty<DocumentSummary>();
            
                // alias _AND_ tag
                if (partialResults)
                {
                    resultSet = result.Intersect(tagEntity.Documents);
                }
                else
                {
                    resultSet = tagEntity.Documents;
                }

                result.Clear();

                foreach (var docSummary in resultSet)
                {
                    result.Add(docSummary);
                }

                partialResults = true;
            }

            // nothing more to do?
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return result.OrderBy(r => r.Title).ToList();
            }

            // no list to filter further
            if (partialResults && result.Count < 1)
            {
                return result.ToList();
            }

            // find documents that match
            var documents = await context.Documents.Where(
                d => d.Title.Contains(searchText) ||
                d.Description.Contains(searchText) ||
                d.Markdown.Contains(searchText))
                .ToListAsync();

            // now only intersect with alias/tag constraints
            if (partialResults)
            {
                var uids = result.Select(ds => ds.Uid).ToList();
                documents = documents.Where(d => uids.Contains(d.Uid))
                    .ToList();
            }

            return documents.Select(d => new DocumentSummary(d))
                 .OrderBy(ds => ds.Title).ToList();
        }

        public async Task<List<string>> SearchAuthorsAsync(string searchText)
        {
            using var context = factory.CreateDbContext();
            return await context.Authors.Where(
                a => a.Alias.Contains(searchText))
                .Select(a => a.Alias)
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<List<string>> SearchTagsAsync(string searchText)
        {
            using var context = factory.CreateDbContext();
            return await context.Tags.Where(
                t => t.TagName.Contains(searchText))
                .Select(t => t.TagName)
                .OrderBy(t => t)
                .ToListAsync();
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            using var context = factory.CreateDbContext();

            await HandleMetaAsync(context, document);

            context.Attach(document);
            context.Entry(document).State = EntityState.Modified;

            await context.SaveChangesAsync();
        }

        private static async Task<Document> LoadDocNoTrackingAsync(
            DocsContext context, Document document) =>
                await context.Documents
                    .WithPartitionKey(document.Uid)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(d => d.Uid == document.Uid);

        private static async Task HandleMetaAsync(DocsContext context, Document document)
        {
            var authorChanged = 
                await CheckAuthorChangedAsync(context, document);
            await HandleTagsAsync(context, document, authorChanged);
        }

        private static async Task HandleTagsAsync(
            DocsContext context, 
            Document document,
            bool authorChanged)
        {
            var refDoc = await LoadDocNoTrackingAsync(context, document);

            // did the title change?
            var updatedTitle = refDoc != null && refDoc.Title != document.Title;

            // tags removed need summary taken away
            if (refDoc != null)
            {
                var removed = refDoc.Tags.Where(
                    t => !document.Tags.Any(dt => dt == t));

                foreach (var removedTag in removed)
                {
                    var tag = await context.FindMetaAsync<Tag>(removedTag);

                    if (tag != null)
                    {
                        var docSummary =
                            tag.Documents.FirstOrDefault(
                                d => d.Uid == document.Uid);
                        
                        if (docSummary != null)
                        {
                            tag.Documents.Remove(docSummary);
                            context.Entry(tag).State = EntityState.Modified;
                        }
                    }
                }
            }

            // figure out new tags
            var tagsAdded = refDoc == null ?
                document.Tags : document.Tags.Where(
                    t => !refDoc.Tags.Any(rt => rt == t));

            // do existing tags need title updated?
            if (updatedTitle || authorChanged)
            {
                // added ones will be handled later
                var tagsToChange = document.Tags.Except(tagsAdded);

                foreach (var tagName in tagsToChange)
                {
                    var tag = await context.FindMetaAsync<Tag>(tagName);
                    var ds = tag.Documents.Single(ds => ds.Uid == document.Uid);
                    ds.Title = document.Title;
                    ds.AuthorAlias = document.AuthorAlias;
                    context.Entry(tag).State = EntityState.Modified;
                }
            }

            // brand new tags (for the document)
            foreach (var tagAdded in tagsAdded)
            {
                var tag = await context.FindMetaAsync<Tag>(tagAdded);
                
                // new tag (overall)
                if (tag == null)
                {
                    tag = new Tag { TagName = tagAdded };
                    context.SetPartitionKey(tag, tagAdded);
                    context.Add(tag);
                }
                else
                {
                    context.Entry(tag).State = EntityState.Modified;
                }

                // either way, add the document summary
                tag.Documents.Add(new DocumentSummary(document));
            }
        }

        private static async Task<bool> CheckAuthorChangedAsync(
            DocsContext context, Document document)
        {
            var changed = false;
            var refDoc = await LoadDocNoTrackingAsync(context, document);

            // did the title change?
            if (refDoc != null && refDoc.AuthorAlias == document.AuthorAlias
                && refDoc.Title != document.Title)
            {
                var author = await context.FindMetaAsync<Author>(document.AuthorAlias);
                var docSummary = author.Documents.Single(ds => ds.Uid == document.Uid);
                docSummary.Title = document.Title;
                context.Entry(author).State = EntityState.Modified;
            }

            // did the author change? (always true for a new document)
            if (refDoc == null || refDoc.AuthorAlias != document.AuthorAlias)
            {
                if (refDoc != null)
                {
                    changed = true;
                    var oldAuthor = refDoc.AuthorAlias;
                    var author = await context.FindMetaAsync<Author>(oldAuthor);
                    var doc = author.Documents.SingleOrDefault(
                        d => d.Uid == document.Uid);
                    if (doc != null)
                    {
                        author.Documents.Remove(doc);
                        context.Entry(author).State = EntityState.Modified;
                    }
                }

                var newAuthor = await context.FindMetaAsync<Author>(
                    document.AuthorAlias);
                
                if (newAuthor == null)
                {
                    newAuthor = new Author { Alias = document.AuthorAlias };
                    context.SetPartitionKey(newAuthor, newAuthor.Alias);
                    context.Add(newAuthor);
                }
                else
                {
                    context.Entry(newAuthor).State = EntityState.Modified;
                }
                newAuthor.Documents.Add(new DocumentSummary(document));
            }

            return changed;
        }
    }
}
