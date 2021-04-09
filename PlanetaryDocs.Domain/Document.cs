using System;
using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    public class Document
    {
        public string Uid { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime PublishDate { get; set; }

        public string Markdown { get; set; }

        public string Html { get; set; }

        public string AuthorAlias { get; set; }

        public List<string> Tags { get; private set; }
            = new List<string>();

        public string ETag { get; set; }

        public override int GetHashCode() => Uid.GetHashCode();

        public override bool Equals(object obj) =>
            obj is Document document && document.Uid == Uid;

        public override string ToString() =>
            $"Document {Uid} by {AuthorAlias} with {Tags.Count} tags: {Title}.";

        public Document Clone() =>
            new ()
            {
                AuthorAlias = AuthorAlias,
                Description = Description,
                ETag = ETag,
                Html = Html,
                Markdown = Markdown,
                PublishDate = PublishDate,
                Tags = new List<string>(Tags),
                Title = Title,
                Uid = Uid
            };
    }
}
