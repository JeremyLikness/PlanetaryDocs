namespace PlanetaryDocs.Domain
{
    public class DocumentSummary
    {
        public DocumentSummary()
        {
        }

        public DocumentSummary(Document doc)
        {
            Uid = doc.Uid;
            Title = doc.Title;
            AuthorAlias = doc.AuthorAlias;
        }

        public string Uid { get; set; }

        public string Title { get; set; }

        public string AuthorAlias { get; set; }


        public override int GetHashCode() => Uid.GetHashCode();

        public override bool Equals(object obj) =>
            obj is DocumentSummary ds && ds.Uid == Uid;

        public override string ToString() =>
            $"Summary for {Uid} by {AuthorAlias}: {Title}.";

    }
}
