using System;

namespace PlanetaryDocs.Domain
{
    public class DocumentAuditSummary
    {
        public DocumentAuditSummary()
        {
        }

        public DocumentAuditSummary(DocumentAudit documentAudit)
        {
            Id = documentAudit.Id;
            Uid = documentAudit.Uid;
            Timestamp = documentAudit.Timestamp;
            var doc = documentAudit.GetDocumentSnapshot();
            Alias = doc.AuthorAlias;
            Title = doc.Title;
        }

        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Uid { get; set; }
        public string Alias { get; set; }
        public string Title { get; set; }
    }
}
