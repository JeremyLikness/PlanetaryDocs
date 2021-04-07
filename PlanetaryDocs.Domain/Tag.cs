using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    public class Tag : IDocSummaries
    {
        public string TagName { get; set; }

        public List<DocumentSummary> Documents { get; private set; }
            = new List<DocumentSummary>();

        public string ETag { get; set; }

        public override int GetHashCode() => TagName.GetHashCode();

        public override bool Equals(object obj) =>
            obj is Tag tag && tag.TagName == TagName;

        public override string ToString() =>
            $"Tag {TagName} tagged by {Documents.Count} documents.";
    }
}
