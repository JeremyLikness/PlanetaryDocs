using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    public interface IDocSummaries
    {
        List<DocumentSummary> Documents { get; }
    }
}
