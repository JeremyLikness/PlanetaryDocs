using System;
using System.Text.Json;

namespace PlanetaryDocs.Domain
{
    public class DocumentAudit
    {
        public DocumentAudit()
        {
        }

        public DocumentAudit(Document document)
        {
            Id = Guid.NewGuid();
            Uid = document.Uid;
            Document = JsonSerializer.Serialize(document);
            Timestamp = DateTimeOffset.UtcNow;
        }

        public Guid Id { get; set; }

        public string Uid { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Document { get; set; }
    }
}
