using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlanetaryDocs.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlanetaryDocs.DataAccess
{
    public sealed class DocsContext : DbContext
    {
        public const string PartitionKey = nameof(PartitionKey);
        private const string Meta = nameof(Meta);
        
        public DocsContext(DbContextOptions<DocsContext> options)
            : base(options) =>
                 SavingChanges += DocsContext_SavingChanges;

        public DbSet<DocumentAudit> Audits { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {           
            modelBuilder.Entity<DocumentAudit>()
                .HasNoDiscriminator()
                .ToContainer(nameof(Audits))
                .HasPartitionKey(da => da.Uid)
                .HasKey(da => new { da.Id, da.Uid });

            var docModel = modelBuilder.Entity<Document>();

            docModel.ToContainer(nameof(Documents))
                .HasNoDiscriminator()
                .HasKey(d => d.Uid);

            docModel.HasPartitionKey(d => d.Uid)
                .Property(p => p.ETag)
                .IsETagConcurrency();

            docModel.Property(d => d.Tags)
                .HasConversion(
                    t => ToJson(t),
                    t => FromJson<List<string>>(t));

            var docSummaryConverter =
                new ValueConverter<List<DocumentSummary>, string>(
                    ds => ToJson(ds),
                    dsJson => FromJson<List<DocumentSummary>>(dsJson));

            var docSummaryValueComparer =
                new ValueComparer<DocumentSummary>(
                    (d1, d2) => d1.Uid == d2.Uid,
                    d => d.Uid.GetHashCode());

            var tagModel = modelBuilder.Entity<Tag>();

            tagModel.Property<string>(PartitionKey);

            tagModel.ToContainer(Meta)
                .HasKey(nameof(Tag.TagName), PartitionKey);

            tagModel.Property(t => t.ETag)
                .IsETagConcurrency();

            tagModel.Property(t => t.Documents)
                .HasConversion(
                    docSummaryConverter,
                    docSummaryValueComparer);

            var authorModel = modelBuilder.Entity<Author>();

            authorModel.Property<string>(PartitionKey);

            authorModel.ToContainer(Meta)
                .HasKey(nameof(Author.Alias), PartitionKey);

            authorModel.Property(a => a.ETag)
                .IsETagConcurrency();

            authorModel.Property(t => t.Documents)
                .HasConversion(
                    docSummaryConverter,
                    docSummaryValueComparer);

            base.OnModelCreating(modelBuilder);
        }

        public override void Dispose()
        {
            SavingChanges -= DocsContext_SavingChanges;
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            SavingChanges -= DocsContext_SavingChanges;
            return base.DisposeAsync();
        }

        public async ValueTask<T> FindMetaAsync<T>(string key)
            where T : class, IDocSummaries
        {
            var partitionKey = ComputePartitionKey<T>(key);
            try
            {
                return await FindAsync<T>(key, partitionKey);
            }
            catch(CosmosException ce)
            {
                if (ce.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }

        public static string ComputePartitionKey<T>(string key)
            where T : class, IDocSummaries =>
            $"{typeof(T).Name}|{key}";

        public void SetPartitionKey<T>(T entity, string key)
            where T : class, IDocSummaries =>
                Entry(entity).Property(PartitionKey).CurrentValue =
                    ComputePartitionKey<T>(key);

        private void DocsContext_SavingChanges(object sender, SavingChangesEventArgs e)
        {
            var entries = ChangeTracker.Entries<Document>()
                .Where(
                    e => e.State == EntityState.Added ||
                    e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();
            
            foreach (var docEntry in entries)
            {
                Audits.Add(new DocumentAudit(docEntry));               
            }            
        }

        private static string ToJson<T>(T item) => 
            JsonSerializer.Serialize(item);

        private static T FromJson<T>(string json) =>
            JsonSerializer.Deserialize<T>(json);
    }
}
