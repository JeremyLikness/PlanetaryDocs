// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// Simple class representing a document snapshot.
    /// </summary>
    public class DocumentAuditSummary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAuditSummary"/> class.
        /// </summary>
        public DocumentAuditSummary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAuditSummary"/> class
        /// and initializes it with the <see cref="DocumentAudit"/>.
        /// </summary>
        /// <param name="documentAudit">The <see cref="DocumentAudit"/> to summarize.</param>
        public DocumentAuditSummary(DocumentAudit documentAudit)
        {
            Id = documentAudit.Id;
            Uid = documentAudit.Uid;
            Timestamp = documentAudit.Timestamp;
            var doc = documentAudit.GetDocumentSnapshot();
            Alias = doc.AuthorAlias;
            Title = doc.Title;
        }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the audit event.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the document.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the author alias for the snapshot.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the title at the time of the snapshot.
        /// </summary>
        public string Title { get; set; }
    }
}
