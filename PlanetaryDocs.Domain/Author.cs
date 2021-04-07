using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    public class Author : IDocSummaries
    {
        public string Alias { get; set; }

        public List<DocumentSummary> Documents { get; private set; }
            = new List<DocumentSummary>();

        public string ETag { get; set; }

        public override int GetHashCode() => Alias.GetHashCode();

        public override bool Equals(object obj) =>
            obj is Author author && author.Alias == Alias;

        public override string ToString() =>
            $"Author {Alias} has {Documents.Count} documents.";
    }
}
