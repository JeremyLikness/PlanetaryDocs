using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetaryDocs.Domain
{
    public interface IDocumentService
    {
        Task InsertDocumentAsync(Document document);

        Task UpdateDocumentAsync(Document document);

        Task<List<string>> SearchTagsAsync(string searchText);

        Task<List<string>> SearchAuthorsAsync(string searchText);

        Task<List<DocumentSummary>> QueryDocumentsAsync(
            string searchText,
            string authorAlias,
            string tag);

        Task<Document> LoadDocumentAsync(string uid);
    }
}
